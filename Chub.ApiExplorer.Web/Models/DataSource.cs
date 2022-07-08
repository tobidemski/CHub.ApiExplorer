namespace Chub.ApiExplorer.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Stylelabs.M.Sdk.Contracts.Base;

    public class DataSource
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public DataSourceType Type { get; set; }
        public IList<IDataSourceValue> Values { get; set; } = new List<IDataSourceValue>();
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public bool IsSystemOwned { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
