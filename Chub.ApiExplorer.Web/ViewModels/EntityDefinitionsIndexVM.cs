namespace Chub.ApiExplorer.Web.ViewModels
{
    using System.Collections.Generic;
    using Chub.ApiExplorer.Web.Models;

    public class EntityDefinitionsIndexVM : Pagination
    {
        public List<EntityDefinition> Items { get; set; } = new List<EntityDefinition>();
    }
}
