using ABC.Shared.Interfaces;
using ABC.Shared.Interfaces.Repository;
using ABC.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ABC.Integration.Managers
{
    public class StatisticsManager : BaseManager, ISocialStatistics
    {
        IRepository _repository;
        private readonly IConfiguration _configuration;
        ILogger _logger;

        public StatisticsManager(IRepository repo, IConfiguration configuration, ILogger<StatisticsManager> logger)
        {
            _repository = repo;
            _configuration = configuration;
            _logger = logger;
        }
        public SocialStatistics GetTwitterStatistics(DateTime requestTime)
        {

            var countInfo = (from feather in _repository.GetAll<TweetFeather>()
                             where feather.TweetStamp < requestTime
                             group feather by 1 into featherGroup
                             select new
                             {
                                 EmojiCount = featherGroup.Count(x => x.HasEmoji),
                                 UrlCount = featherGroup.Count(x => x.HasUrl),
                                 ImageCount = featherGroup.Count(x => x.HasImage),
                                 TotalCount = featherGroup.Count()
                             })?.FirstOrDefault();

            SocialStatistics tweetStatistics = new SocialStatistics()
            {
                TotalItems = countInfo?.TotalCount ?? 0,
                ItemsHavingEmojiCount = countInfo?.EmojiCount ?? 0,
                ItemsHavingPhotoUrlCount = countInfo?.ImageCount ?? 0,
                ItemsHavingUrlCount = countInfo?.UrlCount ?? 0
            };


            // top hashtags
            var tweetHashTagInfo = (
                     from feather in _repository.GetAll<TweetFeather>().Where(x => x.TweetStamp <= requestTime && x.HasHashTag == true)
                     join tweetHash in _repository.GetAll<TweetHashTag>() on feather.ChirpId equals tweetHash.ChirpId
                     group tweetHash by tweetHash.HashTag into hashGroup
                     orderby hashGroup.Count() descending
                     select new
                     {
                         HashTag = hashGroup.Key,
                         HashCount = hashGroup.Count()
                     })?.Take(3).ToList();

            tweetStatistics.TopHashtags = tweetHashTagInfo?.Select(x => x.HashTag).ToList();

            //top domains
            var tweetDomainInfo = (
                    from feather in _repository.GetAll<TweetFeather>().Where(x => x.TweetStamp <= requestTime && x.HasUrl == true)
                    join tweetUrl in _repository.GetAll<TweetUrl>() on feather.ChirpId equals tweetUrl.ChirpId
                    group tweetUrl by tweetUrl.Domain into urlGroup
                    orderby urlGroup.Count() descending
                    select new
                    {
                        Domain = urlGroup.Key,
                        DomainCount = urlGroup.Count()
                    })?.Take(3).ToList();


            tweetStatistics.TopDomains = tweetDomainInfo?.Select(x => x.Domain).ToList();

            //top Emoji 
            var tweetEmojiInfo = (
                    from feather in _repository.GetAll<TweetFeather>().Where(x => x.TweetStamp <= requestTime && x.HasEmoji == true)
                    join tweetEmoji in _repository.GetAll<TweetEmoji>() on feather.ChirpId equals tweetEmoji.ChirpId
                    join emojiTbl in _repository.GetAll<Shared.Models.Emoji>() on tweetEmoji.EmojiId equals emojiTbl.EmojiId
                    group emojiTbl by emojiTbl.EmojiName into emojiGroup
                    orderby emojiGroup.Count() descending
                    select new
                    {
                        EmojiName = emojiGroup.Key,
                        EmojiCount = emojiGroup.Count()
                    })?.Take(3).ToList();

            tweetStatistics.TopEmojis = tweetEmojiInfo?.Select(x => x.EmojiName).ToList();

            if (tweetStatistics.TotalItems > 0)
            {
                var minProcessDate = _repository.GetAll<TweetFeather>().Select(item => item.TweetStamp).Min();
                var hoursPassed = (DateTime.Now - minProcessDate).TotalHours;
                var minutesPassed = (DateTime.Now - minProcessDate).TotalMinutes;
                var secondsPassed = (DateTime.Now - minProcessDate).TotalSeconds;
                var totalTweets = tweetStatistics.TotalItems;

                tweetStatistics.ItemsPerHour = (int)((double)totalTweets / (hoursPassed > 0 ? hoursPassed : 1));
                tweetStatistics.ItemsPerMinute = (int)((double)totalTweets / (minutesPassed > 0 ? minutesPassed : 1));
                tweetStatistics.ItemsPerSecond = (int)((double)totalTweets / (secondsPassed > 0 ? secondsPassed : 1));
                tweetStatistics.PctItemsHavingUrl = Decimal.Round((decimal)tweetStatistics.ItemsHavingUrlCount / (decimal)tweetStatistics.TotalItems, 3);
                tweetStatistics.PctItemsHavingPhotoUrl = Decimal.Round((decimal)tweetStatistics.ItemsHavingPhotoUrlCount / (decimal)tweetStatistics.TotalItems, 3);
                tweetStatistics.PctItemsHavingEmoji = Decimal.Round((decimal)tweetStatistics.ItemsHavingEmojiCount / (decimal)tweetStatistics.TotalItems, 3);

            }

            tweetStatistics.ProcessedTime = DateTime.Now;

            return tweetStatistics;

        }

    }
}
