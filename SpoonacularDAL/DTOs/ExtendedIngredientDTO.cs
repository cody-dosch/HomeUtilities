using System;
using System.Net;
using Newtonsoft.Json;

namespace SpoonacularDAL.DTOs
{
    public class ExtendedIngredientDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("aisle")]
        public string Aisle { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("measures")]
        public MeasureDTO Measures { get; set; }
    }
}
