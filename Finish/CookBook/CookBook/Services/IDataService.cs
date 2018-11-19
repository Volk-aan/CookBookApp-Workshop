using CookBook.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookBook.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Recipe>> GetRecipesAsync();
    }
}
