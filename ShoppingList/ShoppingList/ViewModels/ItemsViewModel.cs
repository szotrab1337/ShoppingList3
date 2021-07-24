﻿using Acr.UserDialogs;
using ShoppingList.Models;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
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

            AddItemCommand = new Command(AddItemAction);
            EditItemCommand = new Command<Item>(EditItemAction);
            DeleteItemCommand = new Command<Item>(DeleteItemAction);
            AbsentItemCommand = new Command<Item>(AbsentItemAction);
            EnlargePhotoCommand = new Command<Item>(EnlargePhotoAction);

            LoadItems();
            MessagingCenter.Subscribe<AddEditItemViewModel>(this, "RefreshItemsList", (LoadAgain) => { LoadItems(); });
        }

        public ICommand AddItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand AbsentItemCommand { get; set; }
        public ICommand EnlargePhotoCommand { get; set; }

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
    }
}
