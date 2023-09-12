using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TwitterMongoDb.Models;

namespace TwitterMongoDb.Services
{
    public class TweetsService
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;
        private readonly IMongoCollection<User> _usersCollection;

        public TweetsService(
            IOptions<UsersStoreDatabaseSettings> userStoreDatabaseSettings)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var mongoClient = new MongoClient(
                config["ConnectionString"]);

            var mongoDatabase = mongoClient.GetDatabase(
                config["DatabaseName"]);

            _tweetsCollection = mongoDatabase.GetCollection<Tweet>(
                config["TweetsCollectionName"]);
            _usersCollection = mongoDatabase.GetCollection<User>(
                config["UsersCollectionName"]); ;
        }


        public async Task<List<Tweet>> GetTweetsAsync() =>
            await _tweetsCollection.Find(_ => true).ToListAsync();

        public async Task<Tweet?> GetTweetAsync(string id) =>
            await _tweetsCollection.Find(x => x.tweetId == id).FirstOrDefaultAsync();

        public async Task CreateTweetAsync(Tweet newTweet)
        {
            newTweet.tweetCreatedAt = DateTime.Now;
            await _tweetsCollection.InsertOneAsync(newTweet);
        }

        public async Task UpdateTweetAsync(string id, Tweet updatedTweet) =>
            await _tweetsCollection.ReplaceOneAsync(x => x.tweetId == id, updatedTweet);

        public async Task RemoveTweetAsync(string id) =>
            await _tweetsCollection.DeleteOneAsync(x => x.tweetId == id);

        public async Task<List<UserWithTweets>> GetUserTweets()
        {
            var pipeline = new[]
            {
                // İkinci aşama: Users ve Tweets koleksiyonlarını birleştir
                 new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "tweets" },
                        { "localField", "username" },
                        { "foreignField", "tweetUsername" },
                        { "as", "userTweets" }
                    }),
                new BsonDocument("$project", new BsonDocument
                    {
                           { "_id", 0 },
                           { "userId", "$_id" },
                           { "username", 1 },
                           { "name", 1 },
                           { "userTweets", 1 }
                    })
                };

            var aggregation = _usersCollection.Aggregate<UserWithTweets>(pipeline);
            var result = await aggregation.ToListAsync();

            return result;
        }
    }
}



