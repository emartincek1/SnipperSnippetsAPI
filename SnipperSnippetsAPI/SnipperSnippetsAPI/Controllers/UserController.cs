using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Models;
using SnipperSnippetsAPI.Services;
using System.Security.Claims;

namespace SnipperSnippetsAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IIdentityService _identityService;

        public UserController(IIdentityService identityService, IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _identityService = identityService;
        }

        [HttpPost]
        public ActionResult<User> CreateUser()
        {
            User? user = HttpContext.Items["User"] as User;

            if (user == null)
            {
                return BadRequest();
            }

            return Ok(new { Id = user.id, Email = user.email });
        }

        [HttpPost("login")]
        public ActionResult Login()
        {
            // Fetch authenticated user from HttpContext Items
            User? user = HttpContext.Items["User"] as User;

            // If null, the authentication failed
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or password." });
            }

            var token = _identityService.GenerateToken(user);

            return Ok(new { token, user.id, user.email });
        }

        [HttpGet]
        [Authorize]
        public ActionResult<User> GetUser()
        {
            // // Retrieving the authenticated User (ClaimsPrincipal) from HttpContext which is populated by JwtMiddleware
            var user = User;

            // If null, the authentication failed
            if (user == null)
            {
                return Unauthorized(new { error = "Couldn't access user data." });
            }

            // Parsing the user's Id and Email from the user claims
            long Id = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            string Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            // Don't send back hashed password
            return Ok(new { Id = Id, Email = Email });
        }
    }
}
 