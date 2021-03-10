using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ABC.Shared.Models;

namespace ABC.Data
{
    public class ABCDbContext : DbContext
    {
        public ABCDbContext(DbContextOptions<ABCDbContext> options) : base(options) { }
        
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Emoji> Emojis { get; set; }
        public DbSet<TweetEmoji> TweetEmojis { get; set; }
        public DbSet<TweetFeather> TweetFeathers { get; set; }
        public DbSet<TweetUrl> TweetUrls { get; set; }
        public DbSet<SocialStatistics> TweetStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Tweet>(x => x.HasKey(key=> key.ChirpId));
            modelBuilder.Entity<Emoji>(x => x.HasKey(key => key.EmojiId));
            modelBuilder.Entity<TweetEmoji>(x => x.HasKey(key => key.TweetEmojiId));
            modelBuilder.Entity<TweetFeather>(x => x.HasKey(key => key.FeatherId));
            modelBuilder.Entity<TweetUrl>(x => x.HasKey(key => key.TweetUrlId));
            modelBuilder.Entity<TweetHashTag>(x => x.HasKey(key => key.TweetHashTagId));
            modelBuilder.Entity<SocialStatistics>(x => x.HasNoKey());
            modelBuilder.Entity<SocialStatistics>().Ignore(i => i.TopEmojis);
            modelBuilder.Entity<SocialStatistics>().Ignore(i => i.TopHashtags);
            modelBuilder.Entity<SocialStatistics>().Ignore(i => i.TopDomains);

    }

    }
}
