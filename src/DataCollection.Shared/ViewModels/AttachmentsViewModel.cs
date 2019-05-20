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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
#if WPF
using System.Windows;
#elif NETFX_CORE
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class AttachmentsViewModel : BaseViewModel
    {
        private PopupManager _popupManager;

        /// <summary>
        /// Gets or sets the PopupManager for the attachment
        /// </summary>
        public PopupManager PopupManager
        {
            get => _popupManager;
            set
            {
                _popupManager = value;
                OnPropertyChanged();
            }
        }

        private string _newAttachmentPath;

        /// <summary>
        /// Gets or sets the path for the new attachment to be added to layer
        /// </summary>
        public string NewAttachmentPath
        {
            get => _newAttachmentPath;
            set
            {
                _newAttachmentPath = value;
                if (_newAttachmentPath != null && File.Exists(_newAttachmentPath))
                {
                    AddNewAttachment(_newAttachmentPath);
                }
            }
        }

#if NETFX_CORE
        private StorageFile _newAttachmentFile;

        public StorageFile NewAttachmentFile 
        {
            get => _newAttachmentFile;
            set
            {
                _newAttachmentFile = value;
                if (_newAttachmentFile != null)
                {
                    try
                    {
                        GetBytesAsync(_newAttachmentFile).ContinueWith(async t =>
                        {
                            var newAttachment = PopupManager.AttachmentManager.AddAttachment(_newAttachmentFile.Name, _newAttachmentFile.ContentType, t.Result);
                            // load the new attachment into a StagedAttachment and add it to Attachments list to display
                            var stagedAttachment = new StagedAttachment();
                            await stagedAttachment.LoadAsync(newAttachment);

                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                                Attachments.Add(stagedAttachment);
                            });
                        });
                    }
                    catch (Exception ex)
                    {
                        UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentsViewModel"/> class.
        /// </summary>
        public AttachmentsViewModel(PopupManager popupManager, FeatureTable featureTable)
        {
            if (popupManager != null)
            {
                AttachmentManager = popupManager.AttachmentManager;
                _popupManager = popupManager;

                LoadAttachments();
            }
        }

        /// <summary>
        /// Gets or sets the collection of attachments to be displayed 
        /// </summary>
        public ObservableCollection<StagedAttachment> Attachments { get; private set; } = new ObservableCollection<StagedAttachment>();

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

                            // HACK: This workflow is in place until API changes occur to save the attachment with its proper name and extension
                            // when an attachment is downloaded, the API generates a random temp file name that cannot be opened unless renamed to have a proper extension
                            // when an attachment is newly added, the API points to the file the user selected, so the name and extension are valid and the file can be opened directly 

                            var fileInfo = new FileInfo(attachment.Filename);
                            var attachmentLocalPath = "";

                            // if attachment was just added, its path still points to the location on disk, so open from there
                            // otherwise, use dowload path 
                            if (fileInfo.Exists)
                            {
                                if (fileInfo.Name == attachment.Name)
                                {
                                    attachmentLocalPath = attachment.Filename;
                                }
                                else
                                {
                                    // create temp directory from the random attachment file name 
                                    var directory = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(".", ""));

                                    if (!Directory.Exists(directory))
                                        Directory.CreateDirectory(directory);

                                    // place file into the newly created temp directory
                                    attachmentLocalPath = Path.Combine(directory, attachment.Name);
                                    if (!File.Exists(attachmentLocalPath))
                                        File.Copy(attachment.Filename, attachmentLocalPath);
                                }
#if WPF
                                // in WPF, let Windows open the file with the application the user has set as default
                                System.Diagnostics.Process.Start(attachmentLocalPath);
#elif NETFX_CORE
                                try
                                {
                                    var storageFile = await StorageFile.GetFileFromPathAsync(attachmentLocalPath);
                                    await Windows.System.Launcher.LaunchFileAsync(storageFile);
                                }
                                catch(Exception ex)
                                {
                                    UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                                }
#else
                                // will throw if another platform is added without handling this 
                                throw new NotImplementedException();
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
                            // wait for response from the user if they truly want to delete the attachment
                            bool deleteConfirmed = await UserPromptMessenger.Instance.AwaitConfirmation(
                                Resources.GetString("DeleteConfirmationAttachment_Title"),
                                Resources.GetString("DeleteConfirmationAttachment_Message"),
                                false,
                                null,
                                Resources.GetString("DeleteButton_Content"));

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
        internal async Task LoadAttachments()
        {
            // clear any existing attachments in the collection
            Attachments.Clear();

            // create list of tasks to run in parallel
            List<Task> tasks = new List<Task>();

            // loop through attachments and add them to the collection
            foreach (var attachment in AttachmentManager.Attachments)
            {
                var stagedAttachment = new StagedAttachment();
                var loadTask = stagedAttachment.LoadAsync(attachment);


#if WPF
                // add attachment to collection using the UI thread (for the binding to work)
                Application.Current.Dispatcher.Invoke(new Action(() => { Attachments.Add(stagedAttachment); }));
#else
                // add attachment to collection
                Attachments.Add(stagedAttachment);
#endif

                tasks.Add(loadTask);
            }

            // run parallel tasks
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Add new attachment file to the attachment manager
        /// </summary>
        private async void AddNewAttachment(string filePath)
        {
            //retrieve file extension
            var extension = Path.GetExtension(filePath);

            // determine type based on extension
            var contentType = FileExtensionHelper.GetTypeFromExtension(extension);

            // add new attachment to layer
            var newAttachment = AttachmentManager.AddAttachment(filePath, contentType);

            // load the new attachment into a StagedAttachment and add it to Attachments list to display
            var stagedAttachment = new StagedAttachment();
            await stagedAttachment.LoadAsync(newAttachment);
            Attachments.Add(stagedAttachment);
        }

#if NETFX_CORE
        /// <summary>
        /// Converts a StorageFile to byte array
        /// </summary>
        public static async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            if (file == null) return null;
            using (var stream = await file.OpenReadAsync())
            {
                var fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
                return fileBytes;
            }
        }
#endif
    }
}
