using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Core.Services;

public class WorkSummaryService : IWorkSummaryService
{
    public IEnumerable<IGrouping<(int Year, int Month), KeyValuePair<DateOnly, WorkdaySummary>>> GroupByMonth(Dictionary<DateOnly, WorkdaySummary> dailySummaries)
    {
        return dailySummaries.GroupBy(d => (d.Key.Year, d.Key.Month));
    }

    public (double totalNormal, double totalOvertime, double totalAll) CalculateTotals(IEnumerable<WorkdaySummary> summaries)
    {
        var totalNormal = summaries.Sum(s => s.NormalHours);
        var totalOvertime = summaries.Sum(s => s.OvertimeHours);
        var totalAll = totalNormal + totalOvertime;
        return (totalNormal, totalOvertime, totalAll);
    }
}
