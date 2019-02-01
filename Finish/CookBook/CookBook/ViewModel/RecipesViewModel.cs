using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

using System.Linq;
using CookBook.Model;
using System.Diagnostics;
using System.Collections.ObjectModel;
using CookBook.Helper;
using Newtonsoft.Json;
using Plugin.Permissions.Abstractions;

namespace CookBook.ViewModel
{
    public class RecipesViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<Recipe> Recipes { get; }

        #endregion

        #region Commands

        public Command GetRecipesCommand { get; }
        public Command GetClosestCommand { get; }

        #endregion
       
        #region Constructors
        
        public RecipesViewModel() : base(title: "Recipes")
        {
            Recipes = new ObservableCollection<Recipe>();

            GetRecipesCommand = new Command(async () => await GetRecipesAsync());
            GetClosestCommand = new Command(async () => await GetClosestAsync());
        }

        #endregion

        #region Methods

        private async Task GetRecipesAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                string jsonRecipes = await Client.GetStringAsync("http://www.croustipeze.com/ressources/recipesdata.json");
                Recipe[] recipes = JsonConvert.DeserializeObject<Recipe[]>(jsonRecipes, Converter.Settings);

                Recipes.Clear();
                foreach (var recipe in recipes)
                    Recipes.Add(recipe);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get recipes: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetClosestAsync()
        {
            if (IsBusy || Recipes == null || !Recipes.Any())
                return;

            try
            {
                if (await PermissionsManager.RequestPermissions(new[] {Permission.Location}))
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location == null)
                    {
                        location = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30)
                        });
                    }

                    var closestRecipe = Recipes
                        .OrderBy(m => location.CalculateDistance(new Location(m.Latitude, m.Longitude), DistanceUnits.Miles))
                        .FirstOrDefault();

                    if(closestRecipe == null)
                        await Application.Current.MainPage.DisplayAlert("No recipe found", "Something went wrong !", "OK");
                    else
                        await Application.Current.MainPage.DisplayAlert("Closest recipe", closestRecipe.Name + " at " + closestRecipe.Location, "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to query location: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        #endregion
    }
}
