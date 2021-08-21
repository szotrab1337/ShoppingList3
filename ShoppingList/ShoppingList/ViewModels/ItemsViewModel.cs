using Acr.UserDialogs;
using ShoppingList.Models;
using ShoppingList.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using ShoppingList.Models.Api;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShoppingList.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ItemsViewModel(INavigation navigation, Shop shop)
        {
            this.Navigation = navigation;
            this.Shop = shop;
            this.Title = Shop.Name;

            AddItemCommand = new Command(AddItemAction);
            EditItemCommand = new Command<Item>(EditItemAction);
            DeleteItemCommand = new Command<Item>(DeleteItemAction);
            AbsentItemCommand = new Command<Item>(AbsentItemAction);
            EnlargePhotoCommand = new Command<Item>(EnlargePhotoAction);
            CheckCommand = new Command<Item>(CheckAction);
            DeleteBoughtCommand = new Command(DeleteBoughtAction);
            DeleteAllCommand = new Command(DeleteAllAction);
            RefreshCommand = new Command(() =>
            {
                IsRefreshing = true;
                LoadItems();
                IsRefreshing = false;
            });

            LoadItems();
            MessagingCenter.Subscribe<AddEditItemViewModel>(this, "RefreshItemsList", (LoadAgain) => { LoadItems(); });
        }

        public ICommand AddItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand AbsentItemCommand { get; set; }
        public ICommand EnlargePhotoCommand { get; set; }
        public ICommand CheckCommand { get; set; }
        public ICommand DeleteBoughtCommand { get; set; }
        public ICommand DeleteAllCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public INavigation Navigation { get; set; }
        public Shop Shop { get; set; }

        public ObservableCollection<Item> Items
        {
            get => _Items;
            set { _Items = value; OnPropertyChanged("Items"); }
        }
        private ObservableCollection<Item> _Items;

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set { _IsRefreshing = value; OnPropertyChanged("IsRefreshing"); }
        }
        private bool _IsRefreshing;

        private async void LoadItems()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Ładowanie...", MaskType.Black);

                Items = new ObservableCollection<Item>((await App.Database.GetShopItemsAsync(Shop.ShopId)).OrderBy(x => x.IsBought).ThenBy(x => x.Absent).ToList());
                Items.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);
                MessagingCenter.Send(this, "RefreshItemsCount");

                UserDialogs.Instance.HideLoading();
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
                await Navigation.PushAsync(new AddEditItemPage(null, Shop));
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
                if (item is null)
                    return;

                await Navigation.PushAsync(new AddEditItemPage(item, Shop));
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
                if (item is null)
                    return;

                bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = "Czy na pewno chcesz usunąć przedmiot " + item.Name + "?",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie",
                    AndroidStyleId = 2131689474
                });

                if (!result)
                    return;

                await App.Database.DeleteItemAsync(item);
                Items.Remove(item);
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

        private void EnlargePhotoAction(Item item)
        {
            try
            {
                if (item is null || string.IsNullOrEmpty(item.Image))
                    return;

                Navigation.ShowPopup(new ImagePopup(item.Image));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void CheckAction(Item item)
        {
            try
            {
                if (item is null)
                    return;

                item.IsBought = !item.IsBought;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void DeleteBoughtAction()
        {
            try
            {
                if (Items is null || Items.Where(x => x.IsBought).Count() == 0)
                    return;

                bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = $"Czy na pewno chcesz usunąć wszystkie kupione przedmioty? " +
                    $"({Items.Where(x => x.IsBought).Count()} {QuantityToString(Items.Where(x => x.IsBought).Count())})",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie",
                    AndroidStyleId = 2131689474
                });

                if (!result)
                    return;

                foreach (Item item in Items.Where(x => x.IsBought).ToList())
                {
                    await App.Database.DeleteItemAsync(item);
                    Items.Remove(item);
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void DeleteAllAction()
        {
            try
            {
                if (Items is null || Items.Count == 0)
                    return;

                bool result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = $"Czy na pewno chcesz usunąć wszystkie przedmioty? ({Items.Count()} {QuantityToString(Items.Count())})",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie",
                    AndroidStyleId = 2131689474
                });

                if (!result)
                    return;

                foreach (Item item in Items.ToList())
                {
                    await App.Database.DeleteItemAsync(item);
                    Items.Remove(item);
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private string QuantityToString(int quantity)
        {
            if (quantity == 1)
                return "element";

            if (quantity >= 2 && quantity <= 4)
                return "elementy";

            return "elementów";
        }
    }
}
