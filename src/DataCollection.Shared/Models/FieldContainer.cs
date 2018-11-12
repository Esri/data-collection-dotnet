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
using Esri.ArcGISRuntime.Mapping.Popups;
using System.Collections.Generic;
using System.Linq;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models
{
    public struct FieldContainer
    {
        public PopupFieldValue PopupFieldValue { get; set; }
        public Data.Field OriginalField { get; set; }

        public static IEnumerable<FieldContainer> GetFields(PopupManager popupManager)
        {
            /// <summary>
            /// Gets the underlying Field property for the PopupField in order to retrieve FieldType and Domain
            /// This is a workaround until Domain and FieldType are exposed on the PopupManager
            /// </summary>
            if (popupManager != null)
            {
                return popupManager.EditableDisplayFields.Join(((Feature)popupManager.Popup.GeoElement).FeatureTable.Fields, i =>
                i.Field.FieldName, i => i.Name, (i, j) => new FieldContainer() { PopupFieldValue = i, OriginalField = j });
            }
            return null;
        }
    }
}
