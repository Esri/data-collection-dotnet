/*******************************************************************************
  * Copyright 2019 Esri
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

using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.UI.Controls;
using System;
#if NETFX_CORE
using Windows.UI.Xaml;
#elif DOT_NET_CORE_TEST
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Tests.Mocks;
using System.Windows;
#else
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utils
{
    public class LocationDisplayController : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserLocationController"/> class.
        /// </summary>
        public LocationDisplayController() { }

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
                if (_mapViewWeakRef == null)
                    _mapViewWeakRef = new WeakReference<MapView>(value);
                else
                    _mapViewWeakRef.SetTarget(value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DataSourceProperty"/> property
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(nameof(DataSource), typeof(LocationDataSource), typeof(LocationDisplayController), 
            new PropertyMetadata(null, OnLocationDataSourceChanged));

        /// <summary>
        /// Invoked when the DataSourceProperty changes
        /// </summary>
        private static void OnLocationDataSourceChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is LocationDataSource && (dependency as LocationDisplayController).MapView != null)
            {
                var locationDisplay = (dependency as LocationDisplayController).MapView.LocationDisplay;
                locationDisplay.DataSource = (LocationDataSource)args.NewValue;
            }
        }

        /// <summary>
        /// Gets or sets the DataSource property
        /// </summary>
        public LocationDataSource DataSource
        {
            get { return MapView?.LocationDisplay.DataSource; }
            set { SetValue(DataSourceProperty, value); }
        }
    }
}
