namespace Net2WorkingHours.Core.Models;

public class WorkEntry
{
    public DateTime DateTime { get; set; }
    public required string User { get; set; }
    public required string Location { get; set; }
    public required string Event { get; set; }
    public bool IsEntry { get; set; }
    public bool IsExit { get; set; }
}
