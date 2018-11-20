using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using CookBook.Model;

namespace CookBook.ViewModel
{
    public class RecipeDetailsViewModel : BaseViewModel
    {
        #region Properties

        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set
            {
                if (recipe == value)
                    return;

                recipe = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public Command OpenMapCommand { get; }

        #endregion

        #region Constructors
        
        public RecipeDetailsViewModel() : base(title: "Details")
        {
            OpenMapCommand = new Command(async () => await OpenMapAsync()); 
        }

        public RecipeDetailsViewModel(Recipe recipe) : base(title: $"{recipe.Name} Details")
        {
            Recipe = recipe;
            OpenMapCommand = new Command(async () => await OpenMapAsync());
        }

        #endregion

        #region Methods

        private async Task OpenMapAsync()
        {
            try
            {
                await Maps.OpenAsync(Recipe.Latitude, Recipe.Longitude);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to launch maps: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error, no Maps app!", ex.Message, "OK");
            }
        }

        #endregion
    }
}
