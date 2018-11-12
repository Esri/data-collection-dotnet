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

using Esri.ArcGISRuntime.Mapping.Popups;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions
{
    /// <summary>
    /// Extension methods for the PopupManager
    /// </summary>
    internal static class PopupManagerExtensions
    {
        /// <summary>
        /// Extension method that determines if the PopupManager has validation errors on any of its fields
        /// </summary>
        internal static bool HasValidationErrors(this PopupManager popupManager)
        {
            foreach (var field in popupManager.EditableDisplayFields)
            {
                if (popupManager.GetValidationError(field.Field) != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Extension method that determines if the PopupManager has had any of its fields modified
        /// </summary>
        internal static bool HasEdits(this PopupManager popupManager)
        {
            foreach (var field in popupManager.EditableDisplayFields)
            {
                if (field.HasChanges)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
