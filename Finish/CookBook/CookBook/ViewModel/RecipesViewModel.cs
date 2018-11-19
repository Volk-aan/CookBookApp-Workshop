using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

using System.Linq;
using CookBook.Model;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace CookBook.ViewModel
{
    public class RecipesViewModel : BaseViewModel
    {
        public ObservableCollection<Recipe> Recipes { get; }
        public Command GetRecipesCommand { get; }
        public Command GetClosestCommand { get; }
        public RecipesViewModel()
        {
            Title = "Recipes";
            Recipes = new ObservableCollection<Recipe>();
            GetRecipesCommand = new Command(async () => await GetRecipesAsync());
            GetClosestCommand = new Command(async () => await GetClosestAsync());
        }

    
        async Task GetRecipesAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var recipes = await DataService.GetRecipesAsync();

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

        async Task GetClosestAsync()
        {
            if (IsBusy || Recipes.Count == 0)
                return;
            try
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

                var first = Recipes.OrderBy(m => location.CalculateDistance(
                    new Location(m.Latitude, m.Longitude), DistanceUnits.Miles))
                    .FirstOrDefault();

                await Application.Current.MainPage.DisplayAlert("", first.Name + " " +
                    first.Location, "OK");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to query location: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
        }


    }
}
