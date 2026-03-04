using Net2WorkingHours.Core.Models;

namespace Net2WorkingHours.Core.Utils;

public static class WorkTimeCalculator
{
    public static Dictionary<DateOnly, WorkdaySummary> CalculateDailyWorkSummaries(IEnumerable<WorkEntry> entries, WorkTimeSettings settings)
    {
        var grouped = entries
            .Where(e => e.IsEntry || e.IsExit)
            .GroupBy(e => DateOnly.FromDateTime(e.DateTime));
        var result = new Dictionary<DateOnly, WorkdaySummary>();
        foreach (var day in grouped)
        {
            var times = day.OrderBy(e => e.DateTime).ToList();
            var entry = times.FirstOrDefault(e => e.IsEntry);
            var exit = times.LastOrDefault(e => e.IsExit);
            if (entry == null || exit == null) continue;
            var weekday = day.Key.DayOfWeek;
            var daySetting = settings.WorkTimes.FirstOrDefault(x => x.Day == weekday);
            double breakMinutes = daySetting != null ? daySetting.Break.TotalMinutes : 0;
            var workSpan = exit.DateTime - entry.DateTime - TimeSpan.FromMinutes(breakMinutes);
            if (workSpan < TimeSpan.Zero) workSpan = TimeSpan.Zero;
            var karenz = settings.OvertimeGrace;
            double overtime = 0;
            double normal = 0;
            if (daySetting != null)
            {
                var normalSpan = (daySetting.End - daySetting.Start - daySetting.Break);
                var entryTimeOnly = TimeOnly.FromDateTime(entry.DateTime);
                var exitTimeOnly = TimeOnly.FromDateTime(exit.DateTime);
                var earlyStart = daySetting.EarlyPossibleStart;
                var regularStart = daySetting.Start;
                var regularEnd = daySetting.End;
                // Arbeitszeit vor regulärem Start (aber nach EarlyPossibleStart)
                TimeSpan earlyOver = TimeSpan.Zero;
                if (entryTimeOnly < regularStart && entryTimeOnly >= earlyStart)
                {
                    earlyOver = regularStart - entryTimeOnly;
                    if (earlyOver > karenz)
                        overtime += earlyOver.TotalHours;
                }
                // Arbeitszeit nach regulärem Ende
                TimeSpan lateOver = TimeSpan.Zero;
                if (exitTimeOnly > regularEnd)
                {
                    lateOver = exitTimeOnly - regularEnd;
                    if (lateOver > karenz)
                        overtime += lateOver.TotalHours;
                }
                // Normale Arbeitszeit ist maximal normalSpan, aber nicht mehr als tatsächlich gearbeitet wurde (abzüglich Überstunden)
                var totalNormal = workSpan - earlyOver - lateOver;
                if (totalNormal > normalSpan)
                    normal = normalSpan.TotalHours;
                else if (totalNormal > TimeSpan.Zero)
                    normal = totalNormal.TotalHours;
                // Falls die gesamte Arbeitszeit kleiner als normalSpan ist, gibt es keine Überstunden
                if (workSpan <= normalSpan)
                    overtime = 0;
            }
            result[day.Key] = new WorkdaySummary
            {
                EntryTime = entry.DateTime,
                ExitTime = exit.DateTime,
                NormalHours = normal,
                OvertimeHours = overtime,
                BreakMinutes = breakMinutes
            };
        }
        return result;
    }
}
