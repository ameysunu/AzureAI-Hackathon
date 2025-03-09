namespace PacifyAspire.Web.Components.Models
{
    public class MoodLogs
    {
        public string Mood { get; set; } = string.Empty;
        public string MoodDescription { get; set; } = string.Empty;
        public string MoodIntensity { get; set; } = "Low";
        public DateTime MoodDate { get; set; } = DateTime.Today;

    }
}
