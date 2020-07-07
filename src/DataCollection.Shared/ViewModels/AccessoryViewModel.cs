using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    /// <summary>
    /// ViewModel tracks the state of accessories, including bookmarks, legend, TOC, etc.
    /// </summary>
    public class AccessoryViewModel : BaseViewModel
    {
        private bool _isBookmarksOpen = false;
        private bool _isLegendOpen = false;
        private bool _isTocOpen = false;
        private bool _isBasemapsOpen = false;

        public bool IsBookmarksOpen
        {
            get => _isBookmarksOpen;
            set
            {
                if (_isBookmarksOpen != value)
                {
                    _isBookmarksOpen = value;
                    OnPropertyChanged();
                }

                if (_isBookmarksOpen)
                {
                    IsLegendOpen = IsTocOpen = IsBasemapsOpen = false;
                }
            }
        }

        public bool IsLegendOpen
        {
            get => _isLegendOpen;
            set
            {
                if (_isLegendOpen != value)
                {
                    _isLegendOpen = value;
                    OnPropertyChanged();
                }

                if (_isLegendOpen)
                {
                    IsBookmarksOpen = IsBasemapsOpen = IsTocOpen = false;
                }
            }
        }
        public bool IsTocOpen
        {
            get => _isTocOpen;
            set
            {
                if (_isTocOpen != value)
                {
                    _isTocOpen = value;
                    OnPropertyChanged();
                }

                if (_isTocOpen)
                {
                    IsBasemapsOpen = IsBookmarksOpen = IsLegendOpen = false;
                }
            }
        }
        public bool IsBasemapsOpen
        {
            get => _isBasemapsOpen;
            set
            {
                if (_isBasemapsOpen != value)
                {
                    _isBasemapsOpen = value;
                    OnPropertyChanged();
                }

                if (_isBasemapsOpen)
                {
                    IsBookmarksOpen = IsTocOpen = IsLegendOpen = false;
                }
            }
        }

        private ICommand _closeBookmarksCommand;
        private ICommand _closeTocCommand;
        private ICommand _closeBasemapsCommand;
        private ICommand _closeLegendCommand;

        private ICommand _toggleBookmarksCommand;
        private ICommand _toggleTocCommand;
        private ICommand _toggleLegendCommand;
        private ICommand _toggleBasemapsCommand;

        public ICommand CloseBookmarksCommand
        {
            get => _closeBookmarksCommand ?? new DelegateCommand((x) =>
            {
                IsBookmarksOpen = false;
            });
        }

        public ICommand CloseTocCommand
        {
            get => _closeTocCommand ?? new DelegateCommand((x) =>
            {
                IsTocOpen = false;
            });
        }

        public ICommand CloseBasemapsCommand
        {
            get => _closeBasemapsCommand ?? new DelegateCommand((x) =>
            {
                IsBasemapsOpen = false;
            });
        }

        public ICommand CloseLegendCommand
        {
            get => _closeLegendCommand ?? new DelegateCommand((x) =>
            {
                IsLegendOpen = false;
            });
        }

        public ICommand ToggleBookmarksCommand
        {
            get => _toggleBookmarksCommand ?? new DelegateCommand((x) =>
            {
                IsBookmarksOpen = !IsBookmarksOpen;
            });
        }

        public ICommand ToggleTocCommand
        {
            get => _toggleTocCommand ?? new DelegateCommand((x) =>
            {
                IsTocOpen = !IsTocOpen;
            });
        }

        public ICommand ToggleBasemapsCommand
        {
            get => _toggleBasemapsCommand ?? new DelegateCommand((x) =>
            {
                IsBasemapsOpen = !IsBasemapsOpen;
            });
        }

        public ICommand ToggleLegendCommand
        {
            get => _toggleLegendCommand ?? new DelegateCommand((x) =>
            {
                IsLegendOpen = !IsLegendOpen;
            });
        }
    }
}
