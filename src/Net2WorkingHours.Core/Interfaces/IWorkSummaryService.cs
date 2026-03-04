using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Core.Interfaces;

public interface IWorkSummaryService
{
    IEnumerable<IGrouping<(int Year, int Month), KeyValuePair<DateOnly, WorkdaySummary>>> GroupByMonth(Dictionary<DateOnly, WorkdaySummary> dailySummaries);
    (double totalNormal, double totalOvertime, double totalAll) CalculateTotals(IEnumerable<WorkdaySummary> summaries);
}