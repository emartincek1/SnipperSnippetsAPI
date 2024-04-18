using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using SnipperSnippetsAPI.Entities;

namespace SnipperSnippetsAPI.Repositories.Contracts
{
    public interface IIdentityServiceRepository
    {
        Task<User> CreateUser(string email, string password);
        Task<User> AuthenticateUser(string email, string password);
    }
}
