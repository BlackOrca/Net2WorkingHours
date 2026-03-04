using System.Windows.Controls;
using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Controls;

public partial class EmployeeTabsControl : UserControl
{
    public EmployeeTabsControl()
    {
        InitializeComponent();
    }

    public async Task SetEmployeesAsync(List<Employee> employees, IWorkSummaryService workSummaryService, IExcelExportService excelExportService, ISettingsService settingsService)
    {
        EmployeeTabControl.Items.Clear();
        foreach (var emp in employees)
        {
            var monthGroups = workSummaryService.GroupByMonth(emp.Workdays)
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);
            var userTab = new TabItem { Header = emp.Name };
            var monthTabControl = new TabControl();
            foreach (var monthGroup in monthGroups)
            {
                var monthName = new System.DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1).ToString("MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
                var monthTab = new TabItem { Header = monthName };
                var monthSummary = new MonthSummaryControl();
                await monthSummary.SetMonthDataAsync(emp, monthGroup, excelExportService, settingsService);
                monthTab.Content = monthSummary;
                monthTabControl.Items.Add(monthTab);
            }
            userTab.Content = monthTabControl;
            EmployeeTabControl.Items.Add(userTab);
        }
    }
}
