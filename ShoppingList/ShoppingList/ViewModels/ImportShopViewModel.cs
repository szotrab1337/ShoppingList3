using Acr.UserDialogs;
using Newtonsoft.Json;
using ShoppingList.Models;
using ShoppingList.Models.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Xamarin.Essentials;
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
            ImportFromClipboardCommand = new Command(ImportFromClipboardAction);
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
        public ICommand ImportFromClipboardCommand { get; set; }

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
                    "Sprawdź czy jesteś w lokalnej sieci domowej oraz czy komputer stacjonarny jest włączony.", "Błąd", "OK");
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

        private async void ImportFromClipboardAction()
        {
            try
            {
                string shopJson = await Clipboard.GetTextAsync();
                ApiShop apiShop = JsonConvert.DeserializeObject<ApiShop>(shopJson);

                if(apiShop.Items.Count == 0)
                {
                    UserDialogs.Instance.Toast("Na liście nie ma żadnych przedmiotów.");
                    return;
                }

                InitializeImportAction(apiShop);
            }
            catch
            {
                UserDialogs.Instance.Toast("Skopiowany teskt jest nieprawidłowy.");
            }
        }
        
        private async void InitializeImportAction(ApiShop shop)
        {
            try
            {
                string[] choices = new[] { "Utwórz nowy sklep", "Importuj do istniejącego sklepu" };

                string result = await UserDialogs.Instance.ActionSheetAsync("Wybierz...", string.Empty, "Anuluj", CancellationToken.None, choices);

                if (string.IsNullOrWhiteSpace(result))
                    return;

                if (result.Equals("Utwórz nowy sklep"))
                    ImportToNewShop(shop);

                if (result.Equals("Importuj do istniejącego sklepu"))
                    ImportToExistingShop(shop);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void ImportToNewShop(ApiShop shop)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Trwa importowanie...", MaskType.Black);

                Shop shopDB = await App.Database.GetShopByNameAsync(shop.Name);
                List<Shop> shops = await App.Database.GetShopsAsync();

                Shop newShop = new Shop()
                {
                    Name = shopDB != null ? shop.Name + "_Imp_" + shop.ShopId.ToString() : shop.Name,
                    Number = shops != null ? shops.Count + 1 : 1 
                };

                await App.Database.InsertShopAsync(newShop);
                shopDB = await App.Database.GetShopByNameAsync(newShop.Name);

                List<Item> items = new List<Item>();

                foreach (ApiItem item in shop.Items)
                {
                    items.Add(new Item()
                    {
                        ShopId = shopDB.ShopId,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        IsBought = false,
                        Absent = false,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? "-1" : item.Description,
                        UnitId = item.UnitId
                    });
                }

                foreach (Item item in items)
                {
                    await App.Database.InsertItemAsync(item);
                }

                UserDialogs.Instance.HideLoading();
                MessagingCenter.Send(this, "RefreshItemsAfterImport");
                UserDialogs.Instance.Toast("Import zakończony powodzeniem.");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void ImportToExistingShop(ApiShop shop)
        {
            try
            {
                List<Shop> choicesRaw = (await App.Database.GetShopsAsync()).ToList();
                choicesRaw.ForEach(x => x.Name = x.Name + " ID: " + x.ShopId);
                string[] choices = choicesRaw.Select(x => x.Name).ToArray();

                if (choices.Length == 0)
                {
                    UserDialogs.Instance.Toast("Brak sklepu, do którego można zaimportować listę.");
                    return;
                }

                string result = await UserDialogs.Instance.ActionSheetAsync("Wybierz...", string.Empty, "Anuluj", CancellationToken.None, choices);

                UserDialogs.Instance.Toast(result.ToString());

                if (string.IsNullOrWhiteSpace(result))
                    return;

                int shopId = Convert.ToInt32(result.Substring(result.IndexOf("ID: ") + 4));

                List<Item> items = new List<Item>();

                foreach (ApiItem item in shop.Items)
                {
                    items.Add(new Item()
                    {
                        ShopId = shopId,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        IsBought = false,
                        Absent = false,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? "-1" : item.Description,
                        UnitId = item.UnitId
                    });
                }

                foreach (Item item in items)
                {
                    await App.Database.InsertItemAsync(item);
                }

                UserDialogs.Instance.HideLoading();
                MessagingCenter.Send(this, "RefreshItemsAfterImport");
                UserDialogs.Instance.Toast("Import zakończony powodzeniem.");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
    }
}
