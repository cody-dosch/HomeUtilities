using System;
using System.Net;
using Newtonsoft.Json;

namespace SpoonacularDAL.Requests
{
    public class GetRandomRecipesRequest
    {
        public bool LimitLicense { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> IncludedTags { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExcludedTags { get; set; }

        public int Quantity { get; set; }
    }
}
