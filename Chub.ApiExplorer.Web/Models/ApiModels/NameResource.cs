namespace Chub.ApiExplorer.Web.Models.ApiModels
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    // Based on
    // Stylelabs.M.Sdk.Resources.Base.EntityDefinitionResource
    public class NameResource : Resource
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; set; }

        public NameResource()
        {
            this.Labels = new Dictionary<string, string>();
        }
    }
}
