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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utils
{
    public sealed class IdentifyController : INotifyPropertyChanged
    {
        private WeakReference<MapView> _mapViewWeakRef;
        private bool _isIdentifyInProgress;
        private bool _wasMapViewDoubleTapped;
        private CancellationTokenSource _canellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Gets or sets the MapView on which to perform identify operations
        /// </summary>
        public MapView MapView
        {
            get
            {
                MapView mapView = null;
                _mapViewWeakRef?.TryGetTarget(out mapView);
                return mapView;
            }
            set
            {
                var mapView = MapView;
                if (mapView != value)
                {
                    var oldMapView = mapView;
                    var newMapView = value;
                    if (oldMapView != null)
                    {
                        oldMapView.GeoViewTapped -= MapView_Tapped;
                        oldMapView.GeoViewDoubleTapped -= MapView_DoubleTapped;
                    }

                    if (_mapViewWeakRef == null)
                    {
                        _mapViewWeakRef = new WeakReference<MapView>(newMapView);
                    }
                    else
                    {
                        _mapViewWeakRef.SetTarget(newMapView);
                    }

                    if (newMapView != null)
                    {
                        newMapView.GeoViewTapped += MapView_Tapped;
                        newMapView.GeoViewDoubleTapped += MapView_DoubleTapped;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when GeoViewDoubleTapped event is firing
        /// </summary>
        private void MapView_DoubleTapped(object sender, GeoViewInputEventArgs e)
        {
            // set flag to true to help distinguish between a double tap and a single tap
            _wasMapViewDoubleTapped = true;
        }

        /// <summary>
        /// Gets or sets the location where user has tapped to identify
        /// </summary>
        private Geometry.Geometry _tappedLocation;

        /// <summary>
        /// Invoked when GeoViewTapped event is firing
        /// </summary>
        private async void MapView_Tapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // Wait for double tap to fire
                // Identify is only peformed on single tap. The delay is used to detect and ignore double taps
                await Task.Delay(300);

                // If view has been double tapped, set tapped to handled and flag back to false
                // If view has been tapped just once, perform identify
                if (_wasMapViewDoubleTapped)
                {
                    e.Handled = true;
                    _wasMapViewDoubleTapped = false;
                    return;
                }

                if (IsIdentifyInProgress)
                {
                    _canellationTokenSource.Cancel();
                    _canellationTokenSource = new CancellationTokenSource();
                }

                IsIdentifyInProgress = true;
                var mapView = (MapView)sender;
                var target = Target;


                if (target is ILoadable loadable && loadable.LoadStatus == LoadStatus.NotLoaded)
                {
                    await loadable.LoadAsync();
                }

                // get the tap location in screen units and geographic coordinates
                var tapScreenPoint = e.Position;
                _tappedLocation = e.Location;

                // set identify parameters
                var pixelTolerance = 10;
                var returnPopupsOnly = false;
                var maxResultCount = 5;

                IReadOnlyList<IdentifyLayerResult> layerResults = null;
                IReadOnlyList<IdentifyGraphicsOverlayResult> graphicsOverlayResults = null;
                CancellationTokenSource _currentSource = _canellationTokenSource;
                if (target == null)
                {
                    // An identify target is not specified, so identify all layers and overlays
                    var identifyLayersTask = mapView.IdentifyLayersAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResultCount, _currentSource.Token);
                    var identifyOverlaysTask = mapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResultCount);
                    await Task.WhenAll(identifyLayersTask, identifyOverlaysTask);
                    if (_currentSource.IsCancellationRequested)
                    {
                        return;
                    }
                    layerResults = identifyLayersTask.Result;
                    graphicsOverlayResults = identifyOverlaysTask.Result;
                }
                else if (target is Layer targetLayer && mapView.Map.OperationalLayers.Contains(target as Layer))
                {
                    // identify features in the target layer, passing the tap point, tolerance, types to return, and max results
                    var identifyResult = await mapView.IdentifyLayerAsync(targetLayer, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResultCount, _currentSource.Token);
                    layerResults = new List<IdentifyLayerResult> { identifyResult }.AsReadOnly();
                }
                else if (target is GraphicsOverlay targetOverlay && mapView.GraphicsOverlays.Contains(target as GraphicsOverlay))
                {
                    // identify features in the target layer, passing the tap point, tolerance, types to return, and max results
                    var identifyResult = await mapView.IdentifyGraphicsOverlayAsync(targetOverlay, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResultCount);
                    if (_currentSource.IsCancellationRequested)
                    {
                        return;
                    }
                    graphicsOverlayResults = new List<IdentifyGraphicsOverlayResult> { identifyResult }.AsReadOnly();
                }
                else if (target is ArcGISSublayer sublayer)
                {
                    var layer = mapView?.Map?.AllLayers?.OfType<ArcGISMapImageLayer>()?.Where(l => l.Sublayers.Contains(sublayer))?.FirstOrDefault();

                    if (layer != null)
                    {

                        // identify features in the target layer, passing the tap point, tolerance, types to return, and max results
                        var topLevelIdentifyResult = await mapView.IdentifyLayerAsync(layer, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResultCount, _currentSource.Token);
                        var sublayerIdentifyResult = topLevelIdentifyResult?.SublayerResults?.Where(r => r.LayerContent.Equals(sublayer)).FirstOrDefault();

                        layerResults = new List<IdentifyLayerResult> { sublayerIdentifyResult }.AsReadOnly();
                    }
                }
                OnIdentifyCompleted(layerResults, graphicsOverlayResults);
            }
            catch (TaskCanceledException)
            {
                // Ignore, not really an error.
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("IdentifyError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);
            } 
            finally
            {
                IsIdentifyInProgress = false;
            }
        }

        /// <summary>
        /// Gets or sets the layer or overlay on which to perform identify operations
        /// </summary>
        public IPopupSource Target { get; set; }

        /// <summary>
        /// Find the GeoElement closest to the specified location
        /// </summary>
        /// <returns>The closest GeoElement to the input point</returns>
        internal GeoElement FindNearestGeoElement(IReadOnlyList<GeoElement> geoElements)
        {
            if (geoElements == null || geoElements.Count == 0)
            {
                return null;
            }
            else if (geoElements.Count == 1)
            {
                return geoElements[0];
            }
            else
            {
                // Sort list of GeoElements by comparing the distance between them and the tapped screen location
                var sortableGeoElements = geoElements.ToList();
                sortableGeoElements.Sort((a, b) => GeometryEngine.Distance(_tappedLocation, a.Geometry).CompareTo(GeometryEngine.Distance(_tappedLocation, b.Geometry)));
                return sortableGeoElements[0];
            }
        }

        public event EventHandler<IdentifyEventArgs> IdentifyCompleted;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnIdentifyCompleted(IReadOnlyList<IdentifyLayerResult> layerResults,
            IReadOnlyList<IdentifyGraphicsOverlayResult> graphicsOverlayResults)
        {
            IdentifyCompleted?.Invoke(this, new IdentifyEventArgs(layerResults, graphicsOverlayResults));
        }

        /// <summary>
        /// Property so that identify work in progress can be shown in the UI.
        /// </summary>
        public bool IsIdentifyInProgress
        {
            get => _isIdentifyInProgress;
            set
            {
                if (_isIdentifyInProgress != value)
                {
                    _isIdentifyInProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIdentifyInProgress)));
                }
            }
        }
    }
}
