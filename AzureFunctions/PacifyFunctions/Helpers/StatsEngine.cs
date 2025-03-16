using Microsoft.Extensions.Logging;
using PacifyFunctions.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Helpers
{
    public class StatsEngine
    {
        ILogger log;
        public StatsModels statsModel;

        public StatsEngine(ILogger logger, StatsModels models)
        {
            statsModel = models;
            log = logger;
        }

        public String GetMostFrequentWords(String word, List<MoodLogs> moods)
        {
            switch (word)
            {

                case "intensity":

                    var mostFrequentIntensity = moods.GroupBy(m => m.moodIntensity).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key;
                    return mostFrequentIntensity;

                case "mood":

                    var mostFrequentMood = moods.GroupBy(m => m.mood).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key;
                    return mostFrequentMood;

                default:
                    break;


            }

            return "";
        }

        public void GetMoodIntensityStatistics(List<MoodLogs> moods)
        {

            var intensityMapping = new Dictionary<string, int>
            {
                { "Low", 1 },
                { "Medium", 2 },
                { "High", 3 }
            };

            var intensityCounts = moods
                .Where(m => intensityMapping.ContainsKey(m.moodIntensity))
                .GroupBy(m => m.moodIntensity)
                .ToDictionary(g => g.Key, g => g.Count());

            int total = intensityCounts.Values.Sum();

            log.LogInformation("Mood Intensity Distribution:");
            foreach (var entry in intensityMapping.Keys)
            {
                double percentage = intensityCounts.ContainsKey(entry) ?
                                    (intensityCounts[entry] / (double)total) * 100 : 0;
                log.LogInformation($"{entry}: {percentage:F2}%");

                statsModel.moodIntensityDistribution.Add(entry, percentage);
            }

            var intensities = moods
                .Where(m => intensityMapping.ContainsKey(m.moodIntensity))
                .Select(m => intensityMapping[m.moodIntensity])
                .ToList();

            double mean = intensities.Average();
            double variance = intensities.Select(i => Math.Pow(i - mean, 2)).Average();

            log.LogInformation($"Mood Intensity Variance: {variance:F2}");
            statsModel.moodIntensityVariance = variance;
        }

        public void GetTimeBasedMoodTrends(List<MoodLogs> moods)
        {

            moods = moods.OrderBy(m => m.moodDate).ToList();

            var dailyChanges = moods.GroupBy(m => m.moodDate.Date)
                .ToDictionary(g => g.Key, g => g.Select(m => m.mood).Distinct().Count());

            log.LogInformation("Daily Mood Changes:");
            foreach (var entry in dailyChanges)
            {
                log.LogInformation($"{entry.Key.ToShortDateString()} - {entry.Value} mood(s) recorded");
                statsModel.dailyMoodChanges.Add($"{entry.Key.ToShortDateString()} - {entry.Value} mood(s) recorded");
            }

            log.LogInformation("Most Common Moods Per Timeframe:");

            var dailyMoods = GetMostCommonMood(moods, m => m.moodDate.Date);
            var weeklyMoods = GetMostCommonMood(moods, m => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.moodDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
            var monthlyMoods = GetMostCommonMood(moods, m => new { m.moodDate.Year, m.moodDate.Month });

            log.LogInformation($"Per Day: {string.Join(", ", dailyMoods.Select(m => $"{m.Key}: {m.Value}"))}");
            log.LogInformation($"Per Week: {string.Join(", ", weeklyMoods.Select(m => $"Week {m.Key}: {m.Value}"))}");
            log.LogInformation($"Per Month: {string.Join(", ", monthlyMoods.Select(m => $"{m.Key.Year}/{m.Key.Month}: {m.Value}"))}");

            statsModel.commonMoodsPerTimeframe.Add("daily",  string.Join(", ", dailyMoods.Select(m => $"{m.Key}: {m.Value}")));
            statsModel.commonMoodsPerTimeframe.Add("weekly", string.Join(", ", weeklyMoods.Select(m => $"Week {m.Key}: {m.Value}")));
            statsModel.commonMoodsPerTimeframe.Add("monthly", string.Join(", ", weeklyMoods.Select(m => $"Week {m.Key}: {m.Value}")));

            log.LogInformation("Longest Mood Streak:");
            var longestStreak = GetLongestMoodStreak(moods);
            log.LogInformation($"Mood: {longestStreak.Mood}, Streak: {longestStreak.Streak} days");

            statsModel.longestMoodStreak.Add(longestStreak.Mood, longestStreak.Streak);


            Console.WriteLine("Mood Changes Over Time:");
            PrintMoodGraph(moods);
        }

        public static Dictionary<T, string> GetMostCommonMood<T>(List<MoodLogs> moods, Func<MoodLogs, T> groupByFunc)
        {
            return moods.GroupBy(groupByFunc)
                .ToDictionary(g => g.Key, g => g.GroupBy(m => m.mood)
                                                .OrderByDescending(m => m.Count())
                                                .First().Key);
        }

        public static (string Mood, int Streak) GetLongestMoodStreak(List<MoodLogs> moods)
        {
            string longestMood = "";
            int longestStreak = 0;
            int currentStreak = 1;
            string currentMood = moods.First().mood;

            for (int i = 1; i < moods.Count; i++)
            {
                if (moods[i].mood == currentMood && (moods[i].moodDate.Date - moods[i - 1].moodDate.Date).Days == 1)
                {
                    currentStreak++;
                }
                else
                {
                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                        longestMood = currentMood;
                    }
                    currentMood = moods[i].mood;
                    currentStreak = 1;
                }
            }

            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
                longestMood = currentMood;
            }

            return (longestMood, longestStreak);
        }

        public void PrintMoodGraph(List<MoodLogs> moods)
        {
            var groupedByDay = moods.GroupBy(m => m.moodDate.Date)
                                    .OrderBy(g => g.Key);

            foreach (var group in groupedByDay)
            {
                string moodsLine = string.Join(" ", group.Select(m => m.mood.Substring(0, 1)));
                log.LogInformation($"{group.Key.ToShortDateString()}: {moodsLine}");
            }
        }

        public void DetectAnomalies(List<MoodLogs> moods)
        {
            if (moods.Count < 2)
            {
                log.LogInformation("Not enough data to analyze anomalies.");
                statsModel.suddenMoodShift.Add("Not enough data to analyze anomalies.");
                return;
            }

            moods = moods.OrderBy(m => m.moodDate).ToList();

            log.LogInformation("Sudden Mood Shifts:");
            for (int i = 1; i < moods.Count; i++)
            {
                if (IsSuddenShift(moods[i - 1], moods[i]))
                {
                    log.LogInformation($"{moods[i - 1].moodDate.ToShortDateString()} ({moods[i - 1].mood}) --> {moods[i].moodDate.ToShortDateString()} ({moods[i].mood})");
                    statsModel.suddenMoodShift.Add($"{moods[i - 1].moodDate.ToShortDateString()} ({moods[i - 1].mood}) --> {moods[i].moodDate.ToShortDateString()} ({moods[i].mood})");
                }
            }

            log.LogInformation("Unusual Mood Patterns:");
            var highIntensityStreaks = DetectHighIntensityStreaks(moods, 3);
            if (highIntensityStreaks.Count == 0)
            {
                log.LogInformation("No unusual high-intensity mood patterns detected.");
                statsModel.unusualHighIntensityMoodPattern.Add("No unusual high-intensity mood patterns detected.");
            }
            else
            {
                foreach (var streak in highIntensityStreaks)
                {
                    log.LogInformation($"High-intensity streak from {streak.StartDate.ToShortDateString()} to {streak.EndDate.ToShortDateString()} ({streak.StreakLength} days)");
                    statsModel.unusualHighIntensityMoodPattern.Add($"High-intensity streak from {streak.StartDate.ToShortDateString()} to {streak.EndDate.ToShortDateString()} ({streak.StreakLength} days)");
                }
            }
        }

        public static bool IsSuddenShift(MoodLogs prev, MoodLogs current)
        {
            var moodScale = new Dictionary<string, int>
        {
            { "Happy", 3 }, { "Neutral", 2 }, { "Sad", 1 }, { "Angry", 0 }
        };

            return moodScale.ContainsKey(prev.mood) && moodScale.ContainsKey(current.mood) &&
                   Math.Abs(moodScale[prev.mood] - moodScale[current.mood]) >= 2;
        }

        public static List<(DateTime StartDate, DateTime EndDate, int StreakLength)> DetectHighIntensityStreaks(List<MoodLogs> moods, int minStreak)
        {
            List<(DateTime StartDate, DateTime EndDate, int StreakLength)> streaks = new();
            int currentStreak = 0;
            DateTime streakStart = moods.First().moodDate;

            foreach (var mood in moods)
            {
                if (mood.moodIntensity == "High")
                {
                    if (currentStreak == 0) streakStart = mood.moodDate;
                    currentStreak++;
                }
                else
                {
                    if (currentStreak >= minStreak)
                    {
                        streaks.Add((streakStart, mood.moodDate.AddDays(-1), currentStreak));
                    }
                    currentStreak = 0;
                }
            }

            if (currentStreak >= minStreak)
            {
                streaks.Add((streakStart, moods.Last().moodDate, currentStreak));
            }

            return streaks;

        }
    }
}
