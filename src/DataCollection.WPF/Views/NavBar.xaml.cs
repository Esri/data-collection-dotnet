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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System.Windows;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for NavBar.xaml
    /// </summary>
    public partial class NavBar
    {
        public NavBar()
        {
            InitializeComponent();
        }

        public ModernMapPanel ParentPanel
        {
            get => (ModernMapPanel)GetValue(ParentPanelProperty);
            set => SetValue(ParentPanelProperty, value);
        }

        // Using a DependencyProperty as the backing store for ParentPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register(nameof(ParentPanel), typeof(ModernMapPanel), typeof(NavBar), new PropertyMetadata(null));
    }
}
