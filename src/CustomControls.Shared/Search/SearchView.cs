using System;
using System.Collections.Generic;
using System.Text;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Threading.Tasks;
#if __WPF__
using System.Windows.Controls;
using System.Windows;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public partial class SearchView : Control, INotifyPropertyChanged
    {
        private Map _lastUsedMap;
        public SearchView()
        {
            DefaultStyleKey = typeof(SearchView);
            DataContext = this;
            SearchViewModel = new SearchViewModel();
            NoResultMessage = "No Results";
            DefaultPlaceholder = "Search for a place or address";
            EnableAutoconfiguration = true;
            ClearCommand = new DelegateCommand(() => { SearchViewModel?.ClearSearch();});
            SearchCommand = new DelegateCommand(() => { SearchViewModel?.CommitSearch();});
        }

        public GeoView GeoView
        {
            get { return (GeoView)GetValue(GeoViewProperty); }
            set { SetValue(GeoViewProperty, value); }
        }

        public string NoResultMessage
        {
            get { return (string)GetValue(NoResultMessageProperty); }
            set { SetValue(NoResultMessageProperty, value); }
        }

        public SearchViewModel SearchViewModel
        {
            get { return (SearchViewModel)GetValue(SearchViewModelProperty); }
            set { SetValue(SearchViewModelProperty, value); }
        }

        public bool EnableAutoconfiguration
        {
            get { return (bool)GetValue(EnableAutoconfigurationProperty); }
            set { SetValue(EnableAutoconfigurationProperty, value); }
        }

        public bool EnableResultListView
        {
            get { return (bool)GetValue(EnableResultListViewProperty); }
            set { SetValue(EnableResultListViewProperty, value); }
        }

        public string DefaultPlaceholder
        {
            get { return (string)GetValue(DefaultPlaceholderProperty); }
            set { SetValue(DefaultPlaceholderProperty, value); }
        }

        public ICommand ClearCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public static readonly DependencyProperty NoResultMessageProperty =
            DependencyProperty.Register("NoResultMessage", typeof(string), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultPlaceholderProperty =
            DependencyProperty.Register("DefaultPlaceholder", typeof(string), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty GeoViewProperty =
            DependencyProperty.Register("GeoView", typeof(GeoView), typeof(SearchView), new PropertyMetadata(null, OnGeoViewPropertyChanged));
        public static readonly DependencyProperty EnableAutoconfigurationProperty =
            DependencyProperty.Register("EnableAutoconfiguration", typeof(bool), typeof(SearchView), new PropertyMetadata(true));
        public static readonly DependencyProperty SearchViewModelProperty =
            DependencyProperty.Register("SearchViewModel", typeof(SearchViewModel), typeof(SearchView), new PropertyMetadata(null, OnViewModelChanged));
        public static readonly DependencyProperty EnableResultListViewProperty =
            DependencyProperty.Register("EnableResultListView", typeof(bool), typeof(SearchView), new PropertyMetadata(true, OnEnableResultListViewChanged));

        private static void OnGeoViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchView sender = ((SearchView)d);
            if (sender.EnableAutoconfiguration)
            {
                sender.ConfigureForCurrentMap();
            }
            if (e.OldValue is INotifyPropertyChanged oldNotifier)
            {
                oldNotifier.PropertyChanged -= sender.HandleMapChange;
                sender._lastUsedMap = null;
            }

            if (e.NewValue is INotifyPropertyChanged newNotifier)
            {
                newNotifier.PropertyChanged += sender.HandleMapChange;
            }
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchView sender = ((SearchView)d);

            if (e.OldValue is SearchViewModel svm)
            {
                svm.PropertyChanged -= sender.SearchViewModel_PropertyChanged;
            }
            if (e.NewValue is SearchViewModel newSvm)
            {
                newSvm.PropertyChanged += sender.SearchViewModel_PropertyChanged;
            }
        }

        private static void OnEnableResultListViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchView sender = ((SearchView)d);
            sender.NotifyPropertyChange(nameof(ResultViewVisibility));
        }

        private void HandleMapChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map) || e.PropertyName == nameof(Scene))
            {
                ConfigureForCurrentMap();
            }
            // When binding, MapView is unreliable about notifying about map changes, especially when first connecting to the view
            if (e.PropertyName == nameof(MapView.DrawStatus) && _lastUsedMap == null && GeoView is MapView mv && mv.Map != null)
            {
                // Add workaround for Scenes later
                _lastUsedMap = mv.Map;
                ConfigureForCurrentMap();
            }
        }

        private void ConfigureForCurrentMap()
        {
            if (GeoView is MapView mv && mv.Map is Map map)
            {
                _ = SearchViewModel.ConfigureFromMap(map);
            }
            else if (GeoView is SceneView sv)
            {

            }
        }

        #region WaitingBehavior
        private bool _waitFlag;
        //Separate flag for behavior where query text matches accepted suggestion
        private bool _acceptingSuggestionFlag;

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task Search_TextChanged()
        {
            if (_waitFlag || _acceptingSuggestionFlag) { return; }

            _waitFlag = true;
            // TODO - make configurable
            await Task.Delay(200);
            _waitFlag = false;

            await SearchViewModel.UpdateSuggestions().ConfigureAwait(false);
        }

        private async void SearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchViewModel.CurrentQuery))
                await Search_TextChanged();
            else if (e.PropertyName == nameof(SearchViewModel.SearchMode))
                NotifyPropertyChange(nameof(ResultViewVisibility));
            else if (e.PropertyName == nameof(SearchViewModel.Results))
                NotifyPropertyChange(nameof(ResultViewVisibility));
            else if (e.PropertyName == nameof(SearchViewModel.SelectedResult))
                NotifyPropertyChange(nameof(ResultViewVisibility));
        }

        #endregion WaitingBehavior

        #region BindingWorkaroundBehavior
        // This looks like it has been ommitted from the common design; mainly just working around
        // binding limitations with listview; I'd have preferred to bind a command, but this will have to do
        public SearchSuggestion SelectedSuggestion
        {
            set
            {
                // ListView calls selecteditem binding with null when collection is clear, this avoids overflow
                if(value == null) return;

                SearchSuggestion userSelection = value;
                _acceptingSuggestionFlag = true;
                _ = SearchViewModel.AcceptSuggestion(userSelection)
                    .ContinueWith(tt => _acceptingSuggestionFlag = false, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        // Convenience property to hide or show results view
        public Visibility ResultViewVisibility { 
            get
            {
                if (!EnableResultListView) return Visibility.Collapsed;
                if (SearchViewModel.SearchMode == SearchResultMode.Single) return Visibility.Collapsed;
                if (SearchViewModel.SelectedResult != null) return Visibility.Collapsed;

                if (SearchViewModel.Results != null)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            } 
        }
        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
