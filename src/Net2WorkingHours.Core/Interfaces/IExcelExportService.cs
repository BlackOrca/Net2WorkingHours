
using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Core.Interfaces;
public interface IExcelExportService
{
    Task<byte[]> CreateWorkhoursExcelAsync(IEnumerable<Employee> employees);
}
