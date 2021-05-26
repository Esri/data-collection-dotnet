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
    public partial class SearchView : Control
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
            SearchViewModel.PropertyChanged += SearchViewModel_PropertyChanged;
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

        public string DefaultPlaceholder
        {
            get { return (string)GetValue(DefaultPlaceholderProperty); }
            set { SetValue(DefaultPlaceholderProperty, value); }
        }

        public static readonly DependencyProperty NoResultMessageProperty =
            DependencyProperty.Register("NoResultMessage", typeof(string), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultPlaceholderProperty =
            DependencyProperty.Register("DefaultPlaceholder", typeof(string), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty GeoViewProperty =
            DependencyProperty.Register("GeoView", typeof(GeoView), typeof(SearchView), new PropertyMetadata(null, OnGeoViewPropertyChanged));
        public static readonly DependencyProperty EnableAutoconfigurationProperty =
            DependencyProperty.Register("EnableAutoconfiguration", typeof(bool), typeof(SearchView), new PropertyMetadata(true));
        public static readonly DependencyProperty SearchViewModelProperty =
            DependencyProperty.Register("SearchViewModel", typeof(SearchViewModel), typeof(SearchView), new PropertyMetadata(null));

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
        private async Task Search_TextChanged()
        {
            if (_waitFlag) { return; }

            _waitFlag = true;
            // TODO - make configurable
            await Task.Delay(200);
            _waitFlag = false;

            await SearchViewModel.UpdateSuggestions().ConfigureAwait(false);
        }

        private async void SearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchViewModel.CurrentQuery))
            {
                await Search_TextChanged();
            }
        }

        #endregion WaitingBehavior

        // This looks like it has been ommitted from the common design; mainly just working around
        // binding limitations with listview; I'd have preferred to bind a command, but this will have to do
        #region BindingWorkaroundBehavior
        public SearchSuggestion SelectedSuggestion
        {
            set
            {
                // ListView calls selecteditem binding with null when collection is clear, this avoids overflow
                if(value == null) return;

                SearchSuggestion userSelection = value;
                _ = SearchViewModel.AcceptSuggestion(userSelection);
            }
        }
        #endregion
    }
}
