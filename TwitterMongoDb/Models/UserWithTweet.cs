namespace TwitterMongoDb.Models
{
    public class UserWithTweet
    {
        public int userId { get; set; }
        public string username { get; set; }
        public List<string> userTweets { get; set; }
    }
}
