using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Helpers
{
    class MediaHelper
    {
        public static async Task<StorageFile> RecordMediaAsync()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = false;

            StorageFile storageFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.PhotoOrVideo);

            if (storageFile == null)
            {
                // User cancelled photo capture
                return null;
            }

            //var textBox = new TextBox();
            //textBox.Text = "Photo1";
            //textBox.SelectAll();

            //ContentDialog cd = new ContentDialog();
            //cd.Title = "File name";
            //cd.PrimaryButtonText = "OK";
            ////cd.PrimaryButtonClick += Cd_PrimaryButtonClick;
            //cd.Content = textBox;
            //await cd.ShowAsync();

            //await storageFile.RenameAsync(textBox.Text);
            return storageFile;
        }

        public static async Task<StorageFile> GetFileFromUser()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            // create list of supported file for the picker filter
            foreach (var extension in FileExtensionHelper.AllowedExtensions)
            {
                picker.FileTypeFilter.Add(extension.Key);
            }

            var file = await picker.PickSingleFileAsync();
            return file;
        }
    }
}
