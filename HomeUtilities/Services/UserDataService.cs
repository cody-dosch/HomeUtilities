using HomeUtilities.Models.UserData;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;

public class UserDataService
{
    private readonly string _appDataRoot = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
    private ConcurrentDictionary<string, UserDataModel> _userDataDictionary;

    public UserDataService()
    {
        // Load all user data on startup
        _userDataDictionary = new ConcurrentDictionary<string, UserDataModel>();
        LoadAllUserDataAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public async Task<List<int>> GetSavedRecipesAsync()
    {
        // TODO: In the future we will pull the user id from session at this point.
        var userId = "app";

        try
        {
            var userData = await GetUserData(userId);
            return userData?.SavedRecipeIds ?? new List<int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading saved recipes for user {userId}: {ex.Message}");
            return new List<int>(); 
        }
    }

    public async Task<bool> AddSavedRecipeAsync(int recipeId)
    {
        bool success = false;

        // TODO: In the future we will pull the user id from session at this point.
        var userId = "app";

        try
        {
            var userData = await GetUserData(userId);

            // Add the recipe to the saved recipes list if it isn't there already
            if (userData?.SavedRecipeIds == null)
                userData.SavedRecipeIds = new List<int>();

            if (!userData?.SavedRecipeIds?.Any(r => r == recipeId) ?? true)
                userData.SavedRecipeIds.Add(recipeId);

            success = await SaveUserDataModel(userId, userData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding saved recipe for user {userId}: {ex.Message}");
        }

        return success;
    }

    public async Task<bool> RemoveSavedRecipeAsync(int recipeId)
    {
        bool success = false;

        // TODO: In the future we will pull the user id from session at this point.
        var userId = "app";

        try
        {
            var userData = await GetUserData(userId);

            // Remove the recipe from the saved recipes list if it is there
            if (userData?.SavedRecipeIds == null)
                userData.SavedRecipeIds = new List<int>();

            if (userData?.SavedRecipeIds?.Any(r => r == recipeId) ?? true)
                userData.SavedRecipeIds.Remove(recipeId);

            success = await SaveUserDataModel(userId, userData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing recipe for user {userId}: {ex.Message}");
        }

        return success;
    }

    private async Task LoadAllUserDataAsync()
    {
        // Get all user data files from App_Data and load them into our dictionary
        if (Directory.Exists(_appDataRoot))
        {
            var files = Directory.GetFiles(_appDataRoot, "user_data_*.json");
            foreach (var filePath in files)
            {
                try
                {
                    var userId = Path.GetFileNameWithoutExtension(filePath).Substring("user_data_".Length);
                    var jsonString = await File.ReadAllTextAsync(filePath);
                    var userData = JsonConvert.DeserializeObject<UserDataModel>(jsonString);
                    if (userData != null)
                    {
                        _userDataDictionary.TryAdd(userId, userData);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not deserialize user data from {filePath}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading user data from {filePath}: {ex.Message}");
                }
            }
        }
        else
        {
            Directory.CreateDirectory(_appDataRoot);
        }
    }

    private string GetUserFilePath(string userId)
    {
        return Path.Combine(_appDataRoot, $"user_data_{userId}.json");
    }

    private async Task<UserDataModel> GetUserData(string userId)
    {
        try
        {
            if (_userDataDictionary.ContainsKey(userId))
            {
                return _userDataDictionary[userId];
            }
            else
            {
                var userData = await LoadUserDataAsync(userId);
                _userDataDictionary.TryAdd(userId, userData);
                return userData;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user data for user {userId}: {ex.Message}");
            return new UserDataModel();
        }
    }

    private async Task<UserDataModel> LoadUserDataAsync(string userId)
    {
        string filePath = GetUserFilePath(userId);
        try
        {
            if (File.Exists(filePath))
            {
                var jsonString = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<UserDataModel>(jsonString) ?? new UserDataModel();
            }
            else
            {
                return new UserDataModel();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user data for user {userId}: {ex.Message}");
            return new UserDataModel();
        }
    }

    private async Task<bool> SaveUserDataModel(string userId, UserDataModel userData)
    {
        try
        {
            string filePath = GetUserFilePath(userId);

            var userDataJson = JsonConvert.SerializeObject(userData);
            await File.WriteAllTextAsync(filePath, userDataJson);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user data for user {userId}: {ex.Message}");
            return false;
        }
    }
}