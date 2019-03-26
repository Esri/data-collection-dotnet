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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP
{
    /// <summary>
    /// A map page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
	    {
            InitializeComponent();
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
                                                        Settings.Default.OAuthRefreshToken);

        /// <summary>
        /// Event handler clears the respective viewmodels when the user closes the blade
        /// </summary>
        private void BladeView_BladeClosed(object sender, BladeItem e)
        {
            if (e.Name == "IdentifiedFeatureBlade")
                MainViewModel.IdentifiedFeatureViewModel = null;
            else if (e.Name == "OriginRelationshipBlade")
                MainViewModel.IdentifiedFeatureViewModel.SelectedOriginRelationship = null;
            else if (e.Name == "DestinationRelationshipBlade")
                MainViewModel.IdentifiedFeatureViewModel.SelectedDestinationRelationship = null;
        }

        /// <summary>
        /// Event handler removes the border when the user collapses the blade
        /// </summary>
        private void BladeItem_Collapsed(object sender, System.EventArgs e)
        {
            var bladeItem = sender as BladeItem;
            bladeItem.BorderThickness = new Windows.UI.Xaml.Thickness(0);
        }

        /// <summary>
        /// Event handler restores the border when the user expands the blade
        /// </summary>
        private void BladeItem_Expanded(object sender, System.EventArgs e)
        {
            var bladeItem = sender as BladeItem;
            bladeItem.BorderThickness = new Windows.UI.Xaml.Thickness(1);
        }
    }
}