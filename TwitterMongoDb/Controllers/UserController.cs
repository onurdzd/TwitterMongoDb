using Amazon.Runtime;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitterMongoDb.Models;
using TwitterMongoDb.Services;

namespace TwitterMongoDb.Controllers

{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
      //jwt token sanırım oluşturdu.ancak front end den buraya jwt nasıl gönderişlir her seferinde yeniden token checkmi yapılacak çözemedim..

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
                    var tokenExpiration = DateTime.UtcNow.AddHours(24); // Örnek olarak 1 saatlik bir süre ekleyin
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.username),
                            // Diğer isteğe bağlı iddia bilgilerini ekleyin
                            new Claim(ClaimTypes.Role, existUser.role),

                        };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(secretKey); // Tokeni şifrelemek için kullanılan gizli anahtar
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = tokenExpiration,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(securityToken);

                    // JWT tokenini bir HTTP Cookie olarak ekleyin
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
