using ABC.Integration.Managers;
using ABC.Shared.Interfaces.Repository;
using ABC.Shared.Models;
using Microsoft.Extensions.Configuration;
using SocialOpinionAPI.Core;
using SocialOpinionAPI.Models.SampledStream;
//using SocialOpinionAPI.Services.SampledStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using EmojiData;
using Microsoft.Extensions.Logging;
using ABC.Shared.Extensions;
using static SocialOpinionAPI.Services.SampledStream.ABC.SampledStreamService;
using SocialOpinionAPI.Services.SampledStream.ABC;
using ABC.Shared.Interfaces;

namespace ABC.Integration.Managers
{
    public class TwitterManager : BaseManager, ISocialManager
    {

        IRepository _repository;
        private readonly IConfiguration _configuration;
        SocialOpinionAPI.Services.SampledStream.ABC.SampledStreamService _twitterStreamService;
        ILogger _logger;

        public TwitterManager(IRepository repo, IConfiguration configuration, ILogger<TwitterManager> logger)
        {
            _repository = repo;
            _configuration = configuration;
            _logger = logger;

            string _ConsumerKey = _configuration["AppSettings:ConsumerKey"];
            string _ConsumerSecret = _configuration["AppSettings:ConsumerSecret"];
            string _AccessToken = _configuration["AppSettings:AccessToken"];
            string _AccessTokenSecret = _configuration["AppSettings:AccessTokenSecret"];

            OAuthInfo oAuthInfo = new OAuthInfo
            {
                AccessSecret = _AccessTokenSecret,
                AccessToken = _AccessToken,
                ConsumerSecret = _ConsumerSecret,
                ConsumerKey = _ConsumerKey
            };

            _twitterStreamService = new SampledStreamService(oAuthInfo);
            var resourceName = _configuration["AppSettings:EmojiResource"];
            LoadEmojiDataFromFileResource(resourceName, _repository);
        }
 
        public void StartService()
        {

            var twitterApiUrl = _configuration["AppSettings:TwitterApiUrl"];
            var maxTweets = int.Parse(_configuration["AppSettings:MaxTweets"]);
            var maxConnectAttempts = int.Parse(_configuration["AppSettings:MaxConnectAttempts"]);
            // stream//"https://api.twitter.com/2/tweets/sample/stream?expansions=attachments.poll_ids,attachments.media_keys,author_id,entities.mentions.username,geo.place_id,in_reply_to_user_id,referenced_tweets.id,referenced_tweets.id.author_id"
            //filter//"https://api.twitter.com/2/tweets/search/stream?tweet.fields=attachments,author_id,context_annotations,conversation_id,created_at,entities,geo,id,in_reply_to_user_id,lang,public_metrics,possibly_sensitive,referenced_tweets,source,text,withheld&expansions=author_id&user.fields=created_at,description,entities,id,location,name,pinned_tweet_id,profile_image_url,protected,public_metrics,url,username,verified,withheld"
            _twitterStreamService.DataReceivedEvent += StreamService_DataReceivedEvent;
            _twitterStreamService.StartStream(twitterApiUrl, maxTweets, maxConnectAttempts);
        }

        protected  void StreamService_DataReceivedEvent(object sender, EventArgs e)
        {
            try
            {
                DataReceivedEventArgs eventArgs = e as DataReceivedEventArgs;
                SampledStreamModel model = eventArgs.StreamDataResponse;

                var aTweet = new Tweet
                {
                    ChirpId = Guid.NewGuid(),
                    TweetContent = model?.data?.text,
                    FullTweetDataJson = model.data.source
                };


                _repository.Insert<Tweet>(new List<Tweet> { aTweet });
                _repository.Save();


                Task.Run(() => ParseTweetMedtaData(aTweet, model));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + e?.ToString());
            }

        }

