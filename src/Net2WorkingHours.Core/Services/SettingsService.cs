using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using System.Text.Json;

namespace Net2WorkingHours.Core.Services;

public class SettingsService() : ISettingsService
{
    private readonly string FilePath = "settings.json";

    public async ValueTask<WorkTimeSettings> LoadAsync()
    {
        if (!File.Exists(FilePath))
        {
            return GetDefault();
        }
        var json = await File.ReadAllTextAsync(FilePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<WorkTimeSettings>(json, options) ?? GetDefault();
        return result;
    }

    public async ValueTask SaveAsync(WorkTimeSettings settings)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(settings, options);
        await File.WriteAllTextAsync(FilePath, json);
    }

    private static WorkTimeSettings GetDefault()
    {
        return new WorkTimeSettings
        {
            WorkTimes = new List<DaySetting>
            {
                new DaySetting { Day = DayOfWeek.Monday, EarlyPossibleStart = new TimeOnly(8,0), Start = new TimeOnly(8,0), End = new TimeOnly(17,0), Break = TimeSpan.FromMinutes(60) },
                new DaySetting { Day = DayOfWeek.Tuesday, EarlyPossibleStart = new TimeOnly(8,0), Start = new TimeOnly(8,0), End = new TimeOnly(17,0), Break = TimeSpan.FromMinutes(60) },
                new DaySetting { Day = DayOfWeek.Wednesday, EarlyPossibleStart = new TimeOnly(8,0), Start = new TimeOnly(8,0), End = new TimeOnly(17,0), Break = TimeSpan.FromMinutes(60) },
                new DaySetting { Day = DayOfWeek.Thursday, EarlyPossibleStart = new TimeOnly(8,0), Start = new TimeOnly(8,0), End = new TimeOnly(17,0), Break = TimeSpan.FromMinutes(60) },
                new DaySetting { Day = DayOfWeek.Friday, EarlyPossibleStart = new TimeOnly(8,0), Start = new TimeOnly(8,0), End = new TimeOnly(17,0), Break = TimeSpan.FromMinutes(60) },
                new DaySetting { Day = DayOfWeek.Saturday, EarlyPossibleStart = new TimeOnly(0,0), Start = new TimeOnly(0,0), End = new TimeOnly(0,0), Break = TimeSpan.FromMinutes(0) },
                new DaySetting { Day = DayOfWeek.Sunday, EarlyPossibleStart = new TimeOnly(0,0), Start = new TimeOnly(0,0), End = new TimeOnly(0,0), Break = TimeSpan.FromMinutes(0) },
            },
            OvertimeGrace = TimeSpan.FromMinutes(15)
        };
    }
}
