namespace PacifyAspire.Web.Components.Models
{
    public class StatsModels
    {
        public string frequentMoodIntensity { get; set; }
        public string frequentMood { get; set; }
        public Dictionary<string, double> moodIntensityDistribution { get; set; }
        public double moodIntensityVariance { get; set; }
        public List<string> dailyMoodChanges { get; set; }
        public Dictionary<string, string> commonMoodsPerTimeframe { get; set; }
        public Dictionary<string, double> longestMoodStreak { get; set; }
        public List<string> suddenMoodShift { get; set; }
        public List<string> unusualHighIntensityMoodPattern { get; set; }
        public string userId { get; set; }
        public string id { get; set; }
    }
}
