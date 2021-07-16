using Acr.UserDialogs;
using ShoppingList.Models;
using ShoppingList.Views;
using System;
using System.Collections.Generic;
using System.IO;
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
        public AddEditItemViewModel(INavigation navigation, bool addMode, Item item)
        {
            this.Navigation = navigation;
            this.Item = item;
            Title = addMode ? "Nowy przedmiot" : "Edycja przedmiotu";

            Image = ImageSource.FromResource("ShoppingList.Images.imagePlaceholder.png");

            SelectImageCommand = new Command(SelectImageAction);
            EnlargePhotoCommand = new Command(EnlargePhotoAction);
        }
        public INavigation Navigation { get; set; }
        public ICommand SelectImageCommand {  get; set; }
        public ICommand EnlargePhotoCommand {  get; set; }

        public Item Item
        {
            get { return _Item; }
            set { _Item = value; OnPropertyChanged("Item"); }
        }
        private Item _Item;
        
        public ImageSource Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged("Image"); }
        }
        private ImageSource _Image;
        
        public string ImagePath
        {
            get { return _ImagePath; }
            set { _ImagePath = value; OnPropertyChanged("ImagePath"); }
        }
        private string _ImagePath;

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
        
        private void EnlargePhotoAction()
        {
            try
            {
                byte[] b = File.ReadAllBytes(ImagePath); 
                string Image64 = Convert.ToBase64String(b);

                Navigation.ShowPopup(new ImagePopup(Image64));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
    }
}
