using CookBook.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CookBook.ViewModel
{
    public class RecipeDetailsViewModel : BaseViewModel
    {
        public Command OpenMapCommand { get; }

        public RecipeDetailsViewModel()
        {

            OpenMapCommand = new Command(async () => await OpenMapAsync()); 
        }

        public RecipeDetailsViewModel(Recipe recipe) 
            : this()
        {
            Recipe = recipe;
            Title = $"{Recipe.Name} Details";
        }
        Recipe recipe;
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

        async Task OpenMapAsync()
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
    }
}
