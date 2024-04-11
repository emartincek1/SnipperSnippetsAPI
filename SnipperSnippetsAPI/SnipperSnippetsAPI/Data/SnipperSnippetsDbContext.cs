using Microsoft.EntityFrameworkCore;
using SnipperSnippetsAPI.Entities;

namespace SnipperSnippetsAPI.Data
{
    public class SnipperSnippetsDbContext: DbContext
    {
        public SnipperSnippetsDbContext(DbContextOptions<SnipperSnippetsDbContext> options) : base(options)
        {
            
        }

        public DbSet<Snippet> Snippets { get; set; }
    }
}
