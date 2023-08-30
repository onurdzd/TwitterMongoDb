using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TwitterMongoDb.Models;

namespace TwitterMongoDb.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _booksCollection;

        public UsersService(
            IOptions<UsersStoreDatabaseSettings> userStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                userStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                userStoreDatabaseSettings.Value.DatabaseName);

            _booksCollection = mongoDatabase.GetCollection<User>(
                userStoreDatabaseSettings.Value.UsersCollectionName);
        }

        public async Task<List<User>> GetAsync() =>
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<User?> GetAsync(string id) =>
            await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(User newUser) =>
            await _booksCollection.InsertOneAsync(newUser);

        public async Task UpdateAsync(string id, User updatedUser) =>
            await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task RemoveAsync(string id) =>
            await _booksCollection.DeleteOneAsync(x => x.Id == id);
    }
}
