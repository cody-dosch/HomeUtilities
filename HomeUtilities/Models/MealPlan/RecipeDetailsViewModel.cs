using Microsoft.AspNetCore.Mvc.Rendering;
using SpoonacularDAL.DTOs;
using SpoonacularDAL.Responses;
using System.ComponentModel.DataAnnotations;

namespace HomeUtilities.Models.MealPlan
{
    public class RecipeDetailsViewModel
    {
        public int SpoonacularId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string DishTypes { get; set; }

        public decimal? Servings { get; set; }

        public int? ReadyInMinutes { get; set; }

        public List<string> Instructions { get; set; }

        public List<string> Ingredients { get; set; }

        public string ImageUrl { get; set; }

        public string SourceUrl { get; set; }

        public bool IsSaved { get; set; }

        public bool IsSelected { get; set; }
    }
}
