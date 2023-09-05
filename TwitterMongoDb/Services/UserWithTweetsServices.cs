using MongoDB.Bson;
using MongoDB.Driver;

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

            public void CreateView()
            {
            var pipeline = new BsonDocument[]
            {
                // İlk aşama: 'tweets' koleksiyonunu 'users' koleksiyonuyla birleştirme
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "users" },
                        { "localField", "userId" }, // 'users' koleksiyonundaki alan
                        { "foreignField", "userId" }, // 'tweets' koleksiyonundaki alan
                        { "as", "userTweets" } // Birleştirilmiş koleksiyonun adı
                    }
                ),
                // İkinci aşama: İstediğiniz alanları seçme
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 0 }, // _id alanını hariç tut
                        { "userId", 1 }, // userId alanını dahil et
                        { "username", 1 }, // userName alanını dahil et
                        { "userTweets.tweetText", 1 } // userTweets içindeki tweetText alanını dahil et
                    }
                )
            };

            var command = new BsonDocument
            {
                { "create", "userWithTweetCollection" }, // Görünümün adı
                { "viewOn", "users" }, // Hangi koleksiyon üstünde oluşturulacak
                { "pipeline", new BsonArray(pipeline) } // Tanımlanan aggregation pipeline
            };

                _mongoDatabase.RunCommand<BsonDocument>(command);
            }
        }
}
