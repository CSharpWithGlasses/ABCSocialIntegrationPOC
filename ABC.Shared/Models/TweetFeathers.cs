using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models
{
    public class TweetFeather
    {
        public Guid FeatherId { get; set; }
        public Guid ChirpId { get; set; }
        public bool HasEmoji { get; set; } = false;
        public bool HasUrl { get; set; } = false;
        public bool HasImage { get; set; } = false;
        public bool HasHashTag { get; set; } = false;
        public DateTime TweetStamp { get; set; }
    }
}
