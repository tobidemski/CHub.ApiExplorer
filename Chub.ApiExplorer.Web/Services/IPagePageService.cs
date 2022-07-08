namespace Chub.ApiExplorer.Web.Services
{
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Models;
    using Stylelabs.M.Sdk.Contracts.Base;

    public interface IPagePageService
    {
        Task<Page> BuildPage(IEntity entity);
    }
}