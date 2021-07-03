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
    public partial class ShopsPage : ContentPage
    {
        ShopsViewModel viewModel;
        public ShopsPage()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new ShopsViewModel(Navigation);
        }
    }
}