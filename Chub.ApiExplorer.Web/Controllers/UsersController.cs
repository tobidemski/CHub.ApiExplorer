namespace Chub.ApiExplorer.Web.Controllers
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Services;
    using Chub.ApiExplorer.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Stylelabs.M.Base.Querying;
    using Stylelabs.M.Framework.Essentials.LoadConfigurations;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.WebClient;

    public class UsersController : Controller
    {
        private int _take = 5;
        private CultureInfo _defaultLanguage = new CultureInfo("en-US");

        private readonly IWebMClient _mClient;
        private readonly IUserPageService _userService;

        public UsersController(IWebMClient mClient, IUserPageService userService)
        {
            this._mClient = mClient;
            this._userService = userService;
        }

        public async Task<IActionResult> Index(int page = 0)
        {
            if (page < 0)
            {
                page = 0;
            }

            int skip = page * this._take;

            Query query = Query.CreateQuery(q => q.Where(e => e.DefinitionName == Constants.User.DefinitionName));

            query.Skip = skip;
            query.Take = this._take;

            IEntityQueryResult queryResult = await this._mClient.Querying.QueryAsync(
                query,
                new EntityLoadConfiguration(
                    new CultureLoadOption(this._defaultLanguage),
                    new PropertyLoadOption(
                        Constants.User.Username,
                        "LastLoginDateTime",
                        "HasToken"),
                    new RelationLoadOption(
                        Constants.User.UserGroupToUser,
                        Constants.User.UserToUserProfile)
                    ));

            UsersIndexVM model = new()
            {
                Page = page,
                Take = this._take,
                TotalItemsCount = queryResult.TotalNumberOfResults,
                Controller = this.GetControllerName(),
                Action = nameof(UsersController.Index)
            };

            foreach (IEntity item in queryResult.Items)
            {
                model.Users.Add(await this._userService.BuildUser(item));
            }


            return this.View(model);
        }

        [Route("{controller}/{id}")]
        public async Task<IActionResult> User(string id)
        {
            User? user = await this._userService.GetUser(id);

            if (user == null)
            {
                return new NotFoundResult();
            }

            UsersUserVM model = new()
            {
                User = user
            };

            return this.View(model);
        }
    }
}
