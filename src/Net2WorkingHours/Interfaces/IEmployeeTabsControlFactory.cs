using Net2WorkingHours.Controls;
using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Interfaces;

public interface IEmployeeTabsControlFactory
{
    Task<EmployeeTabsControl> CreateAsync(List<Employee> employees);
}
