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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class AttachmentsViewModel : BaseViewModel
    {
        private PopupManager _popupManager;

        public PopupManager PopupManager
        {
            get => _popupManager;
            set
            {
                _popupManager = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentsViewModel"/> class.
        /// </summary>
        public AttachmentsViewModel(PopupManager popupManager, FeatureTable featureTable)
        {
            if (popupManager != null)
            {
                AttachmentManager = popupManager.AttachmentManager;

                _popupManager = popupManager;

                Attachments = new ObservableCollection<AttachmentWithThumbnail>();

                LoadAttachments();
            }
        }

        /// <summary>
        /// Gets or sets the collection of attachments to be displayed 
        /// </summary>
        public ObservableCollection<AttachmentWithThumbnail> Attachments { get; private set; }

        /// <summary>
        /// Gets or sets the attachment manager for the feature
        /// </summary>
        public PopupAttachmentManager AttachmentManager { get; private set; }

        private ICommand _openAttachmentCommand;

        /// <summary>
        /// Gets the command to open the attachment the user tapped on
        /// </summary>
        public ICommand OpenAttachmentCommand
        {
            get
            {
                return _openAttachmentCommand ?? (_openAttachmentCommand = new DelegateCommand(
                    async (x) =>
                    {
                        if (x != null && x is PopupAttachment attachment)
                        {
                            if (attachment.LoadStatus != LoadStatus.Loaded)
                            {
                                await attachment.LoadAsync();
                            }

                            // create file name from runtime attachment filename hash and the actual attachment name
                            var runtimeFileHash = attachment.Filename.Split('.').FirstOrDefault();
                            var newFileName = runtimeFileHash + "." + attachment.Name;

                            // if the file exists, rename it to contain the proper extension
                            if (File.Exists(attachment.Filename))
                            {
                                File.Move(attachment.Filename, newFileName);
                            }

                            // if the renamed file exists, open it in the user's preferred application
                            if (File.Exists(newFileName))
                            {
#if WPF
                                // in WPF, let Windows open the file with the application the user has set as default
                                System.Diagnostics.Process.Start(newFileName);
#endif
                            }
                            else
                            {
                                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                     Resources.GetString("FileNotFound_Title"),
                                     Resources.GetString("FileNotFound_Message"),
                                     true);
                                return;
                            }
                        }
                    }));
            }
        }

        private ICommand _deleteAttachmentCommand;

        /// <summary>
        /// Gets the command to delete attachment 
        /// </summary>
        public ICommand DeleteAttachmentCommand
        {
            get
            {
                return _deleteAttachmentCommand ?? (_deleteAttachmentCommand = new DelegateCommand(
                    async (x) =>
                    {
                        if (x != null && x is PopupAttachment attachment)
                        {
                            bool deleteConfirmed = false;

                            // wait for response from the user if they truly want to delete the attachment
                            UserPromptMessenger.Instance.ResponseValueChanged += handler;

                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("DeleteConfirmationAttachment_Title"),
                                Resources.GetString("DeleteConfirmationAttachment_Message"),
                                false,
                                null,
                                Resources.GetString("DeleteButton_Content"));

                            void handler(object o, UserPromptResponseChangedEventArgs e)
                            {
                                {
                                    UserPromptMessenger.Instance.ResponseValueChanged -= handler;
                                    if (e.Response)
                                    {
                                        deleteConfirmed = true;
                                    }
                                }
                            }

                            if (deleteConfirmed)
                            {
                                try
                                {
                                    AttachmentManager.DeleteAttachment(attachment);

                                    // reload the attachments panel after deleting attachment
                                    await LoadAttachments();
                                }
                                catch (Exception ex)
                                {
                                    UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                                }
                            }
                        }
                    }));
            }
        }

        /// <summary>
        /// Loads attachments from the attachments manager
        /// This method is also called when an attachment is added or removed to refresh the list 
        /// </summary>
        private async Task LoadAttachments()
        {
            // clear any existing attachments in the collection
            Attachments.Clear();

            // loop through attachments and add them to the collection
            foreach (var attachment in AttachmentManager.Attachments)
            {
                var attachmentWithThumbnail = new AttachmentWithThumbnail();
                await attachmentWithThumbnail.LoadAsync(attachment);

                // add attachment to collection using the UI thread (for the binding to work)
                Application.Current.Dispatcher.Invoke(new Action(() => { Attachments.Add(attachmentWithThumbnail); }));
            }
        }
    }
}
