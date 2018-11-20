using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

using System.Linq;
using CookBook.Model;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Newtonsoft.Json;

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
            GetByPictureCommand = new Command(async () => await GetRecipeByImage());
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to query location: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        private async Task GetRecipeByImage()
        {
            try
            {
                var image = await TakePhoto();
                if(image != null)
                {
                    var tag = await GetTagByPictureAsync(image);
                    await Application.Current.MainPage.DisplayAlert("Ca a l'air bon !", $"On affiche des recette de {tag.TagName} ? ({tag.Probability:P1} sûr)", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get tags: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        private async Task<Stream> TakePhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "Aucun appareil photo détecté", "Votre appareil ne dispose pas ou ne trouve pas d'appareil photo.");
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                CompressionQuality = 92,
            });

            Stream fileStream = file.GetStream();

            return fileStream;
        }

        async Task<PredictionModel> GetTagByPictureAsync(Stream image)
        {
            string SouthCentralUsEndpoint = "https://southcentralus.api.cognitive.microsoft.com";

            // Add your training & prediction key from the settings page of the portal
            string trainingKey = "2879f3d6240a4805bffe13b64758e02c";
            string predictionKey = "ddbeaf61ee2e4ef99432de15336630d1";

            // Create the Api, passing in the training key
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = trainingKey,
                Endpoint = SouthCentralUsEndpoint
            };

            // Create a new project
            Guid projectId = new Guid("1bb9aeab-ce73-4025-947a-e04f216ea804");
            var project = trainingApi.GetProject(projectId);

            // Now there is a trained endpoint, it can be used to make a prediction

            // Create a prediction endpoint, passing in obtained prediction key
            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = predictionKey,
                Endpoint = SouthCentralUsEndpoint
            };

            // Make a prediction against the new project
            Console.WriteLine("Making a prediction:");
            var result = endpoint.PredictImage(project.Id, image);
            var tag = result.Predictions.FirstOrDefault();

            return tag;
        }
    }
}
