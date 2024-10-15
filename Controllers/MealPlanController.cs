using HomeUtilities.Models;
using HomeUtilities.Models.MealPlan;
using HomeUtilities.Session;
using Microsoft.AspNetCore.Mvc;
using SpoonacularDAL;
using System.Diagnostics;
using System.Text;

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

        public async Task<IActionResult> GenerateMeals(MealPlanSearchModel searchModel)
        {
            var searchResults = HttpContext.Session.GetObject<List<RecipeSummaryModel>>(SessionKeys.RecipeResults);

            // If we already have search results, and we don't want to refresh them - return the existing results
            if (searchResults != null && !searchModel.RefreshResults)
            {
                searchModel.Results = searchResults;
                return View("Results", searchModel);
            }              

            // Otherwise, make the spoonacular request to get new search results
            var getMealsRequest = new SpoonacularDAL.Requests.GetRandomRecipesRequest
            {
                Quantity = searchModel.TotalRecipes
            };

            var getMealsResponse = await _spoonacularDAL.GetRandomRecipes(getMealsRequest);
            searchModel.Results = getMealsResponse.Recipes.Select(r => new RecipeSummaryModel
            {
                SpoonacularId = r.Id,
                Title = r.Title,
                ReadyInMinutes = r.ReadyInMinutes,
                SourceUrl = r.SourceUrl,
                ImageUrl = r.ImageUrl,
                Summary = r.Summary,
                DishTypes = string.Join(',', r.DishTypes)
            }
            ).ToList();

            // Store the new results in the session
            HttpContext.Session.SetObject(SessionKeys.RecipeResults, searchModel.Results);         

            return View("Results", searchModel);
        }

        public async Task<IActionResult> RecipeDetails(int? spoonacularId)
        {
            var recipeDetailsViewModel = new RecipeDetailsViewModel();

            // 1. Pull the recipe from Spoonacular
            var recipe = await _spoonacularDAL.GetRecipeInformation(spoonacularId);

            if (recipe == null)
            {
                // TODO: Error
            }

            recipeDetailsViewModel.SpoonacularId = recipe.Id;
            recipeDetailsViewModel.Title = recipe.Title;
            recipeDetailsViewModel.Summary = recipe.Summary;
            recipeDetailsViewModel.DishTypes = string.Join(',', recipe.DishTypes);
            recipeDetailsViewModel.ReadyInMinutes = recipe.ReadyInMinutes;
            recipeDetailsViewModel.Instructions = recipe.AnalyzedInstructions?.FirstOrDefault()?.Steps?.Select(i => i?.Step)?.ToList() ?? new List<string>();
            recipeDetailsViewModel.Ingredients = recipe.Ingredients?.Select(i => i.Original)?.ToList() ?? new List<string>();
            recipeDetailsViewModel.ImageUrl = recipe.ImageUrl;
            recipeDetailsViewModel.SourceUrl = recipe.SourceUrl;

            // 2. Return the recipe details view
            return View("RecipeDetails", recipeDetailsViewModel);
        }
    }
}
