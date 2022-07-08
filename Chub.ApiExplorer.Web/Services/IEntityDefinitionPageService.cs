namespace Chub.ApiExplorer.Web.Services
{
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Models;
    using Stylelabs.M.Sdk.Contracts.Base;

    public interface IEntityDefinitionPageService
    {
        Task<EntityDefinition?> GetEntityDefinition(string id, bool loadMembers = false);
        Task<EntityDefinition> GetEntityDefinition(IEntityDefinition entity, bool loadMembers = false);
    }
}