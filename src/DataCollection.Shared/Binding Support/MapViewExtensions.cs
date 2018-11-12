/*******************************************************************************
  * Copyright 2018 Esri
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

using Esri.ArcGISRuntime.UI.Controls;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif


namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utils
{
    public class MapViewExtensions : DependencyObject
    {
        /// <summary>
        ///  Identifies the <see cref="ViewpointControllerProperty"/> property
        /// </summary>
        public static readonly DependencyProperty ViewpointControllerProperty =
            DependencyProperty.Register(nameof(ViewpointController), typeof(ViewpointController), typeof(MapView), new PropertyMetadata(null, OnViewpointControllerChanged));

        /// <summary>
        /// Invoked when the ViewpointController's value has changed
        /// </summary>
        private static void OnViewpointControllerChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is ViewpointController)
                ((ViewpointController)args.NewValue).SetMapView(dependency as MapView);
        }

        /// <summary>
        /// ViewpointController getter method
        /// </summary>
        public static ViewpointController GetViewpointControllerController(DependencyObject mapView)
        {
            return (mapView as MapView)?.GetValue(ViewpointControllerProperty) as ViewpointController;
        }

        /// <summary>
        /// ViewpointController setter method
        /// </summary>
        public static void SetViewpointController(DependencyObject mapView, ViewpointController viewpointController)
        {
            (mapView as MapView)?.SetValue(ViewpointControllerProperty, viewpointController);
        }

        /// <summary>
        ///  Identifies the <see cref="LocationDisplayControllerProperty"/> property
        /// </summary>
        public static readonly DependencyProperty LocationDisplayControllerProperty =
                DependencyProperty.Register(nameof(LocationDisplayController), typeof(LocationDisplayController), typeof(MapView), new PropertyMetadata(null, OnLocationDisplayControllerChanged));

        /// <summary>
        /// Invoked when the LocationDisplayController's value has changed
        /// </summary>
        private static void OnLocationDisplayControllerChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is LocationDisplayController)
                ((LocationDisplayController)args.NewValue).SetMapView(dependency as MapView);
        }

        /// <summary>
        /// LocationDisplayController getter method
        /// </summary>
        public static LocationDisplayController GetLocationDisplayController(DependencyObject mapView)
        {
            return (mapView as MapView)?.GetValue(LocationDisplayControllerProperty) as LocationDisplayController;
        }

        /// <summary>
        /// LocationDisplayController setter method
        /// </summary>
        public static void SetLocationDisplayController(DependencyObject mapView, LocationDisplayController locationDisplayController)
        {
            (mapView as MapView)?.SetValue(LocationDisplayControllerProperty, locationDisplayController);
        }

        /// <summary>
        /// Identifies the <see cref="IdentifyControllerProperty"/> property
        /// </summary>
        public static readonly DependencyProperty IdentifyControllerProperty = DependencyProperty.RegisterAttached(
            nameof(IdentifyController),
            typeof(IdentifyController),
            typeof(MapViewExtensions),
            new PropertyMetadata(null, OnIdentifyControllerChanged));

        /// <summary>
        /// Invoked when the IdentifyController's value has changed
        /// </summary>
        private static void OnIdentifyControllerChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is IdentifyController)
                ((IdentifyController)args.NewValue).MapView = dependency as MapView;
        }

        /// <summary>
        /// IdentifyController getter method
        /// </summary>
        public static IdentifyController GetIdentifyController(DependencyObject mapView)
        {
            return (mapView as MapView)?.GetValue(IdentifyControllerProperty) as IdentifyController;
        }

        /// <summary>
        /// IdentifyController setter method
        /// </summary>
        public static void SetIdentifyController(DependencyObject mapView, IdentifyController identifyController)
        {
            (mapView as MapView)?.SetValue(IdentifyControllerProperty, identifyController);
        }
    }
}
