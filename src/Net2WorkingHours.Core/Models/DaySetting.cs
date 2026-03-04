namespace Net2WorkingHours.Core.Models;

public sealed class DaySetting
{
    public DayOfWeek Day { get; init; }
    public TimeOnly EarlyPossibleStart { get; init; } // Earliest possible start for overtime before regular start
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
    public TimeSpan Break { get; init; }
}
