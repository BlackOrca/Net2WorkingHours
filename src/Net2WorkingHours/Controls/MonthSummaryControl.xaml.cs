using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Net2WorkingHours.Controls;

public partial class MonthSummaryControl : UserControl
{
    public MonthSummaryControl()
    {
        InitializeComponent();
    }

    public async Task SetMonthDataAsync(Employee emp, IGrouping<(int Year, int Month), KeyValuePair<DateOnly, WorkdaySummary>> monthGroup, IExcelExportService excelExportService, ISettingsService settingsService)
    {
        // Panel leeren, aber DataGrid nicht entfernen
        if (MonthSummaryPanel.Children.Count > 0 && MonthSummaryPanel.Children[0] is DataGrid)
        {
            // DataGrid bleibt erhalten
            while (MonthSummaryPanel.Children.Count > 1)
                MonthSummaryPanel.Children.RemoveAt(1);
        }
        else
        {
            MonthSummaryPanel.Children.Clear();
            MonthSummaryPanel.Children.Add(MonthSummaryDataGrid);
        }

        var workTimeSettings = await settingsService.LoadAsync();
        var items = CreateMonthSummaryItems(monthGroup, workTimeSettings);

        MonthSummaryDataGrid.ItemsSource = items;

        var totalNormal = monthGroup.Sum(d => d.Value.NormalHours);
        var totalOvertime = monthGroup.Sum(d => d.Value.OvertimeHours);
        var totalAll = totalNormal + totalOvertime;

        StackPanel summaryPanel = CreateSummaryPanel(totalNormal, totalOvertime, totalAll);
        MonthSummaryPanel.Children.Add(summaryPanel);

        Button exportButton = CreateExcelExportButton(emp, monthGroup, excelExportService);
        MonthSummaryPanel.Children.Add(exportButton);
    }

    private static List<MonthSummary> CreateMonthSummaryItems(
        IGrouping<(int Year, int Month), KeyValuePair<DateOnly, WorkdaySummary>> monthGroup,
        WorkTimeSettings workTimeSettings)
    {
        return [.. monthGroup.Select(d =>
        {
            var daySetting = workTimeSettings.WorkTimes.FirstOrDefault(w => w.Day == d.Key.DayOfWeek);
            bool isEntryInWork = false;
            bool isExitInWork = false;
            if (daySetting != null)
            {
                var entry = d.Value.EntryTime;
                var exit = d.Value.ExitTime;
                var entryTimeOnly = TimeOnly.FromDateTime(entry);
                var exitTimeOnly = TimeOnly.FromDateTime(exit);
                isEntryInWork = entryTimeOnly >= daySetting.Start && entryTimeOnly <= daySetting.End;
                isExitInWork = exitTimeOnly >= daySetting.Start && exitTimeOnly <= daySetting.End;
            }
            return new MonthSummary
            {
                Date = d.Key.ToString(),
                Weekday = GetGermanWeekday(d.Key.DayOfWeek),
                EntryTime = d.Value.EntryTime.ToString("HH:mm"),
                ExitTime = d.Value.ExitTime.ToString("HH:mm"),
                NormalHours = d.Value.NormalHours.ToString("0.##"),
                OvertimeHours = d.Value.OvertimeHours.ToString("0.##"),
                BreakMinutes = d.Value.BreakMinutes.ToString("0"),
                IsEntryTimeInWork = isEntryInWork,
                IsExitTimeInWork = isExitInWork
            };
        })];
    }

    private static StackPanel CreateSummaryPanel(double totalNormal, double totalOvertime, double totalAll)
    {
        var summaryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
        summaryPanel.Children.Add(new TextBlock { Text = $"Gesamt Arbeitszeit: {totalNormal:0.##} h", Margin = new Thickness(0, 0, 20, 0), FontWeight = FontWeights.Bold });
        summaryPanel.Children.Add(new TextBlock { Text = $"Gesamt Mehrarbeitszeit: {totalOvertime:0.##} h", Margin = new Thickness(0, 0, 20, 0), FontWeight = FontWeights.Bold });
        summaryPanel.Children.Add(new TextBlock { Text = $"Gesamt Stunden: {totalAll:0.##} h", FontWeight = FontWeights.Bold });
        return summaryPanel;
    }

    private static Button CreateExcelExportButton(Employee emp, IGrouping<(int Year, int Month), KeyValuePair<DateOnly, WorkdaySummary>> monthGroup, IExcelExportService excelExportService)
    {
        var exportButton = new Button
        {
            Content = "Als Excel Exportieren und Öffnen",
            HorizontalAlignment = HorizontalAlignment.Right,
            Padding = new Thickness(10, 5, 10, 5),
            Margin = new Thickness(0, 0, 0, 10)
        };

        exportButton.Click += async (s, e) =>
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel-Datei (*.xlsx)|*.xlsx",
                FileName = SanitizeFileName($"{emp.Name}_{new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1):MM_yyyy}.xlsx")
            };

            if (dlg.ShowDialog() == true)
            {
                var filePath = dlg.FileName;
                var exportEntries = emp.Entries.Where(x => x.DateTime.Year == monthGroup.Key.Year && x.DateTime.Month == monthGroup.Key.Month).ToList();

                byte[]? excelBytes = null;
                try
                {
                    excelBytes = await excelExportService.CreateWorkhoursExcelAsync([
                        new() {
                            Name = emp.Name,
                            Entries = exportEntries,
                            Workdays = monthGroup.ToDictionary(d => d.Key, d => d.Value)
                        }
                    ]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Erstellen der Excel-Datei:\n{ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clipboard.SetText(ex.Message);
                    return;
                }

                try
                {
                    await File.WriteAllBytesAsync(filePath, excelBytes);
                    MessageBox.Show($"Export erfolgreich: {filePath}. Excel wird jetzt gestartet.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern der Datei:\n{ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clipboard.SetText(ex.Message);
                    return;
                }

                try
                {
                    Process.Start(
                        new ProcessStartInfo(filePath)
                        {
                            FileName = filePath,
                            UseShellExecute = true,
                        });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Öffnen der Excel-Datei:\n{ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clipboard.SetText(ex.Message);
                    return;
                }
            }
        };
        return exportButton;
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(c, '_');

        foreach (var c in fileName.Where(c => c == ',' || c == ';' || Char.IsWhiteSpace(c)))
            fileName = fileName.Replace(c, '_');

        return fileName;
    }

    private static string GetGermanWeekday(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Montag",
            DayOfWeek.Tuesday => "Dienstag",
            DayOfWeek.Wednesday => "Mittwoch",
            DayOfWeek.Thursday => "Donnerstag",
            DayOfWeek.Friday => "Freitag",
            DayOfWeek.Saturday => "Samstag",
            DayOfWeek.Sunday => "Sonntag",
            _ => "?"
        };
    }
}
