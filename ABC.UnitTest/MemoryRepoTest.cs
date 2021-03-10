using ABC.Data;
using ABC.Shared.Interfaces.Repository;
using ABC.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using ABC.Integration.Managers;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.Extensions.Logging;

namespace ABC.UnitTest
{
    [TestClass]
    public class MemoryRepoTest : TestBase
    {
        private IRepository _abcRepo;
        private IConfiguration _config;

        public MemoryRepoTest()
        {
           _abcRepo = SetupABCDBRepo();
           _config = GetConfigSetings();
        }


        private void AddFakeDataToRepo()
        { 

            _abcRepo.Insert<Tweet>(new List<Tweet>
            {
                new Tweet{TweetContent=@"This is a tweet test. #Awesome ", ChirpId=Guid.NewGuid(), ReceivedDate=DateTime.Now},
                 new Tweet{TweetContent=@"This is a another tweet test. #Awesome ", ChirpId=Guid.NewGuid(), ReceivedDate=DateTime.Now}
            });

            _abcRepo.Insert<Emoji>(new List<Emoji>
            {
                new Emoji{ EmojiNonQualified = ":-)", EmojiId=Guid.NewGuid(), EmojiUnified="1" },
                 new Emoji{EmojiNonQualified = ":-O", EmojiId=Guid.NewGuid(), EmojiUnified="2"}
            });

            _abcRepo.Insert<Tweet>(new List<Tweet>
            {
                new Tweet{TweetContent=@"This is a tweet test. :-) #Awesome ", ChirpId=Guid.NewGuid(), ReceivedDate=DateTime.Now},
                 new Tweet{TweetContent=@"This is a another tweet test. :-) #Awesome ", ChirpId=Guid.NewGuid(), ReceivedDate=DateTime.Now}
            });

            _abcRepo.Save();

            _abcRepo.GetAll<Tweet>().ToListAsync<Tweet>().Result.ForEach(item =>  {

                _abcRepo.Insert<TweetUrl>(new List<TweetUrl>
                {
                    new TweetUrl{ ChirpId = item.ChirpId, Domain="cnn.com", IsImage=false, TweetUrlId=Guid.NewGuid()}
                });

                _abcRepo.Insert<TweetEmoji>(new List<TweetEmoji>
                {
                    new TweetEmoji{TweetEmojiId=Guid.NewGuid(), ChirpId=item.ChirpId, EmojiId= _abcRepo.GetAll<Emoji>().First(x=> x.EmojiUnified=="1").EmojiId}
                });


                _abcRepo.Insert<TweetFeather>(new List<TweetFeather>
                {
                    new TweetFeather{ ChirpId=item.ChirpId, FeatherId=Guid.NewGuid(), HasEmoji=true, HasHashTag=true, HasImage=false, HasUrl=true}
                });
            });

            _abcRepo.Save();
        }

        [TestMethod]
        public void TestEmojiResourceFileLoadToRepo()
        { 
            var mgr = new TwitterManager(_abcRepo, _config, Mock.Of<ILogger<TwitterManager>>() );
            mgr.LoadEmojiDataFromFileResource("ABC.Integration.Resources.emoji.json", _abcRepo);
            var count = _abcRepo.GetAll<Emoji>().Count();

            Assert.IsTrue(_abcRepo.GetAll<Emoji>().Count() > 10);
        }

        [TestMethod]
        public void TestEmojiRepoLoad()
        {
            AddFakeDataToRepo();

            var tweetInfo =
                    from tweety in _abcRepo.GetAll<Tweet>()
                    join tweetmoji in _abcRepo.GetAll<TweetEmoji>() on tweety.ChirpId equals tweetmoji.ChirpId
                    select new
                    {
                        ChirpId = tweety.ChirpId, 
                        Moji = tweetmoji.EmojiId
                    };


            Assert.IsTrue(tweetInfo.Any());
            
        }

    }
}
