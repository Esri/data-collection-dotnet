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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models
{
    public class StagedAttachment : INotifyPropertyChanged
    {
        private ImageSource _thumbnail;

        /// <summary>
        /// Gets the Thumbnail image for the attachment 
        /// </summary>
        public ImageSource Thumbnail
        {
            get => _thumbnail;
            private set
            {
                _thumbnail = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the attachment metadata 
        /// </summary>
        public PopupAttachment Attachment { get; private set; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal async Task LoadAsync(PopupAttachment attachment)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Attachment = attachment;

            // this workflow is dependent on image location so it's fully segregated per platform

#if WPF
            switch (Attachment.Type)
            {
                // create thumbnail for the image attachments
                case PopupAttachmentType.Image:
                    var image = await Attachment.CreateThumbnailAsync(64, 64);
                    Thumbnail = await image.ToImageSourceAsync();
                    break;
                // use placeholder images for the rest of the attachments
                case PopupAttachmentType.Video:
                    Thumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/AttachmentVideo.png"));
                    break;
                case PopupAttachmentType.Document:
                    Thumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/AttachmentDocument.png"));
                    break;
                default:
                    Thumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/AttachmentOther.png"));
                    break;
            }

            Thumbnail?.Freeze();

#elif NETFX_CORE
            switch (Attachment.Type)
            {
                // create thumbnail for the image attachments
                // must do this on UI thread due to bindings 
                case PopupAttachmentType.Image:
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                    {
                        var image = await Attachment.CreateThumbnailAsync(64, 64);
                        Thumbnail = await image.ToImageSourceAsync();
                    });
                    break;

                // use placeholder images for the rest of the attachments
                case PopupAttachmentType.Video:
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/AttachmentVideo.png"));
                    });
                    break;
                case PopupAttachmentType.Document:
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/AttachmentDocument.png"));
                    });
                    break;
                default:
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/AttachmentOther.png"));
                    });
                    break;
            }
#else
                    // will throw if another platform is added without handling this 
                    throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Raises the <see cref="BaseViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
