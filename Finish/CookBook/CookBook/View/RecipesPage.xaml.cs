using CookBook.Model;
using Xamarin.Forms;

namespace CookBook.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var recipe = e.SelectedItem as Recipe;
            if (recipe == null)
                return;

            await Navigation.PushAsync(new DetailsPage(recipe));

            ((ListView)sender).SelectedItem = null;
        }
    }
}
