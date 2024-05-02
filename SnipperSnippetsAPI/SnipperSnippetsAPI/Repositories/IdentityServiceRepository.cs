using Microsoft.EntityFrameworkCore;
using SnipperSnippetsAPI.Data;
using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Repositories.Contracts;
using System.Security.Cryptography;

namespace SnipperSnippetsAPI.Repositories
{
    public class IdentityServiceRepository : IIdentityServiceRepository
    {
        private readonly SnipperSnippetsDbContext _snipperSnippetsDbContext;

        public IdentityServiceRepository(SnipperSnippetsDbContext snipperSnippetsDbContext)
        {
            _snipperSnippetsDbContext = snipperSnippetsDbContext;
        }

        public async Task<User> AuthenticateUser(string email, string password)
        {
            try
            {
                var foundUser = await (from user in _snipperSnippetsDbContext.Users
                                where user.email == email
                                select user).FirstOrDefaultAsync();

                if (foundUser != null)
                {
                    var saltString = foundUser.password.Substring(0, 28);
                    var savedPasswordHashString = foundUser.password.Substring(28);

                    using (var deriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(saltString), 10000, HashAlgorithmName.SHA256))
                    {
                        byte[] testPasswordHash = deriveBytes.GetBytes(20);

                        if (Convert.ToBase64String(testPasswordHash) == savedPasswordHashString)
                        {
                            return foundUser;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> CreateUser(string email, string password)
        {
            try
            {
                var hashedPassword = "";
                using (var deriveBytes = new Rfc2898DeriveBytes(password, 20, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] salt = deriveBytes.Salt;
                    byte[] buffer = deriveBytes.GetBytes(20);
                    hashedPassword = Convert.ToBase64String(salt) + Convert.ToBase64String(buffer);
                }
                var user = new User { email = email, password = hashedPassword };

                var result = await _snipperSnippetsDbContext.Users.AddAsync(user);
                await _snipperSnippetsDbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User?> GetUserById(long id)
        {
            return await _snipperSnippetsDbContext.Users.SingleOrDefaultAsync(c => c.id == id);
        }
    }
}
