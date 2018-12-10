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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    class AttachmentsViewModel : BaseViewModel
    {
        private FeatureTable _featureTable;
        private PopupManager _popupManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentsViewModel"/> class.
        /// </summary>
        public AttachmentsViewModel(PopupManager popupManager, FeatureTable featureTable)
        {
            AttachmentManager = popupManager.AttachmentManager;
            _featureTable = featureTable;
            _popupManager = popupManager;

            AttachmentMode = AttachmentMode.View;
            Attachments = new ObservableCollection<AttachmentWithThumbnail>();

            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += (s, l) =>
            {
                // event handler for when a new attachment is selected through the view
                if (l.Args.Key == BroadcastMessageKey.NewAttachmentFile && l.Args.Value != null)
                {
                    if (l.Args.Value != null)
                    {
                        var filePath = l.Args.Value.ToString();
                        SelectedAttachment = new AttachmentWithEditableProperties()
                        {
                            Name = Path.GetFileName(filePath),
                            Size = "300",
                            LocalFilePath = filePath,
                        };
                    }
                }
            };
        }

        /// <summary>
        /// Gets or sets the collection of attachments to be displayed 
        /// </summary>
        public ObservableCollection<AttachmentWithThumbnail> Attachments { get; private set; }

        /// <summary>
        /// Gets or sets the attachment manager for the feature
        /// </summary>
        public PopupAttachmentManager AttachmentManager { get; private set; }

        /// <summary>
        /// Initialization code for the AttachmentsViewModel
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadAttachments();
        }

        private AttachmentMode _attachmentMode;

        /// <summary>
        /// Gets or sets the AttachmentMode which determines if the user is viewing, adding or editing an attachment
        /// </summary>
        public AttachmentMode AttachmentMode
        {
            get => _attachmentMode;
            set
            {
                _attachmentMode = value;
                OnPropertyChanged();
            }
        }

        private AttachmentWithEditableProperties _selectedAttachment;

        /// <summary>
        /// Gets or sets the attachment that is selected and is being edited
        /// </summary>
        public AttachmentWithEditableProperties SelectedAttachment
        {
            get => _selectedAttachment;
            set
            {
                if (_selectedAttachment != value)
                {
                    _selectedAttachment = value;
                    OnPropertyChanged();
                }
            }
        }

        private ICommand _closeAttachmentsCommand;

        /// <summary>
        /// Gets the command to clear the attachments
        /// </summary>
        public ICommand CloseAttachmentsCommand
        {
            get
            {
                return _closeAttachmentsCommand ?? (_closeAttachmentsCommand = new DelegateCommand(
                    (x) =>
                    {
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.AttachmentViewModelCreated);
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
                                    // open popup manager for editing
                                    _popupManager.StartEditing();

                                    // delete attachment
                                    AttachmentManager.DeleteAttachment(attachment);

                                    // close popup manager editing session
                                    await _popupManager.FinishEditingAsync();

                                    // apply edits to feature table
                                    await _featureTable.ApplyEdits();

                                    // reload the attachments panel
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

        private ICommand _saveAttachmentCommand;

        /// <summary>
        /// Gets the command to save attachment that has been added
        /// </summary>
        public ICommand SaveAttachmentCommand
        {
            get
            {
                //THIS DOES NOT WORK. SUBMITTED BUG IN RUNTIME
                return _saveAttachmentCommand ?? (_saveAttachmentCommand = new DelegateCommand(
                    async (x) =>
                    {
                        if (SelectedAttachment != null)
                        {
                            // retrieve file extension
                            var extension = Path.GetExtension(SelectedAttachment.LocalFilePath);

                            // determine type based on extension
                            var contentType = GetTypeFromExtension(extension);

                            // open popup manager for editing
                            _popupManager.StartEditing();

                            try
                            {
                                AttachmentManager.AddAttachment(SelectedAttachment.LocalFilePath, contentType);
                                await _popupManager.FinishEditingAsync();
                                await _featureTable.ApplyEdits();
                            }
                            catch (Exception ex)
                            {
                                var z = ex.Message;
                            }

                            SelectedAttachment = null;

                            AttachmentMode = AttachmentMode.View;

                            // reload the attachments panel
                            await LoadAttachments();
                        }
                    }));
            }
        }

        /// <summary>
        /// Method to retrieve mime type from extension
        /// </summary>
        private string GetTypeFromExtension(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        /// <summary>
        /// Dictionary with extensions and mime type mappings
        /// </summary>
        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            // TODO: Add more types from the AGOL list
            { ".bmp", "image/bmp"},
            {".gif", "image/gif"},
            {".jfif", "image/pjpeg"},
            {".jpe", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".png", "image/png"},
            {".tif", "image/tiff"},
            {".tiff", "image/tiff"}
        };

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
                // limit allowed attachments to images
                if (attachment.Type == PopupAttachmentType.Image)
                {
                    var attachmentWithThumbnail = new AttachmentWithThumbnail();
                    await attachmentWithThumbnail.LoadAsync(attachment);

                    Attachments.Add(attachmentWithThumbnail);
                }

                //TODO: Add support for other types of attachments. Use pre-existing images in place of thumbnails
            }
        }
    }
}
