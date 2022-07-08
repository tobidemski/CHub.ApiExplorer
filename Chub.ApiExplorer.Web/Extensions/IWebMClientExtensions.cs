namespace Chub.ApiExplorer.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Interfaces.Queries;
    using Chub.ApiExplorer.Web.Models.ApiModels;
    using Chub.ApiExplorer.Web.Models.Queries;
    using Microsoft.Extensions.Configuration;
    using Stylelabs.M.Base.Web.Api.Models;
    using Stylelabs.M.Framework.Essentials.LoadConfigurations;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk;
    using Stylelabs.M.Sdk.Clients;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.Models.Querying;
    using Stylelabs.M.Sdk.WebClient;
    using Stylelabs.M.Sdk.WebClient.Http;

    public static class IWebMClientExtensions
    {
        public static string GetHost(this IWebMClient client, IConfiguration config)
        {
            string? connectionString = config.GetConnectionString("ContentHub");

            DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            dbConnectionStringBuilder.TryGetValue("Endpoint", out object? endpointObj);

            return endpointObj is not null ? endpointObj.ToString()! : string.Empty;
        }

        public static async Task<string> GetUserNameOrNotFound(this IUsersClient client, long id)
        {
            IEntity user = await client.GetUserAsync(
                id,
                new EntityLoadConfiguration(
                    CultureLoadOption.Default,
                    new PropertyLoadOption(Constants.User.Username),
                    RelationLoadOption.None));

            if (user == null)
            {
                return "[User not found]";
            }

            return await user.GetPropertyValueAsync<string>(Constants.User.Username);
        }

        public static async Task<IEntityDefinitionQueryResult> EntityDefinitions_Get(
            this IWebMClient client, string searchTerm, int skip, int take)
        {
            Dictionary<string, string> variables = new()
            {
                { "skip", skip.ToString() },
                { "take", take.ToString() },
                { "filter", searchTerm }
            };

            Link link = (await client.Api.GetApiRoutesAsync().ConfigureAwait(continueOnCapturedContext: false))["entitydefinitions"].Bind(variables);
            HttpResponseMessage response = await client.Raw.GetAsync(link.Uri);
            ListResource<NameResource> resource = await response.Content.ReadAsJsonAsync<ListResource<NameResource>>();

            IEnumerable<string> relevantDefinitionNames = resource.Items.Select(x => x.Name);

            IList<IEntityDefinition> results = await client.EntityDefinitions.GetManyAsync(relevantDefinitionNames);

            IEntityDefinitionQueryResult model = new EntityDefinitionQueryResult(client, results, resource.TotalItems!.Value, (long)resource.Offset!);

            return model;
        }


        public static async Task<IDataSourceQueryResult> SearchDataSources(
            this IDataSourcesClient client, string? searchTerm = null, int skip = 0, int take = 25)
        {
            IList<string> datasourceStrings = await client.GetAllAsync();

            IEnumerable<string> searchedDatasourceStrings = datasourceStrings;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchedDatasourceStrings = searchedDatasourceStrings.Where(x =>
                    x.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase));
            }

            searchedDatasourceStrings = searchedDatasourceStrings.OrderBy(x => x);

            List<string>? filteredDatasources = searchedDatasourceStrings.Skip(skip).Take(take).ToList();

            List<IDataSource> entries = new();

            foreach (string? dsString in filteredDatasources)
            {
                try
                {
                    IDataSource? ds = await client.GetAsync(dsString);
                    entries.Add(ds);
                }
                catch (ArgumentException argEx)
                {
                    // FONTS WTF??? - No labels defined Keys error
                    continue;
                }
            }

            IDataSourceQueryResult result = new DataSourceQueryResult(
                entries, searchedDatasourceStrings.Count(), skip);

            return result;
        }

        public static async Task<IDataSourceQueryResult> GetDataSources(this IDataSourcesClient client, int skip, int take)
        {
            IList<string> datasourceStrings = await client.GetAllAsync();

            List<string> currentDatasourceStrings = datasourceStrings.OrderBy(x => x).Skip(skip).Take(take).ToList();

            List<IDataSource> entries = new();

            foreach (string? dsString in currentDatasourceStrings)
            {
                try
                {
                    IDataSource? ds = await client.GetAsync(dsString);
                    entries.Add(ds);
                }
                catch (ArgumentException argEx)
                {
                    // FONTS WTF??? - No labels defined Keys error
                    continue;
                }
            }

            IDataSourceQueryResult result = new DataSourceQueryResult(entries, datasourceStrings.Count, skip);

            return result;
        }

        public static async Task<IEntity> Users_GetUserProfileAsync(
            this IWebMClient client, IEntity entity, IEntityLoadConfiguration? loadConfiguration)
        {
            IParentToOneChildRelation userToProfileRelation = await entity.GetRelationAsync<IParentToOneChildRelation>(Constants.User.UserToUserProfile);

            IEntity userToProfile = await client.Entities.GetAsync(
                userToProfileRelation.Child!.Value, loadConfiguration);

            return userToProfile;
        }


        public static async Task<IList<IEntity>> Users_GetUserGroupsAsync(
            this IWebMClient client, IEntity entity, IEntityLoadConfiguration? loadConfiguration)
        {
            IChildToManyParentsRelation userGroupsRelation = await entity.GetRelationAsync<IChildToManyParentsRelation>(Constants.User.UserGroupToUser);

            if (!userGroupsRelation.Parents.Any())
            {
                return new List<IEntity>();
            }

            IList<IEntity> userGroups = await client.Users.GetUserGroupsAsync(
                userGroupsRelation.Parents, loadConfiguration);

            return userGroups;
        }


    }
}
