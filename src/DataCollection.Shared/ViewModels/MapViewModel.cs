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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using System.Linq;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    /// <summary>
    /// Provides map data to the application
    /// </summary>
    public class MapViewModel : BaseViewModel
    {
        private Location.Location _lastLocation;
        private int _defaultZoomScale = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        public MapViewModel(Map map, ConnectivityMode connectivityMode, int defaultZoomScale)
        {
            ConnectivityMode = connectivityMode;
            _defaultZoomScale = defaultZoomScale;

            // Initialize the map
            Map = map;

            // Initialize the location data source for device location
            LocationDataSource = new SystemLocationDataSource();
            LocationDataSource.LocationChanged += (s, l) =>
            {
                _lastLocation = l;
                IsLocationStarted = true;
            };

            // Load the map
            Map.LoadStatusChanged += Map_LoadStatusChanged;
            Map.LoadAsync();
        }

        public ConnectivityMode ConnectivityMode { get; }

        private Map _map;

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        private LocationDataSource _locationDataSource;

        /// <summary>
        /// Gets or sets the current location data source
        /// </summary>
        public LocationDataSource LocationDataSource
        {
            get { return _locationDataSource; }
            set
            {
                if (_locationDataSource != value)
                {
                    _locationDataSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLocationStarted;

        /// <summary>
        /// Gets or sets a flag showing whether the location services are started
        /// This should be replaced with LocationDataSource.IsStarted when it becomes properly exposed in the API
        /// </summary>
        public bool IsLocationStarted
        {
            get => _isLocationStarted;
            set
            {
                if (_isLocationStarted != value)
                {
                    _isLocationStarted = value;
                    OnPropertyChanged();
                }
            }
        }


        private Viewpoint _areaOfInterest;

        /// <summary>
        /// Gets or sets the current area of interest
        /// </summary>
        public Viewpoint AreaOfInterest
        {
            get { return _areaOfInterest; }
            set
            {
                if (_areaOfInterest != value)
                {
                    _areaOfInterest = value;
                    // PropertyChange omitted because of issue when two-way x:Bind bound on UWP
                }
            }
        }

        private ICommand _moveToCurrentLocationCommand;

        /// <summary>
        /// Gets the command to move to user's current location
        /// </summary>
        public ICommand MoveToCurrentLocationCommand
        {
            get
            {
                return _moveToCurrentLocationCommand ?? (_moveToCurrentLocationCommand = new DelegateCommand(
                    (x) =>
                    {
                        // Set viewpoint to the user's current location
                        if (_lastLocation != null)
                        {
                            AreaOfInterest = new Viewpoint(_lastLocation?.Position, _defaultZoomScale);
                            OnPropertyChanged(nameof(AreaOfInterest));
                        }
                    }));
            }
        }

        /// <summary>
        /// Perform feature selection, clearing all other selections if <paramref name="clearAll"/> is true.
        /// </summary>
        internal void SelectFeature(Feature feature, bool clearAll = true)
        {
            if (feature?.FeatureTable?.Layer is FeatureLayer featureLayer)
            {
                if (clearAll)
                {
                    // Clear all selected features in all map feature layers
                    foreach (var layer in Map.OperationalLayers.OfType<FeatureLayer>())
                    {
                        try
                        {
                            layer.ClearSelection();
                        }
                        catch { }
                    }
                }

                // Select feature
                featureLayer.SelectFeature(feature);
            }
        }

        /// <summary>
        /// Command attached to the Clear button to clear map pins and selections
        /// </summary>
        internal void ClearSelection()
        {
            foreach (var featureLayer in Map.OperationalLayers.OfType<FeatureLayer>())
            {
                featureLayer.ClearSelection();
            }
        }

        /// <summary>
        /// Invoked when the load status of the Map changes
        /// Need to wait until the map is loaded in order to set the target layer on the IdentifyController
        /// </summary>
        private async void Map_LoadStatusChanged(object sender, LoadStatusEventArgs e)
        {
            // if map fails to load, let user know 
            if (e.Status == LoadStatus.FailedToLoad)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("MapLoadingError_Title"), 
                    ((Map)sender).LoadError.Message, 
                    true, 
                    ((Map)sender).LoadError.StackTrace);
            }
            else if (e.Status == LoadStatus.Loaded)
            {
                // try starting location services
                try
                {
                    await LocationDataSource.StartAsync();
                }
                catch
                { // if location services cannot be started, do not do anything, button will just be disabled
                }
            }
        }
    }
}
