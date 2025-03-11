using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Models
{
    public class Thoughts
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("image")]
        public String Image { get; set; }
    }

    public class MoodLogs
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string mood { get; set; }
        public string moodDescription { get; set; }
        public string moodIntensity { get; set; }
        public DateTime moodDate { get; set; }
        public string userId { get; set; }
    }

    public class MoodViewData
    {
        public string userId { get; set; }
    }
}
