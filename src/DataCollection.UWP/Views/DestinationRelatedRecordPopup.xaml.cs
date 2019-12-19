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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Views
{
    /// <summary>
    /// Interaction logic for DestinationRelatedRecordPopup.xaml
    /// </summary>
    public sealed partial class DestinationRelatedRecordPopup : UserControl, INotifyPropertyChanged
    {
        public DestinationRelatedRecordPopup()
        {
            InitializeComponent();

            // set the DestinationRelationshipViewModel property when DataContext changes and DestinationRelationshipViewModel is set
            DataContextChanged += (s, e) =>
            {
                if (DataContext is IdentifiedFeatureViewModel identifiedFeatureViewModel)
                {
                    identifiedFeatureViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName == "SelectedDestinationRelationship")
                            DestinationRelationshipViewModel = identifiedFeatureViewModel.SelectedDestinationRelationship;
                    };
                }
            };
        }

        private DestinationRelationshipViewModel _destinationRelationshipViewModel;

        /// <summary>
        /// Gets or sets the DestinationRelationshipViewModel
        /// </summary>
        public DestinationRelationshipViewModel DestinationRelationshipViewModel
        {
            get => _destinationRelationshipViewModel;
            set
            {
                _destinationRelationshipViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        private async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Shared.Properties.Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);

                // This exception should never happen; if it does, the app is in an unknown state and should terminate.
                Environment.FailFast("Error invoking property change event handler.", ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
