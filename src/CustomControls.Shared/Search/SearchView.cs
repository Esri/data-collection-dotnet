﻿using System;
using System.Collections.Generic;
using System.Text;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Esri.ArcGISRuntime.UI;
using System.Linq;
using Esri.ArcGISRuntime.Geometry;
#if __WPF__
using System.Windows.Controls;
using System.Windows;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public partial class SearchView : Control, INotifyPropertyChanged
    {
        private Map _lastUsedMap;
        private GraphicsOverlay _resultOverlay;
        private bool _hasViewpointChangedPostSearch;
        public SearchView()
        {
            DefaultStyleKey = typeof(SearchView);
            DataContext = this;
            SearchViewModel = new SearchViewModel();
            NoResultMessage = "No Results";
            DefaultPlaceholder = "Search for a place or address";
            EnableAutoconfiguration = true;
            _resultOverlay = new GraphicsOverlay();
            ClearCommand = new DelegateCommand(() => { SearchViewModel?.ClearSearch();});
            SearchCommand = new DelegateCommand(() => { 
                UpdateModelForNewViewpoint();
                SearchViewModel?.CommitSearch();
            });
            RepeatSearchHereCommand = new DelegateCommand(() =>
            {
                UpdateModelForNewViewpoint();
                SearchViewModel?.CommitSearch(true);
            });
        }

        private bool _ignoreViewpointChangedFlag = false;

        private void GeoView_ViewpointChanged(object sender, EventArgs e)
        {
            // Don't push updates from automatic navigation
            // TODO = intent is to prevent IsEligibleForReSearch from becoming true after selecting result
            //        find a better way; maybe pass a flag to viewmodel indicating whether navigation was automatic or manual
            if (_ignoreViewpointChangedFlag)
                return;
            UpdateModelForNewViewpoint();
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

        public double MultipleResultZoomBuffer
        {
            get { return (double)GetValue(MultipleResultZoomBufferProperty); }
            set { SetValue(MultipleResultZoomBufferProperty, value); }
        }

        public ICommand ClearCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand RepeatSearchHereCommand { get; private set; }
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
        public static readonly DependencyProperty MultipleResultZoomBufferProperty =
            DependencyProperty.Register("MultipleResultZoomBuffer", typeof(double), typeof(SearchView), new PropertyMetadata(64.0));

        private static void OnGeoViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchView sender = ((SearchView)d);
            if (sender.EnableAutoconfiguration)
            {
                sender.ConfigureForCurrentMap();
            }

            if (e.OldValue is GeoView gv)
            {
                gv.ViewpointChanged -= sender.GeoView_ViewpointChanged;
                sender._lastUsedMap = null;
                (gv as INotifyPropertyChanged).PropertyChanged -= sender.HandleMapChange;
                if (gv.GraphicsOverlays.Contains(sender._resultOverlay))
                {
                    gv.GraphicsOverlays.Remove(sender._resultOverlay);
                }
            }
            if (e.NewValue is GeoView ngv)
            {
                (ngv as INotifyPropertyChanged).PropertyChanged += sender.HandleMapChange;
                ngv.ViewpointChanged += sender.GeoView_ViewpointChanged;
                ngv.GraphicsOverlays.Add(sender._resultOverlay);
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
            await Task.Delay(75);
            _waitFlag = false;

            UpdateModelForNewViewpoint();
            await SearchViewModel.UpdateSuggestions();
        }

        private async void SearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchViewModel.CurrentQuery))
                await Search_TextChanged();
            else if (e.PropertyName == nameof(SearchViewModel.SearchMode))
                NotifyPropertyChange(nameof(ResultViewVisibility));
            else if (e.PropertyName == nameof(SearchViewModel.Results))
            {
                NotifyPropertyChange(nameof(ResultViewVisibility));
                if (SearchViewModel.Results == null)
                {
                    _resultOverlay.Graphics.Clear();
                }
                else if (SearchViewModel.SelectedResult == null)
                {
                    _resultOverlay.Graphics.Clear();
                    foreach(var result in SearchViewModel.Results)
                    {
                        AddResultToGeoView(result);
                    }
                    if (GeoView != null)
                    {
                        var zoomableResults = SearchViewModel.Results
                                                .Where(res => res.GeoElement?.Geometry != null)
                                                .Select(res => res.GeoElement.Geometry).ToList();

                        if (zoomableResults.Count > 1)
                        {
                            _ignoreViewpointChangedFlag = true;
                            var newViewpoint = Geometry.GeometryEngine.CombineExtents(zoomableResults);
                            if (GeoView is MapView mv)
                            {
                                _ = mv.SetViewpointGeometryAsync(newViewpoint, MultipleResultZoomBuffer);
                            }
                            else
                            {
                                // TODO - figure out what this will mean with Scenes
                                GeoView.SetViewpoint(new Viewpoint(newViewpoint));
                            }
                            _ignoreViewpointChangedFlag = false;
                        }
                    }
                }
            }
            else if (e.PropertyName == nameof(SearchViewModel.SelectedResult))
            {
                NotifyPropertyChange(nameof(ResultViewVisibility));
                if (SearchViewModel.SelectedResult != null)
                {
                    _resultOverlay.Graphics.Clear();
                    if (SearchViewModel?.SelectedResult is SearchResult selectedResult)
                    {
                        AddResultToGeoView(selectedResult);
                        // Handle feature layers later
                        // Zoom to the feature
                        if (selectedResult.SelectionViewpoint != null && GeoView != null)
                        {
                            _ignoreViewpointChangedFlag = true;
                            GeoView.SetViewpoint(selectedResult.SelectionViewpoint);
                            _ignoreViewpointChangedFlag = false;
                        }
                        if (GeoView != null)
                        {
                            // This isn't a result of user tap, why is the API trying to force me to pass a tap location or a separate MapPoint; The GeoElement knows where it is...
                            GeoView.ShowCalloutForGeoElement(selectedResult.GeoElement, new Point(0,0), selectedResult.CalloutDefinition);
                        }
                    }
                }
                else
                {
                    GeoView.DismissCallout();
                }
            }
        }

        private void AddResultToGeoView(SearchResult result)
        {
            result?.OwningSource?.NotifySelected(result);
            if (result?.GeoElement is Graphic graphic)
            {
                _resultOverlay.Graphics.Add(graphic);
            }
            // Handle other layers (e.g. feature layers) later
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
                UpdateModelForNewViewpoint();
                _acceptingSuggestionFlag = true;
                _ = SearchViewModel.AcceptSuggestion(userSelection)
                    .ContinueWith(tt => _acceptingSuggestionFlag = false, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void UpdateModelForNewViewpoint()
        {
            if (GeoView is MapView mv)
            {
                if (mv.GetCurrentViewpoint(ViewpointType.BoundingGeometry)?.TargetGeometry is Geometry.Geometry newView)
                {
                    SearchViewModel.QueryCenter = null;
                    SearchViewModel.QueryArea = newView;
                }
            }
            else if (GeoView is SceneView sv)
            {
                var newviewpoint = sv.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                if (newviewpoint?.TargetGeometry is MapPoint mp)
                {
                    SearchViewModel.QueryArea = null;
                    SearchViewModel.QueryCenter = mp;
                }
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
