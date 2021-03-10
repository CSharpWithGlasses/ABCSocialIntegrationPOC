using ABC.Shared.Interfaces.Repository;
using ABC.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ABC.Integration.Managers;
using Microsoft.Extensions.Logging;
using Moq;
using ABC.Shared.Interfaces;


namespace ABC.UnitTest
{
    [TestClass]
    public class TwitterApiTest : TestBase
    {
        private TwitterManager _twitterManager;
        private IRepository _abcRepo;
        private ISocialStatistics _socialStatistics;

        public TwitterApiTest()
        {
            _abcRepo = SetupABCDBRepo();
            var config = GetConfigSetings();
            var logger = Mock.Of<ILogger<TwitterManager>>();
            var loggerStats = Mock.Of<ILogger<StatisticsManager>>();
            _twitterManager = new TwitterManager(_abcRepo, config, logger);
            _socialStatistics = new StatisticsManager(_abcRepo, config, loggerStats);
        }

        [TestMethod]
        public void GetTweetTest()
        {

            _twitterManager.StartService();
            var isData = _abcRepo.GetAll<Tweet>().Any();
            var result = _socialStatistics.GetTwitterStatistics(DateTime.Now);

            Assert.IsTrue(isData);
        }
    }
}
