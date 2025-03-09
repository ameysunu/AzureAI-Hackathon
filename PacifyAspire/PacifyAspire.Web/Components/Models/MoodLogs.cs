namespace PacifyAspire.Web.Components.Models
{
    public class MoodLogs
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Mood { get; set; } = string.Empty;
        public string MoodDescription { get; set; } = string.Empty;
        public string MoodIntensity { get; set; } = "Low";
        public DateTime MoodDate { get; set; } = DateTime.Today;

        public string UserId { get; set; }

    }
}
