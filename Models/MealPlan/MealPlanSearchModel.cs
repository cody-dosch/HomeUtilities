using Microsoft.AspNetCore.Mvc.Rendering;
using SpoonacularDAL.DTOs;
using SpoonacularDAL.Responses;
using System.ComponentModel.DataAnnotations;

namespace HomeUtilities.Models.MealPlan
{
    public class MealPlanSearchModel
    {
        [Display(Name = "Total Meals")]
        public int TotalRecipes { get; set; }

        [Display(Name = "Saved Meals")]
        public int NumSavedMeals { get; set; }

        [Display(Name = "Included Tags")]
        public List<string> IncludedTags { get; set; }

        [Display(Name = "Excluded Tags")]
        public List<string> ExcludedTags { get; set; }

        public List<SelectListItem> AllTags { get; set; }

        // TODO: Make this a unified list of recipes that will take a generic shape, so that our shared recipes can be in the same list
        public List<RecipeDTO> Results { get; set; }
    }
}
