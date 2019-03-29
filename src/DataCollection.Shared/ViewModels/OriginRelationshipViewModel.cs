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


using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class OriginRelationshipViewModel : EditableFeatureViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationRelationshipViewModel"/> class.
        /// </summary>
        public OriginRelationshipViewModel(ArcGISFeatureTable relatedTable, ConnectivityMode connectivityMode)
        {
            FeatureTable = relatedTable;
            ConnectivityMode = connectivityMode;
        }

        private TaskCompletionSource<bool> _initTcs;

        /// <summary>
        /// Initialization code for the OriginRelationshipViewModel
        /// </summary>
        public Task InitializeAsync(RelatedFeatureQueryResult relatedFeatureQueryResult, RelationshipInfo relationshipInfo)
        {
            if (_initTcs == null)
            {
                _initTcs = new TaskCompletionSource<bool>();
                // Run initialization
                LoadViewModel(relatedFeatureQueryResult, relationshipInfo).ContinueWith(t =>
                {
                    // When init completes, set the task to complete
                    _initTcs.TrySetResult(true);
                });
            }

            return _initTcs.Task;
        }

        /// <summary>
        /// Loads the necessary prerequisites for OriginRelationshipViewModel
        /// </summary>
        public async Task LoadViewModel(RelatedFeatureQueryResult relatedFeatureQueryResult, RelationshipInfo relationshipInfo)
        {
            if (relatedFeatureQueryResult.Count() > 0)
            {
                OriginRelatedRecords = new ObservableCollection<PopupManager>();

                foreach (var relatedRecord in relatedFeatureQueryResult)
                {
                    // load feature to be able to access popup
                    if (relatedRecord is ArcGISFeature loadableFeature)
                    {
                        await loadableFeature.LoadAsync();
                    }

                    var popupManager = new PopupManager(new Popup(relatedRecord, relatedRecord.FeatureTable.PopupDefinition));

                    OriginRelatedRecords.Add(popupManager);
                    RelationshipInfo = relationshipInfo;
                }

                // sort list if more than one record
                if (OriginRelatedRecords.Count > 1)
                {
                    OriginRelatedRecords = new ObservableCollection<PopupManager>(OriginRelatedRecords.OrderByDescending(x => x.DisplayedFields.First().Value));
                }
            }
        }

        /// <summary>
        /// Gets or sets the RelationshipInfo which keeps track of information about the relationship for editing purposes
        /// </summary>
        public RelationshipInfo RelationshipInfo { get; private set; }

        private ObservableCollection<PopupManager> _originRelatedRecords;

        /// <summary>
        /// Gets or sets the collection of origin related records for the current table
        /// </summary>
        public ObservableCollection<PopupManager> OriginRelatedRecords
        {
            get => _originRelatedRecords;
            set
            {
                _originRelatedRecords = value;
                OnPropertyChanged();
            }
        }

        private ICommand _editRelatedRecordCommand;

        /// <summary>
        /// Gets the command to begin editing the related record selected
        /// </summary>
        public ICommand EditRelatedRecordCommand
        {
            get
            {
                return _editRelatedRecordCommand ?? (_editRelatedRecordCommand = new DelegateCommand(
                    (x) =>
                    {
                        EditViewModel = new EditViewModel(ConnectivityMode);
                        PopupManager.StartEditing();
                    }));
            }
        }

        private ICommand _saveEditsCommand;

        /// <summary>
        /// Gets the command to save changes the user made to the feature
        /// </summary>
        public ICommand SaveEditsCommand
        {
            get
            {
                return _saveEditsCommand ?? (_saveEditsCommand = new DelegateCommand(
                    async (x) =>
                    {
                        var feature = await EditViewModel.SaveEdits(PopupManager, FeatureTable, null);

                        if (feature != null)
                        {
                            // if a new record was added, we want to refresh the list so it shows up in the UI
                            GetAddedRecordAndRefresh(feature);
                        }

                        EditViewModel = null;

                        // re-sort the records to account for any edits
                        if (OriginRelatedRecords.Count > 1)
                        {
                            OriginRelatedRecords = new ObservableCollection<PopupManager>(OriginRelatedRecords.OrderByDescending(o => o.DisplayedFields.First().Value));
                        }
                    }));
            }
        }

        /// <summary>
        /// Deletes selected related record
        /// </summary>
        internal async Task<bool> DeleteRelatedRecord()
        {
            var feature = PopupManager?.Popup?.GeoElement as ArcGISFeature;

            if (feature != null)
            {
                try
                {
                    await feature.FeatureTable?.DeleteFeature(feature);
                    await feature.FeatureTable?.ApplyEdits();

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
        /// Once a new record is added successfully, query for it and add it to the list of records in the viewmodel
        /// </summary>
        private void GetAddedRecordAndRefresh(Feature feature)
        {
            var popupManager = new PopupManager(new Popup(feature, feature.FeatureTable.PopupDefinition));

            try
            {
                // if there are no records present, add the feature
                // if records are present, check to see if feature is already there, if not, add it
                if (OriginRelatedRecords == null)
                {
                    OriginRelatedRecords = new ObservableCollection<PopupManager>();
                }
                else
                {
                    foreach (var relatedRecord in OriginRelatedRecords)
                    {
                        if (relatedRecord.Popup.GeoElement.Attributes["OBJECTID"] == feature.Attributes["OBJECTID"])
                        {
                            return;
                        }
                    }
                }
                OriginRelatedRecords.Add(popupManager);

                // set the selected record to the new popup manager
                PopupManager = popupManager;
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
            }
        }
    }
}
