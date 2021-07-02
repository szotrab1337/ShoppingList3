using Acr.UserDialogs;
using ShoppingList.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class ShopsViewModel : BaseViewModel
    {
        public ShopsViewModel()
        {
            Title = "About";
            Test = new Command(Test1);

            Shops = new ObservableCollection<Shop>();
            AddShops();
        }

        public ObservableCollection<Shop> Shops
        {
            get { return _Shops; }
            set { _Shops = value; OnPropertyChanged("Shops"); }
        }
        private ObservableCollection<Shop> _Shops;

        public void AddShops()
        {
            Shops.Add(new Shop()
            {
                ShopId = 1,
                CreatedOn = DateTime.Now,
                Name = "Sklep 1",
                Number = 1
            });

            Shops.Add(new Shop()
            {
                ShopId = 2,
                CreatedOn = DateTime.Now,
                Name = "Sklep 2",
                Number = 2
            });

            Shops.Add(new Shop()
            {
                ShopId = 3,
                CreatedOn = DateTime.Now,
                Name = "Sklep 3",
                Number = 3
            });
        }

        public ICommand Test { get; set; }

        public void Test1(object sender)
        {
            Shop shop = (Shop)sender;

            //UserDialogs.Instance.Alert(shop.Name, "Błąd", "OK");
        }
    }
}
