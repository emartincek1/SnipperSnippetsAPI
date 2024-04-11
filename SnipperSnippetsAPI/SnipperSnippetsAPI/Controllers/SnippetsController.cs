using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnipperSnippetsAPI.Repositories.Contracts;

namespace SnipperSnippetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnippetsController : ControllerBase
    {
        private readonly ISnipperSnippetRepository snipperSnippetRepository;

        public SnippetsController(ISnipperSnippetRepository snipperSnippetRepository)
        {
            this.snipperSnippetRepository = snipperSnippetRepository;
        }
    }
}
