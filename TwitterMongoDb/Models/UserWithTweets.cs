using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;

namespace TwitterMongoDb.Models
{
    public class UserWithTweets
    {

        [BsonElement("userId")]
        public string? userId { get; set; }
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("userTweets")]
        public List<string> userTweets { get; set; } // userTweets özelliğini List<string> olarak tanımla
    }
}