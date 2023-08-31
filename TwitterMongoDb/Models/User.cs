using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Identity;

namespace TwitterMongoDb.Models
{
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("password")]
        public string password { get; set; }

        [BsonElement("tweets")]
        public string tweets { get; set; }
        [BsonElement("likes")]
        public string likes { get; set; }
        [BsonElement("retweets")]
        public string retweets { get; set; }
        [BsonElement("role")]
        public string role { get; set; }

    }
}
