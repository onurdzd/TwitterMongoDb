using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TwitterMongoDb.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;

namespace TwitterMongoDb.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService(
            IOptions<UsersStoreDatabaseSettings> userStoreDatabaseSettings)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var mongoClient = new MongoClient(
                config["ConnectionString"]);

            var mongoDatabase = mongoClient.GetDatabase(
                config["DatabaseName"]);

            _usersCollection = mongoDatabase.GetCollection<User>(
                config["UsersCollectionName"]);
        }

        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<User?> GetAsync(string id) =>
            await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(User newUser) =>
            await _usersCollection.InsertOneAsync(newUser);

        public async Task UpdateAsync(string id, User updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }
}
