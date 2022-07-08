namespace Chub.ApiExplorer.Web.ViewModels
{
    using System.Collections.Generic;
    using Chub.ApiExplorer.Web.Models;
    using Stylelabs.M.Sdk.Contracts.Base;

    public class DatasourcesIndexVM : Pagination
    {
        public IList<IDataSource> Datasources { get; set; } = new List<IDataSource>();
    }
}
