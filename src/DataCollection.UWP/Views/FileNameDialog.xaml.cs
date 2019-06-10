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

using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Views
{
    public sealed partial class FileNameDialog : ContentDialog
    {
        private const string Filter = @"^[^\\\/:*?""<>|\s]+$";

        /// <summary>
        /// Gets or sets the name of the file to be saved
        /// </summary>
        public string FileName { get; private set; }

        public FileNameDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler that validates file name entered by user based on the regular expression in Filter
        /// </summary>
        private void FileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(FileNameTextBox.Text, Filter) && !string.IsNullOrEmpty(FileNameTextBox.Text))
            {
                IsPrimaryButtonEnabled = true;
                ValidationTextBlock.Text = "";
            }
            else
            {
                IsPrimaryButtonEnabled = false;
                ValidationTextBlock.Text = "Invalid file name";
            }
        }

        /// <summary>
        /// Event handler sets the FileName property when user hits OK
        /// </summary>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            FileName = FileNameTextBox.Text;
        }
    }
}
