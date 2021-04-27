/*******************************************************************************
  * Copyright 2020 Esri
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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System.Windows;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Control for visualizing the navigation stack provided by <see cref="ModernMapPanel"/>.
    /// </summary>
    public partial class NavBar
    {
        public NavBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// A reference to the parent panel is needed for binding to the <see cref="ModernMapPanel.NavigationTitles" /> property which is shown in the nav bar..
        /// </summary>
        public ModernMapPanel ParentPanel { get => (ModernMapPanel)GetValue(ParentPanelProperty); set => SetValue(ParentPanelProperty, value); }

        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register(nameof(ParentPanel), typeof(ModernMapPanel), typeof(NavBar), new PropertyMetadata(null));
    }
}
