namespace Net2WorkingHours.Core.Models;

public class WorkdaySummary
{
    public required DateTime EntryTime { get; set; }
    public required DateTime ExitTime { get; set; }
    public required double NormalHours { get; set; }
    public required double OvertimeHours { get; set; }
    public required double BreakMinutes { get; set; }
}
