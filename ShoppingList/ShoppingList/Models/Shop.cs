using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

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

        public DateTime CreatedOn 
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; OnPropertyChanged("CreatedOn"); }
        }
        private DateTime _CreatedOn;

        public DateTime? ModifiedOn
        {
            get { return _ModifiedOn; }
            set { _ModifiedOn = value; OnPropertyChanged("ModifiedOn"); }
        }
        private DateTime? _ModifiedOn;

        [Ignore]
        public int? Number
        {
            get { return _Number; }
            set { _Number = value; OnPropertyChanged("Number"); }
        }
        private int? _Number;
    }
}
