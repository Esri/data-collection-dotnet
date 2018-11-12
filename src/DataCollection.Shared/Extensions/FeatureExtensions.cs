/*******************************************************************************
  * Copyright 2018 Esri
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
using System;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions
{
    internal static class FeatureExtensions
    {
        /// <summary>
        /// Determines if feature is new and has not yet been added to the table
        /// </summary>
        internal static bool IsNewFeature(this Feature feature)
        {
            // refresh feature in case it was just commited
            feature.Refresh();

            // get OID field from feature table and determine if OID is null
            if (feature.FeatureTable is ServiceFeatureTable serviceFeatureTable)
            {
                if (feature.Attributes[serviceFeatureTable.ObjectIdField] == null)
                {
                    return true;
                }
                return false;
            }
            else if (feature.FeatureTable is GeodatabaseFeatureTable geodatabaseFeatureTable)
            {
                if (feature.Attributes[geodatabaseFeatureTable.ObjectIdField] == null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
