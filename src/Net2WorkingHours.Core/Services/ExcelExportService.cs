using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using OfficeOpenXml;

namespace Net2WorkingHours.Core.Services;
public class ExcelExportService() : IExcelExportService
{
    public async Task<byte[]> CreateWorkhoursExcelAsync(IEnumerable<Employee> employees)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Net2-Workinghours");
        using var package = new ExcelPackage();
        foreach (var employee in employees)
        {
            var worksheet = package.Workbook.Worksheets.Add(employee.Name);
            worksheet.Cells[1, 1].Value = "Datum";
            worksheet.Cells[1, 2].Value = "Wochentag";
            worksheet.Cells[1, 3].Value = "Normalarbeitszeit (h)";
            worksheet.Cells[1, 4].Value = "Überstunden (h)";
            worksheet.Cells[1, 5].Value = "Gesamtstunden (h)";
            int row = 2;
            double sumNormal = 0;
            double sumOvertime = 0;
            foreach (var day in employee.Workdays)
            {
                worksheet.Cells[row, 1].Value = day.Key.ToShortDateString();
                worksheet.Cells[row, 2].Value = GetGermanWeekday(day.Key.DayOfWeek);
                worksheet.Cells[row, 3].Value = Math.Round(day.Value.NormalHours, 2);
                worksheet.Cells[row, 4].Value = Math.Round(day.Value.OvertimeHours, 2);
                worksheet.Cells[row, 5].Value = Math.Round(day.Value.NormalHours + day.Value.OvertimeHours, 2);
                sumNormal += day.Value.NormalHours;
                sumOvertime += day.Value.OvertimeHours;
                row++;
            }
            // Summenzeile
            worksheet.Cells[row, 1].Value = "Summe";
            worksheet.Cells[row, 3].Value = Math.Round(sumNormal, 2);
            worksheet.Cells[row, 4].Value = Math.Round(sumOvertime, 2);
            worksheet.Cells[row, 5].Value = Math.Round(sumNormal + sumOvertime, 2);
        }

        return await package.GetAsByteArrayAsync();
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
