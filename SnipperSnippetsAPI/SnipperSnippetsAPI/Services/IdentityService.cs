using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SnipperSnippetsAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly JwtSettings? _jwtSettings;

        public IdentityService(IOptions<JwtSettings> jwtSettings ) 
        { 
            _jwtSettings = jwtSettings.Value;
        }

        public ClaimsPrincipal? CheckToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (_jwtSettings is null || _jwtSettings.Secret is null)
            {
                throw new ApplicationException("JwtSettings.Secret is null. Ensure it is set in configuration.");
            }
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                // Validate the token
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                // If successful, return a ClaimsPrincipal containing the claims from the token
                return claims;
            }
            catch (SecurityTokenValidationException)
            {
                // Token validation failed
                return null;
            }
            catch (ArgumentException)
            {
                // The token was not well-formed
                return null;
            }
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (_jwtSettings is null || _jwtSettings.Secret is null)
            {
                throw new ApplicationException("JwtSettings.Secret is null. Ensure it is set in configuration.");
            }
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(ClaimTypes.Email, user.email),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
