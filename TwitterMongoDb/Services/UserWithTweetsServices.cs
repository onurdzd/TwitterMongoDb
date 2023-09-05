using MongoDB.Bson;
using MongoDB.Driver;
using TwitterMongoDb.Models;

namespace TwitterMongoDb.Services
{
    public class UserWithTweetsServices
    {
        //join ile birleşim yapmak için oluşturudlu
            private readonly IMongoDatabase _mongoDatabase;

            public UserWithTweetsServices(IMongoDatabase mongoDatabase)
            {
                _mongoDatabase = mongoDatabase;
            }

            public static List<UserWithTweet> CreateView(IMongoCollection<User> userCollection)
            {
            var pipeline = new BsonDocument[]
                 {
                    // İlk aşama: User koleksiyonundan kullanıcıları al
                    new BsonDocument("$lookup",
                        new BsonDocument
                        {
                            { "from", "tweets" }, // tweets koleksiyonu
                            { "localField", "_id" }, // User koleksiyonundaki userId
                            { "foreignField", "userId" }, // Tweet koleksiyonundaki userId
                            { "as", "userTweets" } // Sonucu userTweets alanına sakla
                        }
                    ),
                    // İkinci aşama: User koleksiyonundaki alanları yeniden düzenle
                    new BsonDocument("$project",
                        new BsonDocument
                        {
                            { "userId", 1 },
                            { "username", 1 },
                            { "userTweets.tweetText", 1 } // userTweets içindeki tweetText alanını seç
                        }
                    ),
                    //// Üçüncü aşama: userTweets alanını bir liste olarak düzenle
                    new BsonDocument("$group",
                        new BsonDocument
                        {
                            { "_id", "$userId" },
                            { "username", new BsonDocument("$first", "$username") }, // username alanını koru
                            { "userTweets", new BsonDocument("$push", "$userTweets.tweetText") } // userTweets içindeki tweetText alanlarını bir listeye ekle
                        }
                    ),
                    //// Son aşama: UserWithTweet sınıfını oluştur
                    new BsonDocument("$project",
                        new BsonDocument
                        {
                            { "_id", 0 }, // _id alanını kaldır
                            { "userId", "$_id" }, // _id alanını userId olarak değiştir
                            { "username", 1 },
                            { "userTweets", 1 }
                        }
                    )
                 };

            var result = userCollection.Aggregate<UserWithTweet>(pipeline).ToList();
            return result;
        }
        }
}
