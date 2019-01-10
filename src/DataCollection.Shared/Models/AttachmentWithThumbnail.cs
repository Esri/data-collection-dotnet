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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UI;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models
{
    public class AttachmentWithThumbnail
    {
        /// <summary>
        /// Gets or sets the Thumbnail image for the attachment 
        /// </summary>
        public ImageSource Thumbnail { get; private set; }

        /// <summary>
        /// Gets or sets the attachment metadata 
        /// </summary>
        public PopupAttachment Attachment { get; private set; }

        internal async Task LoadAsync(PopupAttachment attachment)
        {
            Attachment = attachment;


            switch (Attachment.Type)
            {
                // create thumbnail for the image attachments
                case PopupAttachmentType.Image:
                    try
                    {
                        var rtImage = await attachment.CreateThumbnailAsync(50, 50);
                        Thumbnail = await rtImage.ToImageSourceAsync();
                        Thumbnail?.Freeze();
                    }
                    catch (Exception ex)
                    {
                        UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                    }
                    break;

                // use placeholder image for the rest of the attachments
                case PopupAttachmentType.Video:
                case PopupAttachmentType.Document:
                default:
                    Thumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/PlaceholderThumbnail.png"));
                    Thumbnail?.Freeze();
                    break;
            }
        }
    }
}
