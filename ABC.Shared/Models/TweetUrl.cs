using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models
{
    public class TweetUrl
    {

        public Guid TweetUrlId { get; set; }

        public Guid ChirpId { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }
        public bool IsImage { get; set; }
    }
}
