using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace ABC.UnitTest
{
    [TestClass]
    public class TweetParseTest : TestBase
    {
        [TestMethod]
        public void TestEmojiDataParse()
        {
            var testdata = @"
                    What good is a product image without a catchy title? 
                    Useless right? 🤔

                    You bet it is.
                    How about we learn how to optimize our Amazon product title? 📈
 
                    #AmazonSeo
                    #AmazonTips
                    #AmazonTitles
                    #SunkenStone https://t.co/SQMEkJFlZR
                    ";

            var x = EmojiData.Emoji.EmojiRegex.Matches(testdata);

            Assert.IsTrue(x != null && x.Count() > 0);
        }
    }
}
