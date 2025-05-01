using System;
using System.Net;
using Newtonsoft.Json;
using SpoonacularDAL.DTOs;

namespace SpoonacularDAL.Responses
{
    public class GetRecipeInformationResponse : BaseResponse
    {
        // TODO: All of these properties are the same as RecipeDTO - use inheritance somehow to maintain these properties in one location.

        [JsonProperty("vegetarian")]
        public bool IsVegetarian { get; set; }

        [JsonProperty("veryHealthy")]
        public bool IsVeryHealthy { get; set; }

        [JsonProperty("cheap")]
        public bool IsCheap { get; set; }

        [JsonProperty("preparationMinutes")]
        public int? PreparationMinutes { get; set; }

        [JsonProperty("cookingMinutes")]
        public int? CookingMinutes { get; set; }

        [JsonProperty("extendedIngredients")]
        public List<ExtendedIngredientDTO> Ingredients { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("readyInMinutes")]
        public int? ReadyInMinutes { get; set; }

        [JsonProperty("servings")]
        public decimal? Servings { get; set; }

        [JsonProperty("sourceUrl")]
        public string SourceUrl { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }       

        [JsonProperty("dishTypes")]
        public List<string> DishTypes { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        [JsonProperty("analyzedInstructions")]
        public List<AnalyzedInstructionsDTO> AnalyzedInstructions { get; set; }

        [JsonProperty("spoonacularSourceUrl")]
        public string SpoonacularSourceUrl { get; set; }
    }
}
