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
                    tweet.tweetUsername = user.username; // Varsayılan olarak username dizesini alın
                }
            });

            return tweets;
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
        //[Authorize]
        public async Task<IActionResult> Update(string id, Tweet updatedTweet)
        {
            var tweet = await _tweetsService.GetTweetAsync(id);

            if (tweet is null)
            {
                return NotFound();
            }

            updatedTweet.tweetId = tweet.tweetId;

            await _tweetsService.UpdateTweetAsync(id, updatedTweet);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var tweet = await _tweetsService.GetTweetAsync(id);

            if (tweet is null)
            {
                return NotFound();
            }

            await _tweetsService.RemoveTweetAsync(id);

            return Ok(id + " nolu tweet silindi!");
        }
    }
}
