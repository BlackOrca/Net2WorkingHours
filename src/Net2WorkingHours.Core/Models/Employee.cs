using System.Collections.Generic;

namespace Net2WorkingHours.Core.Models;
public class Employee
{
    public required string Name { get; set; }
    public List<WorkEntry> Entries { get; set; } = new();
    public Dictionary<DateOnly, WorkdaySummary> Workdays { get; set; } = new();
}
