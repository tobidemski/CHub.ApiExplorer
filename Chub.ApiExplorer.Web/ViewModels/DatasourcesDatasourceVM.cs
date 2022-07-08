namespace Chub.ApiExplorer.Web.ViewModels
{
    using Stylelabs.M.Sdk.Contracts.Base;

    public class DatasourcesDatasourceVM
    {
        public IDataSource DataSource { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
