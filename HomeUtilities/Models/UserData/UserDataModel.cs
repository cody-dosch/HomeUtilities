using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HomeUtilities.Models.UserData
{
    public class UserDataModel
    {
        public List<int> SavedRecipeIds { get; set; }
    }
}
