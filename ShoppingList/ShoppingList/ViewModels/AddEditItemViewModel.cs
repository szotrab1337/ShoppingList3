using Acr.UserDialogs;
using ShoppingList.Models;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class AddEditItemViewModel : BaseViewModel
    {
        public AddEditItemViewModel(INavigation navigation, bool addMode, Item item, Shop shop)
        {
            Navigation = navigation;
            Item = item;
            Shop = shop;
            Title = addMode ? "Nowy przedmiot" : "Edycja przedmiotu";

            Image = ImageSource.FromResource("ShoppingList.Images.imagePlaceholder.png");

            SelectImageCommand = new Command(SelectImageAction);
            EnlargePhotoCommand = new Command(EnlargePhotoAction);
            SaveItemCommand = new Command(SaveItemAction);

            LoadUnits();
            SelectedUnit = Units.FirstOrDefault();
        }
        public INavigation Navigation { get; set; }
        public ICommand SelectImageCommand {  get; set; }
        public ICommand EnlargePhotoCommand {  get; set; }
        public ICommand SaveItemCommand {  get; set; }

        public Item Item
        {
            get { return _Item; }
            set { _Item = value; OnPropertyChanged("Item"); }
        }
        private Item _Item;
        
        public Shop Shop
        {
            get { return _Shop; }
            set { _Shop = value; OnPropertyChanged("Shop"); }
        }
        private Shop _Shop;
        
        public Unit SelectedUnit
        {
            get { return _SelectedUnit; }
            set { _SelectedUnit = value; OnPropertyChanged("SelectedUnit"); }
        }
        private Unit _SelectedUnit;

        public List<Unit> Units
        {
            get { return _Units; }
            set { _Units = value; OnPropertyChanged("Units"); }
        }
        private List<Unit> _Units;
        
        public ImageSource Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged("Image"); }
        }
        private ImageSource _Image;
        
        public string ImagePath
        {
            get { return _ImagePath; }
            set
            {
                _ImagePath = value; OnPropertyChanged("ImagePath");

                if (!string.IsNullOrWhiteSpace(ImagePath))
                {
                    byte[] bytes = File.ReadAllBytes(ImagePath);
                    Image64 = Convert.ToBase64String(bytes);
                }
            }
        }
        private string _ImagePath;
        
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value; OnPropertyChanged("Name");

                SaveIsEnabled = ValidateInputs();
            }
        }
        private string _Name;
        
        public bool SaveIsEnabled
        {
            get { return _SaveIsEnabled; }
            set { _SaveIsEnabled = value; OnPropertyChanged("SaveIsEnabled"); }
        }
        private bool _SaveIsEnabled;
        
        public string Description
        {
            get { return _Description; }
            set { _Description = value; OnPropertyChanged("Description"); }
        }
        private string _Description;
        
        public string Image64
        {
            get { return _Image64; }
            set { _Image64 = value; OnPropertyChanged("Image64"); }
        }
        private string _Image64;
        
        public string Quantity
        {
            get { return _Quantity; }
            set
            {
                _Quantity = value; OnPropertyChanged("Quantity");

                SaveIsEnabled = ValidateInputs();
            }
        }
        private string _Quantity;

        private async void SelectImageAction()
        {
            try
            {
                string[] choices = new[] { "Wczytaj z urządzenia", "Zrób zdjęcie" };

                string result = await UserDialogs.Instance.ActionSheetAsync("Wybierz...", string.Empty, "Anuluj",  CancellationToken.None, choices);

                if (string.IsNullOrWhiteSpace(result))
                    return;

                if (result.Equals("Wczytaj z urządzenia"))
                    LoadImageFromDevice();

                if (result.Equals("Zrób zdjęcie"))
                    MakeImage();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private void LoadUnits()
        {
            try
            {
                Units = new List<Unit>(UnitBase.GetUnits());
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void LoadImageFromDevice()
        {
            try
            {
                FileResult image = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Wybierz zdjęcie"
                });

                ImagePath = image.FullPath;

                Stream stream = await image.OpenReadAsync();
                Image = ImageSource.FromStream(() => stream);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void MakeImage()
        {
            try
            {
                FileResult image = await MediaPicker.CapturePhotoAsync();

                ImagePath = image.FullPath;

                Stream stream = await image.OpenReadAsync();
                Image = ImageSource.FromStream(() => stream);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private async void SaveItemAction()
        {
            try
            {
                Item item = new Item()
                {
                    Name = Name,
                    IsBought = false,
                    Absent = false,
                    ShopId = Shop.ShopId
                };

                if (!string.IsNullOrWhiteSpace(Description))
                    item.Description = Description;
                else
                    item.Description = "-1";

                if(!string.IsNullOrWhiteSpace(Quantity))
                {
                    item.Quantity = Convert.ToDouble(Quantity);
                    item.UnitId = SelectedUnit.UnitId;
                }

                if(!string.IsNullOrWhiteSpace(Image64))
                    item.Image = Image64;

                await App.Database.InsertItemAsync(item);
                MessagingCenter.Send(this, "RefreshItemsList");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private void EnlargePhotoAction()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ImagePath))
                    return;

                Navigation.ShowPopup(new ImagePopup(Image64));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private bool ValidateInputs()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(Name))
                    return false;

                if (!string.IsNullOrWhiteSpace(Quantity))
                    if (double.TryParse(Quantity, out double duantityParsed))
                        return false;

                return true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
                return false;
            }

        }
    }
}
