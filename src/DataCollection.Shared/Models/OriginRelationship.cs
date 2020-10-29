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
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System.Collections.ObjectModel;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models
{
    /// <summary>
    /// Contains a relationship of type Origin 
    /// </summary>
    public class OriginRelationship
    {
        public OriginRelationship(ArcGISFeatureTable relatedTable, RelationshipInfo relationshipInfo, ObservableCollection<OriginRelationshipViewModel> originRelationshipViewModelCollection)
        {
            RelatedTable = relatedTable;
            RelationshipInfo = relationshipInfo;
            OriginRelationshipViewModelCollection = originRelationshipViewModelCollection;
        }

        /// <summary>
        /// Gets the collection of viewmodels for the relationship
        /// </summary>
        public ObservableCollection<OriginRelationshipViewModel> OriginRelationshipViewModelCollection { get; }

        /// <summary>
        /// Gets the realted table for the relationship
        /// </summary>
        public ArcGISFeatureTable RelatedTable { get; }

        /// <summary>
        /// Gets the relationship info for the relationship
        /// </summary>
        public RelationshipInfo RelationshipInfo { get; }
    }
}
