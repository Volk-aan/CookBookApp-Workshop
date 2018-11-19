using CookBook.Model;
using CookBook.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CookBook.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
        }
        public DetailsPage(Recipe recipe)
        {
            InitializeComponent();
            BindingContext = new RecipeDetailsViewModel(recipe);
        }
    }
}