using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using System.Windows.Controls;

namespace Net2WorkingHours.Controls;

public partial class SettingsControl : UserControl
{
    private readonly ISettingsService SettingsService;
    public WorkTimeSettings? CurrentSettings {get; private set;}

    public SettingsControl(ISettingsService settingsService)
    {
        InitializeComponent();
        SettingsService = settingsService;
        Loaded += SettingsControl_Loaded;
    }

    private async void SettingsControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        CurrentSettings = await SettingsService.LoadAsync();
        if (CurrentSettings == null)
        {
            System.Windows.MessageBox.Show("Einstellungen konnten nicht geladen werden!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }
        SetSettings(CurrentSettings);
    }

    private async void OnSaveClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        var workTimes = new List<DaySetting>
        {
            new DaySetting { Day = DayOfWeek.Monday, EarlyPossibleStart = ParseTimeOnly(EarlyMonday.Text), Start = ParseTimeOnly(StartMonday.Text), End = ParseTimeOnly(EndMonday.Text), Break = ParseMinutes(BreakMonday.Text) },
            new DaySetting { Day = DayOfWeek.Tuesday, EarlyPossibleStart = ParseTimeOnly(EarlyTuesday.Text), Start = ParseTimeOnly(StartTuesday.Text), End = ParseTimeOnly(EndTuesday.Text), Break = ParseMinutes(BreakTuesday.Text) },
            new DaySetting { Day = DayOfWeek.Wednesday, EarlyPossibleStart = ParseTimeOnly(EarlyWednesday.Text), Start = ParseTimeOnly(StartWednesday.Text), End = ParseTimeOnly(EndWednesday.Text), Break = ParseMinutes(BreakWednesday.Text) },
            new DaySetting { Day = DayOfWeek.Thursday, EarlyPossibleStart = ParseTimeOnly(EarlyThursday.Text), Start = ParseTimeOnly(StartThursday.Text), End = ParseTimeOnly(EndThursday.Text), Break = ParseMinutes(BreakThursday.Text) },
            new DaySetting { Day = DayOfWeek.Friday, EarlyPossibleStart = ParseTimeOnly(EarlyFriday.Text), Start = ParseTimeOnly(StartFriday.Text), End = ParseTimeOnly(EndFriday.Text), Break = ParseMinutes(BreakFriday.Text) },
            new DaySetting { Day = DayOfWeek.Saturday, EarlyPossibleStart = ParseTimeOnly(EarlySaturday.Text), Start = ParseTimeOnly(StartSaturday.Text), End = ParseTimeOnly(EndSaturday.Text), Break = ParseMinutes(BreakSaturday.Text) },
            new DaySetting { Day = DayOfWeek.Sunday, EarlyPossibleStart = ParseTimeOnly(EarlySunday.Text), Start = ParseTimeOnly(StartSunday.Text), End = ParseTimeOnly(EndSunday.Text), Break = ParseMinutes(BreakSunday.Text) },
        };
        var overtimeGrace = ParseMinutes(OvertimeGrace.Text);
        var settings = new WorkTimeSettings
        {
            WorkTimes = workTimes,
            OvertimeGrace = overtimeGrace
        };
        try
        {
            await SettingsService.SaveAsync(settings);
            System.Windows.MessageBox.Show("Einstellungen wurden gespeichert.", "Erfolg", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            CurrentSettings = settings;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Fehler beim Speichern der Einstellungen: {ex.Message}", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }


    private static TimeOnly ParseTimeOnly(string text)
        => TimeOnly.TryParse(text, out var t) ? t : TimeOnly.MinValue;

    private static TimeSpan ParseMinutes(string text)
        => int.TryParse(text, out var min) ? TimeSpan.FromMinutes(min) : TimeSpan.Zero;

    private void SetSettings(WorkTimeSettings settings)
    {

        void SetDay(DayOfWeek day, TextBox early, TextBox start, TextBox end, TextBox pause)
        {
            var ds = settings.WorkTimes.FirstOrDefault(x => x.Day == day);
            if (ds != null)
            {
                early.Text = ds.EarlyPossibleStart.ToString("HH:mm");
                start.Text = ds.Start.ToString("HH:mm");
                end.Text = ds.End.ToString("HH:mm");
                pause.Text = ((int)ds.Break.TotalMinutes).ToString();
            }
            else
            {
                early.Text = start.Text = end.Text = pause.Text = "Fehler!";
            }
        }

        SetDay(DayOfWeek.Monday, EarlyMonday, StartMonday, EndMonday, BreakMonday);
        SetDay(DayOfWeek.Tuesday, EarlyTuesday, StartTuesday, EndTuesday, BreakTuesday);
        SetDay(DayOfWeek.Wednesday, EarlyWednesday, StartWednesday, EndWednesday, BreakWednesday);
        SetDay(DayOfWeek.Thursday, EarlyThursday, StartThursday, EndThursday, BreakThursday);
        SetDay(DayOfWeek.Friday, EarlyFriday, StartFriday, EndFriday, BreakFriday);
        SetDay(DayOfWeek.Saturday, EarlySaturday, StartSaturday, EndSaturday, BreakSaturday);
        SetDay(DayOfWeek.Sunday, EarlySunday, StartSunday, EndSunday, BreakSunday);

        OvertimeGrace.Text = ((int)settings.OvertimeGrace.TotalMinutes).ToString();
    }
}
