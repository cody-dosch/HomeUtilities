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
        private UserDataService _userDataService;

        public MealPlanController(ILogger<HomeController> logger, SpoonacularDAL.SpoonacularDAL spoonacularDAL, UserDataService userDataService)
        {
            _logger = logger;
            _spoonacularDAL = spoonacularDAL;
            _userDataService = userDataService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var searchModel = HttpContext.Session.GetObject<MealPlanSearchModel>(SessionKeys.RecipeSearchParameters) ?? new MealPlanSearchModel();

            searchModel.AllTags = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Main Course", Value = "main course" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Appetizer", Value = "Appetizer" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Side Dish", Value = "side dish" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Dessert", Value = "dessert" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Salad", Value = "salad" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Bread", Value = "bread" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Breakfast", Value = "breakfast" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Soup", Value = "soup" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Beverage", Value = "beverage" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Sauce", Value = "sauce" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Marinade", Value = "marinade" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Fingerfood", Value = "fingerfood" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Snack", Value = "snack" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Drink", Value = "drink" }
            };

            searchModel.AllTags = searchModel.AllTags.OrderBy(t => t.Text).ToList();

            return View(searchModel);
        }

        public async Task<IActionResult> GenerateMeals(MealPlanSearchModel searchModel)
        {
            // If we already have search results, and we don't want to refresh them - return the existing results
            var searchResults = HttpContext.Session.GetObject<List<RecipeSummaryModel>>(SessionKeys.RecipeResults);

            if (searchResults != null && !searchModel.RefreshResults)
            {
                searchModel.Results = searchResults;
                return View("Results", searchModel);
            }

            // Otherwise, make the spoonacular request to get new search results
            var getMealsRequest = new SpoonacularDAL.Requests.GetRandomRecipesRequest
            {
                Quantity = searchModel.TotalRecipes,
                IncludedTags = searchModel.IncludedTags,
                ExcludedTags = searchModel.ExcludedTags
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

            // Store the new search parameters and results in the session
            HttpContext.Session.SetObject(SessionKeys.RecipeResults, searchModel.Results);

            // Set RefreshResults to false so the back button doesn't load new results
            searchModel.RefreshResults = false;
            HttpContext.Session.SetObject(SessionKeys.RecipeSearchParameters, searchModel);

            return View("Results", searchModel);
        }

        public async Task<IActionResult> GenerateMoreMeals()
        {
            // Use the existing search parameters to search for more recipes and append them to the current search results
            var searchModel = HttpContext.Session.GetObject<MealPlanSearchModel>(SessionKeys.RecipeSearchParameters);

            var searchResults = HttpContext.Session.GetObject<List<RecipeSummaryModel>>(SessionKeys.RecipeResults);
            searchModel.Results = searchResults;

            // Otherwise, make the spoonacular request to get new search results
            var getMealsRequest = new SpoonacularDAL.Requests.GetRandomRecipesRequest
            {
                Quantity = searchModel.TotalRecipes,
                IncludedTags = searchModel.IncludedTags,
                ExcludedTags = searchModel.ExcludedTags
            };

            var getMealsResponse = await _spoonacularDAL.GetRandomRecipes(getMealsRequest);
            searchModel.Results.AddRange(getMealsResponse.Recipes.Select(r => new RecipeSummaryModel
            {
                SpoonacularId = r.Id,
                Title = r.Title,
                ReadyInMinutes = r.ReadyInMinutes,
                SourceUrl = r.SourceUrl,
                ImageUrl = r.ImageUrl,
                Summary = r.Summary,
                DishTypes = string.Join(',', r.DishTypes)
            }
            ).ToList());

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
            recipeDetailsViewModel.DishTypes = string.Join(", ", recipe.DishTypes);
            recipeDetailsViewModel.Servings = recipe.Servings;
            recipeDetailsViewModel.ReadyInMinutes = recipe.ReadyInMinutes;
            recipeDetailsViewModel.Instructions = recipe.AnalyzedInstructions?.FirstOrDefault()?.Steps?.Select(i => i?.Step)?.ToList() ?? new List<string>();
            recipeDetailsViewModel.Ingredients = recipe.Ingredients?.Select(i => i.Original)?.ToList() ?? new List<string>();
            recipeDetailsViewModel.ImageUrl = recipe.ImageUrl;
            recipeDetailsViewModel.SourceUrl = recipe.SourceUrl;

            // Check if the recipe is in our saved recipes
            var savedRecipes = await _userDataService.GetSavedRecipesAsync();
            recipeDetailsViewModel.IsSaved = savedRecipes.Contains(recipe.Id);

            // 2. Return the recipe details view
            return View("RecipeDetails", recipeDetailsViewModel);
        }

        public async Task<IActionResult> RecipeDetailsBack()
        {
            // Return to the results page with RefreshResults set to false
            var searchModel = HttpContext.Session.GetObject<MealPlanSearchModel>(SessionKeys.RecipeSearchParameters) ?? new MealPlanSearchModel();
            searchModel.RefreshResults = false;

            // 2. Return the recipe details view
            return await GenerateMeals(searchModel);
        }

        public async Task<IActionResult> SaveRecipe([FromQuery] int recipeId)
        {
            var success = await _userDataService.AddSavedRecipeAsync(recipeId);
            return new JsonResult(success);
        }

        public async Task<IActionResult> RemoveSavedRecipe([FromQuery] int recipeId)
        {
            var success = await _userDataService.RemoveSavedRecipeAsync(recipeId);
            return new JsonResult(success);
        }
    }
}
