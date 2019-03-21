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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities
{
    /// <summary>
    /// This class contains workflows that are specific to the Tree Survey dataset used as sample dataset for the app
    /// These workflows show good practices in how to use the Geometry Engine and the geocoder
    /// </summary>
    internal static class TreeSurveyWorkflows
    {
        /// <summary>
        /// Gets or sets the field name associated with the tree Neighborhood 
        /// </summary>
        public static string NeighborhoodNameField { get; set; }

        /// <summary>
        /// Gets or sets the layer ID for the later containing the neighborhood polygons
        /// </summary>
        public static string NeighborhoodOperationalLayerId { get; set; }

        /// <summary>
        /// Gets or sets the attribute for the neighborhood in the tree table
        /// </summary>
        public static string NeighborhoodAttribute { get; set; }

        /// <summary>
        /// Gets or sets the attribute for the address in the tree table
        /// </summary>
        public static string AddressAttribute { get; set; }

        /// <summary>
        /// Gets or sets the url for the geocoder used to get addresses for new trees
        /// </summary>
        public static string GeocodeUrl { get; set; }

        /// <summary>
        /// Gets or sets the url for the webmap used by the app
        /// </summary>
        public static string WebmapURL { get; set; }

        /// <summary>
        /// Gets or sets the url for the tree webmap
        /// </summary>
        public static string TreeDatasetWebmapUrl { get; set; }

        /// <summary>
        /// Gets or sets the path for the offline locator used for reverse geocoding when device is offline
        /// </summary>
        public static string OfflineLocatorPath { get; set; }

        /// <summary>
        /// Gets or sets the field name corresponding to the tree condition in the tree table
        /// </summary>
        public static string TreeConditionAttribute { get; set; }

        /// <summary>
        /// Gets or sets the field name corresponding to the tree DBH in the tree table
        /// </summary>
        public static string TreeDBHAttribute { get; set; }

        /// <summary>
        /// Gets or sets the field name corresponding to the tree condition in the inspections table
        /// </summary>
        public static string InspectionConditionAttribute { get; set; }

        /// <summary>
        /// Gets or sets the field name corresponding to the tree DBH in the inspections table
        /// </summary>
        public static string InspectionDBHAttribute { get; set; }

        public static async Task PerformNewTreeWorkflow(LayerCollection layers, Feature feature, MapPoint newFeatureGeometry)
        {
            // if the webmap used in the app is not the tree dataset map, this method will not work
            if (WebmapURL != TreeDatasetWebmapUrl)
            {
                return;
            }

            // Perform the following steps only if the trees sample dataset is present in the map
            var neighborhoodsTable = ((FeatureLayer)layers[Convert.ToInt32(NeighborhoodOperationalLayerId)])?.FeatureTable;
            if (neighborhoodsTable != null && neighborhoodsTable.GeometryType == GeometryType.Polygon &&
                 feature.Attributes.ContainsKey(NeighborhoodAttribute))
            {
                // run custom app workflows to set neighborhood attribute
                feature.Attributes[NeighborhoodAttribute] = await GetNeighborhoodForAddedFeature(
                    neighborhoodsTable, newFeatureGeometry);
            }

            if (feature.Attributes.ContainsKey(AddressAttribute))
            {
                // run custom workflow to set address attribute
                feature.Attributes[AddressAttribute] = await GetAddressForAddedFeature(newFeatureGeometry);
            }
        }


        /// <summary>
        /// Get the neighborhood name for the new tree by running a spatial intersect 
        /// </summary>
        internal static async Task<string> GetNeighborhoodForAddedFeature(FeatureTable neighborhoodsTable, MapPoint newTreePoint)
        {
            // if the webmap used in the app is not the tree dataset map, this method will not work
            if (WebmapURL != TreeDatasetWebmapUrl)
            {
                return null;
            }

            // set the parameters for the query
            // we want only one neighborhood that intersects the geometry of the newly added tree
            var queryParams = new QueryParameters()
            {
                ReturnGeometry = false,
                Geometry = newTreePoint,
                SpatialRelationship = SpatialRelationship.Intersects,
            };

            try
            {
                await neighborhoodsTable.LoadAsync();
                var featureQueryResult = await neighborhoodsTable.QueryFeaturesAsync(queryParams);

                // get the first result and return it's name 
                if (featureQueryResult.Count() > 0)
                {
                    return featureQueryResult.First().Attributes[NeighborhoodNameField].ToString();
                }
            }
            catch { } // if unable to get the neighborhood, just don't populate it

            return null;
        }

        /// <summary>
        /// Get the address for the new tree by running a reverse geocode operation
        /// </summary>
        internal static async Task<string> GetAddressForAddedFeature(MapPoint newTreePoint)
        {
            // if the webmap used in the app is not the tree dataset map, this method will not work
            if (WebmapURL != TreeDatasetWebmapUrl)
            {
                return null;
            }

            try
            {
                LocatorTask locator;

                // try using the online geocoder
                // if that fails, use the sideloaded offline locator
                try
                {
                    locator = await LocatorTask.CreateAsync(new Uri(GeocodeUrl));
                }
                catch
                {
                    locator = await LocatorTask.CreateAsync(
                        new Uri(Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString(), OfflineLocatorPath)));
                }

                // call reverse geocoder
                var matches = await locator.ReverseGeocodeAsync(newTreePoint, new ReverseGeocodeParameters() { IsForStorage = true });

                // in online mode only get the address as that is what the table already has in it
                // in offline mode, only the full address is avaialble, so get that
                if (matches.First().Attributes.ContainsKey("Address"))
                {
                    return matches.First().Attributes["Address"].ToString().ToUpper();
                }
                else
                {
                    return matches.First().Attributes["Match_addr"].ToString().ToUpper();
                }
            }
            catch { } // if unable to get address just don't populate it

            return null;
        }

        /// <summary>
        /// Updates the condition and DBH for the selected tree
        /// This is only used for the Update tree condition custom workflow
        /// </summary>
        internal static async Task UpdateIdentifiedFeature(ObservableCollection<PopupManager> originRelatedRecords, Feature feature, PopupManager popupManager)
        {
            // if the webmap used in the app is not the tree dataset map, this method will not work
            if (WebmapURL != TreeDatasetWebmapUrl)
            {
                return;
            }

            // if there are no inspections available, reset condition and dbh and save feature
            if (originRelatedRecords.Count == 0)
            {
                feature.Attributes[TreeConditionAttribute] = "";
                feature.Attributes[TreeDBHAttribute] = null;
                try
                {
                    await feature.FeatureTable.UpdateFeature(feature);
                    await feature.FeatureTable.ApplyEdits();
                }
                catch (Exception ex)
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(
                        Resources.GetString("ConditionDBH_UpdateError_Title"),
                        ex.Message,
                        true,
                        ex.StackTrace);
                }

                return;
            }

            var OrderedOriginRelationshipCollection = originRelatedRecords.OrderByDescending(PopupManager => PopupManager.DisplayedFields.First().Value);
            var lastInspection = OrderedOriginRelationshipCollection.First();

            // if the condition and dbh are still the same, do not update feature
            if (lastInspection.DisplayedFields.Where(x => x.Field.FieldName == InspectionConditionAttribute) ==
                popupManager.DisplayedFields.Where(x => x.Field.FieldName == TreeConditionAttribute) &&
                lastInspection.DisplayedFields.Where(x => x.Field.FieldName == InspectionDBHAttribute) ==
                popupManager.DisplayedFields.Where(x => x.Field.FieldName == TreeDBHAttribute))
            {
                return;
            }
            else
            {
                // set the new atributes
                // set the condition to empty string if user left it blank
                // set the dbh to 0 if user left it blank
                feature.Attributes[TreeConditionAttribute] =
                    lastInspection.DisplayedFields.Where(x => x.Field.FieldName == InspectionConditionAttribute).First().Value ?? "";
                feature.Attributes[TreeDBHAttribute] =
                    lastInspection.DisplayedFields.Where(x => x.Field.FieldName == InspectionDBHAttribute).First().Value ?? 0f;

                try
                {
                    await feature.FeatureTable.UpdateFeature(feature);
                    await feature.FeatureTable.ApplyEdits();
                }
                catch (Exception ex)
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(
                        Resources.GetString("ConditionDBH_UpdateError_Title"),
                        ex.Message,
                        true,
                        ex.StackTrace);
                }

                // HACK to refresh the popup manager when non editable fields are edited
                try
                {
                    if (!popupManager.IsEditing)
                        popupManager.StartEditing();
                    await popupManager.FinishEditingAsync();
                }
                catch
                {
                    popupManager.CancelEditing();
                }
            }
        }
    }
}