        private async Task ParseTweetMedtaData(Tweet theTweet, SampledStreamModel streamModel)
        {

            var hasEmoji = false;
            var hasHashTag = false;
            var hasUrl = false;
            var hasImage = false;

            if (_repository == null) throw new Exception("WTF!!");

            dynamic twitterJsonResult = JObject.Parse(streamModel.data.source);
            var emojiMatches = EmojiData.Emoji.EmojiRegex.Matches(theTweet.TweetContent);
            hasEmoji = emojiMatches.Count() > 0;
            hasUrl = streamModel.data?.entities?.urls != null ? streamModel.data.entities.urls.Any() : false;

            if (hasEmoji)
            {

                var tweetMojis =  ProcessEmojis(theTweet.TweetContent.Codepoints(), theTweet);
                if (tweetMojis != null && tweetMojis.Any())
                {
                    await _repository.Insert<TweetEmoji>(tweetMojis);
                    await _repository.Save();
                }
            }


            if (hasUrl)
            {
                var tweetUrls = new List<TweetUrl>();
                streamModel.data.entities.urls.ForEach(item =>
                {
                    hasImage = item.expanded_url.Contains("pic.twitter.com", StringComparison.InvariantCultureIgnoreCase)
                                || item.expanded_url.Contains("Instagram", StringComparison.InvariantCultureIgnoreCase);

                    var tUrl = new TweetUrl
                    {
                        ChirpId = theTweet.ChirpId,
                        TweetUrlId = Guid.NewGuid(),
                        Url = item.expanded_url,
                        IsImage = hasImage,
                        Domain = new Uri(item.expanded_url).Host
                    };
                    tweetUrls.Add(tUrl);
                });
                await _repository.Insert<TweetUrl>(tweetUrls);
                await _repository.Save();
            }


            if (twitterJsonResult != null && twitterJsonResult.data.entities !=null)
            {
                if (twitterJsonResult?.data?.entities?.hashtags != null)
                {
         
                    hasHashTag = true;

                    var hashEntries = twitterJsonResult.data.entities.hashtags.Count;
                    List<TweetHashTag> tweetHashTags = new List<TweetHashTag>();
                    for (var counter = 0; counter < hashEntries; counter++)
                    {
                        tweetHashTags.Add(new TweetHashTag
                        {
                            ChirpId = theTweet.ChirpId,
                            TweetHashTagId = Guid.NewGuid(),
                            HashTag = twitterJsonResult.data.entities.hashtags[counter].tag
                        });
                           
                    }
                    await _repository.Insert<TweetHashTag>(tweetHashTags);
                    await _repository.Save();
                                              ;
                }
            }

            var feathers = new List<TweetFeather> {
                                    new TweetFeather {
                                        ChirpId = theTweet.ChirpId,
                                        FeatherId = Guid.NewGuid(),
                                         HasEmoji = hasEmoji,
                                         HasHashTag = hasHashTag,
                                         HasImage = hasImage,
                                         HasUrl = hasUrl,
                                         TweetStamp = DateTime.Now
                                    } };

            await _repository.Insert<TweetFeather>(feathers) ;

            await _repository.Save();

        }

        private List<TweetEmoji> ProcessEmojis(IEnumerable<Codepoint> codePointItems, Tweet theTweet)
        {

            var tweetEmojis = new List<TweetEmoji>();

           codePointItems.ToList().ForEach(item =>
           {

               var emojiCodeArray = item.ToString().Trim().Split("+");
               var emojiCode = string.Empty;
               if (emojiCodeArray.Length > 1)
               {
                   //for multi code emojis our datbase seperates them with -
                   emojiCode = emojiCodeArray[1].Trim().Replace(' ', '-');
               }
               else emojiCode = emojiCodeArray[0];

               var ourEmoji = _repository.GetAll<Shared.Models.Emoji>().FirstOrDefault(x => x.EmojiUnified == emojiCode || x.EmojiNonQualified == emojiCode);


               if (ourEmoji != null && ourEmoji.EmojiId != Guid.Empty)
               {
                   tweetEmojis.Add(new TweetEmoji
                   {
                       ChirpId = theTweet.ChirpId,
                       TweetEmojiId = Guid.NewGuid(),
                       EmojiId = ourEmoji.EmojiId
                   });
               }

           });

            return tweetEmojis;

        }
    }
}
