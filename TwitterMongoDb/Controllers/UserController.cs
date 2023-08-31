using Amazon.Runtime;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using TwitterMongoDb.Models;
using TwitterMongoDb.Services;

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
            var existUser=await _usersService.GetAsyncUsername(newUser.username);
            if(existUser is null) {
                await _usersService.CreateAsync(newUser);
                return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
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
                    var expirationMinutes = 6000; // Token süresini dakika cinsinden buraya ekleyin

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.username), // Kullanıcı adını buraya ekleyin
                        new Claim(ClaimTypes.Role, existUser.role), // Kullanıcının rolünü buraya ekleyin
                    };
                    GenerateToken token1=new GenerateToken();
                    var token = token1.GenerateJwtToken(secretKey,expirationMinutes, claims);
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = true, // Tarayıcı tarafından erişilemez
                        SameSite = SameSiteMode.Strict, // Güvenlik açısından daha katı
                        Expires = DateTime.UtcNow.AddMinutes(expirationMinutes), // Çerez süresi
                                                                                 // Diğer çerez seçenekleri
                    });
                    return Ok("Giriş başarılı!");
                }
            }
            
            return Unauthorized("Giriş başarısız!");
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

            return Ok(id +" nolu user silindi!");
        }
    }
}
