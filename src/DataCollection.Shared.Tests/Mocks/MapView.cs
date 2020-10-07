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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Tests.Mocks
{
    public class MapView : DependencyObject
    {
        public event EventHandler<GeoViewInputEventArgs> GeoViewTapped;
        public event EventHandler<GeoViewInputEventArgs> GeoViewDoubleTapped;
        public Viewpoint GetCurrentViewpoint(ViewpointType type) => null;
        public async Task<IdentifyGraphicsOverlayResult> IdentifyGraphicsOverlaysAsync(System.Windows.Point position, double pixelTolerance, bool returnPopupsOnly, int maxResultCount) => null;
        public async Task<IdentifyLayerResult> IdentifyLayerAsync(Layer layer, System.Windows.Point position, double pixelTolerance, bool returnPopupsOnly, int maxResultCount, CancellationToken token) => null;
        public async Task<IReadOnlyList<IdentifyLayerResult>> IdentifyLayersAsync(System.Windows.Point position, double pixelTolerance, bool returnPopupsOnly, int maxResultCount, CancellationToken token) => null;
        public event EventHandler<EventArgs> ViewpointChanged;
        public async Task SetViewpointScaleAsync(double scale) { }
        public async Task SetViewpointAsync(Viewpoint viewpoint) { }
        public void SetViewpoint(Viewpoint viewpoint) { }
        public Map Map { get; set;}
        public LocationDisplay LocationDisplay { get; set; }
    }
}
