using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models
{
    public class SocialStatistics
    {

        public Guid StatId { get; set; } = Guid.NewGuid();
        public DateTime ProcessedTime { get; set; }
        public List<String> TopEmojis { get; set; } = new List<string>();
        public List<String> TopDomains { get; set; } = new List<string>();
        public List<String> TopHashtags { get; set; } = new List<string>();
        public int ItemsHavingUrlCount { get; set; } = 0;
        public int ItemsHavingPhotoUrlCount { get; set; } = 0;
        public int ItemsHavingEmojiCount { get; set; } = 0;
        public int ItemsPerHour { get; set; }
        public int ItemsPerMinute { get; set; }
        public int ItemsPerSecond { get; set; }
        public int TotalItems { get; set; } = 0;
        public decimal PctItemsHavingUrl { get; set; }
        public decimal PctItemsHavingPhotoUrl { get; set; }
        public decimal PctItemsHavingEmoji { get; set; }

    }
}
