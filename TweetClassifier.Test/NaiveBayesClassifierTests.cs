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
    public class NaiveBayesClassifierTests
    {
        [TestMethod()]
        public void TrainTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ClassifyTest()
        {
            var classifier = new NaiveBayesClassifier();

            var trainingData = new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("Food", new[] { "apple", "orange" }),
                new Tuple<string, IEnumerable<string>>("Food", new[] { "apple", "cake", "banana" }),

                new Tuple<string, IEnumerable<string>>("Animal", new[] { "cat", "dog" }),
                new Tuple<string, IEnumerable<string>>("Animal", new[] { "bird", "cat", "apple" }),
            };

            classifier.Train(trainingData);

            var category = classifier.Classify(new[] { "dog" });
            Assert.AreEqual("Animal", category);

            category = classifier.Classify(new[] { "apple", "banana" });
            Assert.AreEqual("Food", category);
        }
    }
}