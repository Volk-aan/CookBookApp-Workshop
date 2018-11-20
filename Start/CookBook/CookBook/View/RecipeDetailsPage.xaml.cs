using CookBook.Model;
using CookBook.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CookBook.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecipeDetailsPage : ContentPage
    {
        public RecipeDetailsPage()
        {
            InitializeComponent();
        }

        public RecipeDetailsPage(Recipe recipe)
        {
            InitializeComponent();
            BindingContext = new RecipeDetailsViewModel(recipe);
        }
    }
}