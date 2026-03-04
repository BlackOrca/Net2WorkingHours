using Net2WorkingHours.Controls;
using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using Net2WorkingHours.Interfaces;

namespace Net2WorkingHours.Services;

public class EmployeeTabsControlFactory(IWorkSummaryService workSummaryService, IExcelExportService excelExportService, ISettingsService settingsService) : IEmployeeTabsControlFactory
{
    private readonly IWorkSummaryService WorkSummaryService = workSummaryService;
    private readonly IExcelExportService ExcelExportService = excelExportService;
    private readonly ISettingsService SettingsService = settingsService;

    public async Task<EmployeeTabsControl> CreateAsync(List<Employee> employees)
    {
        var control = new EmployeeTabsControl();
        await control.SetEmployeesAsync(employees, WorkSummaryService, ExcelExportService, SettingsService);
        return control;
    }
}
