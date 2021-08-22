using Acr.UserDialogs;
using Newtonsoft.Json;
using ShoppingList.Models;
using ShoppingList.Models.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class ImportShopViewModel : BaseViewModel
    {
        public ImportShopViewModel(INavigation navigation)
        {
            Navigation = navigation;
            Title = "Importowanie listy";

            DeleteItemCommand = new Command<ApiShop>(DeleteShopAction);
            InitializeImportCommand = new Command<ApiShop>(InitializeImportAction);
            RefreshCommand = new Command(() =>
            {
                IsRefreshing = true;
                LoadListsAvailableToImport();
                IsRefreshing = false;
            });

            LoadListsAvailableToImport();
        }

        public INavigation Navigation { get; set; }

        public ICommand RefreshCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand InitializeImportCommand { get; set; }

        public ObservableCollection<ApiShop> Shops
        {
            get => _Shops;
            set { _Shops = value; OnPropertyChanged("Shops"); }
        }
        private ObservableCollection<ApiShop> _Shops;

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set { _IsRefreshing = value; OnPropertyChanged("IsRefreshing"); }
        }
        private bool _IsRefreshing;

        private void LoadListsAvailableToImport()
        {
            try
            {
                List<ApiShop> apiShops = JsonConvert.DeserializeObject<List<ApiShop>>(ApiAdapter.GetShops());
                Shops = new ObservableCollection<ApiShop>(apiShops.Where(x => x.Items.Count > 0).OrderByDescending(x => x.LastModifiedOnDate));
                AssignNumbers();
            }
            catch
            {
                UserDialogs.Instance.Alert("Nie udało się pobrać list zakupów. " +
                    "Sprawdź czy jesteś w lokalnej sieci domowej oraz czy komputer stacjonarny jest włączony", "Błąd", "OK");
                Shops = new ObservableCollection<ApiShop>();
            }
        }

        private void AssignNumbers()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Numerowanie...", MaskType.Black);

                Shops.ToList().ForEach(x => x.Number = Shops.IndexOf(x) + 1);

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void DeleteShopAction(ApiShop shop)
        {
            try
            {
                bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = "Czy na pewno chcesz usunąć listę do sklepu " + shop.Name + "?",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie",
                    AndroidStyleId = 2131689474
                });

                if (!result)
                    return;

                ApiAdapter.DeleteShop(shop.ShopId.ToString());
                Shops.Remove(shop);
                AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void InitializeImportAction(ApiShop shop)
        {
            try
            {
                bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = "Czy na pewno chcesz usunąć listę do sklepu " + shop.Name + "?",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie",
                    AndroidStyleId = 2131689474
                });

                if (!result)
                    return;

                ApiAdapter.DeleteShop(shop.ShopId.ToString());
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
