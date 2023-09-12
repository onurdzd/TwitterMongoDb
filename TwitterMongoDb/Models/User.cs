using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Identity;

namespace TwitterMongoDb.Models
{
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string? userId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("password")]
        public string password { get; set; }
        [BsonElement("role")]
        public string role { get; set; }
        [BsonElement("userCreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime userCreatedAt { get; set; }

    }
}
