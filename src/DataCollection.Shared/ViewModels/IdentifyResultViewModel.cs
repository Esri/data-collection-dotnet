/*******************************************************************************
  * Copyright 2020 Esri
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
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    /// <summary>
    /// ViewModel for managing multiple identify results, including selecting results and maintaining a list of candidate results.
    /// </summary>
    public class IdentifyResultViewModel : BaseViewModel
    {
        private int? _featureIndex;
        private List<IdentifiedFeatureViewModel> _identifiedFeatures;
        private ICommand _nextResultCommand;
        private ICommand _previousResultCommand;
        private ICommand _clearResultsCommand;
        private ICommand _clearSelectionCommand;

        /// <summary>
        /// Gets the count of the identify results.
        /// </summary>
        public int ResultCount => _identifiedFeatures?.Count ?? 0;

        /// <summary>
        /// Gets or sets the index of the currently selected feature. Setting this value will select
        /// the feature at that index. Setting this value to null will clear selection.
        /// </summary>
        public int? CurrentFeatureIndex
        {
            get => _featureIndex;
            set
            {
                if (value != _featureIndex)
                {
                    _featureIndex = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentlySelectedFeature));
                }
            }
        }

        /// <summary>
        /// Gets the identified features.
        /// </summary>
        public List<IdentifiedFeatureViewModel> IdentifiedFeatures
        {
            get => _identifiedFeatures;
            private set
            {
                if (value != _identifiedFeatures)
                {
                    _identifiedFeatures = value;
                    CurrentFeatureIndex = null;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ResultCount));
                    OnPropertyChanged(nameof(CurrentlySelectedFeature));
                }
            }
        }

        /// <summary>
        /// Gets the currently selected feature. To change selection, see <see cref="CurrentFeatureIndex"/>.
        /// </summary>
        public IdentifiedFeatureViewModel CurrentlySelectedFeature
        {
            get
            {
                if (CurrentFeatureIndex != null)
                {
                    return _identifiedFeatures?.ElementAtOrDefault(CurrentFeatureIndex.Value);
                }
                return null;
            }
        }

        /// <summary>
        /// Updates the identify results, loading features and relationships asynchronously.
        /// </summary>
        public async void SetNewIdentifyResult(IEnumerable<IdentifiedFeatureViewModel> results)
        {
            // Set the updated list.
            IdentifiedFeatures = results?.ToList();

            // Load all of the features, then load all of the relationships.
            var loadTasks = new List<Task>();
            var relationshipTaks = new List<Task>();

            foreach (var feature in IdentifiedFeatures)
            {
                if (feature.Feature is ArcGISFeature arcGISFeature)
                {
                    if (arcGISFeature.LoadStatus != LoadStatus.Loaded)
                    {
                        loadTasks.Add(arcGISFeature.LoadAsync());
                    }
                    relationshipTaks.Add(feature.GetRelationshipInfoForFeature(arcGISFeature));
                }
            }
            try
            {
                await Task.WhenAll(loadTasks);
                await Task.WhenAll(relationshipTaks);
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
            }

            // Select the only feature if there is only one feature
            if (IdentifiedFeatures != null && IdentifiedFeatures.Count() == 1)
            {
                CurrentFeatureIndex = 0;
            }
        }

        /// <summary>
        /// Gets the command that selects the next result in the list, with wraparound.
        /// </summary>
        public ICommand NextResultCommand => _nextResultCommand ?? (_nextResultCommand = new DelegateCommand((parameter) =>
        {
            if (CurrentFeatureIndex != null && ResultCount > 0)
            {
                CurrentFeatureIndex = (CurrentFeatureIndex + 1) % ResultCount;
            }
        }));

        /// <summary>
        /// Gets the command that selects the previous result in the list, with wraparound.
        /// </summary>
        public ICommand PreviousResultCommand => _previousResultCommand ?? (_previousResultCommand = new DelegateCommand((parameter) =>
        {
            if (CurrentFeatureIndex != null && ResultCount > 0)
            {
                CurrentFeatureIndex = ((CurrentFeatureIndex - 1) + ResultCount) % ResultCount;
            }
        }));

        /// <summary>
        /// Gets the command that clears all identify results.
        /// </summary>
        public ICommand ClearResultsCommand => _clearResultsCommand ?? (_clearResultsCommand = new DelegateCommand((parameter) =>
        {
            IdentifiedFeatures = null;
        }));

        /// <summary>
        /// Gets the command that clears the current selection, maintaining the list of results.
        /// </summary>
        public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new DelegateCommand((parameter) =>
        {
            CurrentFeatureIndex = null;
        }));
    }
}