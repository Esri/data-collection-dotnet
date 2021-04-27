/*******************************************************************************
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

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Cards
{
    /// <summary>
    /// Interaction logic for AttachmentsView.xaml
    /// </summary>
    public sealed partial class AttachmentsView : UserControl, INotifyPropertyChanged
    {
        public AttachmentsView()
        {
            InitializeComponent();

            // set the AttachmentsViewModel property when DataContext changes and AttachmentsViewModel is set
            DataContextChanged += (s, e) =>
            {
                if (DataContext is IdentifiedFeatureViewModel identifiedFeatureViewModel)
                {
                    identifiedFeatureViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName == "AttachmentsViewModel")
                            AttachmentsViewModel = identifiedFeatureViewModel.AttachmentsViewModel;
                    };
                }
                else if (DataContext is OriginRelationshipViewModel originRelationshipViewModel)
                {
                    originRelationshipViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName == "AttachmentsViewModel")
                            AttachmentsViewModel = originRelationshipViewModel.AttachmentsViewModel;
                    };
                }
            };
        }
        private AttachmentsViewModel _attachmentsViewModel;

        /// <summary>
        /// Gets or sets the AttachmentsViewModel
        /// </summary>
        public AttachmentsViewModel AttachmentsViewModel
        {
            get => _attachmentsViewModel;
            set
            {
                _attachmentsViewModel = value;
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
