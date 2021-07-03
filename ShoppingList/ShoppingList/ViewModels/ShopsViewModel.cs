using Acr.UserDialogs;
using ShoppingList.Models;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class ShopsViewModel : BaseViewModel
    {
        public ShopsViewModel(INavigation navigation)
        {
            this.Navigation = navigation;

            Title = "Lista sklepów";
            

            OpenShopCommand = new Command(OpenShopAction);
            AddShopCommand = new Command(AddShopAction);
            EditShopCommand = new Command(EditShopAction);
            DeleteShopCommand = new Command(DeleteShopAction);

            LoadShops();
        }

        public INavigation Navigation { get; set; }

        public ICommand OpenShopCommand { get; set; }
        public ICommand AddShopCommand { get; set; }
        public ICommand EditShopCommand { get; set; }
        public ICommand DeleteShopCommand { get; set; }

        public ObservableCollection<Shop> Shops
        {
            get { return _Shops; }
            set { _Shops = value; OnPropertyChanged("Shops"); }
        }
        private ObservableCollection<Shop> _Shops;

        public async void LoadShops()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Ładowanie...", MaskType.Black);

                Shops = new ObservableCollection<Shop>(await App.Database.GetShopsAsync());
                AssignNumbers();

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void AssignNumbers()
        {
            UserDialogs.Instance.ShowLoading("Ładowanie...", MaskType.Black);
            Shops.ToList().ForEach(x => x.Number = Shops.IndexOf(x) + 1);
            UserDialogs.Instance.HideLoading();
        }

        public void OpenShopAction(object sender)
        {
            try
            {
                Shop shop = (Shop)sender;

                //UserDialogs.Instance.Alert(shop.Name, "Błąd", "OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public async void AddShopAction()
        {
            try
            {
                Shop initialShop = new Shop();
                Shop shop = await Navigation.ShowPopupAsync(new AddEditShopPopup(initialShop, "Nowy sklep"));

                if (shop is null)
                    return;

                await App.Database.SaveShopAsync(shop);

                Shops.Add(shop);
                AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        public async void EditShopAction(object sender)
        {
            try
            {
                Shop initialShop = (Shop)sender;
                Shop shop = await Navigation.ShowPopupAsync(new AddEditShopPopup(initialShop, "Edycja sklepu"));

                if (shop is null)
                    return;

                await App.Database.UpdateShopAsync(shop);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        public async void DeleteShopAction(object sender)
        {
            try
            {
                Shop shop = (Shop)sender;                

                if (shop is null)
                    return;

                bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = "Czy na pewno chcesz usunąć sklep " + shop.Name + "?",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie",
                    AndroidStyleId = 2131689474
                });

                if (!result)
                    return;

                await App.Database.DeleteShopAsync(shop);
                Shops.Remove(shop);
                AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }


    }
}
