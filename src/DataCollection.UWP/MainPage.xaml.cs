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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP
{
    /// <summary>
    /// A map page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            // create event handler for when receiving a message to display to the user
            UserPromptMessenger.Instance.MessageValueChanged += UserPrompt_MessageValueChanged;
            BusyWaitingMessenger.Instance.WaitStatusChanged += OnWaitStatusChanged;

            this.Unloaded += OnUnloaded;

            // load settings for the custom tree survey dataset
            LoadTreeSurveySettings();
        }

        private void OnWaitStatusChanged(object sender, WaitStatusChangedEventArgs e)
        {
            MainViewModel.BusyWaitingMessage = e.Message;
            MainViewModel.IsBusyWaiting = e.IsBusy;
        }

        /// <summary>
        /// Gets the view-model that provides mapping capabilities to the view
        /// </summary>
        public MainViewModel MainViewModel { get; } = new MainViewModel();

        public AuthViewModel AuthViewModel { get; } = new AuthViewModel(
                                                        Settings.Default.WebmapURL,
                                                        Settings.Default.ArcGISOnlineURL,
                                                        Settings.Default.AppClientID,
                                                        Settings.Default.RedirectURL,
                                                        Settings.Default.AuthenticatedUserName,
                                                        Settings.Default.OAuthRefreshToken);

        /// <summary>
        /// Event handler for displaying a message to the user
        /// </summary>
        private async void UserPrompt_MessageValueChanged(object sender, UserPromptMessageChangedEventArgs e)
        {
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    var contentDialog = new ContentDialog()
                    {
                        Title = e.MessageTitle,
                        Content = e.Message,
                    };

                    if (e.IsError)
                    {
                        contentDialog.PrimaryButtonText = Shared.Properties.Resources.GetString("GenericAffirmativeButton_Content");
                    }
                    else
                    {
                        contentDialog.PrimaryButtonText = string.IsNullOrEmpty(e.AffirmativeActionButtonContent) ?
                            Shared.Properties.Resources.GetString("GenericAffirmativeButton_Content") :
                            e.AffirmativeActionButtonContent;

                        contentDialog.SecondaryButtonText = string.IsNullOrEmpty(e.NegativeActionButtonContent) ?
                            Shared.Properties.Resources.GetString("GenericNegativeButton_Content") :
                            e.NegativeActionButtonContent;

                        contentDialog.DefaultButton = ContentDialogButton.Primary;
                    }

                    contentDialog.Closed += (s, ea) =>
                    {
                        if (ea.Result == ContentDialogResult.Primary)
                        {
                            UserPromptMessenger.Instance.RaiseResponseValueChanged(true);
                        }
                        else
                        {
                            UserPromptMessenger.Instance.RaiseResponseValueChanged(false);
                        }
                    };

                    try
                    {
                        await contentDialog.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                Environment.FailFast("Couldn't display error message.", ex);
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

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from events.
            this.Unloaded -= OnUnloaded;
            UserPromptMessenger.Instance.MessageValueChanged -= UserPrompt_MessageValueChanged;
            BusyWaitingMessenger.Instance.WaitStatusChanged -= OnWaitStatusChanged;
        }
    }
}