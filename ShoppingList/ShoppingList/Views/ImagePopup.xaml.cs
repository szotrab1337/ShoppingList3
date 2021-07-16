using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingList.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePopup : Popup
    {
        public ImagePopup(string Image64)
        {
            InitializeComponent();

            ImageControl.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(Image64)));
        }
    }
}