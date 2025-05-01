using System;
using System.Net;
using Newtonsoft.Json;

namespace SpoonacularDAL.DTOs
{
    public class MeasureDetailsDTO
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("unitShort")]
        public string UnitShort { get; set; }

        [JsonProperty("unitLong")]
        public string UnitLong { get; set; }
    }
}
