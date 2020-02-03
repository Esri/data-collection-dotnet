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

using Esri.ArcGISRuntime.Geometry;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    public sealed partial class DownloadView : UserControl
    {
        public DownloadView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Identifies the <see cref="MapViewProperty"/> property
        /// </summary>
        public static readonly DependencyProperty VisibleAreaProperty = DependencyProperty.Register(
            nameof(VisibleArea),
            typeof(Polygon),
            typeof(DownloadView),
            new PropertyMetadata(null, null));

        public Polygon VisibleArea
        {
            get { return (Polygon)GetValue(VisibleAreaProperty); }
            set { SetValue(VisibleAreaProperty, value); }
        }
    }
}
