/*******************************************************************************
  * Copyright 2020 Esri
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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System.Windows;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views.Cards
{
    /// <summary>
    /// Card for showing a list of identify feature results for the user to choose from.
    /// </summary>
    public partial class IdentifyResultCard
    {
        public IdentifyResultCard()
        {
            InitializeComponent();
        }

        public IdentifyResultViewModel ViewModel
        {
            get { return (IdentifyResultViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(IdentifyResultViewModel), typeof(IdentifyResultCard), new PropertyMetadata(null));
    }
}
