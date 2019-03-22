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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
using System.Text;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Helpers
{
    /// <summary>
    /// Class contains helper methods to avoid code duplication in code behind when prompting user to browse for new attachment
    /// </summary>
    class BrowseHelper
    {
        /// <summary>
        /// Prompts user to select a supported file to add a new attachment and returns file path
        /// If user cancels, null is returned
        /// </summary>
        internal static string GetFileFromUser()
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                var extensionStringBuilder = new StringBuilder("Supported files() |");

                // create list of supported file for the dialog filter
                foreach (var extension in FileExtensionHelper.AllowedExtensions)
                {
                    extensionStringBuilder.Replace(") |", string.Format(" *{0},) |", extension.Key));
                    extensionStringBuilder.Append(string.Format(" *{0};", extension.Key));
                }

                // list of file types supported as attachments
                // https://developers.arcgis.com/rest/services-reference/query-attachments-feature-service-layer-.htm

                dialog.Filter = extensionStringBuilder.ToString();
                dialog.Title = "Please select a file to add as attachment.";
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }

            return null;
        }
    }
}
