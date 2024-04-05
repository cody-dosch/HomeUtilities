using SpoonacularDAL.DTOs;
using SpoonacularDAL.Responses;

namespace HomeUtilities.Models.MealPlan
{
    public class MealPlanResults
    {
        // TODO: Make this a unified list of recipes that will take a generic shape, so that our shared recipes can be in the same list
        public List<RecipeDTO> Recipes { get; set; }
    }
}
