namespace Chub.ApiExplorer.Web.Models.ApiModels
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Stylelabs.M.Base.Web.Api.Models;

    // Stylelabs.M.Sdk.Resources.Base.ListResource<T>
    public class ListResource<T> : Resource where T : Resource
    {
        [JsonProperty("items")]
        public List<T> Items { get; set; }

        [JsonProperty("total_items", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? TotalItems { get; set; }

        [JsonProperty("returned_items", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? ReturnedItems { get; set; }

        [JsonProperty("offset", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Offset { get; set; }

        [JsonProperty("next", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Link Next { get; set; }

        [JsonProperty("previous", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Link Previous { get; set; }

        [JsonProperty("identifier", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Identifier { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; private set; } = new Dictionary<string, JToken>();


        public ListResource()
        {
            this.ExtensionData = new Dictionary<string, JToken>();
        }

        public void AddPagingLinks(int? start, int length, int pageSize, long totalNumberOfItems, Func<object, string> routeFactory)
        {
            int valueOrDefault = start.GetValueOrDefault();
            base.Self = new Link(routeFactory(start), "This collection");
            if (valueOrDefault + length < totalNumberOfItems)
            {
                int num = valueOrDefault + pageSize;
                this.Next = new Link(routeFactory(num), "The next page in this collection");
            }
            if (valueOrDefault > 0)
            {
                int? num2 = valueOrDefault - pageSize;
                if (num2 <= 0)
                {
                    num2 = null;
                }
                this.Previous = new Link(routeFactory(num2), "The previous page in this collection");
            }
        }
    }
}
