namespace Chub.ApiExplorer.Web.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Interfaces;
    using Chub.ApiExplorer.Web.Interfaces.Queries;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Models.Tabs;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.WebClient;

    public class DatasourcePageService : ITabSearchModelBuilder<DatasourceTab>, IDatasourcePageService
    {
        private readonly IWebMClient _mClient;

        public string TabIdentifier => this.TabTitle.ToLower();
        public string TabTitle => "Datasources";
        public string IconCssClass => "m-icon m-icon-format-list-checkbox red";

        public DatasourcePageService(IWebMClient mClient)
        {
            this._mClient = mClient;
        }

        public async Task<DatasourceTab> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            return await this.SearchDataSources(searchTerm, countOnly, skip, take);
        }

        async Task<ITabModel> ITabSearchModelBuilder.GetModel(string searchTerm, bool countOnly, int skip, int take)
        {
            return await this.GetModel(searchTerm, countOnly, skip, take);
        }

        public async Task<DataSource?> GetDataSource(string name, CultureInfo? culture = null)
        {
            IDataSource datasource = await this._mClient.DataSources.GetAsync(name);

            if (datasource is null)
            {
                return null;
            }

            if (culture is null)
            {
                culture = new("en-US");
            }

            string label = datasource.Labels != null && datasource.Labels.ContainsKey(culture)
                ? datasource.Labels[culture] : string.Empty;

            string createdByUser = datasource.CreatedBy.HasValue
                ? await this._mClient.Users.GetUserNameOrNotFound(datasource.CreatedBy.Value) + $" (ID: {datasource.CreatedBy})"
                : string.Empty;

            string modifiedByUser = datasource.CreatedBy.HasValue
                ? await this._mClient.Users.GetUserNameOrNotFound(datasource.CreatedBy.Value) + $" (ID: {datasource.CreatedBy})"
                : string.Empty;

            DataSource model = new()
            {
                Label = label,
                CurrentCulture = culture,
                Name = datasource.Name,
                Type = datasource.Type,
                Values = datasource.GetDataSourceValues(),
                CreatedBy = createdByUser,
                ModifiedBy = modifiedByUser,
                CreatedOn = datasource.CreatedOn,
                ModifiedOn = datasource.ModifiedOn,
                IsSystemOwned = datasource.IsSystemOwned
            };

            return model;
        }

        private async Task<DatasourceTab> SearchDataSources(string searchTerm, bool countOnly, int skip, int take)
        {
            DatasourceTab model = new()
            {
                ViewPath = "~/Views/Datasources/Search.cshtml",
                Model = new List<IDataSource>(),
                TotalItemCount = 0
            };

            IDataSourceQueryResult datasources;

            if (string.IsNullOrEmpty(searchTerm))
            {
                datasources = await this._mClient.DataSources.SearchDataSources(skip: skip, take: take);
            }
            else
            {
                datasources = await this._mClient.DataSources.SearchDataSources(searchTerm, skip, take);
            }

            model.TotalItemCount = datasources.TotalNumberOfResults;

            if (!countOnly)
            {
                model.Model = datasources.Items;
            }

            return model;
        }
    }
}
