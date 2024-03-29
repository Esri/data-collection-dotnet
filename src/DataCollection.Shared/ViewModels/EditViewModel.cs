﻿/*******************************************************************************
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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Mapping;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    public class EditViewModel : BaseViewModel
    {
        public EditViewModel(ConnectivityMode connectivityMode)
        {
            ConnectivityMode = connectivityMode;
        }

        public ConnectivityMode ConnectivityMode { get; }

        /// <summary>
        /// Cancels any edits the user is making without saving changes
        /// </summary>
        internal void CancelEdits(PopupManager popupManager)
        {
            try
            {
                // the API throws a System.FormatException when an invalid value is present rather than discarding the value
                // this is a workaround to clear all values before canceling, until the issue is resolved in the API
                foreach (var field in popupManager.EditableDisplayFields)
                {
                    if (field.ValidationError != null)
                        field.Value = field.OriginalValue;
                }
                popupManager.CancelEditing();
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
            }
        }

        /// <summary>
        /// Logic to create a feature
        /// Adds geometry to the feature and adds it to the table
        /// </summary>
        internal void CreateFeature(MapPoint point, ArcGISFeature feature, PopupManager popupManager)
        {
            try
            {
                popupManager.StartEditing();
                // if appropriate, add geometry to the newly created feature
                if (point != null)
                {
                    // assign geometry and add the new feature to the table
                    feature.Geometry = point;
                }
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
            }
        }

        /// <summary>
        /// Logic to save the edits made to a feature 
        /// </summary>
        internal async Task<Feature> SaveEdits(PopupManager popupManager, FeatureTable table, ObservableCollection<DestinationRelationshipViewModel> destinationRelationships)
        {
            // feature that was edited
            var editedFeature = popupManager.Popup.GeoElement as ArcGISFeature;

            // verify that the PopupManager doesn't have any errors and prompt the user to fix errors before allowing them to save
            if (popupManager.HasValidationErrors())
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(Properties.Resources.GetString("InvalidInput_Title"),
                                     Properties.Resources.GetString("InvalidInput_Message"),
                                     true);
                return null;
            }

            try
            {
                BusyWaitingMessenger.Instance.SetBusy(true,
                    Properties.Resources.GetString("SavingEditsWait_Message"));

                // exit the PopupManager edit session
                try
                {
                    // TODO - find out why feature can't be saved without manual editing
                    await popupManager.FinishEditingAsync();
                }
                catch (Exception popupException)
                {
                    throw new Exception("You must make edits before saving", popupException);
                }
                
                // get relationship changes (if they exist) and relate them to the feature
                foreach (var destinationRelationship in destinationRelationships ?? new ObservableCollection<DestinationRelationshipViewModel>())
                {
                    if (destinationRelationship.PopupManager != null)
                    {
                        try
                        {
                            editedFeature.RelateFeature((ArcGISFeature) destinationRelationship.PopupManager.Popup.GeoElement, destinationRelationship.RelationshipInfo);
                        }
                        catch (Exception ex)
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                        }
                    }
                }

                // update feature
                await table.UpdateFeatureAsync(editedFeature);

                // apply edits to save the changes
                await editedFeature.FeatureTable.ApplyEdits();

                // refresh the attributes and geometry of the feature after saving
                // this will returna valid ObjectID
                editedFeature.Refresh();

                // re-select the feature (new features get unselected after the ObjectID is refreshed)
                if (editedFeature.Geometry != null && table.Layer is FeatureLayer featureLayer)
                {
                    featureLayer.SelectFeature(editedFeature);
                }

                return editedFeature;
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                return null;
            }
            finally
            {
                BusyWaitingMessenger.Instance.SetBusy(false, "");
            }
        }
    }
}
