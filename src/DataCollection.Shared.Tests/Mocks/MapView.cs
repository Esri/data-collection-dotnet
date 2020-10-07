using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Text;
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
