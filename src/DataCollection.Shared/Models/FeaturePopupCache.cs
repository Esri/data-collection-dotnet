/*******************************************************************************
  * Copyright 2021 Esri
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
using Esri.ArcGISRuntime.Mapping.Popups;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models
{
    /// <summary>
    /// Represents a global cache of PopupManagers used to enable selection from existing values for
    /// a related table (e.g. to show a popup list of available species for a tree record).
    /// </summary>
    public class FeaturePopupCache
    {
        private static SemaphoreSlim SlowStuffSemaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Static reference is available for shared use
        /// </summary>
        public static FeaturePopupCache Instance = new FeaturePopupCache();

        /// <summary>
        /// Stores cached query results by table.
        /// </summary>
        public Dictionary<FeatureTable, IEnumerable<PopupManager>> PopupManagersByTable = new Dictionary<FeatureTable, IEnumerable<PopupManager>>();

        /// <summary>
        /// Performs the query, populating the cache with an ordered list of popup managers, each manager representing a feature in the table.
        /// </summary> 
        public async Task PopulateCache(FeatureTable table, bool invalidateCache = false)
        {
            await SlowStuffSemaphore.WaitAsync();
            if (!invalidateCache && PopupManagersByTable.ContainsKey(table))
            {
                SlowStuffSemaphore.Release();
                return;
            }
            if (table is ServiceFeatureTable sft)
            {
                sft.FeatureRequestMode = FeatureRequestMode.ManualCache;
                await sft.LoadAsync();

                var allFeatures = await sft.PopulateFromServiceAsync(new QueryParameters { WhereClause = "1=1" }, true, new[] { "*" });

                PopupManagersByTable[table] = allFeatures
                    .Select(feature => new PopupManager(new Popup(feature, feature.FeatureTable.PopupDefinition)))
                    .OrderBy(popup => popup?.DisplayedFields?.FirstOrDefault()?.Value)
                    .ToList();
            }
            // Handle offline case
            else if (table is ArcGISFeatureTable aft)
            {
                var allFeatures = await aft.QueryFeaturesAsync(new QueryParameters { WhereClause = "1=1" });

                PopupManagersByTable[table] = allFeatures
                    .Select(feature => new PopupManager(new Popup(feature, feature.FeatureTable.PopupDefinition)))
                    .OrderBy(popupmanager => popupmanager?.DisplayedFields?.FirstOrDefault()?.Value)
                    .ToList();
            }
            SlowStuffSemaphore.Release();
        }
    }
}
