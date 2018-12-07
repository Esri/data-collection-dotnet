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

        public AttachmentsViewModel()
        {
            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += (s, l) =>
            {
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

        public ObservableCollection<AttachmentWithThumbnail> Attachments { get; private set; }

        /// <summary>
        /// Initialization code for the AttachmentsViewModel
        /// </summary>
        public async Task InitializeAsync(PopupManager popupManager, FeatureTable featureTable)
        {
            AttachmentManager = popupManager.AttachmentManager;
            _featureTable = featureTable;
            _popupManager = popupManager;
            AttachmentMode = AttachmentMode.View;
            Attachments = new ObservableCollection<AttachmentWithThumbnail>();

            // Run initialization
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

        private AttachmentMode _attachmentMode;

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

        public PopupAttachmentManager AttachmentManager { get; private set; }

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
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.AttachmentViewModel);
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
                    (x) =>
                    {
                        // TODO: Add logic to delete the attachment
                    }));
            }
        }

        private ICommand _saveAttachmentCommand;

        public ICommand SaveAttachmentCommand
        {
            get
            {
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
                                //await _featureTable.ApplyEdits();
                            }
                            catch (Exception ex)
                            {
                                var z = ex.Message;
                            }

                            SelectedAttachment = null;

                            AttachmentMode = AttachmentMode.View;

                            // refresh AttachmentManager
                            await AttachmentManager.FetchAttachmentsAsync();

                            
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
    }
}
