using Microsoft.AspNetCore.Mvc;
using TwitterMongoDb.Models;
using TwitterMongoDb.Services;

namespace TwitterMongoDb.Controllers
{
    [ApiController]
    [Route("api/tweet")]
    public class TweetController : Controller
    {
        private readonly TweetsService _tweetsService;
        private readonly UsersService _usersService;

        public TweetController(TweetsService tweetsService,UsersService usersService)
        {
            _tweetsService = tweetsService;
            _usersService = usersService;
        }

        [HttpGet]
        //[Authorize]
        public async Task<List<Tweet>> Get()
        {
            var tweets = await _tweetsService.GetTweetsAsync();
            var users = await _usersService.GetUsersAsync();

            tweets.ForEach(tweet =>
            {
                var user = users.Find(item => item.userId == tweet.userId);
                if (user != null)
                {
                    tweet.tweetUsername = user.username; // Varsayılan olarak username dizesini al
                }
            });
            return tweets;
        }

        [HttpGet("tweetsWithUser")]
        //[Authorize]
        public async Task<ActionResult<List<UserWithTweets>>> GetTweetsWithUser()
        {
            try
            {
                var tweets = await _tweetsService.GetUserTweets();
                return Ok(tweets); // 200 OK yanıtı ile veriyi döndürün
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // 500 Internal Server Error ile hata yanıtı döndürün
            }
        }

        [HttpGet("{id:length(24)}")]
        //[Authorize]
        public async Task<ActionResult<Tweet>> Get(string id)
        {
            var tweet = await _tweetsService.GetTweetAsync(id);

            if (tweet is null)
            {
                return NotFound();
            }
            return tweet;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Tweet newTweet)
        {
            var existUser=await _usersService.GetUserAsync(newTweet.userId);

            if (existUser == null)
            {
                return Unauthorized("UserId geçerli değil!");
            }
            else
            {
                await _tweetsService.CreateTweetAsync(newTweet);
                return CreatedAtAction(nameof(Get), new { id = newTweet.tweetId }, newTweet);
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Tweet updatedTweet)
        {
            var tweet = await _tweetsService.GetTweetAsync(id);
            var user = await _usersService.GetAsyncUsername(tweet.tweetUsername);

            if (tweet is null)
            {
                return NotFound();
            }
            if (user!=null || user?.role == "admin")
            {
                updatedTweet.tweetId = tweet.tweetId;

                await _tweetsService.UpdateTweetAsync(id, updatedTweet);

                return Ok(id + " nolu tweet değiştirildi!");
            }
            return Unauthorized("Tweet sana ait değil değiştiremezsin!");
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id,string tweetUsername)
        {
            var tweet = await _tweetsService.GetTweetAsync(id);
            var user=await _usersService.GetAsyncUsername(tweetUsername);

            if (tweet is null)
            {
                return NotFound();
            }
            if (tweet.tweetUsername.Equals(tweetUsername) || user?.role=="admin")
            {
                await _tweetsService.RemoveTweetAsync(id);

                return Ok(id + " nolu tweet silindi!");
            }
        return Unauthorized("Tweet sana ait değil silenemezsin!");
        }
    }

}
