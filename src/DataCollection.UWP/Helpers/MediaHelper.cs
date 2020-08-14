/*******************************************************************************
  * Copyright 2019 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views;
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Dialogs;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Helpers
{
    /// <summary>
    /// Helper class to process media inputs from both camera and file browsing
    /// </summary>
    class MediaHelper
    {
        /// <summary>
        /// Calls camera API to capture photo or video
        /// </summary>
        public static async Task<StorageFile> RecordMediaAsync()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = false;

            StorageFile storageFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.PhotoOrVideo);

            if (storageFile == null)
            {
                // User cancelled capture
                return null;
            }
            
            // prompt user to enter file name for the new attachment
            FileNameDialog fnd = new FileNameDialog();
            await fnd.ShowAsync();

            // rename file
            await storageFile.RenameAsync(fnd.FileName + storageFile.FileType, NameCollisionOption.ReplaceExisting);
            return storageFile;
        }

        /// <summary>
        /// Prompts user to choose a file from a list of supported files
        /// </summary>
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
