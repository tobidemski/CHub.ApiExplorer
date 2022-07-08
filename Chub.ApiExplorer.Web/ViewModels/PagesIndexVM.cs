namespace Chub.ApiExplorer.Web.ViewModels
{
    using System.Collections.Generic;
    using Chub.ApiExplorer.Web.Models;

    public class PagesIndexVM : Pagination
    {
        public List<Page> Pages { get; set; } = new List<Page>();
    }
}
