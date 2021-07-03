using ShoppingList.ViewModels;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ShoppingList
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
