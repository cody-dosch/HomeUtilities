using HomeUtilities.Models;
using HomeUtilities.Models.MealPlan;
using Microsoft.AspNetCore.Mvc;
using SpoonacularDAL;
using System.Diagnostics;

namespace HomeUtilities.Controllers
{
    public class MealPlanController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SpoonacularDAL.SpoonacularDAL _spoonacularDAL;

        public MealPlanController(ILogger<HomeController> logger, SpoonacularDAL.SpoonacularDAL spoonacularDAL)
        {
            _logger = logger;
            _spoonacularDAL = spoonacularDAL;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GenerateMeals()
        {
            var resultsModel = new MealPlanResults();

            var getMealsRequest = new SpoonacularDAL.Requests.GetRandomRecipesRequest
            {
                Quantity = 1
            };

            var getMealsResponse = await _spoonacularDAL.GetRandomRecipes(getMealsRequest);
            resultsModel.Recipes = getMealsResponse.Recipes;

            return View("Results", resultsModel);
        }
    }
}
