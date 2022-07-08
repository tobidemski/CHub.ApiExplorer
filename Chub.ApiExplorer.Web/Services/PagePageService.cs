namespace Chub.ApiExplorer.Web.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Interfaces;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Models.Tabs;
    using Microsoft.Extensions.Configuration;
    using Stylelabs.M.Base.Querying;
    using Stylelabs.M.Base.Querying.Filters;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.WebClient;

    public class PagePageService : ITabSearchModelBuilder<PageTab>, IPagePageService
    {
        private CultureInfo _defaultLanguage = new("en-US");

        private readonly IWebMClient _mClient;
        private readonly IConfiguration _configuration;

        public string TabIdentifier => "page";
        public string TabTitle => "Pages";
        public string IconCssClass => "m-icon m-icon-file yellow";

        public PagePageService(IWebMClient mClient, IConfiguration configuration)
        {
            this._mClient = mClient;
            this._configuration = configuration;
        }

        public async Task<PageTab> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            return await this.GetModelInternal(searchTerm, countOnly, skip, take);
        }

        async Task<ITabModel> ITabSearchModelBuilder.GetModel(string searchTerm, bool countOnly, int skip, int take)
        {
            return await this.GetModel(searchTerm, countOnly, skip, take);
        }

        private async Task<PageTab> GetModelInternal(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            PageTab model = new()
            {
                ViewPath = "~/Views/Pages/Search.cshtml",
                Model = new List<Page>(),
                TotalItemCount = 0
            };

            Query query = this.GetPageQuery(searchTerm, skip, take);

            query.Skip = countOnly ? 0 : skip;
            query.Take = countOnly ? 1 : take;

            IQueryResult queryResult;

            if (!countOnly)
            {
                IEntityQueryResult entityQueryResult = await this._mClient.Querying.QueryAsync(query);
                queryResult = entityQueryResult;

                foreach (IEntity item in entityQueryResult.Items)
                {
                    model.Model.Add(await this.BuildPage(item));
                }
            }
            else
            {
                queryResult = await this._mClient.Querying.QueryIdsAsync(query);
            }

            model.TotalItemCount = queryResult.TotalNumberOfResults;

            return model;
        }

        public async Task<Page> BuildPage(IEntity entity)
        {
            string? endpoint = this._mClient.GetHost(this._configuration);

            Page page = new()
            {
                Id = entity.Id!.Value,
                Identifier = entity.Identifier,
                Name = await entity.GetPropertyValueAsync<string>("Page.Name"),
                Title = await entity.GetPropertyValueAsync<string>("Page.Title", this._defaultLanguage),
                Path = await entity.GetPropertyValueAsync<string>("Page.Path", this._defaultLanguage),
                IsTitleVisible = await entity.GetPropertyValueAsync<bool>("Page.IsTitleVisible"),
                IsVisible = await entity.GetPropertyValueAsync<bool>("Page.IsVisible"),
                IsInSiteMap = await entity.GetPropertyValueAsync<bool>("Page.IsInSiteMap"),
                IsInMenu = await entity.GetPropertyValueAsync<bool>("Page.IsInMenu"),
                IsInBreadcrumbs = await entity.GetPropertyValueAsync<bool>("Page.IsInBreadcrumbs"),
                IsHomepage = await entity.GetPropertyValueAsync<bool?>("Page.IsHomepage"),
                Url = string.Format("{0}{1}/admin/page/{2}", endpoint, this._defaultLanguage.Name.ToLower(), entity.Id.Value)
            };

            return page;
        }

        private Query GetPageQuery(string? searchTerm, int skip, int take)
        {
            CompositeQueryFilter filter = new()
            {
                Children = new List<QueryFilter> {
                        new DefinitionQueryFilter { Name = "Portal.Page" }
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
                        Property = "Page.Name",
                        DataType = FilterDataType.String,
                        Value = searchTerm,
                        Operator = ComparisonOperator.Contains
                    });

                searchTermQueryFilter.Children.Add(
                    new PropertyQueryFilter
                    {
                        Property = "Page.Title",
                        DataType = FilterDataType.String,
                        Value = searchTerm,
                        Operator = ComparisonOperator.Contains,
                        Culture = this._defaultLanguage
                    });

                searchTermQueryFilter.Children.Add(
                    new PropertyQueryFilter
                    {
                        Property = "Page.Path",
                        DataType = FilterDataType.String,
                        Value = searchTerm,
                        Operator = ComparisonOperator.Contains,
                        Culture = this._defaultLanguage
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

        public async Task<(IEnumerable<Page>, long totalItems)> GetSearchResultPage(string? searchTerm, int skip, int take)
        {
            Query query = this.GetPageQuery(searchTerm, skip, take);
            IEntityQueryResult queryResult = await this._mClient.Querying.QueryAsync(query);
            List<Page> model = new List<Page>();

            foreach (IEntity item in queryResult.Items)
            {
                 model.Add(await this.BuildPage(item));
            }

            return (model, queryResult.TotalNumberOfResults);
        }
    }
}
