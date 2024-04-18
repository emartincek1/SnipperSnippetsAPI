using Microsoft.EntityFrameworkCore;
using SnipperSnippetsAPI.Data;
using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Repositories.Contracts;
using SnipperSnippetsAPI.Utilities;

namespace SnipperSnippetsAPI.Repositories
{
    public class SnipperSnippetRepository : ISnipperSnippetRepository
    {
        private SnipperSnippetsDbContext _snipperSnippetsDbContext;

        private EncryptUtility _encryptUtility;

        public SnipperSnippetRepository(SnipperSnippetsDbContext snipperSnippetsDbContext, EncryptUtility encryptUtility)
        {
            _snipperSnippetsDbContext = snipperSnippetsDbContext;

            _encryptUtility = encryptUtility;
        }

        public async Task<Snippet?> AddSnippet(Snippet snippet)
        {
            try
            {
                if (snippet != null)
                {
                    snippet.id = 0;
                    snippet.code = _encryptUtility.Encrypt(snippet.code);
                    var result = await _snipperSnippetsDbContext.Snippets.AddAsync(snippet);
                    await _snipperSnippetsDbContext.SaveChangesAsync();
                    return result.Entity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Snippet> GetSnippet(int id)
        {
            try
            {
                var snippet = await _snipperSnippetsDbContext.Snippets.FindAsync(id);
                if (snippet != null)
                {
                    snippet.code = _encryptUtility.Decrypt(snippet.code);
                }
                return snippet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Snippet>> GetSnippets()
        {
            try
            {
                var snippets = await _snipperSnippetsDbContext.Snippets.ToListAsync();

                var decodedSnippets = new List<Snippet>();

                if (snippets != null)
                {
                    decodedSnippets = snippets.ConvertAll<Snippet>(snippet => new Snippet
                    {
                        id = snippet.id,
                        language = snippet.language,
                        code = _encryptUtility.Decrypt(snippet.code)
                    });
                }

                return decodedSnippets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Snippet>?> GetSnippetsByLanguage(string language)
        {
            try
            {
                var snippets = await (from snippet in _snipperSnippetsDbContext.Snippets
                                      where snippet.language == language
                                      select snippet).ToListAsync() ?? null;

                var decodedSnippets = new List<Snippet>();

                if (snippets != null)
                {
                    decodedSnippets = snippets.ConvertAll<Snippet>(snippet => new Snippet
                    {
                        id = snippet.id,
                        language = snippet.language,
                        code = _encryptUtility.Decrypt(snippet.code)
                    });
                }

                return decodedSnippets;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
