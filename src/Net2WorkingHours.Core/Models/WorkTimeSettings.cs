namespace Net2WorkingHours.Core.Models;

public sealed class WorkTimeSettings
{
    public required List<DaySetting> WorkTimes { get; init; } = [];
    public required TimeSpan OvertimeGrace { get; init; } = TimeSpan.Zero;
}
