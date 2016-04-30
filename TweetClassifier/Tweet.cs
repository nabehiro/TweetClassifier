using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier
{
    public class Tweet
    {
        public ulong Id { get; set; }
        public string Text { get; set; }
        public string TextOriginal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
