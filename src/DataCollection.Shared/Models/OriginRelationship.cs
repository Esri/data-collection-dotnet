using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models
{
    public class OriginRelationship
    {
        public OriginRelationship(ArcGISFeatureTable relatedTable, RelationshipInfo relationshipInfo, ObservableCollection<OriginRelationshipViewModel> originRelationshipViewModelCollection)
        {
            RelatedTable = relatedTable;
            RelationshipInfo = relationshipInfo;
            OriginRelationshipViewModelCollection = originRelationshipViewModelCollection;
        }
        public ObservableCollection<OriginRelationshipViewModel> OriginRelationshipViewModelCollection { get; }

        public ArcGISFeatureTable RelatedTable { get; }

        public RelationshipInfo RelationshipInfo { get; }
    
    }
}
