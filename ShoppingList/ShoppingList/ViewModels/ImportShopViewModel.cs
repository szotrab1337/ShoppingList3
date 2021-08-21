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
                Shops = new ObservableCollection<ApiShop>(apiShops.OrderByDescending(x => x.LastModifiedOnDate));
                AssignNumbers();
            }
            catch
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + "Nie udało się pobrać list zakupów. " +
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
    }
}
