namespace Chub.ApiExplorer.Web.Services
{
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Models;
    using Stylelabs.M.Sdk.Contracts.Base;

    public interface IUserPageService
    {
        Task<User> BuildUser(IEntity entity);
        Task<SimpleUser> BuildSimpleUser(IEntity entity);
        Task<SimpleUser> GetSimpleUser(long id);
        Task<User?> GetUser(string id);
    }
}