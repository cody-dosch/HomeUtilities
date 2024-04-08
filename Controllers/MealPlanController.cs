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

        [HttpGet]
        public IActionResult Index()
        {
            var searchModel = new MealPlanSearchModel();

            searchModel.AllTags = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Test 1", Value = "Test 1" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Test 2", Value = "Test 2" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Test 3", Value = "Test 3" }
            };

            return View(searchModel);
        }

        public async Task<IActionResult> GenerateMeals()
        {
            var resultsModel = new MealPlanSearchModel();

            var getMealsRequest = new SpoonacularDAL.Requests.GetRandomRecipesRequest
            {
                Quantity = 1
            };

            var getMealsResponse = await _spoonacularDAL.GetRandomRecipes(getMealsRequest);
            resultsModel.Results = getMealsResponse.Recipes;

            return View("Results", resultsModel);
        }
    }
}
