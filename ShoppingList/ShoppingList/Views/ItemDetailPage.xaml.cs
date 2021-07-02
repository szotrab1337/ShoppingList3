using ShoppingList.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace ShoppingList.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}