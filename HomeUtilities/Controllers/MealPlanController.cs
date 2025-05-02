using HomeUtilities.Models;
using HomeUtilities.Models.MealPlan;
using HomeUtilities.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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

        [HttpPost]
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

            return RedirectToAction("Results", searchModel);
        }

        [HttpPost]
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

            return RedirectToAction("Results", searchModel);
        }

        [HttpGet]
        public IActionResult Results()
        {
            var searchModel = HttpContext.Session.GetObject<MealPlanSearchModel>(SessionKeys.RecipeSearchParameters) ?? new MealPlanSearchModel();
            searchModel.Results = HttpContext.Session.GetObject<List<RecipeSummaryModel>>(SessionKeys.RecipeResults) ?? new List<RecipeSummaryModel>();
            return View("Results", searchModel);
        }

        [HttpGet]
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
            var savedRecipeIds = await _userDataService.GetSavedRecipesAsync();
            recipeDetailsViewModel.IsSaved = savedRecipeIds.Contains(recipe.Id);

            // Check if the recipe is in our selected recipes
            var selectedRecipeIds = HttpContext.Session.GetObject<List<int?>>(SessionKeys.SelectedRecipes) ?? new List<int?>();
            recipeDetailsViewModel.IsSelected = selectedRecipeIds.Contains(recipe.Id);

            // 2. Return the recipe details view
            return View("RecipeDetails", recipeDetailsViewModel);
        }

        #region Saved Recipes

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

        public async Task<IActionResult> SavedRecipes()
        {
            // Get the list of saved recipe ids from the user data
            var savedRecipeIds = await _userDataService.GetSavedRecipesAsync();

            var savedRecipes = new List<RecipeSummaryModel>();

            // Fetch the details for each saved recipe
            if (savedRecipeIds?.Any() ?? false) {
                foreach (var savedRecipeId in savedRecipeIds)
                {
                    var recipe = await _spoonacularDAL.GetRecipeInformation(savedRecipeId);

                    if (recipe == null)
                        continue;

                    savedRecipes.Add(new RecipeSummaryModel
                    {
                        SpoonacularId = recipe.Id,
                        Title = recipe.Title,
                        ReadyInMinutes = recipe.ReadyInMinutes,
                        SourceUrl = recipe.SourceUrl,
                        ImageUrl = recipe.ImageUrl,
                        Summary = recipe.Summary,
                        DishTypes = string.Join(',', recipe.DishTypes)
                    });
                }
            }

            return View("SavedRecipes", savedRecipes);
        }

        #endregion

        #region Selected Recipes

        public async Task<IActionResult> SelectRecipe([FromQuery] int recipeId)
        {
            bool success = false;

            // Add the recipe to the selected recipes in Session.
            var selectedRecipeIds = HttpContext.Session.GetObject<List<int?>>(SessionKeys.SelectedRecipes) ?? new List<int?>();

            if (!selectedRecipeIds?.Contains(recipeId) ?? true)
                selectedRecipeIds.Add(recipeId);

            HttpContext.Session.SetObject(SessionKeys.SelectedRecipes, selectedRecipeIds);

            success = true;

            return new JsonResult(success);
        }

        public async Task<IActionResult> UnselectRecipe([FromQuery] int recipeId)
        {
            bool success = false;

            // Remove the recipe from the selected recipes in Session.
            var selectedRecipeIds = HttpContext.Session.GetObject<List<int?>>(SessionKeys.SelectedRecipes) ?? new List<int?>();

            if (selectedRecipeIds?.Contains(recipeId) ?? false)
                selectedRecipeIds.Remove(recipeId);

            HttpContext.Session.SetObject(SessionKeys.SelectedRecipes, selectedRecipeIds);

            success = true;

            return new JsonResult(success);
        }

        public async Task<IActionResult> SelectedRecipes()
        {
            // Get the list of saved recipe ids from the user data
            var selectedRecipeIds = HttpContext.Session.GetObject<List<int?>>(SessionKeys.SelectedRecipes) ?? new List<int?>();

            var selectedRecipes = new List<RecipeSummaryModel>();

            // Fetch the details for each saved recipe
            if (selectedRecipeIds?.Any() ?? false)
            {
                foreach (var selectedRecipeId in selectedRecipeIds)
                {
                    var recipe = await _spoonacularDAL.GetRecipeInformation(selectedRecipeId);

                    if (recipe == null)
                        continue;

                    selectedRecipes.Add(new RecipeSummaryModel
                    {
                        SpoonacularId = recipe.Id,
                        Title = recipe.Title,
                        ReadyInMinutes = recipe.ReadyInMinutes,
                        SourceUrl = recipe.SourceUrl,
                        ImageUrl = recipe.ImageUrl,
                        Summary = recipe.Summary,
                        DishTypes = string.Join(',', recipe.DishTypes)
                    });
                }
            }

            return View("SelectedRecipes", selectedRecipes);
        }

        #endregion
    }
}
