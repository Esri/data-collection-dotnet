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

using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using System;
#if NETFX_CORE
using Windows.UI.Xaml;
#elif DOT_NET_CORE_TEST
using System.Windows;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Tests.Mocks;
#else
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utils
{
    /// <summary>
    /// Provides members for creating a Viewpoint Controller
    /// </summary>
    public class ViewpointController : DependencyObject
    {
        private bool _isMapViewViewpointChangedEventFiring = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewpointController"/> class.
        /// </summary>
        public ViewpointController() { }

        /// <summary>
        /// MapView setter
        /// </summary>
        internal void SetMapView(MapView mapView)
        {
            MapView = mapView;
        }

        private WeakReference<MapView> _mapViewWeakRef;

        /// <summary>
        /// Gets or sets the MapView
        /// </summary>
        private MapView MapView
        {
            get
            {
                MapView mapView = null;
                _mapViewWeakRef?.TryGetTarget(out mapView);
                return mapView;
            }
            set
            {
                if (MapView != null)
                {
                    MapView.ViewpointChanged -= MapView_ViewpointChanged;
                }

                if (_mapViewWeakRef == null)
                {
                    _mapViewWeakRef = new WeakReference<MapView>(value);
                }
                else
                {
                    _mapViewWeakRef.SetTarget(value);
                }

                if (value != null)
                {
                    value.ViewpointChanged += MapView_ViewpointChanged;
                }
            }
        }

        /// <summary>
        /// Invoked when the MapView's Viewpoint value has changed
        /// </summary>
        private void MapView_ViewpointChanged(object sender, EventArgs e)
        {
            _isMapViewViewpointChangedEventFiring = true;
            try
            {
                Viewpoint = (sender as MapView)?.GetCurrentViewpoint(ViewpointType.CenterAndScale);
            }

            // if unable to get the viewpoint, don't do anything
            catch { }

            _isMapViewViewpointChangedEventFiring = false;
        }

        /// <summary>
        /// Creates a ViewpointController property
        /// </summary>
        public static readonly DependencyProperty ViewpointProperty = DependencyProperty.Register(
            nameof(Viewpoint), typeof(Viewpoint), typeof(ViewpointController), new PropertyMetadata(null, OnViewpointChanged));

        /// <summary>
        /// Invoked when the Viewpoint value has changed
        /// </summary>
        private async static void OnViewpointChanged(DependencyObject bindable, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Viewpoint && !(bindable as ViewpointController)._isMapViewViewpointChangedEventFiring)
            {
                await (bindable as ViewpointController)?.MapView?.SetViewpointAsync((Viewpoint)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the Viewpoint property
        /// </summary>
        public Viewpoint Viewpoint
        {
            get { return MapView?.GetCurrentViewpoint(ViewpointType.CenterAndScale); }
            set { SetValue(ViewpointProperty, value); }
        }
    }
}
