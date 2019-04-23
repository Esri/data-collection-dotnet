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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.Mapping.Popups;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class OriginRelationshipViewModel : FeatureViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationRelationshipViewModel"/> class.
        /// </summary>
        public OriginRelationshipViewModel(RelationshipInfo relationshipInfo, ConnectivityMode connectivityMode)
        {
            RelationshipInfo = relationshipInfo;
            ConnectivityMode = connectivityMode;
        }

        /// <summary>
        /// Loads the necessary prerequisites for OriginRelationshipViewModel
        /// </summary>
        public async Task LoadViewModel(Feature feature)
        {
            if (feature is ArcGISFeature loadableFeature)
            {
                await loadableFeature.LoadAsync();
                Feature = loadableFeature;
                FeatureTable = Feature.FeatureTable as ArcGISFeatureTable;
                PopupManager = new PopupManager(new Popup(Feature, Feature.FeatureTable.PopupDefinition));
            }
        }

        /// <summary>
        /// Gets or sets the RelationshipInfo which keeps track of information about the relationship for editing purposes
        /// </summary>
        public RelationshipInfo RelationshipInfo { get; private set; }

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
                        Feature = await EditViewModel.SaveEdits(PopupManager, FeatureTable, null);
                        RaiseFeatureCRUDOperationCompleted(CRUDOperation.Edit);
                        EditViewModel = null;
                    }));
            }
        }
    }
}
