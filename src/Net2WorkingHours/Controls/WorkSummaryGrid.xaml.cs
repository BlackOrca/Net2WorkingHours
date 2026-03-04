using System.Windows;
using System.Windows.Controls;

namespace Net2WorkingHours.Controls;

public partial class WorkSummaryGrid : UserControl
{
    public WorkSummaryGrid()
    {
        InitializeComponent();
    }

    public void SetData(IEnumerable<object> items, double totalNormal, double totalOvertime, double totalAll)
    {
        RootPanel.Children.Clear();
        var grid = new DataGrid { AutoGenerateColumns = false, Height = 400, Margin = new Thickness(0,0,0,10), ItemsSource = items };
        grid.Columns.Add(new DataGridTextColumn { Header = "Datum", Binding = new System.Windows.Data.Binding("Date") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Wochentag", Binding = new System.Windows.Data.Binding("Weekday") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Eintritt", Binding = new System.Windows.Data.Binding("EntryTime") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Austritt", Binding = new System.Windows.Data.Binding("ExitTime") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Normalarbeitszeit", Binding = new System.Windows.Data.Binding("NormalHours") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Mehrarbeitszeit", Binding = new System.Windows.Data.Binding("OvertimeHours") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Pause (Min)", Binding = new System.Windows.Data.Binding("BreakMinutes") });
        RootPanel.Children.Add(grid);
        var summaryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0,0,0,10) };
        summaryPanel.Children.Add(new TextBlock { Text = $"Gesamt Arbeitszeit: {totalNormal:0.##} h", Margin = new Thickness(0,0,20,0), FontWeight = FontWeights.Bold });
        summaryPanel.Children.Add(new TextBlock { Text = $"Gesamt Mehrarbeitszeit: {totalOvertime:0.##} h", Margin = new Thickness(0,0,20,0), FontWeight = FontWeights.Bold });
        summaryPanel.Children.Add(new TextBlock { Text = $"Gesamt Stunden: {totalAll:0.##} h", FontWeight = FontWeights.Bold });
        RootPanel.Children.Add(summaryPanel);
    }
}
