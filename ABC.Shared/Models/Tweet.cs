using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models
{
    public class Tweet
    {

        public Guid     ChirpId { get; set; }

        public string   TweetContent { get; set; }
        public string FullTweetDataJson { get; set; }
        public DateTime ReceivedDate { get; set; } = DateTime.Now;
        public bool Processed { get; set; }
    }
}
