using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models
{
    public class TweetHashTag
    {
        public Guid TweetHashTagId { get; set; }
        public Guid ChirpId { get; set; }
        public string HashTag { get; set; }
    }
}
