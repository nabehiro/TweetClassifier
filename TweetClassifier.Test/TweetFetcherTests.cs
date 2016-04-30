using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetClassifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier.Tests
{
    [TestClass()]
    public class TweetFetcherTests
    {
        [TestMethod()]
        public void GetTimelineTest()
        {
            var fetcher = new TweetFetcher();
            var tweets = fetcher.GetTimeline("nabehiro_").Result;
            Assert.IsTrue(tweets.Count > 980 && tweets.Count < 1100);

    }
}