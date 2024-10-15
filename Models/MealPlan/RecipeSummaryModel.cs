using Microsoft.AspNetCore.Mvc.Rendering;
using SpoonacularDAL.DTOs;
using SpoonacularDAL.Responses;
using System.ComponentModel.DataAnnotations;

namespace HomeUtilities.Models.MealPlan
{
    public class RecipeSummaryModel
    {
        public int SpoonacularId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string DishTypes { get; set; }

        public int? ReadyInMinutes { get; set; }

        public string ImageUrl { get; set; }

        public string SourceUrl { get; set; }
    }
}
