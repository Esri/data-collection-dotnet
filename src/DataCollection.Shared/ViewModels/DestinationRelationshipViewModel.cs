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

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    public class DestinationRelationshipViewModel : FeatureViewModel
    {

        private RelatedFeatureQueryResult _relatedFeatureQueryResult;
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
        /// Populates the popup manager cache if not already loaded, then selects the matching popup manager for this feature from the cache.
        /// If cache is already populated but a matching popup is not found, the cache is invalidated and re-populated.
        /// </summary>
        public async Task LoadViewModel()
        {
            if (!FeaturePopupCache.Instance.PopupManagersByTable.ContainsKey(FeatureTable))
            {
                // get the related records for all the destination relationships to make available for editing
                await FeaturePopupCache.Instance.PopulateCache(FeatureTable);
                OnPropertyChanged(nameof(OrderedAvailableValues));
            }
            await RefreshSelection();

            // Re-try loading table if matching popup isn't found
            if (PopupManager == null && _relatedFeatureQueryResult.Any())
            {
                await RefreshValues();
            }
        }

        /// <summary>
        /// Select the popup manager for this feature if it exists in the list of popup managers.
        /// </summary>
        private async Task RefreshSelection()
        {
            if (_relatedFeatureQueryResult.Count() > 0)
            {
                var relatedRecord = _relatedFeatureQueryResult.First();

                // load feature to be able to access popup
                if (relatedRecord is ArcGISFeature loadableFeature)
                    await loadableFeature.LoadAsync();

                // choose the selected related record from the list of available values
                // this will enable seamless binding during editing to the list of available values and to the selected value
                foreach (var popupManager in FeaturePopupCache.Instance.PopupManagersByTable[FeatureTable])
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
        /// Re-load the popup manager cache, then re-select the matching popup manager from the cache.
        /// </summary>
        public async Task RefreshValues()
        {
            // get the related records for all the destination relationships to make available for editing
            await FeaturePopupCache.Instance.PopulateCache(FeatureTable, true);
            OnPropertyChanged(nameof(OrderedAvailableValues));
            await RefreshSelection();
        }

        /// <summary>
        /// Gets a list of PopupManagers representing valid choices for this field
        /// </summary>
        /// <remarks>
        /// Values are only loaded once per table per run of the app.
        /// </remarks>
        public IEnumerable<PopupManager> OrderedAvailableValues
        {
            get
            {
                if (FeaturePopupCache.Instance.PopupManagersByTable.ContainsKey(FeatureTable))
                {
                    return FeaturePopupCache.Instance.PopupManagersByTable[FeatureTable];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the RelationshipInfo which keeps track of information about the relationship for editing purposes
        /// </summary>
        public RelationshipInfo RelationshipInfo { get; }

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
