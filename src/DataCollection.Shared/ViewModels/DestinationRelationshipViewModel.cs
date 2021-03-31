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
        // Global cache reduces repetitive queries when loading multiple results
        private static Dictionary<ArcGISFeatureTable, IEnumerable<PopupManager>> CachedTableResults = new Dictionary<ArcGISFeatureTable, IEnumerable<PopupManager>>();
        // Synchronizes access to the global cache when loading multiple results in parallel
        private static SemaphoreSlim cacheMutex = new SemaphoreSlim(1, 1);
        // Local copy of available values
        private IEnumerable<PopupManager> _availableValues;

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
            // Loads all popups in the table to enable selection from a list
            await RefreshAvailableValues();
            // Updates the popup representing current selection to ensure it matches entry in list of values (important for combobox binding)
            await LoadSelectedFeaturePopup();
        }

        /// <summary>
        /// Finds the popup in <see cref="OrderedAvailableValues"/> that matches the currently selected value to <see cref="FeatureViewModel.PopupManager"/>.
        /// </summary>
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
            get => _availableValues;
            private set
            {
                _availableValues = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tracks whether a refresh of <see cref="OrderedAvailableValues"/> is in progress
        /// </summary>
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

        /// <summary>
        /// Starts a refresh of the <see cref="OrderedAvailableValues"/> collection.
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
        /// Loads <see cref="OrderedAvailableValues"/> with popups for all of the entries in the relevant table.
        /// Values loaded from cache if <paramref name="invalidateCache"/> is false, otherwise cache is overwritten.
        /// </summary>
        private async Task RefreshAvailableValues(bool invalidateCache = false)
        {
            // Ensures that cache is only updated/checked by one Task at a time
            await cacheMutex.WaitAsync();

            try
            {
                if (!invalidateCache && CachedTableResults.ContainsKey(FeatureTable))
                {
                    OrderedAvailableValues = CachedTableResults[FeatureTable];
                    return;
                }
                IsRefreshingValues = true;

                var queryParams = new QueryParameters { ReturnGeometry = false, WhereClause = "1=1" };

                // Query and load all related records
                var featureQueryResult = (await FeatureTable.QueryFeatures(queryParams)).ToList();
                var availableValues = new List<PopupManager>(featureQueryResult.Count);

                foreach (ArcGISFeature result in featureQueryResult)
                {
                    await result.LoadAsync();
                    availableValues.Add(new PopupManager(new Popup(result, result.FeatureTable.PopupDefinition)));
                }

                // sort the list of related records based on the first display field from the popup manager
                if (availableValues.FirstOrDefault()?.DisplayedFields?.Any() ?? false)
                {
                    OrderedAvailableValues = availableValues.OrderBy(PopupManager => PopupManager?.DisplayedFields?.First().Value).ToList();
                    CachedTableResults[FeatureTable] = OrderedAvailableValues;
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
