namespace Chub.ApiExplorer.Web.Controllers
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Services;
    using Chub.ApiExplorer.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.WebClient;

    public class EntityDefinitionsController : Controller
    {
        private int _take = 25;
        private CultureInfo _defaultLanguage = new CultureInfo("en-US");

        private readonly IWebMClient _mClient;
        private readonly IConfiguration _configuration;
        private readonly IUserPageService _userService;
        private readonly IEntityDefinitionPageService _entityDefinitionPageService;

        public EntityDefinitionsController(IWebMClient mClient, IConfiguration configuration, IUserPageService userService, IEntityDefinitionPageService entityDefinitionPageService)
        {
            this._mClient = mClient;
            this._configuration = configuration;
            this._userService = userService;
            this._entityDefinitionPageService = entityDefinitionPageService;
        }

        public async Task<IActionResult> Index(int page = 0)
        {
            if (page < 0)
            {
                page = 0;
            }

            int skip = page * this._take;

            IEntityDefinitionQueryResult results = await this._mClient.EntityDefinitions.GetRangeAsync(skip, this._take);

            EntityDefinitionsIndexVM model = new()
            {
                Page = page,
                Take = this._take,
                TotalItemsCount = results.TotalNumberOfResults,
                Action = nameof(EntityDefinitionsController.Index),
                Controller = this.GetControllerName()
            };

            foreach (IEntityDefinition item in results.Items)
            {
                EntityDefinition entry = await this._entityDefinitionPageService.GetEntityDefinition(item);

                model.Items.Add(entry);
            }

            return this.View(model);
        }

        [Route("{controller}/{id}")]
        public async Task<IActionResult> EntityDefinition(string id)
        {
            EntityDefinition? entityDefinition = await this._entityDefinitionPageService.GetEntityDefinition(id, true);

            if (entityDefinition == null)
            {
                return new NotFoundResult();
            }

            return this.View(new EntityDefinitionsEntityDefinitionVM
            {
                EntityDefinition = entityDefinition
            });
        }
    }
}
