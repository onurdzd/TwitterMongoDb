﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TwitterMongoDb.Models
{
    public class Tweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("tweetId")]
        public string? tweetId { get; set; }

        [BsonElement("userId")]
        public string userId { get; set; }

        [BsonElement("tweetUsername")]
        public string tweetUsername { get; set; }

        [BsonElement("tweetText")]
        public string tweetText { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime tweetCreatedAt { get; set; }
    }
}
