using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    public class MapAccessoryViewModel : BaseViewModel
    {
        private WeakReference<MapView> _mapViewWeakRef;

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

        private ICommand _toggleBookmarksCommand;
        private ICommand _toggleTableOfContentsCommand;
        private ICommand _toggleLegendCommand;

        private ICommand _goHomeCommand;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;

        private bool _bookmarksOpen;
        private bool _tableOfContentsOpen;
        private bool _legendOpen;

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
                }
            }
        }

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
                }
            }
        }

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
                }
            }
        }

        private bool _isAttributionOpen = false;
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

        public ICommand ToggleBookmarksCommand => _toggleBookmarksCommand ?? (_toggleBookmarksCommand = new DelegateCommand((parm) =>
        {
            IsBookmarksOpen = !IsBookmarksOpen;
        }));

        public ICommand ToggleTocCommand => _toggleTableOfContentsCommand ?? (_toggleTableOfContentsCommand = new DelegateCommand((parm) =>
        {
            IsTableOfContentsOpen = !IsTableOfContentsOpen;
        }));

        public ICommand ToggleLayersCommand => _toggleLegendCommand ?? (_toggleLegendCommand = new DelegateCommand((parm) =>
        {
            IsLegendOpen = !IsLegendOpen;
        }));


        public ICommand GoHomeCommand => _goHomeCommand ?? (_goHomeCommand = new DelegateCommand((parm) =>
        {
            if (MapView?.Map?.InitialViewpoint is Mapping.Viewpoint initialViewpoint)
            {
                MapView.SetViewpoint(initialViewpoint);
            }
        }));

        public ICommand ZoomInCommand => _zoomInCommand ?? (_zoomInCommand = new DelegateCommand((parm) =>
        {
            if (MapView?.GetCurrentViewpoint(Mapping.ViewpointType.CenterAndScale) is Mapping.Viewpoint currentViewpoint)
            {
                MapView.SetViewpointScaleAsync(currentViewpoint.TargetScale * 0.6);
            }
        }));

        public ICommand ZoomOutCommand => _zoomOutCommand ?? (_zoomOutCommand = new DelegateCommand((parm) =>
        {
            if (MapView?.GetCurrentViewpoint(Mapping.ViewpointType.CenterAndScale) is Mapping.Viewpoint currentViewpoint)
            {
                MapView.SetViewpointScaleAsync(currentViewpoint.TargetScale * 1.4);
            }
        }));

        private ICommand _toggleAttributionCommand;

        public ICommand ToggleAttributionCommand => _toggleAttributionCommand ?? (_toggleAttributionCommand = new DelegateCommand(parm =>
        {
            IsAttributionOpen = !IsAttributionOpen;
        }));

        public void CloseAccessories()
        {
            IsBookmarksOpen = false;
            IsLegendOpen = false;
            IsTableOfContentsOpen = false;
        }
    }
}
