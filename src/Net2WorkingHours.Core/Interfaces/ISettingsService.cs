using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Core.Interfaces;

public interface ISettingsService
{
    ValueTask<WorkTimeSettings> LoadAsync();
    ValueTask SaveAsync(WorkTimeSettings settings);
}
