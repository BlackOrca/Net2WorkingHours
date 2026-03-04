using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using Net2WorkingHours.Core.Utils;

namespace Net2WorkingHours.Core.Services;

public class EmployeeService : IEmployeeService
{
    public List<Employee> ImportEmployeesFromCsv(string csvPath, WorkTimeSettings settings)
    {
        var entries = CsvImporter.Import(csvPath);
        var employees = new List<Employee>();
        var users = entries.Select(e => e.User).Distinct().OrderBy(u => u).ToList();
        foreach (var user in users)
        {
            var userEntries = entries.Where(e => e.User == user).OrderBy(e => e.DateTime).ToList();
            var daily = WorkTimeCalculator.CalculateDailyWorkSummaries(userEntries, settings);
            var emp = new Employee { Name = user, Entries = userEntries, Workdays = new() };
            foreach (var d in daily)
            {
                emp.Workdays[d.Key] = d.Value;
            }
            employees.Add(emp);
        }
        return employees;
    }
}
