using NMeCab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClassifier
{
    public class Tokenizer
    {
        private MeCabTagger mecab = MeCabTagger.Create();

        public Token GetToken(string sentence)
        {
            var token = new Token();

            var node = mecab.ParseToNode(sentence);
            while (node != null)
            {
                if (node.Feature.StartsWith("名詞"))
                {
                    token.Nouns.Add(node.Surface);
                }
                else if (node.Feature.StartsWith("動詞"))
                {
                    var features = node.Feature.Split(',');
                    var verb = features[features.Length - 3];
                    token.Verbs.Add(verb);
                }

                node = node.Next;
            }

            return token;
        }

        
    }
}
