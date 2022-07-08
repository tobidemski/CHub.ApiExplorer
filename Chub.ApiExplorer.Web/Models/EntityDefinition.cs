namespace Chub.ApiExplorer.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class EntityDefinition
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool IsNew { get; }
        public string DisplayTemplate { get; set; }
        public bool IsTaxonomyItemDefinition { get; set; }
        public bool IsPathEnabledDefinition { get; set; }
        public bool IsManualSortingAllowed { get; set; }
        public bool IsSystemOwned { get; set; }
        public string Url { get; set; }

        public string? CreatedBy { get; set; }
        public SimpleUser? CreatedByUser { get; set; }
        public DateTime? CreatedOn { get; set; }

        public SimpleUser? ModifiedByUser { get; set; }
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public List<MemberGroup> MemberGroups { get; set; } = new List<MemberGroup>();
    }
}
