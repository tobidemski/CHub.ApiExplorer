namespace Chub.ApiExplorer.Web.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Services;
    using Chub.ApiExplorer.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.WebClient;

    public class PagesController : Controller
    {
        private int _take = 25;
        private static CultureInfo _defaultLanguage = new CultureInfo("en-US");

        private readonly IWebMClient _mClient;
        private readonly IPagePageService _pagePageService;

        public PagesController(IWebMClient mClient, IPagePageService pagePageService)
        {
            this._mClient = mClient;
            this._pagePageService = pagePageService;
        }

        public async Task<IActionResult> Index(int page = 0, string searchTerm = "")
        {
            if (page < 0)
            {
                page = 0;
            }

            (IEnumerable<Page>, long totalItems) pages =
                await this._pagePageService.GetSearchResultPage(searchTerm, page * this._take, this._take);

            PagesIndexVM model = new()
            {
                Page = page,
                Take = this._take,
                TotalItemsCount = pages.totalItems,
                Action = nameof(PagesController.Index),
                Controller = this.GetControllerName(),
                Pages = pages.Item1.ToList()
            };

            return this.View(model);
        }

        [Route("{controller}/{id}")]
        public async Task<IActionResult> Page(string id)
        {
            IEntity result;

            if (long.TryParse(id, out long defId))
            {
                result = await this._mClient.Entities.GetAsync(defId);
            }
            else
            {
                result = await this._mClient.Entities.GetAsync(id);
            }

            if (result == null)
            {
                return new NotFoundResult();
            }

            Page model = await this._pagePageService.BuildPage(result);


            return this.View(model);
        }
    }
}
