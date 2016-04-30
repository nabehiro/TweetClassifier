using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TweetClassifier
{
    public class TweetFetcher
    {
        private IAuthorizer _auth = new SingleUserAuthorizer()
        {
            CredentialStore = new SingleUserInMemoryCredentialStore
            {
                ConsumerKey = ConfigurationManager.AppSettings[OAuthKeys.TwitterConsumerKey],
                ConsumerSecret = ConfigurationManager.AppSettings[OAuthKeys.TwitterConsumerSecret],
                AccessToken = ConfigurationManager.AppSettings[OAuthKeys.TwitterAccessToken],
                AccessTokenSecret = ConfigurationManager.AppSettings[OAuthKeys.TwitterAccessTokenSecret]
            },
        };

        public async Task<List<Tweet>> GetTimeline(string userName)
        {
            var ctx = new TwitterContext(_auth);
            var statuses = new List<Status>();

            ulong maxId = 0;
            while (true)
            {
                List<Status> response;
                if (maxId != 0)
                    response = await
                        (from tweet in ctx.Status
                         where tweet.Type == StatusType.User &&
                               tweet.ScreenName == userName &&
                               tweet.MaxID == maxId &&
                               tweet.Count == 200
                         select tweet)
                        .ToListAsync();
                else
                    response = await
                        (from tweet in ctx.Status
                         where tweet.Type == StatusType.User &&
                               tweet.ScreenName == userName &&
                               tweet.Count == 200
                         select tweet)
                        .ToListAsync();

                if (response.Count == 0)
                    break;

                statuses.AddRange(response);
                maxId = response.Last().StatusID - 1;
            }

            return statuses.Select(s =>
            {
                var textOrg = Regex.Replace(s.Text, @"\r\n|\r|\n", " ");
                var text = Regex.Replace(textOrg, @"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+", " ");
                text = Regex.Replace(text, @"RT @[^\s]+", " ");
                text = Regex.Replace(text, @"@[^\s]+", " ");

                return new Tweet
                {
                    Id = s.StatusID,
                    TextOriginal = textOrg,
                    Text = text,
                    CreatedAt = s.CreatedAt
                };
            }).ToList();
        }
    }
}
