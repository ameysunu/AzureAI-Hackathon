namespace PacifyAspire.Web.Components.Models
{
    public class CommunityDataModel
    {
        public String id { get; set; } = Guid.NewGuid().ToString();
        public String userId { get; set; }
        public String userName { get; set; }
        public DateTime createdOn { get; set; }
        public String contents { get; set; }
        public int upVotes { get; set; }
        public List<CommunityDataModel> comments { get; set; }
    }
}
