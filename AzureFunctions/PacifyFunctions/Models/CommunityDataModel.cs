using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Models
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

    public class CommInput
    {
        public bool isComments { get; set; }
        public string postId { get; set; }
        public CommunityDataModel contents { get; set; }
    }
}
