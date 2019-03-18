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
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Linq;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions
{
    /// <summary>
    /// Class containing extension methods for handling mobile map packages
    /// </summary>
    internal static class MobileMapPackageExtensions
    {
        /// <summary>
        /// Extension method to close the databases inside the mobile map package
        /// </summary>
        internal static void Close(this MobileMapPackage mobileMapPackage)
        {
            // if a mobile map package is loaded, close it first
            if (mobileMapPackage != null)
            {
                foreach (var map in mobileMapPackage.Maps)
                {
                    foreach (var layer in map.AllLayers.OfType<FeatureLayer>())
                    {
                        if (layer.FeatureTable is GeodatabaseFeatureTable)
                        {
                            ((GeodatabaseFeatureTable)layer.FeatureTable).Geodatabase.Close();
                        }
                    }
                    foreach (var table in map.Tables)
                    {
                        if (table is GeodatabaseFeatureTable)
                        {
                            ((GeodatabaseFeatureTable)table).Geodatabase.Close();
                        }
                    }

                    // set basemap to null and force garbage collection to release vector tiles
                    // TODO: this does not seem to be effective, research another way to resease vtpk
                    map.Basemap = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }
}
