namespace Chub.ApiExplorer.Web.Models
{
    using System.Collections.Generic;
    using System.Globalization;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk.Contracts.Base;

    public class Member
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public MemberDefinitionType Type { get; set; }
        public IDictionary<CultureInfo, string> HelpText { get; set; }
        public bool IsConditional { get; set; }
        public IList<IMemberCondition> Contidions { get; set; }
        public bool CanTriggerConditionalMembers { get; set; }
        public bool IsSystemOwned { get; set; }
        public bool IsSecured { get; set; }
        public bool CanWrite { get; set; }
        public bool AllowUpdates { get; set; }

        public RelationRole Role { get; set; }
        public RelationCardinality Cardinality { get; set; }
        public string AssociatedEntityDefinition { get; set; }
        public bool ChildIsMandatory { get; set; }
        public bool ParentIsMandatory { get; set; }
        public bool InheritsSecurity { get; set; }
        public bool AllowNavigation { get; set; }
        public bool IsNested { get; set; }
        public bool IsTaxonomyRelation { get; set; }
        public bool IsTaxonomyHierarchyRelation { get; set; }
        public bool ContentIsCopied { get; set; }
        public bool CompletionIsCopied { get; set; }
        public bool IspathRelation { get; set; }
        public bool IsPathHierarchyRelation { get; set; }
        public int PathHierarchyScore { get; set; }
        public bool IsRenditionRelation { get; set; }
        public string AssociatedLabels { get; set; }

        public string ContentType { get; set; }
        public string ValidationExpression { get; set; }
        public bool IsIndexed { get; set; }
        public bool IsMultilanguage { get; set; }
        public bool IsMultivalue { get; set; }
        public bool IsUnique { get; set; }
        public bool Boost { get; set; }
        public bool IncludedInContent { get; set; }
        public bool IncludedInCompletion { get; set; }
        public bool IgnoreOnExport { get; set; }
        public bool StoredInGraph { get; set; }

        public bool IsRequired { get; set; }
        public bool IsRelationType { get; set; }

        // StringPropDef Members

        public string DataSourceName { get; set; }
    }
}