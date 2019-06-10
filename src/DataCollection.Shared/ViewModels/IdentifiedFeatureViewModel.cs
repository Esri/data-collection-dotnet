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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
#if WPF
using System.Windows;
#elif NETFX_CORE
using Windows.UI.Core;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class IdentifiedFeatureViewModel : FeatureViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiedFeatureViewModel"/> class.
        /// </summary>
        public IdentifiedFeatureViewModel(Feature feature, ConnectivityMode connectivityMode)
        {
            if (feature != null)
            {
                Feature = feature;
                FeatureTable = feature.FeatureTable as ArcGISFeatureTable;
                PopupManager = new PopupManager(new Popup(feature, FeatureTable.PopupDefinition));
                ConnectivityMode = connectivityMode;
            }
        }

        private DestinationRelationshipViewModel _selectedDestinationRelationship;

        /// <summary>
        /// Gets or sets the active viewmodel for the destination relationships
        /// </summary>
        public DestinationRelationshipViewModel SelectedDestinationRelationship
        {
            get { return _selectedDestinationRelationship; }
            set
            {
                if (_selectedDestinationRelationship != value)
                {
                    // clear the other relationship only if the value isn't null
                    if (value != null)
                    {
                        if (SelectedOriginRelationship != null)
                        {
                            SelectedOriginRelationship = null;
                        }
                    }
                    _selectedDestinationRelationship = value;
                    OnPropertyChanged();
                }
            }
        }

        private OriginRelationshipViewModel _selectedOriginRelationship;

        /// <summary>
        /// Gets or sets the active viewmodel for the origin relationship
        /// </summary>
        public OriginRelationshipViewModel SelectedOriginRelationship
        {
            get { return _selectedOriginRelationship; }
            set
            {
                if (_selectedOriginRelationship != value)
                {
                    _selectedOriginRelationship = value;
                    OnPropertyChanged();

                    // clear the other relationship only if the value isn't null
                    if (value != null)
                    {
                        SelectedDestinationRelationship = null;
                        SelectedOriginRelationship.FeatureCRUDOperationCompleted += OriginRelatedFeature_FeatureCRUDOperationCompleted;

                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of view models that handle the related features to which the identified feature is Destination
        /// </summary>
        public ObservableCollection<DestinationRelationshipViewModel> DestinationRelationships { get; } = new ObservableCollection<DestinationRelationshipViewModel>();

        /// <summary>
        /// Gets or sets the collection of collections of table and view model key value pairs that handle the related features to which the identified feature is Origin
        /// </summary>
        public ObservableCollection<OriginRelationship> OriginRelationships { get; } = new ObservableCollection<OriginRelationship>();

        private ICommand _setSelectedDestinationRelationshipCommand;

        /// <summary>
        /// Gets the command to set the selected destination relationship
        /// </summary>
        public ICommand SetSelectedDestinationRelationshipCommand
        {
            get
            {
                return _setSelectedDestinationRelationshipCommand ?? (_setSelectedDestinationRelationshipCommand = new DelegateCommand(
                    (x) =>
                    {
                        if (x is DestinationRelationshipViewModel destinationRelationshipViewModel)
                        {
                            SelectedDestinationRelationship = destinationRelationshipViewModel;
                        }
                    }));
            }
        }

        private ICommand _setSelectedOriginRelationshipCommand;

        /// <summary>
        /// Gets the command to set the selected origin relationship
        /// </summary>
        public ICommand SetSelectedOriginRelationshipCommand
        {
            get
            {
                return _setSelectedOriginRelationshipCommand ?? (_setSelectedOriginRelationshipCommand = new DelegateCommand(
                    (x) =>
                    {
                        if (x is PopupManager popupManager)
                        {
                            SelectedOriginRelationship = OriginRelationships.First(
                                rels => rels.OriginRelationshipViewModelCollection.FirstOrDefault(
                                    r => r.PopupManager == popupManager) !=
                                    default(OriginRelationshipViewModel)).OriginRelationshipViewModelCollection.FirstOrDefault(r => r.PopupManager == popupManager);
                        }
                    }));
            }
        }

        private ICommand _editFeatureCommand;

        /// <summary>
        /// Gets the command to begin editing the identified feature
        /// </summary>
        public ICommand EditFeatureCommand
        {
            get
            {
                return _editFeatureCommand ?? (_editFeatureCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // clear the related records user may have open
                        SelectedDestinationRelationship = null;
                        if (SelectedOriginRelationship != null && SelectedOriginRelationship.EditViewModel != null)
                        {
                            // if user had edits, wait for response from the user if they truly want to exit editing the related record
                            bool exitWithoutSaving = await UserPromptMessenger.Instance.AwaitConfirmation(
                                Resources.GetString("DiscardEditsConfirmation_Title"),
                                Resources.GetString("DiscardEditsConfirmation_Message"),
                                false,
                                null,
                                Resources.GetString("DiscardButton_Content"));

                            if (!exitWithoutSaving)
                            {
                                return;
                            }
                        }

                        SelectedOriginRelationship = null;
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
                        if (EditViewModel != null)
                        {
                            var editResult = await EditViewModel.SaveEdits(PopupManager, FeatureTable, DestinationRelationships);

                            // close edit session once edit is complete
                            if (editResult != null)
                            {
                                EditViewModel = null;
                            }
                        }
                    }));
            }
        }

        private ICommand _clearRelationshipsCommand;

        /// <summary>
        /// Gets command to clear the related records user has selected
        /// </summary>
        public ICommand ClearRelationshipsCommand
        {
            get
            {
                return _clearRelationshipsCommand ?? (_clearRelationshipsCommand = new DelegateCommand(
                    (x) =>
                    {
                        SelectedOriginRelationship = null;
                        SelectedDestinationRelationship = null;
                    }));
            }
        }

        private ICommand _addOriginRelatedFeatureCommand;

        /// <summary>
        /// Gets the command to add an origin related feature 
        /// </summary>
        public ICommand AddOriginRelatedFeatureCommand
        {
            get
            {
                return _addOriginRelatedFeatureCommand ?? (_addOriginRelatedFeatureCommand = new DelegateCommand(
                    async (x) =>
                    {
                        if (x is OriginRelationship originRelationship)
                        {
                            try
                            {
                                // create a new record and load it
                                var feature = originRelationship.RelatedTable.CreateFeature();

                                if (feature != null && feature is ArcGISFeature)
                                {
                                    // create viewmodel for the feature and set it as selected 
                                    var originRelationshipViewModel = new OriginRelationshipViewModel(originRelationship.RelationshipInfo, ConnectivityMode);
                                    await originRelationshipViewModel.LoadViewModel(feature).ContinueWith(t =>
                                    {
                                        SelectedOriginRelationship = originRelationshipViewModel;
#if WPF
                                        Application.Current.Dispatcher.Invoke(new Action(() => {
                                            originRelationship.OriginRelationshipViewModelCollection.Add(originRelationshipViewModel); }));
#elif NETFX_CORE
                                        Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                                        {
                                            originRelationship.OriginRelationshipViewModelCollection.Add(originRelationshipViewModel);
                                        });
#else
                                        originRelationship.OriginRelationshipViewModelCollection.Add(originRelationshipViewModel);
#endif
                                    });

                                    // related new record to the feature
                                    ((ArcGISFeature)feature).RelateFeature((ArcGISFeature)Feature, SelectedOriginRelationship.RelationshipInfo);

                                    // open editor and finish creating the feature
                                    SelectedOriginRelationship.EditViewModel = new EditViewModel(ConnectivityMode);
                                    SelectedOriginRelationship.EditViewModel.CreateFeature(null, (ArcGISFeature)feature, SelectedOriginRelationship.PopupManager);
                                }
                            }
                            catch (Exception ex)
                            {
                                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                            }
                        }
                    }));
            }
        }

        /// <summary>
        /// Gets relationship information for the identified feature and creates the corresponding viewmodels
        /// </summary>
        internal async Task GetRelationshipInfoForFeature(ArcGISFeature feature)
        {
            // clear related records from previous searches
            DestinationRelationships.Clear();
            OriginRelationships.Clear();

            // get RelationshipInfos from the table
            var relationshipInfos = feature.FeatureTable.GetRelationshipInfos();

            // query only the related tables which match the application rules
            // save destination and origin type relationships separately as origin relates features are editable in the app
            foreach (var relationshipInfo in relationshipInfos)
            {
                var parameters = new RelatedQueryParameters(relationshipInfo);

                // only one related table should return given the specific relationship info passed as parameter
                var relatedTable = feature.FeatureTable.GetRelatedFeatureTable(relationshipInfo);
                var relationships = await feature.FeatureTable.GetRelatedRecords(feature, relationshipInfo);

                if (relationshipInfo.IsValidDestinationRelationship())
                {
                    try
                    {
                        // this is a one to many relationship so it will never return more than one result
                        var relatedFeatureQueryResult = relationships.Where(r => r.IsValidRelationship()).First();

                        var destinationRelationshipViewModel = new DestinationRelationshipViewModel(relationshipInfo, relatedTable, ConnectivityMode);
                        await destinationRelationshipViewModel.InitializeAsync(relatedFeatureQueryResult);

                        DestinationRelationships.Add(destinationRelationshipViewModel);
                    }
                    catch (Exception ex)
                    {
                        UserPromptMessenger.Instance.RaiseMessageValueChanged(null, Resources.GetString("QueryRelatedFeaturesError_Message"), true, ex.StackTrace);
                    }
                }
                else if (relationshipInfo.IsValidOriginRelationship())
                {
                    try
                    {
                        foreach (var relatedFeatureQueryResult in relationships.Where(r => r.IsValidRelationship()))
                        {
                            var originRelationshipsCollection = new ObservableCollection<OriginRelationshipViewModel>();

                            foreach (var relatedFeature in relatedFeatureQueryResult)
                            {
                                var originRelatedFeature = new OriginRelationshipViewModel(relationshipInfo, ConnectivityMode);
                                await originRelatedFeature.LoadViewModel(relatedFeature).ContinueWith(t =>
                                {
                                    originRelationshipsCollection.Add(originRelatedFeature);
                                });
                            }

                            //sort collection
                            SortCollection(originRelationshipsCollection);

                            OriginRelationships.Add(new OriginRelationship(relatedTable, relationshipInfo, originRelationshipsCollection));
                        }
                    }
                    catch (Exception ex)
                    {
                        UserPromptMessenger.Instance.RaiseMessageValueChanged(null, Resources.GetString("GetFeatureRelationshipError_Message"), true, ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler fires when the SelectedOriginRelationship is modified
        /// </summary>
        private async void OriginRelatedFeature_FeatureCRUDOperationCompleted(object sender, FeatureOperationEventArgs e)
        {
            var originRelationshipVM = sender as OriginRelationshipViewModel;
            var originRelationshipVMCollection = (OriginRelationships.FirstOrDefault(o => o.RelationshipInfo == originRelationshipVM.RelationshipInfo)).OriginRelationshipViewModelCollection;

            if (e.Args == CRUDOperation.Delete)
            {
                // remove viewmodel from collection
                originRelationshipVMCollection.Remove(originRelationshipVM);
            }
            else
            {
                //sort collection
                SortCollection(originRelationshipVMCollection);
            }

            try
            {
                // call method to update tree condition and dbh in custom tree workflow
                await TreeSurveyWorkflows.UpdateIdentifiedFeature(originRelationshipVMCollection, Feature, PopupManager);
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);
            }
        }

        /// <summary>
        /// Sorts the collection of origin relationships so it is displayed in order of the first field
        /// </summary>
        /// <param name="originRelationshipVMCollection"></param>
        private void SortCollection(ObservableCollection<OriginRelationshipViewModel> originRelationshipVMCollection)
        {
            List<OriginRelationshipViewModel> sorted = originRelationshipVMCollection.OrderByDescending(x => x.PopupManager?.DisplayedFields?.FirstOrDefault()?.Value).ToList();
            for (int i = 0; i < sorted.Count(); i++)
            {
                originRelationshipVMCollection.Move(originRelationshipVMCollection.IndexOf(sorted[i]), i);
            }
        }
    }
}
