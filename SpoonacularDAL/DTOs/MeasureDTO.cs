using System;
using System.Net;
using Newtonsoft.Json;

namespace SpoonacularDAL.DTOs
{
    public class MeasureDTO
    {
        [JsonProperty("us")]
        public MeasureDetailsDTO US { get; set; }

        [JsonProperty("metric")]
        public MeasureDetailsDTO Metric { get; set; }       
    }
}
