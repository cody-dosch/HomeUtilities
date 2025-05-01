using System;
using System.Net;
using Newtonsoft.Json;

namespace SpoonacularDAL.DTOs
{
    public class AnalyzedInstructionsDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("steps")]
        public List<StepDTO> Steps { get; set; }
    }
}
