namespace Chub.ApiExplorer.Web.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Interfaces.Queries;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Services;
    using Chub.ApiExplorer.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Stylelabs.M.Sdk.WebClient;

    public class DatasourcesController : Controller
    {
        private int _take = 10;
        private CultureInfo _defaultLanguage = new CultureInfo("en-US");

        private readonly IWebMClient _mClient;
        private readonly IDatasourcePageService _datasourcePageService;

        public DatasourcesController(IWebMClient mClient, IDatasourcePageService datasourcePageService)
        {
            this._mClient = mClient;
            this._datasourcePageService = datasourcePageService;
        }

        public async Task<IActionResult> Index(int page = 0)
        {
            IList<string> datasourceStrings = (await this._mClient.DataSources.GetAllAsync());

            if (page < 0)
            {
                page = 0;
            }

            int skip = page * this._take;


            IDataSourceQueryResult datasources = await this._mClient.DataSources.GetDataSources(skip, this._take);

            DatasourcesIndexVM model = new()
            {
                Page = page,
                Take = this._take,
                TotalItemsCount = datasources.TotalNumberOfResults,
                Datasources = datasources.Items,
                Action = nameof(DatasourcesController.Index),
                Controller = this.GetControllerName(),
            };

            return this.View(model);
        }

        [Route("{controller}/{name}")]
        public async Task<IActionResult> Datasource(string name)
        {
            //IDataSource datasource = await this._mClient.DataSources.GetAsync(name);
            //
            //DatasourcesDatasourceVM model = new()
            //{
            //    DataSource = datasource,
            //    CreatedBy = datasource.CreatedBy.HasValue ? await this._mClient.Users.GetUserNameOrNotFound(datasource.CreatedBy.Value) + $" (ID: {datasource.CreatedBy})" : "",
            //    ModifiedBy = datasource.CreatedBy.HasValue ? await this._mClient.Users.GetUserNameOrNotFound(datasource.CreatedBy.Value) + $" (ID: {datasource.CreatedBy})" : ""
            //};

            DataSource? model = await this._datasourcePageService.GetDataSource(name);

            if (model is null)
            {
                return new NotFoundResult();
            }

            return this.View(model);
        }
    }
}
