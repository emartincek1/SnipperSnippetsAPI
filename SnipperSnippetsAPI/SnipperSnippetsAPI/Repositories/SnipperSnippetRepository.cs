using Microsoft.EntityFrameworkCore;
using SnipperSnippetsAPI.Data;
using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Repositories.Contracts;

namespace SnipperSnippetsAPI.Repositories
{
    public class SnipperSnippetRepository : ISnipperSnippetRepository
    {
        private SnipperSnippetsDbContext _snipperSnippetsDbContext;

        public SnipperSnippetRepository(SnipperSnippetsDbContext snipperSnippetsDbContext)
        {
            _snipperSnippetsDbContext = snipperSnippetsDbContext;
        }

        public async Task<Snippet> AddSnippet(string language, string code)
        {
            var snippet = new Snippet
            {
                language = language,
                code = code
            };

            if (snippet != null)
            {
                var result = await _snipperSnippetsDbContext.Snippets.AddAsync(snippet);
                await _snipperSnippetsDbContext.SaveChangesAsync();
                return result.Entity;
            } else
            {
                return null;
            }
        }

        public async Task<Snippet> GetSnippet(int id)
        {
            var snippet = await _snipperSnippetsDbContext.Snippets.FindAsync(id);
            return snippet;
        }

        public async Task<IEnumerable<Snippet>> GetSnippets()
        {
            var snippets = await _snipperSnippetsDbContext.Snippets.ToListAsync();
            return snippets;
        }

        public async Task<IEnumerable<Snippet>> GetSnippetsByLanguage(string language)
        {
            var snippets = await (from snippet in _snipperSnippetsDbContext.Snippets
                                  where snippet.language == language
                                  select snippet).ToListAsync();
            return snippets;
        }
    }
}
