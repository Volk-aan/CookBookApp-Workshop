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
        public RecipeDetailsViewModel()
        {
            
        }

        public RecipeDetailsViewModel(Recipe recipe) 
            : this()
        {
            
        }
    }
}
