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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    /// <summary>
    /// View model used to manage the state of map accessories, like the bookmarks view, legend, and TOC.
    /// </summary>
    public class MapAccessoryViewModel : BaseViewModel
    {
        private WeakReference<MapView> _mapViewWeakRef;

        private ICommand _toggleBookmarksCommand;
        private ICommand _toggleTableOfContentsCommand;
        private ICommand _toggleLegendCommand;

        private ICommand _goHomeCommand;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;
        private ICommand _toggleAttributionCommand;

        private ICommand _toggleOnlinePanelCommand;
        private ICommand _toggleOfflinePanelCommand;
        private ICommand _toggleUserCommand;

        private bool _bookmarksOpen;
        private bool _tableOfContentsOpen;
        private bool _legendOpen;
        private bool _isAttributionOpen = false;

        private bool _isOfflineMapStatusPanelOpen;
        private bool _isUserPanelOpen;
        private bool _isOnlineMapStatusPanelOpen;

        private MainViewModel _mainViewModel;

        public MapAccessoryViewModel(MainViewModel mainVM)
        {
            // Hold a reference to the main view model to enable waiting on it before showing accessories.
            _mainViewModel = mainVM;

            // Close all accessories when prompted
            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += HandleBroadcastMessage;
        }

        private void HandleBroadcastMessage(object sender, BroadcastMessengerEventArgs e)
        {
            if (e.Args.Key == Models.BroadcastMessageKey.ClosePopups)
            {
                CloseAccessories();
            }
        }

        /// <summary>
        /// Hold a reference to the map view.
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
                    }
                }
            }
        }

        /// <summary>
        /// True if the legend should be open and visible.
        /// </summary>
        public bool IsLegendOpen
        {
            get => _legendOpen;
            set
            {
                if (_legendOpen != value)
                {
                    _legendOpen = value;
                    OnPropertyChanged();
                }

                if (_legendOpen)
                {
                    IsBookmarksOpen = false;
                    IsTableOfContentsOpen = false;
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, Models.BroadcastMessageKey.AccessoryOpened);
                }
            }
        }

        /// <summary>
        /// True if bookmarks should be open and visible.
        /// </summary>
        public bool IsBookmarksOpen
        {
            get => _bookmarksOpen;
            set
            {
                if (_bookmarksOpen != value)
                {
                    _bookmarksOpen = value;
                    OnPropertyChanged();
                }

                if (_bookmarksOpen)
                {
                    IsLegendOpen = false;
                    IsTableOfContentsOpen = false;
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, Models.BroadcastMessageKey.AccessoryOpened);
                }
            }
        }

        /// <summary>
        /// True if TOC should be open and visible.
        /// </summary>
        public bool IsTableOfContentsOpen
        {
            get => _tableOfContentsOpen;
            set
            {
                if (_tableOfContentsOpen != value)
                {
                    _tableOfContentsOpen = value;
                    OnPropertyChanged();
                }
                
                if (_tableOfContentsOpen)
                {
                    IsLegendOpen = false;
                    IsBookmarksOpen = false;
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, Models.BroadcastMessageKey.AccessoryOpened);
                }
            }
        }

        /// <summary>
        /// True if extended attribution view should be open and visible.
        /// </summary>
        public bool IsAttributionOpen
        {
            get => _isAttributionOpen;
            set
            {
                if (_isAttributionOpen != value)
                {
                    _isAttributionOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Toggles the display of bookmarks.
        /// </summary>
        public ICommand ToggleBookmarksCommand => _toggleBookmarksCommand ?? (_toggleBookmarksCommand = new DelegateCommand(async (parm) =>
        {
            if (await _mainViewModel.AttemptCloseEditorsAsync())
            {
                IsBookmarksOpen = !IsBookmarksOpen;
            }
        }));

        /// <summary>
        /// Toggles the display of the table of contents.
        /// </summary>
        public ICommand ToggleTableOfContentsCommand => _toggleTableOfContentsCommand ?? (_toggleTableOfContentsCommand = new DelegateCommand(async (parm) =>
        {
            if (await _mainViewModel.AttemptCloseEditorsAsync())
            {
                IsTableOfContentsOpen = !IsTableOfContentsOpen;
            }
        }));

        /// <summary>
        /// Toggles the display of the legend.
        /// </summary>
        public ICommand ToggleLegendCommand => _toggleLegendCommand ?? (_toggleLegendCommand = new DelegateCommand(async (parm) =>
        {
            if (await _mainViewModel.AttemptCloseEditorsAsync())
            {
                IsLegendOpen = !IsLegendOpen;
            }
        }));

        /// <summary>
        /// Go to the map's defined default extent.
        /// </summary>
        public ICommand GoHomeCommand => _goHomeCommand ?? (_goHomeCommand = new DelegateCommand((parm) =>
        {
            if (MapView?.Map?.InitialViewpoint is Mapping.Viewpoint initialViewpoint)
            {
                MapView.SetViewpoint(initialViewpoint);
            }
        }));

        /// <summary>
        /// Zoom into the map.
        /// </summary>
        public ICommand ZoomInCommand => _zoomInCommand ?? (_zoomInCommand = new DelegateCommand((parm) =>
        {
            if (MapView?.GetCurrentViewpoint(Mapping.ViewpointType.CenterAndScale) is Mapping.Viewpoint currentViewpoint)
            {
                MapView.SetViewpointScaleAsync(currentViewpoint.TargetScale * 0.6);
            }
        }));

        /// <summary>
        /// Zoom out of the map.
        /// </summary>
        public ICommand ZoomOutCommand => _zoomOutCommand ?? (_zoomOutCommand = new DelegateCommand((parm) =>
        {
            if (MapView?.GetCurrentViewpoint(Mapping.ViewpointType.CenterAndScale) is Mapping.Viewpoint currentViewpoint)
            {
                MapView.SetViewpointScaleAsync(currentViewpoint.TargetScale * 1.4);
            }
        }));

        /// <summary>
        /// Hide/show the extended attribution view.
        /// </summary>
        public ICommand ToggleAttributionCommand => _toggleAttributionCommand ?? (_toggleAttributionCommand = new DelegateCommand(parm =>
        {
            IsAttributionOpen = !IsAttributionOpen;
        }));

        /// <summary>
        /// Toggles the display of the panel showing the user's profile.
        /// </summary>
        public ICommand ToggleUserPanelCommand => _toggleUserCommand ?? (_toggleUserCommand = new DelegateCommand((parm) =>
        {
            IsUserPanelOpen = !IsUserPanelOpen;
        }));

        /// <summary>
        /// Toggles the display of the panel showing the details of the offline map.
        /// </summary>
        public ICommand ToggleOfflinePanelCommand => _toggleOfflinePanelCommand ?? (_toggleOfflinePanelCommand = new DelegateCommand((parm) =>
        {
            IsOfflinePanelOpen = !IsOfflinePanelOpen;
        }));

        /// <summary>
        /// Toggles the display of the panel showing the details of the online map.
        /// </summary>
        public ICommand ToggleOnlinePanelCommand => _toggleOnlinePanelCommand ?? (_toggleOnlinePanelCommand = new DelegateCommand((parm) =>
        {
            IsMapStatusPanelOpen = !IsMapStatusPanelOpen;
        }));

        /// <summary>
        /// True if the panel showing details about the active offline map should be open.
        /// </summary>
        public bool IsOfflinePanelOpen
        {
            get => _isOfflineMapStatusPanelOpen;
            set
            {
                if (_isOfflineMapStatusPanelOpen != value)
                {
                    _isOfflineMapStatusPanelOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// True if the panel showing details about the active online map should be open.
        /// </summary>
        public bool IsMapStatusPanelOpen
        {
            get => _isOnlineMapStatusPanelOpen;
            set
            {
                if (_isOnlineMapStatusPanelOpen != value)
                {
                    _isOnlineMapStatusPanelOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// True if the panel showing the user's profile should be open.
        /// </summary>
        public bool IsUserPanelOpen
        {
            get => _isUserPanelOpen;
            set
            {
                if (_isUserPanelOpen != value)
                {
                    _isUserPanelOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Closes all open accessory panels/cards.
        /// </summary>
        public void CloseAccessories()
        {
            IsUserPanelOpen = false;
            IsOfflinePanelOpen = false;
            IsMapStatusPanelOpen = false;

            IsBookmarksOpen = false;
            IsLegendOpen = false;
            IsTableOfContentsOpen = false;
            IsAttributionOpen = false;
        }
    }
}
