using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TwitterMongoDb.Models
{
    public class UserWithTweets
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string userId { get; set; }

        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("userTweets")]
        public Tweet[] userTweets { get; set; }
    }
}