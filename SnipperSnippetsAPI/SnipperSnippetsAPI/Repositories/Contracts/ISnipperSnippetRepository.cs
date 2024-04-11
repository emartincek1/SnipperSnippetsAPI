using SnipperSnippetsAPI.Entities;

namespace SnipperSnippetsAPI.Repositories.Contracts
{
    public interface ISnipperSnippetRepository
    {
        Task<Snippet> AddSnippet(string language, string code);
        Task<IEnumerable<Snippet>> GetSnippets();
        Task<Snippet> GetSnippet(int id);
        Task<IEnumerable<Snippet>> GetSnippetsByLanguage(string language);
    }
}
