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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions
{
    internal static class FeatureTableExtensions
    {
        /// <summary>
        /// Retrieves a related table based on information from the relationship info
        /// </summary>
        internal static ArcGISFeatureTable GetRelatedFeatureTable(this FeatureTable featureTable, RelationshipInfo relationshipInfo)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                return serviceFeatureTable.GetRelatedTables(relationshipInfo).First();
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                return geodatabaseFeatureTable.GetRelatedTables(relationshipInfo).First();
            }
            return null;
        }

        /// <summary>
        /// Retrieves records related to a feature based on information from the relationship info
        /// </summary>
        internal static async Task<IReadOnlyList<RelatedFeatureQueryResult>> GetRelatedRecords(this FeatureTable featureTable, Feature feature, RelationshipInfo relationshipInfo)
        {
            var parameters = new RelatedQueryParameters(relationshipInfo);

            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                return await serviceFeatureTable.QueryRelatedFeaturesAsync((ArcGISFeature)feature, parameters, QueryFeatureFields.LoadAll);
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                return await geodatabaseFeatureTable.QueryRelatedFeaturesAsync((ArcGISFeature)feature, parameters);
            }
            return null;
        }

        /// <summary>
        /// Retrieves all relationship infos for a table
        /// </summary>
        internal static IReadOnlyList<RelationshipInfo> GetRelationshipInfos(this FeatureTable featureTable)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                return serviceFeatureTable.LayerInfo.RelationshipInfos;
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                return geodatabaseFeatureTable.LayerInfo.RelationshipInfos;
            }
            return null;
        }

        /// <summary>
        /// Calls update on a feature
        /// </summary>
        internal static async Task UpdateFeature(this FeatureTable featureTable, Feature feature)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                await serviceFeatureTable.UpdateFeatureAsync(feature);
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                await geodatabaseFeatureTable.UpdateFeatureAsync(feature);
            }
        }

        /// <summary>
        /// Calls delete on a feature
        /// </summary>
        internal static async Task DeleteFeature(this FeatureTable featureTable, Feature feature)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                await serviceFeatureTable.DeleteFeatureAsync(feature);
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                await geodatabaseFeatureTable.DeleteFeatureAsync(feature);
            }
        }

        /// <summary>
        /// Runs a query on the specified table
        /// </summary>
        internal static async Task<FeatureQueryResult> QueryFeatures(this FeatureTable featureTable, QueryParameters queryParameters)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                return await serviceFeatureTable.QueryFeaturesAsync(queryParameters, QueryFeatureFields.LoadAll);
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                return await geodatabaseFeatureTable.QueryFeaturesAsync(queryParameters);
            }
            return null;
        }


        /// <summary>
        /// Applies edits to a table
        /// </summary>
        internal static async Task ApplyEdits(this FeatureTable featureTable)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                await serviceFeatureTable.ApplyEditsAsync();
            }
        }

        /// <summary>
        /// Determines if the feature table has attachments enabled
        /// </summary>
        internal static bool HasAttachments(this FeatureTable featureTable)
        {
            if (featureTable is ServiceFeatureTable serviceFeatureTable)
            {
                return serviceFeatureTable.HasAttachments;
            }
            else if (featureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                return geodatabaseFeatureTable.HasAttachments;
            }
            return false;
        }
    }
}
