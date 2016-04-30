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
    public class TokenizerTests
    {
        [TestMethod()]
        public void GetTokenTest()
        {
            var tokenizer = new Tokenizer();
            var token = tokenizer.GetToken("C#って中々良い言語なんで勉強しよう。");
            Assert.AreEqual(3, token.Nouns.Count);  // C#, 言語, 勉強
            Assert.AreEqual(1, token.Verbs.Count);  // する
        }
    }
}