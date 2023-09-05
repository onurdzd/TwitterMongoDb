using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TwitterMongoDb.Models;

namespace TwitterMongoDb.Services
{
    public class TweetsService
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;

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
                     
        }


        public async Task<List<Tweet>> GetTweetsAsync() =>
            await _tweetsCollection.Find(_ => true).ToListAsync();

        public async Task<Tweet?> GetTweetAsync(string id) =>
            await _tweetsCollection.Find(x => x.tweetId == id).FirstOrDefaultAsync();

        public async Task CreateTweetAsync(Tweet newTweet)
        {
            // Veritabanına yeni kullanıcıyı ekleyin
            await _tweetsCollection.InsertOneAsync(newTweet);
        }

        public async Task UpdateTweetAsync(string id, Tweet updatedTweet) =>
            await _tweetsCollection.ReplaceOneAsync(x => x.tweetId == id, updatedTweet);

        public async Task RemoveTweetAsync(string id) =>
            await _tweetsCollection.DeleteOneAsync(x => x.tweetId == id);

    }

}

