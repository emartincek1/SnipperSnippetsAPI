using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Repositories.Contracts;

namespace SnipperSnippetsAPI.Controllers
{
    [Route("api/Snippet")]
    [ApiController]
    public class SnippetsController : ControllerBase
    {
        private readonly ISnipperSnippetRepository snipperSnippetRepository;

        public SnippetsController(ISnipperSnippetRepository snipperSnippetRepository)
        {
            this.snipperSnippetRepository = snipperSnippetRepository;
        }

        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Snippet>>> GetSnippetsByLanguage([FromQuery] string? language)
        {
            try
            {
                if (language != null)
                {
                    var snippets = await snipperSnippetRepository.GetSnippetsByLanguage(language);

                    if (snippets == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(snippets);
                    }
                } 
                else
                {
                    var snippets = await snipperSnippetRepository.GetSnippets();

                    if (snippets == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(snippets);
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                "Error retrieving data from the database");
            }
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Snippet>> GetSnippet(int id)
        {
            try
            {
                var snippet = await snipperSnippetRepository.GetSnippet(id);

                if (snippet == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(snippet);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                               "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Snippet>> PostSnippet([FromBody] Snippet snippet)
        {
            try
            {
                var addedSnippet = await snipperSnippetRepository.AddSnippet(snippet.language, snippet.code);

                if (addedSnippet == null)
                {
                    return NoContent();
                }

                return CreatedAtAction(nameof(GetSnippet), new { id = addedSnippet.id }, addedSnippet);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                                               "Error adding data to the database");
            }
        }
    }
}
