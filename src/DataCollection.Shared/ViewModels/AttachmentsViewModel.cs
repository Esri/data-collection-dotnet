using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.Mapping.Popups;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    class AttachmentsViewModel : BaseViewModel
    {
        public ObservableCollection<AttachmentWithThumbnail> Attachments { get; private set; }

        /// <summary>
        /// Initialization code for the AttachmentsViewModel
        /// </summary>
        public async Task InitializeAsync(IEnumerable<PopupAttachment> attachments)
        {
            Attachments = new ObservableCollection<AttachmentWithThumbnail>();

            // Run initialization
            foreach (var attachment in attachments)
            {
                // limit allowed attachments to images
                if (attachment.Type == PopupAttachmentType.Image)
                {
                    var attachmentWithThumbnail = new AttachmentWithThumbnail();
                    await attachmentWithThumbnail.LoadAsync(attachment);

                    Attachments.Add(attachmentWithThumbnail);
                }
            }
        }
    }
}
