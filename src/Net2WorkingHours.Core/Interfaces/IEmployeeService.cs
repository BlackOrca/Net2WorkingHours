using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Core.Interfaces;

public interface IEmployeeService
{
    List<Employee> ImportEmployeesFromCsv(string csvPath, WorkTimeSettings settings);
}