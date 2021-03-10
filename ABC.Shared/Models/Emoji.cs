using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models
{
    public class Emoji
    {

        public Guid EmojiId { get; set; }
        public string EmojiName { get; set; }
        public string EmojiShortName { get; set; }
        public string EmojiNonQualified { get; set; }
        public string EmojiUnified { get; set; }
        public string EmojiCategory { get; set; }
    }
}
