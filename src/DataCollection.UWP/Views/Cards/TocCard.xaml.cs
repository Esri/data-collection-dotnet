﻿/*******************************************************************************
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
using Windows.UI.Xaml;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Cards
{
    /// <summary>
    /// Card that shows a map's table of contents.
    /// </summary>
    public sealed partial class TocCard
    {
        public TocCard()
        {
            InitializeComponent();
        }

        public MainViewModel MainViewModel { get => (MainViewModel)GetValue(MainViewModelProperty); set => SetValue(MainViewModelProperty, value); }

        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register(nameof(MainViewModel), typeof(MainViewModel), typeof(TocCard),
                new PropertyMetadata(null));
    }
}
