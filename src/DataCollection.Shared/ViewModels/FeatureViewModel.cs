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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.Mapping.Popups;
using System.Collections.Generic;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public abstract class FeatureViewModel : BaseViewModel
    {
        private ConnectivityMode _connectivityMode;

        /// <summary>
        /// Gets or sets the ConnectivityMode
        /// </summary>
        public ConnectivityMode ConnectivityMode
        {
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
                _fields = value;
                OnPropertyChanged();
            }
        }

        private PopupManager _popupManager;

        /// <summary>
        /// Gets the PopupManager for the selected feature
        /// </summary>
        public PopupManager PopupManager
        {
            get => _popupManager;
            set
            {
                if (_popupManager != value)
                {
                    _popupManager = value;
                    if (value != null)
                    {
                        Fields = FieldContainer.GetFields(PopupManager);

                        // If the selected record changes, fetch the attachments and create a new AttachmentsViewModel
                        PopupManager.AttachmentManager.FetchAttachmentsAsync().ContinueWith(t =>
                        {
                            AttachmentsViewModel = new AttachmentsViewModel(PopupManager, FeatureTable);
                        });
                    }
                    OnPropertyChanged();
                }
            }
        }

        private FeatureTable _featureTable;

        /// <summary>
        /// Gets the feature table for the layer
        /// </summary>
        public FeatureTable FeatureTable
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

        private AttachmentsViewModel _attachmentsViewModel;

        /// <summary>
        /// Gets or sets the AttachmentViewModel to handle viewing and editing attachments 
        /// </summary>
        public AttachmentsViewModel AttachmentsViewModel
        {
            get => _attachmentsViewModel;
            set
            {
                _attachmentsViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}
