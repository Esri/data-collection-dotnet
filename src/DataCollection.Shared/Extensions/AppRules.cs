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

using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions
{
    /// <summary>
    /// Application rules for determining identifiable layers and valid relationships. 
    /// </summary>
    public static class AppRules
    {
        /// <summary>
        /// Rule applied to feature layers to determine if they are identifiable
        /// </summary>
        public static bool IsIdentifiable(this Layer layer)
        {
            if (layer is FeatureLayer featureLayer)
            {
                if (featureLayer.IsVisible && featureLayer.IsPopupEnabled && featureLayer.PopupDefinition != null &&
                    featureLayer.FeatureTable.GeometryType == Geometry.GeometryType.Point)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Rule applied to relationships to determine if they are valid in the context of this application
        /// </summary>
        public static bool IsValidRelationship(this RelatedFeatureQueryResult relatedFeatureQueryResult)
        {
            return (relatedFeatureQueryResult.RelatedTable.PopupDefinition != null); 
        }

        /// <summary>
        /// Rule applied to destination relationships to determine if they are valid in the context of this application
        /// </summary>
        public static bool IsValidDestinationRelationship(this RelationshipInfo relationshipInfo)
        {
            if ((relationshipInfo.Cardinality == RelationshipCardinality.OneToMany || relationshipInfo.Cardinality == RelationshipCardinality.OneToOne)
                && relationshipInfo.Role == RelationshipRole.Destination)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Rule applied to origin relationships to determine if they are valid in the context of this application
        /// </summary>
        public static bool IsValidOriginRelationship(this RelationshipInfo relationshipInfo)
        {
            if ((relationshipInfo.Cardinality == RelationshipCardinality.OneToMany || relationshipInfo.Cardinality == RelationshipCardinality.OneToOne)
                && relationshipInfo.Role == RelationshipRole.Origin)
            {
                return true;
            }
            return false;
        }
    }
}
