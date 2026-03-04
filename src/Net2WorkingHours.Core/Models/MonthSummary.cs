namespace Net2WorkingHours.Core.Models;

public sealed class MonthSummary
{
    public required string Date { get; set; }
    public required string Weekday { get; set; }
    public required string EntryTime { get; set; }
    public required string ExitTime { get; set; }
    public required string NormalHours { get; set; }
    public required string OvertimeHours { get; set; }
    public required string BreakMinutes { get; set; }
    public bool IsEntryTimeInWork { get; set; }
    public bool IsExitTimeInWork { get; set; }
}
