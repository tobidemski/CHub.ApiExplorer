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
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.Models.Base.PropertyDefinitions;
    using Stylelabs.M.Sdk.WebClient;

    public class EntityDefinitionPageService : ITabSearchModelBuilder<EntityDefinitionTab>, IEntityDefinitionPageService
    {
        private CultureInfo _defaultLanguage = new("en-US");

        private readonly IWebMClient _mClient;
        private readonly IConfiguration _configuration;
        private readonly IUserPageService _userPageService;

        public string TabIdentifier => "entity-definition";
        public string TabTitle => "Entity Definitions";
        public string IconCssClass => "m-icon m-icon-graph red";

        public EntityDefinitionPageService(IWebMClient mClient, IUserPageService userPageService, IConfiguration configuration)
        {
            this._mClient = mClient;
            this._configuration = configuration;
            this._userPageService = userPageService;
        }

        public async Task<EntityDefinitionTab> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            return await this.GetModelInternal(searchTerm, countOnly, skip, take);
        }

        async Task<ITabModel> ITabSearchModelBuilder.GetModel(string searchTerm, bool countOnly, int skip, int take)
        {
            return await this.GetModel(searchTerm, countOnly, skip, take);
        }

        private async Task<EntityDefinitionTab> GetModelInternal(string searchTerm, bool countOnly = false, int skip = 0, int take = 25)
        {
            EntityDefinitionTab model = new()
            {
                ViewPath = "~/Views/EntityDefinitions/Search.cshtml",
                Model = new List<EntityDefinition>(),
                TotalItemCount = 0
            };

            IEntityDefinitionQueryResult result;

            if (!countOnly)
            {
                result = await this._mClient.EntityDefinitions_Get(searchTerm, skip, take);

                foreach (IEntityDefinition item in result.Items)
                {
                    model.Model.Add(new EntityDefinition
                    {
                        Id = item.Id!.Value,
                        Name = item.Name,
                        Label = item.Labels != null && item.Labels.ContainsKey(this._defaultLanguage) ? item.Labels[this._defaultLanguage] : ""
                    });
                }
            }
            else
            {
                result = await this._mClient.EntityDefinitions_Get(searchTerm, 0, 1);
            }

            model.TotalItemCount = result.TotalNumberOfResults;

            return model;
        }

        public async Task<EntityDefinition> GetEntityDefinition(IEntityDefinition entity, bool loadMembers = false)
        {
            string endpoint = this._mClient.GetHost(this._configuration);

            //TODO: Flatten the model due to performance issues if x entities are called with reference to user obj from the overview page.

            EntityDefinition model = new()
            {
                Id = entity.Id!.Value,
                Name = entity.Name,
                DisplayTemplate = entity.DisplayTemplate,
                IsManualSortingAllowed = entity.IsManualSortingAllowed,
                IsPathEnabledDefinition = entity.IsPathEnabledDefinition,
                IsSystemOwned = entity.IsSystemOwned,
                IsTaxonomyItemDefinition = entity.IsTaxonomyItemDefinition,
                //CreatedBy = entity.CreatedBy.HasValue ? await this._mClient.Users.GetUserNameOrNotFound(entity.CreatedBy.Value) + $" (ID: {entity.CreatedBy})" : "",
                CreatedOn = entity.CreatedOn,
                //ModifiedBy = entity.ModifiedBy.HasValue ? await this._mClient.Users.GetUserNameOrNotFound(entity.ModifiedBy.Value) + $" (ID: {entity.ModifiedBy})" : "",
                ModifiedOn = entity.ModifiedOn,

                //CreatedByUser = entity.CreatedBy.HasValue ? await this._userPageService.GetSimpleUser(entity.CreatedBy.Value) : null,
                //ModifiedByUser = entity.ModifiedBy.HasValue ? await this._userPageService.GetSimpleUser(entity.ModifiedBy.Value) : null,

                Url = string.Format("{0}{1}/admin/definitionmgmt/detail/{2}", endpoint, this._defaultLanguage, entity.Id!.Value),

                Label = entity.Labels != null && entity.Labels.ContainsKey(this._defaultLanguage) ? entity.Labels[this._defaultLanguage] : "",
                MemberGroups = loadMembers ? entity.MemberGroups.Select(mg => new MemberGroup
                {
                    Name = mg.Name,
                    Label = mg.Labels != null && mg.Labels.ContainsKey(this._defaultLanguage) ? mg.Labels[this._defaultLanguage] : "",
                    Members = mg.MemberDefinitions.Select(this.GetMember).ToList()
                }).ToList() : new List<MemberGroup>()
            };

            return model;
        }

        public async Task<EntityDefinition?> GetEntityDefinition(string id, bool loadMembers = false)
        {
            IEntityDefinition result;

            if (long.TryParse(id, out long defId))
            {
                result = await this._mClient.EntityDefinitions.GetAsync(defId);
            }
            else
            {
                result = await this._mClient.EntityDefinitions.GetAsync(id);
            }

            if (result == null)
            {
                return null;
            }

            return await this.GetEntityDefinition(result, loadMembers);
        }

        private Member GetMember(IMemberDefinition md)
        {
            bool isRelationType = md.DefinitionType == MemberDefinitionType.Relation;

            Member member = new Member
            {
                Type = md.DefinitionType,
                Name = md.Name,
                Label = md.Labels == null || !md.Labels.ContainsKey(this._defaultLanguage) ? md.Name : md.Labels[this._defaultLanguage],
                HelpText = md.HelpText,
                IsConditional = md.IsConditional,
                Contidions = md.Conditions,
                //CanTriggerConditionalMembers = md
                IsSystemOwned = md.IsSystemOwned,
                IsSecured = md.IsSecured,
                //CanWrite = md
                AllowUpdates = md.AllowUpdates,
                IsRelationType = isRelationType
            };

            if (!isRelationType)
            {
                IPropertyDefinition propDef = (IPropertyDefinition)md;



                // Stylelabs.M.Sdk.WebClient.Mappers.MemberDefinitionMapper.MapPropertyDefinitionAsync
                if (propDef is StringPropertyDefinition strPropDef)
                {
                    member.DataSourceName = strPropDef.DataSourceName;
                }

                member.IsRequired = propDef.IsMandatory;
                member.IsIndexed = propDef.Indexed;
                member.IsMultilanguage = propDef.IsMultiLanguage;
                member.IsMultivalue = propDef.IsMultiValue;
                member.IsUnique = propDef.IsUnique;
                member.Boost = propDef.Boost;
                member.IncludedInContent = propDef.IncludedInContent;
                member.IncludedInCompletion = propDef.IncludedInCompletion;
                //member.IgnoreOnExport = propDef.
                //member.StoredInGraph = propDef.
            }
            else
            {
                IRelationDefinition relDef = (IRelationDefinition)md;

                member.Role = relDef.Role;
                member.Cardinality = relDef.Cardinality;
                member.AssociatedEntityDefinition = relDef.AssociatedEntityDefinitionName;
                member.ChildIsMandatory = relDef.ChildIsMandatory;
                member.ParentIsMandatory = relDef.ParentIsMandatory;
                member.InheritsSecurity = relDef.InheritsSecurity;
                member.AllowNavigation = relDef.AllowNavigation;
                member.IsNested = relDef.IsNested;
                member.IsTaxonomyRelation = relDef.IsTaxonomyRelation;
                member.IsTaxonomyHierarchyRelation = relDef.IsTaxonomyHierarchyRelation;
                member.ContentIsCopied = relDef.ContentIsCopied;
                member.CompletionIsCopied = relDef.CompletionIsCopied;
                member.IspathRelation = relDef.IsPathRelation;
                member.IsPathHierarchyRelation = relDef.IsPathHierarchyRelation;
                member.PathHierarchyScore = relDef.PathHierarchyScore;
            }

            return member;
        }
    }
}
