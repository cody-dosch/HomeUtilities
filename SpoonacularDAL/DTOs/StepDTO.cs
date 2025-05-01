using System;
using System.Net;
using Newtonsoft.Json;

namespace SpoonacularDAL.DTOs
{
    public class StepDTO
    {
        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("ingredients")]
        public List<IngredientDTO> Ingredients { get; set; }

        [JsonProperty("equipment")]
        public List<EquipmentDTO> Equipment { get; set; }
    }
}
