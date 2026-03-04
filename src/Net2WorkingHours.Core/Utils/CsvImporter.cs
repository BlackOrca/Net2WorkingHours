using Net2WorkingHours.Core.Models;
using System.Globalization;
using System.Text;

namespace Net2WorkingHours.Core.Utils;

public static class CsvImporter
{
    public static List<WorkEntry> Import(string filePath)
    {
        var entries = new List<WorkEntry>();
        var encoding = Encoding.GetEncoding("ISO-8859-1"); // Latin-1 für Umlaute
        var lines = File.ReadAllLines(filePath, encoding);
        foreach (var line in lines.Skip(1)) // Skip header
        {
            var parts = line.Split(';');
            if (parts.Length < 5) continue;
            var user = parts[1].Trim();
            if (user == "Unknown" || string.IsNullOrWhiteSpace(user)) continue;
            var dateStr = parts[0].Trim();
            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                continue;
            var location = parts[3].Trim();
            var eventText = parts[4].Trim();
            bool isEntry = location.EndsWith("(Eintritt)");
            bool isExit = location.EndsWith("(Austritt)");
            entries.Add(new WorkEntry
            {
                DateTime = dt,
                User = user,
                Location = location,
                Event = eventText,
                IsEntry = isEntry,
                IsExit = isExit
            });
        }
        return entries;
    }
}
