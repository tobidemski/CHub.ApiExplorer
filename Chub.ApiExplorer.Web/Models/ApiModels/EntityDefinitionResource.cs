namespace Chub.ApiExplorer.Web.Models.ApiModels
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Stylelabs.M.Base.Web.Api.Models;

    // Based on
    // Stylelabs.M.Sdk.Resources.Base.EntityDefinitionResource
    public class EntityDefinitionResource : Resource
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modified_on")]
        public DateTime ModifiedOn { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("display_template", DefaultValueHandling = DefaultValueHandling.Include)]
        public string DisplayTemplate { get; set; }

        [JsonProperty("is_taxonomy_item_definition")]
        public bool IsTaxonomyItemDefinition { get; set; }

        [JsonProperty("is_path_enabled_definition")]
        public bool IsPathEnabledDefinition { get; set; }

        [JsonProperty("is_manual_sorting_allowed")]
        public bool IsManualSortingAllowed { get; set; }

        [JsonProperty("is_system_owned")]
        public bool IsSystemOwned { get; set; }

        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; set; }

        // TO MANY CUSTOM INTERNAL SHIT - TO LAZY TO COPY PASTE ALL SHIT
        //[JsonProperty("member_groups")]
        //public List<MemberGroup> MemberGroups { get; set; }

        [JsonProperty("entities")]
        public Link Entities { get; set; }

        [JsonProperty("related_paths")]
        public Link RelatedPaths { get; set; }

        [JsonProperty("modified_by")]
        public Link ModifiedBy { get; set; }

        [JsonProperty("created_by")]
        public Link CreatedBy { get; set; }

        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Permissions { get; set; }

        public EntityDefinitionResource()
        {
            this.Labels = new Dictionary<string, string>();
            //this.MemberGroups = new List<MemberGroup>();
        }
    }
}
