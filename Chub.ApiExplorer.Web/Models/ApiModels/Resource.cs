namespace Chub.ApiExplorer.Web.Models.ApiModels
{
    using System;
    using Newtonsoft.Json;
    using Stylelabs.M.Base.Web.Api.Models;

    // Stylelabs.M.Sdk.Resources.Base.Resource
    public class Resource
    {
        [JsonProperty("self", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Link Self { get; set; }

        [JsonIgnore]
        public DateTime? Modified { get; set; }
    }
}
