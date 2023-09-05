using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;

namespace TwitterMongoDb.Models
{
    public class UserWithTweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("userTweets")]
        public List<ArrayList> userTweets { get; set; } // userTweets özelliğini List<string> olarak tanımla
    }
}