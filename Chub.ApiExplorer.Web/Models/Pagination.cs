namespace Chub.ApiExplorer.Web.Models
{
    using System.Collections.Generic;

    public class Pagination
    {
        public int Page { get; set; }
        public int Take { get; set; }
        public long TotalItemsCount { get; set; }

        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, string>? RouteData { get; set; }
    }
}
