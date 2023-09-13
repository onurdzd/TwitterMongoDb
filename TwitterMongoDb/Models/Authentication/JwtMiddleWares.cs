using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitterMongoDb.Services;

namespace TwitterMongoDb.Models.Authentication { 

public class JwtMiddleWares
{
        public static string GenerateToken(User existUser)
        {
            var config = new ConfigurationBuilder()
                         .AddUserSecrets<Program>()
                         .Build();

            // JWT üretme
            var secretKey = config["secretKey"]; // Özel anahtarınızı buraya ekleyin
            var tokenExpiration = DateTime.UtcNow.AddHours(24); // Örnek olarak 24 saatlik bir süre ekleyin
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
        new Claim(ClaimTypes.Role, existUser.role),
    };
            var token = new JwtSecurityToken(config["iss"], config["aud"], claims, expires: tokenExpiration, signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString; // JSON yanıtını döndür
        }

        public static bool? ValidateToken(string token)
    {
        if (token == null)
            return false;
            var config = new ConfigurationBuilder()
                        .AddUserSecrets<Program>()
                        .Build();
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(config["secretKey"]);
      

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidIssuer = config["iss"],
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["secretKey"])),
        };

        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            return true;
        }
        catch (SecurityTokenException)
        {
            // Token validation failed
            return false;
        }
        catch (Exception ex)
        {
            // Other exceptions (e.g., parsing errors)
            // Log the exception for debugging purposes
            // Return null or throw a custom exception as needed
            return false;
        }
    }
} }
