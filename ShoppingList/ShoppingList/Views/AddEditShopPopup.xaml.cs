using ShoppingList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingList.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditShopPopup : Popup<Shop>
    {
        public AddEditShopPopup(Shop shop, string title)
        {
            InitializeComponent();
            this.Shop = shop;
            Title.Text = title;
            Name.Text = Shop.Name;

            if (string.IsNullOrEmpty(Name.Text))
            {
                SaveButton.IsEnabled = false;
                SaveButton.BackgroundColor = Color.FromHex("#004d45");
                SaveButton.TextColor = Color.FromHex("#616161");
            }
        }

        Shop Shop { get; set; }

        private void Confirm_Clicked(object sender, EventArgs e)
        {
            Shop.Name = Name.Text;
            Dismiss(Shop);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Dismiss(null);
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                SaveButton.IsEnabled = false;
                SaveButton.BackgroundColor = Color.FromHex("#004d45");
                SaveButton.TextColor = Color.FromHex("#616161");
            }
            else
            {
                SaveButton.IsEnabled = true;
                SaveButton.BackgroundColor = Color.FromHex("#00897B");
                SaveButton.TextColor = Color.FromHex("#dedede");
            }
        }
    }
}