using HomeUtilities.Models.UserData;
using Newtonsoft.Json;
using System.Text.Json;

public class UserDataService
{
    // TODO: Once we implement different users, this will have to become user specific and read from Session.
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "app_data.json");
    private UserDataModel _userData;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // For thread-safe access

    public UserDataService()
    {
        // Initialize _userData in the constructor
        _userData = LoadUserDataAsync().Result; // Be cautious using .Result, see note below
    }

    public async Task<List<int>> GetSavedRecipesAsync()
    {
        try
        {
            // Get the saved recipe ids from the app data file if it exists
            if (File.Exists(_filePath))
            {
                var jsonString = await File.ReadAllTextAsync(_filePath);
                var userData = JsonConvert.DeserializeObject<UserDataModel>(jsonString);
                return userData?.SavedRecipeIds ?? new List<int>();
            }
            else
            {
                // Return an empty list if the file doesn't exist
                return new List<int>(); 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading saved recipes from {_filePath}: {ex.Message}");
            return new List<int>(); 
        }
    }

    public async Task<bool> AddSavedRecipeAsync(int recipeId)
    {
        bool success = false;

        try
        {
            // Add the recipe to the saved recipes list if it isn't there already
            if (_userData?.SavedRecipeIds == null)
                _userData.SavedRecipeIds = new List<int>();

            if (!_userData?.SavedRecipeIds?.Any(r => r == recipeId) ?? true)
                _userData.SavedRecipeIds.Add(recipeId);

            success = await SaveUserDataModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding saved recipe to {_filePath}: {ex.Message}");
        }

        return success;
    }

    public async Task<bool> RemoveSavedRecipeAsync(int recipeId)
    {
        bool success = false;

        try
        {
            // 2. Remove the recipe from the saved recipes list if it is there
            if (_userData?.SavedRecipeIds == null)
                _userData.SavedRecipeIds = new List<int>();

            if (_userData?.SavedRecipeIds?.Any(r => r == recipeId) ?? true)
                _userData.SavedRecipeIds.Remove(recipeId);

            success = await SaveUserDataModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing recipe from {_filePath}: {ex.Message}");
        }

        return success;
    }

    private async Task<UserDataModel> LoadUserDataAsync()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var jsonString = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<UserDataModel>(jsonString) ?? new UserDataModel();
            }
            else
            {
                return new UserDataModel();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user data from {_filePath}: {ex.Message}");
            return new UserDataModel();
        }
    }

    private async Task<bool> SaveUserDataModel()
    {
        try
        {
            // Use a semaphore to ensure thread-safe writing
            await _semaphore.WaitAsync();
            var userDataJson = JsonConvert.SerializeObject(_userData);
            await File.WriteAllTextAsync(_filePath, userDataJson);
            _semaphore.Release();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user data: {ex.Message}");
            return false;
        }
    }
}