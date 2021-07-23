using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ShoppingList.Models
{
    public class Item : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int ItemId { get; set; }

        public int ShopId { get; set; }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        private string _Name;

        public double? Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; OnPropertyChanged("Quantity"); }
        }
        private double? _Quantity;

        public bool IsBought
        {
            get { return _IsBought; }
            set { _IsBought = value; OnPropertyChanged("IsBought"); }
        }
        private bool _IsBought;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; OnPropertyChanged("Description"); }
        }
        private string _Description;

        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged("Image"); }
        }
        private string _Image;

        public int UnitId
        {
            get { return _UnitId; }
            set { _UnitId = value; OnPropertyChanged("UnitId"); }
        }
        private int _UnitId;

        public bool Absent
        {
            get { return _Absent; }
            set { _Absent = value; OnPropertyChanged("Absent"); }
        }
        private bool _Absent;

        public int? Number
        {
            get { return _Number; }
            set { _Number = value; OnPropertyChanged("Number"); }
        }
        private int? _Number;

        [Ignore]
        public TextDecorations TextDecorations
        {
            get { return _TextDecorations; }
            set { _TextDecorations = value; OnPropertyChanged("TextDecorations"); }
        }
        private TextDecorations _TextDecorations;

    }
}
