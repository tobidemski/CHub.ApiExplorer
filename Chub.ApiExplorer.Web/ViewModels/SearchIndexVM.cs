namespace Chub.ApiExplorer.Web.ViewModels
{
    using System.Collections.Generic;
    using Chub.ApiExplorer.Web.Interfaces;

    public class SearchIndexVM
    {
        public List<ITab> Tabs { get; set; } = new List<ITab>();
        public string SearchTerm { get; set; } = string.Empty;
        public string? CurrentTabIdentifier { get; set; } = string.Empty;
    }
}
