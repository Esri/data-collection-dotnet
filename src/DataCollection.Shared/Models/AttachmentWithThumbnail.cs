using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UI;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models
{
    class AttachmentWithThumbnail
    {
        public BitmapImage Thumbnail { get; private set; }

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

            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Method to get BitmapImage from RuntimeImage
        /// </summary>
        private async Task<BitmapImage> GetImageAsync(RuntimeImage rtImage)
        {
            if (rtImage != null)
            {
                try
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
                catch (Exception ex)
                {
                    var x = ex.Message;
                    return null;
                }
            }
            return null;
        }

    }
}
