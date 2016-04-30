using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier
{
    public class Token
    {
        public List<string> Nouns { get; set; } = new List<string>();
        public List<string> Verbs { get; set; } = new List<string>();
    }
}
