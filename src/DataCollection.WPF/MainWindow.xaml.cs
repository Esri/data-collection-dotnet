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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Views;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            // create event handler for when the message from the viewmodel changes
            UserPromptMessenger.Instance.MessageValueChanged += DialogBoxMessenger_MessageValueChanged;

            // load settings for the authentication viewmodel
            AuthStackPanel.DataContext = new AuthViewModel(
                Settings.Default.WebmapURL,
                Settings.Default.ArcGISOnlineURL,
                Settings.Default.AppClientID,
                Settings.Default.RedirectURL,
                Settings.Default.OAuthRefreshToken);

            _mainViewModel = TryFindResource("MainViewModel") as MainViewModel;

            // load settings for the custom tree survey dataset
            LoadTreeSurveySettings();

            // create event handler for property changed to determine when user chooses to work offline
            // this only needs to run if there is no DownloadPath already set or it's invalid
            if (string.IsNullOrEmpty(_mainViewModel.DownloadPath) || !Directory.Exists(_mainViewModel.DownloadPath))
            {
                _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// Handles the property changed event for the MainViewModel
        /// </summary>
        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // When the IsMapDownloading property becomes true, prompt user to choose folder
            if (e.PropertyName == nameof(MainViewModel.DownloadViewModel) && ((MainViewModel)sender).DownloadViewModel != null)
            {
                PromptUserForDownloadDirectory();
            }
        }

        /// <summary>
        /// Handles changes in messages coming from the viewmodel by opening a dialog box and prompting the user for answer
        /// </summary>
        private void DialogBoxMessenger_MessageValueChanged(object sender, UserPromptMessageChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DialogBoxWindow dialogBox = new DialogBoxWindow(e.Message, e.MessageTitle, e.IsError, e.StackTrace, e.AffirmativeActionButtonContent, e.NegativeActionButtonContent);
                dialogBox.Closing += DialogBox_Closing;

                dialogBox.ShowDialog();
            });
        }

        /// <summary>
        /// Handles closing the dialog box by raising response value changed event
        /// </summary>
        private void DialogBox_Closing(object sender, CancelEventArgs e)
        {
            UserPromptMessenger.Instance.RaiseResponseValueChanged(((DialogBoxWindow)sender).Response);
        }

        /// <summary>
        /// Prompt user to select where the offline version of the map should be saved
        /// This fires only the first time the user selects to download a map

        /// </summary>
        private void PromptUserForDownloadDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.Description = "Data Collection app needs to download the map on your device. Please select the folder where the map can be stored.";
                var result = dialog.ShowDialog();

                // TODO: Test that user has write permissions to the directory
                // TODO: Test that the user selected a valid directory
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(Path.Combine(dialog.SelectedPath, "Mmpk"), BroadcastMessageKey.DownloadPath);

                    // double check that path was set before removing the event handler
                    if (!string.IsNullOrEmpty(Settings.Default.DownloadPath))
                    {
                        _mainViewModel.PropertyChanged -= MainViewModel_PropertyChanged;
                    }
                }
                else
                {
                    _mainViewModel.DownloadViewModel = null;
                }
            }
        }

        /// <summary>
        /// Loads the settings for the custom workflows required bu the tree survey sample dataset
        /// </summary>
        private void LoadTreeSurveySettings()
        {
            // retrieve settings for the tree dataset specific workflows
            TreeSurveyWorkflows.NeighborhoodNameField = Settings.Default.NeighborhoodNameField;
            TreeSurveyWorkflows.GeocodeUrl = Settings.Default.GeocodeUrl;
            TreeSurveyWorkflows.WebmapURL = Settings.Default.WebmapURL;
            TreeSurveyWorkflows.TreeDatasetWebmapUrl = Settings.Default.TreeDatasetWebmapUrl;
            TreeSurveyWorkflows.OfflineLocatorPath = Settings.Default.OfflineLocatorPath;
            TreeSurveyWorkflows.TreeConditionAttribute = Settings.Default.TreeConditionAttribute;
            TreeSurveyWorkflows.TreeDBHAttribute = Settings.Default.TreeDBHAttribute;
            TreeSurveyWorkflows.InspectionConditionAttribute = Settings.Default.InspectionConditionAttribute;
            TreeSurveyWorkflows.InspectionDBHAttribute = Settings.Default.InspectionDBHAttribute;
            TreeSurveyWorkflows.NeighborhoodOperationalLayerId = Settings.Default.NeighborhoodOperationalLayerId;
            TreeSurveyWorkflows.NeighborhoodAttribute = Settings.Default.NeighborhoodAttribute;
            TreeSurveyWorkflows.AddressAttribute = Settings.Default.AddressAttribute;
        }
    }
}
