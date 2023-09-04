using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TwitterMongoDb.Models.Authentication
{
    public class UserLogin
    {
        [BsonElement("username")]
        [Required(ErrorMessage = "Username zorunludur!")]
        public string username { get; set; }
        [BsonElement("password")]
        [Required(ErrorMessage = "Şifre zorunludur!")]
        public string password { get; set; }
    }
}
