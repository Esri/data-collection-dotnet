/*******************************************************************************
  * Copyright 2018 Esri
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

using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UI;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models
{
    class AttachmentWithThumbnail
    {
        /// <summary>
        /// Gets or sets the Thumbnail image for the attachment 
        /// </summary>
        public BitmapImage Thumbnail { get; private set; }

        /// <summary>
        /// Gets or sets the attachment metadata 
        /// </summary>
        public PopupAttachment Attachment { get; private set; }

        internal async Task LoadAsync(PopupAttachment attachment)
        {
            Attachment = attachment;

            try
            {
                // this is a workaround to generating thumbnails as CreateThumbnailAsyncdoesn't work
                // TODO: replace with CreateThumbnailAsync() when fixed
                var rtImage = await attachment.CreateFullImageAsync();
                Thumbnail = await GetImageAsync(rtImage);
            }
            catch
            {
                //TODO: show an error image if unable to retrieve attachment
            }
        }

        /// <summary>
        /// Method to get BitmapImage from RuntimeImage
        /// </summary>
        private async Task<BitmapImage> GetImageAsync(RuntimeImage rtImage)
        {
            if (rtImage != null)
            {
                if (rtImage.LoadStatus != LoadStatus.Loaded)
                {
                    await rtImage.LoadAsync();
                }

                var stream = await rtImage.GetEncodedBufferAsync();
                var image = new BitmapImage();
                image.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
            return null;
        }
    }
}
