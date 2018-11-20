using CookBook.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CookBook.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Recipe selectedRecipe)
            {
                ListView listView = (ListView) sender;
                listView.SelectedItem = null;

                await Navigation.PushAsync(new DetailsPage(selectedRecipe));
            }
        }
    }
}
