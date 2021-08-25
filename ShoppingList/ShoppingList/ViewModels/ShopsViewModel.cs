using Acr.UserDialogs;
using Newtonsoft.Json;
using ShoppingList.Models;
using ShoppingList.Models.Api;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class ShopsViewModel : BaseViewModel
    {
        public ShopsViewModel(INavigation navigation)
        {
            this.Navigation = navigation;

            Title = "Lista sklepów";

            OpenShopCommand = new Command<Shop>(OpenShopAction);
            AddShopCommand = new Command(AddShopAction);
            EditShopCommand = new Command<Shop>(EditShopAction);
            DeleteShopCommand = new Command<Shop>(DeleteShopAction);

            DragStartingCommand = new Command<Shop>(DragStartingAction);
            DragOverCommand = new Command<Shop>(DragOverAction);
            DragLeaveCommand = new Command(DragLeaveAction);
            ExportShopCommand = new Command<Shop>(ExportShopAction);
            DropCommand = new Command<Shop>(DropAction);
            RefreshCommand = new Command(() =>
            {
                IsRefreshing = true;
                LoadShops();
                AssignNumbers();
                IsRefreshing = false;
            });

            LoadShops();

            MessagingCenter.Subscribe<ItemsViewModel>(this, "RefreshItemsCount", (LoadAgain) => { CalculateItemsToBuy(); });
            MessagingCenter.Subscribe<ImportShopViewModel>(this, "RefreshItemsAfterImport", (LoadAgain) =>
            {
                LoadShops();
                AssignNumbers();
            });
        }

        public INavigation Navigation { get; set; }

        public ICommand OpenShopCommand { get; set; }
        public ICommand AddShopCommand { get; set; }
        public ICommand EditShopCommand { get; set; }
        public ICommand DeleteShopCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ExportShopCommand { get; set; }

        public ICommand DragStartingCommand { get; set; }
        public ICommand DragOverCommand { get; set; }
        public ICommand DragLeaveCommand { get; set; }
        public ICommand DropCommand { get; set; }

        public ObservableCollection<Shop> Shops
        {
            get => _Shops;
            set { _Shops = value; OnPropertyChanged("Shops"); }
        }
        private ObservableCollection<Shop> _Shops;

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set { _IsRefreshing = value; OnPropertyChanged("IsRefreshing"); }
        }
        private bool _IsRefreshing;

        private async void LoadShops()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Ładowanie...", MaskType.Black);

                Shops = new ObservableCollection<Shop>(await App.Database.GetShopsAsync());
                CalculateItemsToBuy();

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void CalculateItemsToBuy()
        {
            try
            {
                foreach (Shop shop in Shops)
                {
                    shop.QuantityToBuy = (await App.Database.GetShopItemsAsync(shop.ShopId)).Where(x => !x.IsBought).Count();
                    shop.QuantityAll = (await App.Database.GetShopItemsAsync(shop.ShopId)).Count();
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void AssignNumbers()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Numerowanie...", MaskType.Black);

                Shops.ToList().ForEach(x => x.Number = Shops.IndexOf(x) + 1);
                await App.Database.UpdateShopsAsync(Shops.ToList());

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void OpenShopAction(Shop shop)
        {
            try
            {
                await Navigation.PushAsync(new ItemsPage(shop));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void AddShopAction()
        {
            try
            {
                Shop initialShop = new Shop();
                Shop shop = await Navigation.ShowPopupAsync(new AddEditShopPopup(initialShop, "Nowy sklep"));

                if (shop is null)
                    return;

                await App.Database.InsertShopAsync(shop);

                Shops.Add(shop);
                AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void EditShopAction(Shop initialShop)
        {
            try
            {
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

        private async void DeleteShopAction(Shop shop)
        {
            try
            {
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

                List<Item> items = new List<Item>(await App.Database.GetShopItemsAsync(shop.ShopId));

                if (items != null && items.Count > 0)
                {
                    foreach (Item item in items)
                    {
                        await App.Database.DeleteItemAsync(item);
                    }
                }

                Shops.Remove(shop);
                await App.Database.DeleteShopAsync(shop);
                AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void DropAction(Shop shop)
        {
            try
            {
                Shop shopToMove = Shops.First(i => i.IsBeingDragged);
                Shop shopToInsertBefore = shop;

                if (shopToMove == null || shopToInsertBefore == null || shopToMove == shopToInsertBefore)
                {
                    Shops.ToList().ForEach(x => x.IsBeingDragged = false);
                    return;
                }

                int firstIndex = Shops.IndexOf(shopToMove);
                int insertAtIndex = Shops.IndexOf(shopToInsertBefore);

                Shops.RemoveAt(firstIndex);
                Shops.Insert(insertAtIndex, shopToMove);

                shopToMove.IsBeingDragged = false;
                shopToInsertBefore.IsBeingDraggedOver = false;

                AssignNumbers();
                Shops.ToList().ForEach(x => x.IsBeingDragged = false);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void DragStartingAction(Shop shop)
        {
            try
            {
                Shops.ToList().ForEach(i => i.IsBeingDragged = shop == i);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void DragOverAction(Shop shop)
        {
            try
            {
                Shop shopBeingDragged = Shops.FirstOrDefault(i => i.IsBeingDragged);
                Shops.ToList().ForEach(i => i.IsBeingDraggedOver = shop == i && shop != shopBeingDragged);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void DragLeaveAction()
        {
            try
            {
                Shops.ToList().ForEach(x => x.IsBeingDraggedOver = false);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void ExportShopAction(Shop shop)
        {
            try
            {
                List<Item> items = await App.Database.GetShopItemsAsync(shop.ShopId);

                if(items.Count == 0)
                {
                    UserDialogs.Instance.Toast("W sklepie nie ma przedmiotów do wyeksportowania.");
                    return;
                }

                string[] choices = new[] { "Strona internetowa", "Specjalny tekst" };

                string result = await UserDialogs.Instance.ActionSheetAsync("Wybierz...", string.Empty, "Anuluj", CancellationToken.None, choices);

                if (string.IsNullOrWhiteSpace(result))
                    return;

                if (result.Equals("Strona internetowa"))
                    ExportToWebsite(shop);

                if (result.Equals("Specjalny tekst"))
                    ExportToClipboard(shop);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void ExportToWebsite(Shop shop)
        {
            try
            {
                ApiShop apiShop = new ApiShop()
                {
                    Name = shop.Name,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = null,
                };

                List<Item> items = await App.Database.GetShopItemsAsync(shop.ShopId);
                apiShop.Items = new List<ApiItem>();

                foreach (Item item in items)
                {
                    apiShop.Items.Add(new ApiItem()
                    {
                        Name = item.Name,
                        CreatedOn = DateTime.Now,
                        Description = item.Description == "-1" ? string.Empty : item.Description,
                        ModifiedOn = null,
                        Quantity = item.Quantity,
                        UnitId = item.UnitId,
                    });
                }

                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None,
                    Error = (sender, args) =>
                    {
                        args.ErrorContext.Handled = true;
                    },
                };

                StringContent message = new StringContent(JsonConvert.SerializeObject(apiShop, settings), System.Text.Encoding.UTF8, "application/json");

                ApiAdapter.PostShop(message.ReadAsStringAsync().Result);
                UserDialogs.Instance.Toast("Eksport zakończony powodzeniem.");
            }
            catch 
            {
                UserDialogs.Instance.Alert("Nie udało się wykonać eksportu. " +
                    "Sprawdź czy jesteś w lokalnej sieci domowej oraz czy komputer stacjonarny jest włączony.", "Błąd", "OK");
            }
        }

        private async void ExportToClipboard(Shop shop)
        {
            try
            {
                ApiShop apiShop = new ApiShop()
                {
                    Name = shop.Name,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = null,
                };

                List<Item> items = await App.Database.GetShopItemsAsync(shop.ShopId);
                apiShop.Items = new List<ApiItem>();

                foreach (Item item in items)
                {
                    apiShop.Items.Add(new ApiItem()
                    {
                        Name = item.Name,
                        CreatedOn = DateTime.Now,
                        Description = item.Description == "-1" ? string.Empty : item.Description,
                        ModifiedOn = null,
                        Quantity = item.Quantity,
                        UnitId = item.UnitId,
                    });
                }

                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None,
                    Error = (sender, args) =>
                    {
                        args.ErrorContext.Handled = true;
                    },
                };

                StringContent message = new StringContent(JsonConvert.SerializeObject(apiShop, settings), System.Text.Encoding.UTF8, "application/json");

                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = message.ReadAsStringAsync().Result,
                    Title = "Udostępniona lista zakupów"
                });

                UserDialogs.Instance.Toast("Eksport zakończony powodzeniem.");
            }
            catch
            {
                UserDialogs.Instance.Alert("Nie udało się wykonać eksportu. " +
                    "Sprawdź czy jesteś w lokalnej sieci domowej oraz czy komputer stacjonarny jest włączony.", "Błąd", "OK");
            }
        }

        private async Task<ApiShop> PrepareShop(Shop shop)
        {
            ApiShop apiShop = new ApiShop()
            {
                Name = shop.Name,
                CreatedOn = DateTime.Now,
                ModifiedOn = null,
            };

            List<Item> items = await App.Database.GetShopItemsAsync(shop.ShopId);
            apiShop.Items = new List<ApiItem>();

            foreach (Item item in items)
            {
                apiShop.Items.Add(new ApiItem()
                {
                    Name = item.Name,
                    CreatedOn = DateTime.Now,
                    Description = item.Description == "-1" ? string.Empty : item.Description,
                    ModifiedOn = null,
                    Quantity = item.Quantity,
                    UnitId = item.UnitId,
                });
            }

            return apiShop;
        }
    }
}
