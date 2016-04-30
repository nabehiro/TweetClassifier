using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier
{
    /// <summary>
    /// ナイーブベイズを用いたテキスト分類 - 人工知能に関する断創録 http://aidiary.hatenablog.com/entry/20100613/1276389337
    /// 僕はもう、そんなにナイーブじゃないんだ - Qiita http://qiita.com/cou_z/items/bca93fce0a08b521a3e8
    /// </summary>
    public class NaiveBayesClassifier
    {
        // 全カテゴリ
        private HashSet<string> _categories = new HashSet<string>();
        // 全単語
        private HashSet<string> _words = new HashSet<string>();
        // カテゴリでの単語の出現回数 _categoryWordCount["IT"]["C#"] = (count)
        private Dictionary<string, Dictionary<string, int>> _categoryWordCounts = new Dictionary<string, Dictionary<string, int>>();
        // カテゴリの出現回数
        private Dictionary<string, int> _categoryCounts = new Dictionary<string, int>();
        // P(word|category)の分母の値
        private Dictionary<string, int> _denominators = new Dictionary<string, int>();
        // 総文書数
        private int _docCount;


        // categoryWords =
        // [
        //   { Item1: "IT", Item2: ["C#", "ASP.NET"] },
        //   { Item1: "IT", Item2: ["Javascript", "ASP.NET"] },
        // ]
        public void Train(List<Tuple<string, IEnumerable<string>>> categoryWords)
        {
            foreach (var t in categoryWords)
                Train(t.Item1, t.Item2);

            // P(word|category) を事前計算（高速化のため）
            foreach (var category in _categories)
                _denominators[category] = _categoryWordCounts[category].Values.Sum() + _words.Count;

            // 総文書数
            _docCount = _categoryCounts.Values.Sum();
        }

        private void Train(string category, IEnumerable<string> words)
        {
            _categories.Add(category);

            if (_categoryCounts.ContainsKey(category))
                _categoryCounts[category] += 1;
            else
            {
                _categoryCounts[category] = 1;
                _categoryWordCounts[category] = new Dictionary<string, int>();
            }

            foreach (var word in words)
            {
                _words.Add(word);

                if (_categoryWordCounts[category].ContainsKey(word))
                    _categoryWordCounts[category][word] += 1;
                else
                    _categoryWordCounts[category][word] = 1;
            }
        }


        public string Classify(IEnumerable<string> words)
        {
            string best = null;
            double max = -double.MaxValue;

            foreach (var category in _categories)
            {
                var score = CalcScore(words, category);
                if (score > max)
                {
                    max = score;
                    best = category;
                }
            }

            return best;
        }

        // P(word|cat) を求める。ラプラススムージング適用
        private double CalcWordProbability(string word, string category)
        {
            var words = _categoryWordCounts[category];
            return (double)((words.ContainsKey(word) ? words[word] : 0) + 1) / _denominators[category];
        }

        // log( P(cat|doc) ) を求める
        private double CalcScore(IEnumerable<string> words, string category)
        {
            var score = Math.Log((double)_categoryCounts[category] / _docCount);    // log P(cat)

            foreach (var word in words)
                score += Math.Log(CalcWordProbability(word, category)); // log P(word|cat)

            return score;
        }

    }
}
