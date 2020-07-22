using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        private ICommand _toggleTOCCommand;
        private ICommand _toggleLayersCommand;

        private bool _bookmarksOpen;
        private bool _tocOpen;
        private bool _layersOpen;

        public bool IsLayersOpen
        {
            get => _layersOpen;
            set
            {
                if (_layersOpen != value)
                {
                    _layersOpen = value;
                    OnPropertyChanged();
                }

                if (_layersOpen)
                {
                    IsBookmarksOpen = false;
                    IsTocOpen = false;
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
                    IsLayersOpen = false;
                    IsTocOpen = false;
                }
            }
        }

        public bool IsTocOpen
        {
            get => _tocOpen;
            set
            {
                if (_tocOpen != value)
                {
                    _tocOpen = value;
                    OnPropertyChanged();
                }
                
                if (_tocOpen)
                {
                    IsLayersOpen = false;
                    IsBookmarksOpen = false;
                }
            }
        }

        public ICommand ToggleBookmarksCommand => _toggleBookmarksCommand ?? (_toggleBookmarksCommand = new DelegateCommand((parm) =>
        {
            IsBookmarksOpen = !IsBookmarksOpen;
        }));

        public ICommand ToggleTocCommand => _toggleTOCCommand ?? (_toggleTOCCommand = new DelegateCommand((parm) =>
        {
            IsTocOpen = !IsTocOpen;
        }));

        public ICommand ToggleLayersCommand => _toggleLayersCommand ?? (_toggleLayersCommand = new DelegateCommand((parm) =>
        {
            IsLayersOpen = !IsLayersOpen;
        }));

        public void CloseAccessories()
        {
            IsBookmarksOpen = false;
            IsLayersOpen = false;
            IsTocOpen = false;
        }
    }
}
