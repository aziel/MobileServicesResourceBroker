using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinZack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            CapturePhoto();
        }

        private async void CapturePhoto()
        {
            try
            {

                // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo                
                CameraCaptureUI dialog = new CameraCaptureUI();
                //Size aspectRatio = new Size(16, 9);
                //dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;                
                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if (file != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        bitmapImage.SetSource(fileStream);
                    }
                    CapturedPhoto.Source = bitmapImage;
                    //ResetButton.Visibility = Visibility.Visible;

                    // Store the file path in Application Data
                    //appSettings[photoKey] = file.Path;
                }
                else
                {
                    //rootPage.NotifyUser("No photo captured.", NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }
    }
}
