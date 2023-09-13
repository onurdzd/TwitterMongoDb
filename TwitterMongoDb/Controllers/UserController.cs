using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitterMongoDb.Models;
using TwitterMongoDb.Models.Authentication;
using TwitterMongoDb.Services;
using static System.Net.Mime.MediaTypeNames;
//authentication program.cs içerisine yazıldı ancak postman da bir türlü authenticate olunamadı.ya postmanda token doğru yere yazılmıyor yada server side authentication problemi var
namespace TwitterMongoDb.Controllers

{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        private readonly UsersService _usersService;

        public UserController(UsersService usersService) {
            _usersService = usersService;
        }


        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<List<User>> Get() =>
        await _usersService.GetUsersAsync();

        [HttpGet("{id:length(24)}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var user = await _usersService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<IActionResult> Post(User newUser)
        {
            var existUser=await _usersService.GetAsyncUsername(newUser.username);
            if(existUser is null) {
                if (newUser.role?.ToLower() != "admin") {
                await _usersService.CreateUserAsync(newUser);
                return CreatedAtAction(nameof(Get), new { id = newUser.userId }, newUser);
                }
                else
                {
                    return Unauthorized("Admin kullanıcı yaratma yetkin yok!");
                }
            }
            else
            {
                return Unauthorized("Username geçersiz");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostLogin(UserLogin user)
        {
            var existUser = await _usersService.GetAsyncUsername(user.username);
            if (existUser == null)
            {
                return Unauthorized("Username geçersiz");
            }
            else
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.password, existUser.password);
                if (isPasswordValid)
                {
                    var tokenString = JwtMiddleWares.GenerateToken(existUser);

                    return Ok(new { token = tokenString, id = existUser.userId, name = existUser.name, username = existUser.username, message = "Giriş başarılı!" });
                }
            }

            return Unauthorized("Giriş başarısız!");
        }

        [HttpPut("{id:length(24)}")]
        //[Authorize]
        public async Task<IActionResult> Update(string id, User updatedUser)
        {
            var user = await _usersService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound();
            }
            if (user?.role == "admin")
            {
                updatedUser.userId = user.userId;

                await _usersService.UpdateUserAsync(id, updatedUser);

                return Ok(id + " nolu user bilgileri güncellendi!");
            }
            return Unauthorized("Kullanıcı düzenleme yetkin yok!");
        }

        [HttpDelete("{id:length(24)}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _usersService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            if (user?.role == "admin")
            {
                await _usersService.RemoveUserAsync(id);

                return Ok(id + " nolu user silindi!");
            }
            return Unauthorized("Kullanıcı silme yetkin yok!");
        }
    }
}
