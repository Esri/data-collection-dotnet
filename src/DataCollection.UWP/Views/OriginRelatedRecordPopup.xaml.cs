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
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Views
{
    /// <summary>
    /// Interaction logic for OriginRelatedRecordPopup.xaml
    /// </summary>
    public sealed partial class OriginRelatedRecordPopup : UserControl, INotifyPropertyChanged
    {
        public OriginRelatedRecordPopup()
        {
            InitializeComponent();

            // set the OriginRelationshipViewModel property when DataContext changes and OriginRelationshipViewModel is set
            DataContextChanged += (s, e) =>
            {
                if (DataContext is IdentifiedFeatureViewModel identifiedFeatureViewModel)
                {
                    identifiedFeatureViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName == "SelectedOriginRelationship")
                            OriginRelationshipViewModel = identifiedFeatureViewModel.SelectedOriginRelationship;
                    };
                }
            };
        }
        private OriginRelationshipViewModel _originRelationshipViewModel;

        /// <summary>
        /// Gets or sets the OriginRelationshipViewModel
        /// </summary>
        public OriginRelationshipViewModel OriginRelationshipViewModel
        {
            get => _originRelationshipViewModel;
            set
            {
                _originRelationshipViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        private async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
