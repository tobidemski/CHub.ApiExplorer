namespace Chub.ApiExplorer.Web.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Extensions;
    using Chub.ApiExplorer.Web.Interfaces;
    using Chub.ApiExplorer.Web.Models;
    using Chub.ApiExplorer.Web.Models.Tabs;
    using Microsoft.Extensions.Configuration;
    using Stylelabs.M.Base.Querying;
    using Stylelabs.M.Base.Querying.Filters;
    using Stylelabs.M.Framework.Essentials.LoadConfigurations;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.WebClient;

    public class UserPageService : ITabSearchModelBuilder<UserTab>, IUserPageService
    {
        private CultureInfo _defaultLanguage = new("en-US");

        private readonly IWebMClient _mClient;
        private readonly IConfiguration _configuration;

        public string TabIdentifier => this.TabTitle.ToLower();
        public string TabTitle => "Users";
        public string IconCssClass => "m-icon m-icon-account-multiple yellow";

        public UserPageService(IWebMClient mClient, IConfiguration configuration)
        {
            this._mClient = mClient;
            this._configuration = configuration;
        }

        public async Task<UserTab> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            return await this.GetModelInternal(searchTerm, countOnly, skip, take);
        }

        async Task<ITabModel> ITabSearchModelBuilder.GetModel(string searchTerm, bool countOnly, int skip, int take)
        {
            return await this.GetModel(searchTerm, countOnly, skip, take);
        }

        private async Task<UserTab> GetModelInternal(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            UserTab model = new()
            {
                ViewPath = "~/Views/Users/Search.cshtml",
                Model = new List<User>(),
                TotalItemCount = 0
            };

            IQueryResult queryResult;

            if (!countOnly)
            {
                IEntityQueryResult entityQueryResult = await this._mClient.Querying.QueryAsync(
                    this.GetUsersQuery(searchTerm, skip, take));

                queryResult = entityQueryResult;

                foreach (IEntity item in entityQueryResult.Items)
                {
                    model.Model.Add(await this.BuildUser(item));
                }
            }
            else
            {
                queryResult = await this._mClient.Querying.QueryIdsAsync(
                    this.GetUsersQuery(searchTerm, 0, 1));
            }

            model.TotalItemCount = queryResult.TotalNumberOfResults;

            return model;
        }

        public async Task<User?> GetUser(string id)
        {
            IEntity userEntity;

            if (long.TryParse(id, out long userId))
            {
                userEntity = await this._mClient.Users.GetUserAsync(userId);
            }
            else
            {
                // username
                userEntity = await this._mClient.Users.GetUserAsync(id);
            }

            if (userEntity == null)
            {
                return null;
            }

            return await this.BuildUser(userEntity);
        }

        public async Task<SimpleUser> GetSimpleUser(long id)
        {
            IEntity userEntity = await this._mClient.Users.GetUserAsync(id);

            if (userEntity == null)
            {
                return null;
            }

            return await this.BuildSimpleUser(userEntity);
        }

        public async Task<SimpleUser> BuildSimpleUser(IEntity entity)
        {
            string endpoint = this._mClient.GetHost(this._configuration);

            SimpleUser user = new()
            {
                Id = entity.Id!.Value,
                Username = await entity.GetPropertyValueAsync<string>(Constants.User.Username),
                Url = string.Format("{0}", endpoint)
            };

            return user;
        }

        public async Task<IEntityQueryResult> GetUsersAsync(string? searchTerm, int skip = 0, int take = 25)
        {
            Query query = this.GetUsersQuery(searchTerm, skip, take);

            IEntityQueryResult queryResult = await this._mClient.Querying.QueryAsync(query);

            return queryResult;
        }

        public async Task<User> BuildUser(IEntity entity)
        {
            User user = new()
            {
                Id = entity.Id!.Value,
                Username = await entity.GetPropertyValueAsync<string>(Constants.User.Username),
                CreatedOn = entity.CreatedOn
            };

            IChildToManyParentsRelation userGroupsRelation = await entity.GetRelationAsync<IChildToManyParentsRelation>(Constants.User.UserGroupToUser);

            IList<IEntity> groupEntities = await this._mClient.Users.GetUserGroupsAsync(userGroupsRelation.Parents);

            foreach (IEntity groupEntity in groupEntities)
            {
                user.Groups.Add(new Group
                {
                    Id = groupEntity.Id!.Value,
                    Name = await groupEntity.GetPropertyValueAsync<string>(Constants.UserGroup.GroupName)
                });
            }

            IEntity userToProfile = await this._mClient.Users_GetUserProfileAsync(
                entity,
                new EntityLoadConfiguration(
                    new CultureLoadOption(this._defaultLanguage),
                    new PropertyLoadOption(
                        Constants.UserProfile.Email),
                    RelationLoadOption.None));

            user.Email = await userToProfile.GetPropertyValueAsync<string>(Constants.UserProfile.Email);

            IRendition? downloadOriginalRendition = userToProfile.GetRendition("downloadOriginal");

            if (downloadOriginalRendition != null && downloadOriginalRendition.Items.Any())
            {
                user.AvatarUrl = downloadOriginalRendition.Items[0].Href.AbsoluteUri;
            }

            return user;
        }

        private Query GetUsersQuery(string? searchTerm, int skip, int take)
        {
            CompositeQueryFilter filter = new()
            {
                Children = new List<QueryFilter> {
                        new DefinitionQueryFilter { Name = Constants.User.DefinitionName }
                    },
                CombineMethod = CompositeFilterOperator.And
            };

            if (take <= 0)
            {
                take = 25;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filter.Children.Add(
                    new PropertyQueryFilter
                    {
                        Property = Constants.User.Username,
                        DataType = FilterDataType.String,
                        Value = searchTerm,
                        Operator = ComparisonOperator.Contains
                    });
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
