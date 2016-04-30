using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = MainAsync();
            task.Wait();

            Console.Write("\nPress any key to close console window...");
            Console.ReadKey(true);
        }

        static async Task MainAsync()
        {
            Console.Write("Input Twitter user name: ");
            var userName = Console.ReadLine();
            if (string.IsNullOrEmpty(userName))
                throw new InvalidOperationException();

            List<ClassifiedItem> items = await GetItems(userName);

            Console.WriteLine("Input Categories(split comma)");
            Console.Write(": ");
            var categoriesText = Console.ReadLine();
            var categories = categoriesText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (categories.Length == 0)
                throw new InvalidOperationException();

            Console.WriteLine("Start training...");
            var classifier = new NaiveBayesClassifier();
            Train(items, categories, classifier);

            Console.WriteLine("Classifing...");
            foreach (var item in items)
            {
                item.Category = classifier.Classify(item.Token.Nouns);
            }

            Console.WriteLine("Classified.");

            foreach (var g in items.GroupBy(i => i.Category))
            {
                Console.WriteLine($"{g.Key}: {g.Count()}");
            }

            SaveData(items, $"{userName}.json");
        }

        private static void Train(List<ClassifiedItem> items, string[] categories, NaiveBayesClassifier classifier)
        {
            var question = "Which category(-2:Quit, -1:Skip)? " +
                            string.Join(", ", categories.Select((c, i) => $"{i}:{c}"));
            var random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            var trainingData = new List<Tuple<string, IEnumerable<string>>>();
            while (true)
            {
                var item = items[random.Next(items.Count)];
                Console.WriteLine($"[Tweet] {item.Tweet.Text}");
                Console.WriteLine($"[Token] {string.Join(", ", item.Token.Nouns)}");
                Console.WriteLine(question);
                Console.Write(": ");
                var idxText = Console.ReadLine();
                int idx = 0;

                if (int.TryParse(idxText, out idx) && idx >= -2 && idx < categories.Length)
                {
                    if (idx == -2) break;
                    if (idx == -1) continue;

                    trainingData.Add(new Tuple<string, IEnumerable<string>>(categories[idx], item.Token.Nouns));
                }
                else
                {
                    Console.WriteLine("Input is invalid.");
                }
            }

            classifier.Train(trainingData);
        }

        private static async Task<List<ClassifiedItem>> GetItems(string userName)
        {
            var filePath = $"{userName}.json";
            List<ClassifiedItem> items = null;

            if (File.Exists(filePath))
            {
                Console.WriteLine("Loading previous data from local...");

                var json = File.ReadAllText(filePath, Encoding.UTF8);
                items = JsonConvert.DeserializeObject<List<ClassifiedItem>>(json);

                foreach (var item in items)
                    item.Category = null;

                Console.WriteLine("Loaded previous data.");
            }
            else
            {
                Console.WriteLine("Fetching tweets from Twitter API...");

                var fetcher = new TweetFetcher();
                var tweets = await fetcher.GetTimeline(userName);
                items = tweets.Select(t => new ClassifiedItem { Tweet = t }).ToList();

                Console.WriteLine("Fetched tweets from Twitter API.");

                Console.WriteLine("Tokenizing tweets...");

                var tokenizer = new Tokenizer();
                foreach (var item in items)
                    item.Token = tokenizer.GetToken(item.Tweet.Text);

                Console.WriteLine("Tokenized tweets.");

                SaveData(items, filePath);
            }

            return items;
        }

        private static void SaveData(List<ClassifiedItem> items, string filePath)
        {
            Console.WriteLine("Saving data...");

            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(filePath, json, Encoding.UTF8);

            Console.WriteLine("Saved data.");
        }
    }
}
