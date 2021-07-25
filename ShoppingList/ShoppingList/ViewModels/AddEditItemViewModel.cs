using Acr.UserDialogs;
using ShoppingList.Models;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ShoppingList.ViewModels
{
    public class AddEditItemViewModel : BaseViewModel
    {
        public AddEditItemViewModel(INavigation navigation, Item item, Shop shop)
        {
            ImagePlaceholder = ImageSource.FromResource("ShoppingList.Images.imagePlaceholder.png");
            Navigation = navigation;
            Item = item;
            Shop = shop;
            Title = item is null ? "Nowy przedmiot" : "Edycja przedmiotu";
            Image = ImagePlaceholder;

            SelectImageCommand = new Command(SelectImageAction);
            EnlargePhotoCommand = new Command(EnlargePhotoAction);
            SaveItemCommand = new Command(SaveItemAction);

            LoadUnits();
            SelectedUnit = Units.FirstOrDefault();

            if(item != null)
            {
                Name = item.Name;
                Quantity = item.Quantity.ToString();
                Description = item.Description == "-1" ? string.Empty : item.Description;
                SelectedUnit = Units.FirstOrDefault(x => x.UnitId == item.UnitId) is null ? Units.FirstOrDefault() : Units.FirstOrDefault(x => x.UnitId == item.UnitId);
                Image = string.IsNullOrEmpty(item.Image) ? ImagePlaceholder : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(item.Image)));
            }
        }
        public INavigation Navigation { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand EnlargePhotoCommand { get; set; }
        public ICommand SaveItemCommand { get; set; }

        public ImageSource ImagePlaceholder;

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
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        private string _Name;

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
            set { _Quantity = value; OnPropertyChanged("Quantity"); }
        }
        private string _Quantity;

        private async void SelectImageAction()
        {
            try
            {
                string[] choices;

                if(Image == ImagePlaceholder)
                    choices = new[] { "Wczytaj z urządzenia", "Zrób zdjęcie" };
                else
                    choices = new[] { "Wczytaj z urządzenia", "Zrób zdjęcie", "Usuń" };

                string result = await UserDialogs.Instance.ActionSheetAsync("Wybierz...", string.Empty, "Anuluj", CancellationToken.None, choices);

                if (string.IsNullOrWhiteSpace(result))
                    return;

                if (result.Equals("Wczytaj z urządzenia"))
                    LoadImageFromDevice();

                if (result.Equals("Zrób zdjęcie"))
                    MakeImage();

                if (result.Equals("Usuń"))
                    ClearImage();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void ClearImage()
        {
            try
            {
                Image = ImagePlaceholder;
                Image64 = string.Empty;
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

        private void SaveItemAction()
        {
            try
            {
                if (Item is null)
                    AddNewItem();
                else
                    EditItem();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private async void AddNewItem()
        {
            try
            {
                if (!ValidateInputs())
                    return;

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

                if (!string.IsNullOrWhiteSpace(Quantity))
                {
                    item.Quantity = Convert.ToDouble(Quantity);
                    item.UnitId = SelectedUnit.UnitId;
                }

                if (!string.IsNullOrWhiteSpace(Image64))
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
        
        private async void EditItem()
        {
            try
            {
                if (!ValidateInputs())
                    return;

                Item.Name = Name;
                Item.Image = Image64;

                if (!string.IsNullOrWhiteSpace(Description))
                    Item.Description = Description;
                else
                    Item.Description = "-1";

                if (!string.IsNullOrWhiteSpace(Quantity))
                {
                    Item.Quantity = Convert.ToDouble(Quantity);
                    Item.UnitId = SelectedUnit.UnitId;
                }

                await App.Database.UpdateItemAsync(Item);
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
                if (string.IsNullOrWhiteSpace(Name))
                {
                    UserDialogs.Instance.Alert("Wprowadź nazwę przedmiotu.", "Błąd", "OK");
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(Quantity))
                    if (!double.TryParse(Quantity, out double duantityParsed))
                    {
                        UserDialogs.Instance.Alert("Ilość musi być liczbą.", "Błąd", "OK");
                        return false;
                    }

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
