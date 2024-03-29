﻿/*******************************************************************************
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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views;
using System.ComponentModel;
using System.Windows;
using ControlzEx.Behaviors;
using Microsoft.Xaml.Behaviors;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF
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
            BusyWaitingMessenger.Instance.WaitStatusChanged += OnWaitStatusChanged;

            this.Unloaded += OnUnloaded;

            _mainViewModel = TryFindResource("MainViewModel") as MainViewModel;

            // load settings for the custom tree survey dataset
            LoadTreeSurveySettings();

            // Set up the custom window chrome behavior
            InitializeWindowChromeBehavior();

            // Listen for window state changes, notify the titlebar
            StateChanged += TitleBarControl.MainWindow_StateChanged;
        }

        /// <summary>
        /// Adds the ControlzEx window behavior to this window, enabling proper window resize with the custom window.
        /// </summary>
        private void InitializeWindowChromeBehavior()
        {
            var behavior = new WindowChromeBehavior();
            behavior.EnableMinimize = true;
            behavior.ResizeBorderThickness = new Thickness(6);
            Interaction.GetBehaviors(this).Add(behavior);
        }
        
        /// <summary>
        /// Updates the main view model with changes from the busy waiting messenger.
        /// </summary>
        private void OnWaitStatusChanged(object sender, WaitStatusChangedEventArgs e)
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.IsBusyWaiting = e.IsBusy;
                _mainViewModel.BusyWaitingMessage = e.Message;
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
            UserPromptMessenger.Instance.MessageValueChanged -= DialogBoxMessenger_MessageValueChanged;
            BusyWaitingMessenger.Instance.WaitStatusChanged -= OnWaitStatusChanged;
        }
    }
}
