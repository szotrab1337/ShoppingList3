using Acr.UserDialogs;
using ShoppingList.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ItemsViewModel(INavigation navigation, Shop shop)
        {
            this.Navigation = navigation;
            this.Shop = shop;
            this.Title = Shop.Name + " " + Shop.ShopId;

            OpenItemCommand = new Command<Item>(OpenItemAction);
            AddItemCommand = new Command(AddItemAction);
            EditItemCommand = new Command<Item>(EditItemAction);
            DeleteItemCommand = new Command<Item>(DeleteItemAction);
            AbsentItemCommand = new Command<Item>(AbsentItemAction);

            LoadItems();
        }

        public ICommand OpenItemCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand AbsentItemCommand { get; set; }

        public INavigation Navigation { get; set; }
        public Shop Shop { get; set; }

        public ObservableCollection<Item> Items
        {
            get { return _Items; }
            set { _Items = value; OnPropertyChanged("Items"); }
        }
        private ObservableCollection<Item> _Items;

        private async void LoadItems()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Ładowanie...", MaskType.Black);

                Items = new ObservableCollection<Item>(await App.Database.GetShopItemsAsync(Shop.ShopId));
                Items.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void AssignNumbers()
        {
            UserDialogs.Instance.ShowLoading("Numerowanie...", MaskType.Black);

            Items.ToList().ForEach(x => x.Number = Items.IndexOf(x) + 1);
            await App.Database.UpdateItemsAsync(Items.ToList());

            UserDialogs.Instance.HideLoading();
        }

        private async void OpenItemAction(Item item)
        {
            try
            {
                //await Navigation.PushAsync(new ItemsPage(shop));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void AddItemAction()
        {
            try
            {
                //Shop initialShop = new Shop();
                //Shop shop = await Navigation.ShowPopupAsync(new AddEditShopPopup(initialShop, "Nowy sklep"));

                //if (shop is null)
                //    return;

                //await App.Database.InsertShopAsync(shop);

                //Shops.Add(shop);
                //AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void EditItemAction(Item item)
        {
            try
            {
                //Shop shop = await Navigation.ShowPopupAsync(new AddEditShopPopup(initialShop, "Edycja sklepu"));

                //if (shop is null)
                //    return;

                //await App.Database.UpdateShopAsync(shop);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void AbsentItemAction(Item item)
        {
            try
            {
                item.Absent = !item.Absent;
                await App.Database.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }      

        private async void DeleteItemAction(Item item)
        {
            try
            {
                //if (shop is null)
                //    return;

                //bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                //{
                //    Message = "Czy na pewno chcesz usunąć sklep " + shop.Name + "?",
                //    OkText = "Tak",
                //    CancelText = "Nie",
                //    Title = "Potwierdzenie",
                //    AndroidStyleId = 2131689474
                //});

                //if (!result)
                //    return;

                //await App.Database.DeleteShopAsync(shop);
                //Shops.Remove(shop);
                //AssignNumbers();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "IsBought")
                {
                    Item item = (Item)sender;
                    await App.Database.UpdateItemAsync(item);

                    if (item.IsBought)
                        item.TextDecorations = TextDecorations.Strikethrough;
                    else
                        item.TextDecorations = TextDecorations.None;

                    MessagingCenter.Send(this, "RefreshItemsCount");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
    }
}
