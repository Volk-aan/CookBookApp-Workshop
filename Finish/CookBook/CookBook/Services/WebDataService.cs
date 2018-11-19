using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CookBook.Model;
using CookBook.Services;
using Xamarin.Forms;

[assembly:Dependency(typeof(WebDataService))]
namespace CookBook.Services
{
    public class WebDataService : IDataService
    {
        HttpClient httpClient;
        HttpClient Client => httpClient ?? (httpClient = new HttpClient());
        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            var json = await Client.GetStringAsync("http://www.croustipeze.com/ressources/recipesdata.json");
            var all = Recipe.FromJson(json);
            return all;
        }
    }
}
