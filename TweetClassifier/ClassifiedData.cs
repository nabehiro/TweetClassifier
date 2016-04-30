using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier
{
    public class ClassifiedItem
    {
        public Tweet Tweet { get; set; }
        public Token Token { get; set; }
        public string Category { get; set; }
    }
}
