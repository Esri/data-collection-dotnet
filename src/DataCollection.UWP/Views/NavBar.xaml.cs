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
using Windows.UI.Xaml;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    /// <summary>
    /// Meant to be used as a <see cref="MapRole.CardAppendage"/> to visualize the navigation stack provided by <see cref="ModernMapPanel"/>.
    /// </summary>
    public sealed partial class NavBar
    {
        public NavBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reference to the <see cref="ModernMapPanel"/> providing the navigation stack.
        /// </summary>
        public ModernMapPanel ParentPanel
        {
            get => (ModernMapPanel)GetValue(ParentPanelProperty);
            set => SetValue(ParentPanelProperty, value);
        }

        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register(nameof(ParentPanel), typeof(ModernMapPanel), typeof(NavBar), new PropertyMetadata(null));
    }
}
