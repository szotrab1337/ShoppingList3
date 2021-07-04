using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace ShoppingList.Models
{
    public class Shop : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int ShopId { get; set; }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        private string _Name;

        public int? Number
        {
            get { return _Number; }
            set { _Number = value; OnPropertyChanged("Number"); }
        }
        private int? _Number;

        [Ignore]
        public int QuantityToBuy
        {
            get { return _QuantityToBuy; }
            set { _QuantityToBuy = value; OnPropertyChanged("QuantityToBuy"); }
        }
        private int _QuantityToBuy;

        [Ignore]
        public bool IsBeingDragged
        {
            get { return isBeingDragged; }
            set { isBeingDragged = value; OnPropertyChanged("IsBeingDragged"); }
        }
        private bool isBeingDragged;

        [Ignore]
        public bool IsBeingDraggedOver
        {
            get { return isBeingDraggedOver; }
            set { isBeingDraggedOver = value; OnPropertyChanged("IsBeingDraggedOver"); }
        }
        private bool isBeingDraggedOver;
    }
}
