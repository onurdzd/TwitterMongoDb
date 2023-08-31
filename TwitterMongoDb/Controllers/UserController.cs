using Microsoft.AspNetCore.Mvc;
using TwitterMongoDb.Models;
using TwitterMongoDb.Services;

namespace TwitterMongoDb.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UserController(UsersService usersService) =>
            _usersService = usersService;

        [HttpGet]
        public async Task<List<User>> Get() =>
            await _usersService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var user = await _usersService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<IActionResult> Post(User newUser)
        {
            var newUsername=await _usersService.GetAsyncUsername(newUser.username);
            if(newUsername is null) {
                await _usersService.CreateAsync(newUser);
                return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
            }
            else
            {
                return BadRequest("Username geçersiz");
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, User updatedUser)
        {
            var user = await _usersService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            updatedUser.Id = user.Id;

            await _usersService.UpdateAsync(id, updatedUser);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _usersService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            await _usersService.RemoveAsync(id);

            return NoContent();
        }
    }
}
