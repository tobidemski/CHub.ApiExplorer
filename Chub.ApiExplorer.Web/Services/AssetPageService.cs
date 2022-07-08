namespace Chub.ApiExplorer.Web.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Interfaces;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Models.Tabs;
    using Microsoft.Extensions.Configuration;
    using Stylelabs.M.Base.Querying;
    using Stylelabs.M.Base.Querying.Filters;
    using Stylelabs.M.Sdk;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.WebClient;

    public class AssetPageService : ITabSearchModelBuilder<AssetTab>, IAssetPageService
    {
        private CultureInfo _defaultLanguage = new("en-US");

        private readonly IWebMClient _mClient;
        private readonly IConfiguration _configuration;

        public string TabIdentifier => "asset";
        public string TabTitle => "Assets";
        public string IconCssClass => "";

        public AssetPageService(IWebMClient mClient, IConfiguration configuration)
        {
            this._mClient = mClient;
            this._configuration = configuration;
        }

        public async Task<AssetTab> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            return await this.GetModelInternal(searchTerm, countOnly, skip, take);
        }

        async Task<ITabModel> ITabSearchModelBuilder.GetModel(string searchTerm, bool countOnly, int skip, int take)
        {
            return await this.GetModel(searchTerm, countOnly, skip, take);
        }


        private async Task<AssetTab> GetModelInternal(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            AssetTab model = new()
            {
                ViewPath = "~/Views/Assets/Search.cshtml",
                Model = new List<Asset>(),
                TotalItemCount = 0
            };

            Query query = this.GetAssetQuery(searchTerm, skip, take);

            IQueryResult queryResult;

            if (countOnly)
            {
                query.Skip = 0;
                query.Take = 1;

                queryResult = await this._mClient.Querying.QueryIdsAsync(query);
            }
            else
            {
                IEntityQueryResult entityQueryResult = await this._mClient.Querying.QueryAsync(query);
                queryResult = entityQueryResult;

                foreach (IEntity item in entityQueryResult.Items)
                {
                    IRendition thumbnailRendition = item.GetRendition("thumbnail");

                    model.Model.Add(new Asset
                    {
                        Id = item.Id!.Value,
                        Name = await item.GetPropertyValueAsync<string>("FileName"),
                        Title = await item.GetPropertyValueAsync<string>("Title"),
                        ImageUrl = thumbnailRendition != null && thumbnailRendition.Items.Any() ? thumbnailRendition.Items[0].Href.ToString() : ""
                    });
                }
            }

            model.TotalItemCount = queryResult.TotalNumberOfResults;

            return model;
        }


        private Query GetAssetQuery(string? searchTerm, int skip, int take)
        {
            CompositeQueryFilter filter = new()
            {
                Children = new List<QueryFilter> {
                        new DefinitionQueryFilter { Name = Constants.Asset.DefinitionName }
                    },
                CombineMethod = CompositeFilterOperator.And
            };

            if (take <= 0)
            {
                take = 25;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                CompositeQueryFilter searchTermQueryFilter = new()
                {
                    CombineMethod = CompositeFilterOperator.Or,
                    Children = new List<QueryFilter>()
                };

                searchTermQueryFilter.Children.Add(
                    new PropertyQueryFilter
                    {
                        Property = Constants.Asset.FileName,
                        DataType = FilterDataType.String,
                        Value = searchTerm
                    });

                searchTermQueryFilter.Children.Add(
                    new PropertyQueryFilter
                    {
                        Property = Constants.Asset.Title,
                        DataType = FilterDataType.String,
                        Value = searchTerm,
                        Operator = ComparisonOperator.Contains
                    });

                filter.Children.Add(searchTermQueryFilter);
            }

            Query query = new Query
            {
                Filter = filter,
                Skip = skip,
                Take = take
            };

            return query;
        }

    }
}
