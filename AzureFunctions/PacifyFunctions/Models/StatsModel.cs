namespace PacifyFunctions.Models
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

        public StatsModels()
        {
            moodIntensityDistribution = new Dictionary<string, double>();
            dailyMoodChanges = new List<string>();
            commonMoodsPerTimeframe = new Dictionary<string, string>();
            longestMoodStreak = new Dictionary<string, double>();
            suddenMoodShift = new List<string>();
            unusualHighIntensityMoodPattern = new List<string>();
            userId = Guid.NewGuid().ToString();
            id = Guid.NewGuid().ToString();
        }

    }
}