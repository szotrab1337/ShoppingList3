using ShoppingList.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingList.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportShopPage : ContentPage
    {
        ImportShopViewModel viewModel;
        public ImportShopPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ImportShopViewModel(Navigation);
        }
    }
}