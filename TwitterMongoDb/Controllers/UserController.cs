using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitterMongoDb.Models;
using TwitterMongoDb.Models.Authentication;
using TwitterMongoDb.Services;
//authentication program.cs içerisine yazıldı ancak postman da bir türlü authenticate olunamadı.ya postmanda token doğru yere yazılmıyor yada server side authentication problemi var
namespace TwitterMongoDb.Controllers

{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        private readonly UsersService _usersService;

        public UserController(UsersService usersService) =>
            _usersService = usersService;

        [HttpGet]
        //[Authorize]
        public async Task<List<User>> Get() =>
            await _usersService.GetUsersAsync();

        [HttpGet("{id:length(24)}")]
        //[Authorize]
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
                await _usersService.CreateUserAsync(newUser);
                return CreatedAtAction(nameof(Get), new { id = newUser.userId }, newUser);
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
            if (existUser==null)
            {
                return Unauthorized("Username geçersiz");
            }
            else
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.password, existUser.password);
                if (isPasswordValid)
                {
                    var config = new ConfigurationBuilder()
                        .AddUserSecrets<Program>()
                        .Build();

                    var secretKey = config["secretKey"]; // Özel anahtarınızı buraya ekleyin
                    var tokenExpiration = DateTime.UtcNow.AddHours(24); // Örnek olarak 1 saatlik bir süre ekleyin
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new[]
                    {
                       new Claim(ClaimTypes.Role, existUser.role),
                    };
                    var token = new JwtSecurityToken(config["iss"], config["aud"], claims, expires: tokenExpiration, signingCredentials: creds);
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    //Cookie olarak eklenmek istenirse:
                    Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = tokenExpiration,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        // Diğer gerekli cookie seçeneklerini ayarlayın
                    });

                    return Ok(new { token = tokenString, message = "Giriş başarılı!" });
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

            updatedUser.userId = user.userId;

            await _usersService.UpdateUserAsync(id, updatedUser);

            return NoContent();
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

            await _usersService.RemoveUserAsync(id);

            return Ok(id +" nolu user silindi!");
        }
    }
}
