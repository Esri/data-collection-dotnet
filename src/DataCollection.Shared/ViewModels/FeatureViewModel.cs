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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public abstract class FeatureViewModel : BaseViewModel
    {
        private PopupManager _popupManager;

        /// <summary>
        /// Gets or sets the active related record for the origin relationship
        /// </summary>
        public PopupManager PopupManager
        {
            get { return _popupManager; }
            set
            {
                if (_popupManager != value)
                {
                    _popupManager = value;
                    if (value != null)
                    {
                        Fields = FieldContainer.GetFields(value);

                        // If the selected related record changes, fetch the attachments and create a new AttachmentsViewModel
                        PopupManager.AttachmentManager.FetchAttachmentsAsync().ContinueWith(t =>
                        {
                            AttachmentsViewModel = new AttachmentsViewModel(PopupManager, FeatureTable);
                        });
                    }
                    OnPropertyChanged();
                }
            }
        }

        private AttachmentsViewModel _attachmentsViewModel;

        /// <summary>
        /// Gets or sets the AttachmentViewModel to handle viewing and editing attachments 
        /// </summary>
        public AttachmentsViewModel AttachmentsViewModel
        {
            get => _attachmentsViewModel;
            set
            {
                if (_attachmentsViewModel != value)
                {
                    _attachmentsViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private ArcGISFeatureTable _featureTable;

        /// <summary>
        /// Gets or sets the RelatedTable 
        /// </summary>
        public ArcGISFeatureTable FeatureTable
        {
            get => _featureTable;
            set
            {
                if (_featureTable != value)
                {
                    _featureTable = value;
                    OnPropertyChanged();
                }
            }
        }

        private IEnumerable<FieldContainer> _fields;

        /// <summary>
        /// Gets the underlying Field property for the PopupField in order to retrieve FieldType and Domain
        /// This is a workaroud until Domain and FieldType are exposed on the PopupManager
        /// </summary>
        public IEnumerable<FieldContainer> Fields
        {
            get => _fields;
            set
            {
                if (_fields != value)
                {
                    _fields = value;
                    OnPropertyChanged();
                }
            }
        }

        private Feature _feature;

        /// <summary>
        /// Gets or sets the feature currently selected
        /// </summary>
        public Feature Feature {
            get => _feature;
            set
            {
                if (_feature != value)
                {
                    _feature = value;
                    if (value != null)
                    {
                        if (value.IsNewFeature())
                            IsNewFeature = true;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private bool _isNewFeature;

        /// <summary>
        /// Gets or sets the property designating if current feature is new and has not been commited
        /// </summary>
        public bool IsNewFeature
        {
            get => _isNewFeature;
            set
            {
                if (_isNewFeature != value)
                {
                    _isNewFeature = value;
                    OnPropertyChanged();
                }
            }
        }

        private ConnectivityMode _connectivityMode;

        /// <summary>
        /// Gets or sets the ConnectivityMode
        /// </summary>
        public ConnectivityMode ConnectivityMode {
            get => _connectivityMode;

            set
            {
                if (_connectivityMode != value)
                {
                    _connectivityMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private EditViewModel _editViewModel;

        /// <summary>
        /// Gets or sets the viewmodel for the current edit session
        /// </summary>
        public EditViewModel EditViewModel
        {
            get => _editViewModel;
            set
            {
                if (_editViewModel != value)
                {
                    _editViewModel = value;
                    if (value == null)
                        IsNewFeature = false;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Deletes identified feature
        /// </summary>
        internal async Task<bool> DeleteFeature()
        {
            if (Feature != null)
            {
                try
                {
                    await FeatureTable?.DeleteFeature(Feature);
                    await FeatureTable?.ApplyEdits();
                    RaiseFeatureCRUDOperationCompleted(CRUDOperation.Delete);
                    return true;
                }
                catch (Exception ex)
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Discards edits performed on a feature 
        /// </summary>
        internal async Task<bool> DiscardChanges()
        {
            if (PopupManager.HasEdits())
            {
                // wait for response from the user if the truly want to cancel the edit operation
                bool cancelEdits = await UserPromptMessenger.Instance.AwaitConfirmation(
                    Resources.GetString("DiscardEditsConfirmation_Title"),
                    Resources.GetString("DiscardEditsConfirmation_Message"),
                    false,
                    null,
                    Resources.GetString("DiscardButton_Content"));

                if (!cancelEdits)
                {
                    return false;
                }
            }

            // cancel the edits if the PopupManager doesn't have any edits or if the user chooses to
            EditViewModel.CancelEdits(PopupManager);
            EditViewModel = null;
            await AttachmentsViewModel.LoadAttachments();
            return true;
        }

        public event EventHandler<FeatureOperationEventArgs> FeatureCRUDOperationCompleted;


        /// <summary>
        /// Event handler called when feature is modified
        /// </summary>
        /// <param name="operation"></param>
        internal void RaiseFeatureCRUDOperationCompleted(CRUDOperation operation)
        {
            FeatureCRUDOperationCompleted?.Invoke(this, new FeatureOperationEventArgs()
            {
                Args = operation
            });
        }
    }

    /// <summary>
    /// Event args for CRUD feature operation event handler
    /// </summary>
    public class FeatureOperationEventArgs : EventArgs
    {
        public CRUDOperation Args { get; set; }
    }

    /// <summary>
    /// Enum of edit types done on feature
    /// </summary>
    public enum CRUDOperation
    {
        Add,
        Edit,
        Delete
    }
}
