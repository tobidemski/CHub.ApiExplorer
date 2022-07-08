namespace Chub.ApiExplorer.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Interfaces;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Models.Tabs;
    using Chub.ApiExplorer.Web.Services;
    using Chub.ApiExplorer.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Stylelabs.M.Base.Querying;
    using Stylelabs.M.Base.Querying.Linq;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.WebClient;

    public class SearchController : Controller
    {
        private int _take = 50;
        private CultureInfo _defaultLanguage = new CultureInfo("en-US");

        private readonly IWebMClient _mClient;
        private readonly IEnumerable<ITabSearchModelBuilder> _tabSearchModelBuilder;

        public SearchController(IWebMClient mClient, IEnumerable<ITabSearchModelBuilder> tabSearchModelBuilder)
        {
            this._mClient = mClient;
            this._tabSearchModelBuilder = tabSearchModelBuilder;
        }

        [Route("{controller}")]
        public async Task<IActionResult> Search(string searchTerm, string? tab)
        {
            if (searchTerm == null)
            {
                searchTerm = string.Empty;
            }

            SearchIndexVM model = new()
            {
                SearchTerm = searchTerm,
                CurrentTabIdentifier = tab
            };

            bool anyActiveTab = !string.IsNullOrEmpty(model.CurrentTabIdentifier) && this._tabSearchModelBuilder.Any(x => x.TabIdentifier == model.CurrentTabIdentifier);

            IOrderedEnumerable<ITabSearchModelBuilder> builders = this._tabSearchModelBuilder.OrderBy(x => x.TabTitle);

            bool tabWithResults = false;

            for (int i = 0; i < builders.Count(); i++)
            {
                ITabSearchModelBuilder modelBuilder = builders.ElementAt(i);

                bool getCountOnly = anyActiveTab ? modelBuilder.TabIdentifier != model.CurrentTabIdentifier : tabWithResults;

                ITabModel tabModel = await modelBuilder.GetModel(searchTerm, getCountOnly, 0, this._take);

                if (!tabWithResults && tabModel.TotalItemCount != 0)
                {
                    tabWithResults = true;
                }

                model.Tabs.Add(new Tab
                {
                    IconCssClass = modelBuilder.IconCssClass,
                    Id = modelBuilder.TabIdentifier,
                    Title = modelBuilder.TabTitle,
                    TabModel = tabModel,
                    IsActive = !getCountOnly && tabModel.TotalItemCount > 0
                });
            }

            ITab? activeTab = model.Tabs.FirstOrDefault(x => x.IsActive);

            if (activeTab != null && string.IsNullOrEmpty(model.CurrentTabIdentifier))
            {
                model.CurrentTabIdentifier = activeTab.Id;
            }

            model.Tabs = model.Tabs.ToList();

            //model.Tabs.Add(await this.GetAssetsTab(searchTerm));
            //model.Tabs.Add(await this.GetPageTab(searchTerm));

            return this.View("~/Views/Search/Index.cshtml", model);
        }

        private async Task<ITab> GetAssetsTab(string searchTerm)
        {
            Tab<AssetTab> model = new()
            {
                Id = "assets",
                Title = "Assets",
                TabModel = new AssetTab
                {
                    ViewPath = "~/Views/Assets/Search.cshtml",
                }
            };

            if (string.IsNullOrEmpty(searchTerm))
            {
                return model;
            }

            Query query = Query.CreateEntitiesQuery(q =>
                q.Where(e =>
                    e.DefinitionName == "M.Asset" && (e.Property("Title").Contains(searchTerm) ||
                    e.Property("FileName").Contains(searchTerm))));

            IEntityQueryResult queryResult = await this._mClient.Querying.QueryAsync(query);

            List<Asset> entries = new();

            foreach (IEntity item in queryResult.Items)
            {
                IRendition thumbnailRendition = item.GetRendition("thumbnail");

                entries.Add(new Asset
                {
                    Id = item.Id!.Value,
                    Name = await item.GetPropertyValueAsync<string>("FileName"),
                    Title = await item.GetPropertyValueAsync<string>("Title"),
                    ImageUrl = thumbnailRendition != null && thumbnailRendition.Items.Any() ? thumbnailRendition.Items[0].Href.ToString() : ""
                });
            }

            model.TabModel.TotalItemCount = queryResult.TotalNumberOfResults;
            model.TabModel.Model = entries;

            return model;
        }

    }
}
