using MongoDB.Bson.Serialization.Attributes;

namespace TwitterMongoDb.Models
{
    public class UserLogin
    {
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("password")]
        public string password { get; set; }
    }
}
