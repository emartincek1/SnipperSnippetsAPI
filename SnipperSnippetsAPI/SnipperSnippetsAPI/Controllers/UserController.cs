using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SnipperSnippetsAPI.Entities;

namespace SnipperSnippetsAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public ActionResult<User> CreateUser()
        {
            User? user = HttpContext.Items["User"] as User;

            if (user == null)
            {
                return BadRequest();
            }

            return Ok(new {Id = user.id, Email = user.email});
        }

        [HttpGet]
        public ActionResult<User> GetUser()
        {
            User? user = HttpContext.Items["User"] as User;

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new { Id = user.id, Email = user.email });
        }
    }
}
 