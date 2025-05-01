using System;
using System.Net;
using Newtonsoft.Json;
using SpoonacularDAL.DTOs;

namespace SpoonacularDAL.Responses
{
    public class GetRandomRecipesResponse : BaseResponse
    {
        public List<RecipeDTO> Recipes { get; set; }
    }
}
