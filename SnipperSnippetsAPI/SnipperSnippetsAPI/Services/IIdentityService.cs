using SnipperSnippetsAPI.Entities;
using System.Security.Claims;

namespace SnipperSnippetsAPI.Services
{
    public interface IIdentityService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? CheckToken(string token);
    }
    
}
