using Net2WorkingHours.Core.Models;
using Net2WorkingHours.Core.Services;

namespace Net2WorkingHours.Tests
{
    public class SettingsServiceTests
    {
        [Fact]
        public async Task SaveAndLoadSettings_WorksCorrectly()
        {
            // Arrange
            var settings = new WorkTimeSettings
            {
                WorkTimes =
            [
                new() { Day = DayOfWeek.Monday, EarlyPossibleStart = new TimeOnly(8,0,0), Start = new TimeOnly(8,0,0), End = new TimeOnly(17,0,0), Break = TimeSpan.FromHours(1) }
            ],
                OvertimeGrace = TimeSpan.FromMinutes(15)
            };

            var file = Path.GetTempFileName();
            var service = new SettingsService();

            // SaveAsync/LoadAsync verwenden
            await service.SaveAsync(settings);
            var loaded = await service.LoadAsync();

            // Assert
            var orig = settings.WorkTimes[0];
            var loadedDay = loaded.WorkTimes.Find(x => x.Day == DayOfWeek.Monday);
            Assert.NotNull(loadedDay);
            Assert.Equal(orig.Start, loadedDay.Start);
            Assert.Equal(orig.End, loadedDay.End);
            Assert.Equal(orig.Break, loadedDay.Break);
            Assert.Equal(settings.OvertimeGrace, loaded.OvertimeGrace);
            File.Delete(file);
        }
    }
}