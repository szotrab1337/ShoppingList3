using ShoppingList.Models;
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
    public partial class AddEditItemPage : ContentPage
    {
        AddEditItemViewModel viewModel;
        public AddEditItemPage(bool addMode, Item item)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddEditItemViewModel(Navigation, addMode, item);
        }
    }
}