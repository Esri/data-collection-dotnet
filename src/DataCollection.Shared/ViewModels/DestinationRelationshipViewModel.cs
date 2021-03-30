/*******************************************************************************
  * Copyright 2019 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  https://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using System.Threading;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    public class DestinationRelationshipViewModel : FeatureViewModel
    {
        private static Dictionary<ArcGISFeatureTable, IEnumerable<PopupManager>> CachedTableResults = new Dictionary<ArcGISFeatureTable, IEnumerable<PopupManager>>();
        private static SemaphoreSlim cacheMutex = new SemaphoreSlim(1, 1);
        private RelatedFeatureQueryResult _relatedFeatureQueryResult;
        private bool _isRefreshingValues;
        private ICommand _refreshValuesCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationRelationshipViewModel"/> class.
        /// </summary>
        public DestinationRelationshipViewModel(RelationshipInfo relationshipInfo, ArcGISFeatureTable relatedTable, ConnectivityMode connectivityMode, RelatedFeatureQueryResult relatedFeatureQueryResult)
        {
            RelationshipInfo = relationshipInfo;
            FeatureTable = relatedTable;
            ConnectivityMode = connectivityMode;
            _relatedFeatureQueryResult = relatedFeatureQueryResult;
        }

        /// <summary>
        /// Loads the necessary prerequisites for DestinationRelationshipViewModel
        /// </summary>
        public async Task LoadViewModel()
        {
            // get the related records for all the destination relationships to make available for editing
            await RefreshAvailableValues();
            await LoadSelectedFeaturePopup();
        }

        public async Task LoadSelectedFeaturePopup()
        {
            if (_relatedFeatureQueryResult.FirstOrDefault() is ArcGISFeature relatedRecord)
            {
                // load feature to be able to access popup
                await relatedRecord.LoadAsync();

                // choose the selected related record from the list of available values
                // this will enable seamless binding during editing to the list of available values and to the selected value
                foreach (var popupManager in OrderedAvailableValues)
                {
                    if (popupManager.DisplayedFields.Count() > 0 && AreAttributeValuesTheSame(popupManager, relatedRecord))
                    {
                        PopupManager = popupManager;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the RelationshipInfo which keeps track of information about the relationship for editing purposes
        /// </summary>
        public RelationshipInfo RelationshipInfo { get; }

        /// <summary>
        /// Gets or sets the collection of available values to select from for the related record
        /// </summary>
        public IEnumerable<PopupManager> OrderedAvailableValues
        {
            get => CachedTableResults.ContainsKey(FeatureTable) ? CachedTableResults[FeatureTable] : null;
            set
            {
                CachedTableResults[FeatureTable] = value;
                OnPropertyChanged();
            }
        }

        public bool IsRefreshingValues
        {
            get => _isRefreshingValues;
            set
            {
                if (_isRefreshingValues != value)
                {
                    _isRefreshingValues = value;
                    OnPropertyChanged();
                    (RefreshValuesCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// </summary>
        public ICommand RefreshValuesCommand => _refreshValuesCommand ?? (_refreshValuesCommand = new DelegateCommand(parm =>
		    {
	            _ = RefreshAvailableValues(true).ContinueWith(res => LoadSelectedFeaturePopup());
		    },
            (param) =>
            {
                return !IsRefreshingValues;
            })
        );

        /// <summary>
        /// Get all of the values in the related table to display when editing feature
        /// </summary>
        private async Task RefreshAvailableValues(bool invalidateCache = false)
        {
            await cacheMutex.WaitAsync();

            try
            {
                if (!invalidateCache && CachedTableResults.ContainsKey(FeatureTable))
                {
                    return;
                }
                IsRefreshingValues = true;

                var queryParams = new QueryParameters()
                {
                    ReturnGeometry = false,
                    WhereClause = "1=1",
                };
                // Query and load all related records
                var featureQueryResult = await FeatureTable.QueryFeatures(queryParams);
                var availableValues = new ObservableCollection<PopupManager>();

                foreach (ArcGISFeature result in featureQueryResult)
                {
                    if (result.LoadStatus != LoadStatus.Loaded)
                    {
                        await result.LoadAsync();
                    }
                    availableValues.Add(new PopupManager(new Popup(result, result.FeatureTable.PopupDefinition)));
                }

                // sort the list of related records based on the first display field from the popup manager
                if (availableValues.Count > 0 && availableValues.First().DisplayedFields.Count() > 0)
                {
                    OrderedAvailableValues = availableValues.OrderBy(PopupManager => PopupManager?.DisplayedFields?.First().Value);
                }
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
            }
            finally
            {
                IsRefreshingValues = false;
                cacheMutex.Release();
            }
        }

        /// <summary>
        /// Tests if the popup manager and the feature have the same values for attributes
        /// </summary>
        private bool AreAttributeValuesTheSame(PopupManager popupManager, Feature feature)
        {
            foreach (var field in popupManager.DisplayedFields)
            {
                if (field.Value?.ToString() != feature.Attributes[field.Field.FieldName]?.ToString())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
