using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private ISearchSource _activeSource;
        private SearchResult _selectedResult;
        private string _currentQuery;
        private string _defaultPlaceholder = "Find a place or address";
        private SearchResultMode _searchMode = SearchResultMode.Automatic;
        private Geometry.Geometry _queryArea;
        private MapPoint _queryCenter;

        public ISearchSource ActiveSource { get => _activeSource; set => SetPropertyChanged(value, ref _activeSource); }
        public SearchResult SelectedResult { get => _selectedResult; set => SetPropertyChanged(value, ref _selectedResult); }
        public string CurrentQuery { get => _currentQuery; set => SetPropertyChanged(value, ref _currentQuery); }
        public string DefaultPlaceholder { get => _defaultPlaceholder; set => SetPropertyChanged(value, ref _defaultPlaceholder); }
        public SearchResultMode SearchMode { get => _searchMode; set => SetPropertyChanged(value, ref _searchMode); }
        public Geometry.Geometry QueryArea { get => _queryArea; set => SetPropertyChanged(value, ref _queryArea); }
        public MapPoint QueryCenter { get => _queryCenter; set => SetPropertyChanged(value, ref _queryCenter); }
        
        public ObservableCollection<ISearchSource> Sources { get; } = new ObservableCollection<ISearchSource>();
        public ObservableCollection<SearchResult> Results { get; } = new ObservableCollection<SearchResult>();
        public ObservableCollection<SearchSuggestion> Suggestions { get; } = new ObservableCollection<SearchSuggestion>();

        public async Task CommitSearch()
        {
            Suggestions.Clear();
            Results.Clear();

            var sourcesToSearch = SourcesToSearch();

            foreach(var source in sourcesToSearch)
            {
                source.SearchEnvelope = QueryArea;
                source.SearchLocation = QueryCenter;
            }

            var allResults = await Task.WhenAll(sourcesToSearch.Select(s => s.SearchAsync(CurrentQuery)));

            foreach(var result in allResults.SelectMany(l => l))
            {
                Results.Add(result);
            }
        }

        public async Task UpdateSuggestions()
        {
            Suggestions.Clear();
            if (string.IsNullOrWhiteSpace(CurrentQuery))
            {
                return;
            }
            var sourcesToSearch = SourcesToSearch();

            foreach(var source in sourcesToSearch)
            {
                source.SearchEnvelope = QueryArea;
                source.SearchLocation = QueryCenter;
            }

            var allSuggestions = await Task.WhenAll(sourcesToSearch.Select(s => s.SuggestAsync(CurrentQuery)));

            foreach(var suggestion in allSuggestions.SelectMany(s => s))
            {
                Suggestions.Add(suggestion);
            }
        }

        public async Task AcceptSuggestion(SearchSuggestion suggestion)
        {
            Suggestions.Clear();
            Results.Clear();
            SelectedResult = null;

            if (suggestion == null) return;

            var selectedSource = suggestion.OwningSource;
            var results = await selectedSource.SearchAsync(suggestion);

            // TODO figure out showing no result message
            if (!results.Any())
                return;

            switch (SearchMode)
            {
                case SearchResultMode.Single:
                    Results.Add(results.First());
                    SelectedResult = Results.First();
                    break;
                case SearchResultMode.Multiple:
                    Results.AddRange(results);
                    break;
                case SearchResultMode.Automatic:
                    if (suggestion.SuggestResult?.IsCollection ?? true)
                        Results.AddRange(results);
                    else
                        Results.Add(results.First());

                    if (Results.Count == 1)
                        SelectedResult = Results.First();
                    break;
            }
        }

        public async Task ConfigureFromMap(Map map)
        {
            await map.RetryLoadAsync();
            // Clear existing properties
            this.Sources.Clear();
            this.ActiveSource = null;
            this.CurrentQuery = null;
            // TODO - enable localization
            this.DefaultPlaceholder = "Find a place or address";

            // Read default search hint

            // Add ArcGIS Online
            Sources.Add(new LocatorSearchSource(new LocatorTask(new Uri("https://geocode-api.arcgis.com/arcgis/rest/services/World/GeocodeServer"))));

            // Add any layers from JSON

            // Add any offline locators if map is MMPK
            if (map.Item is LocalItem localItem)
            {
                try
                {
                    var package = await MobileMapPackage.OpenAsync(localItem.Path);
                    if (package?.LocatorTask is LocatorTask offlineTask)
                    {
                        Sources.Add(new LocatorSearchSource(offlineTask));
                    }
                }
                catch (Exception ex)
                {
                    // TODO - handle
                }
            }

        }

        private List<ISearchSource> SourcesToSearch()
        { 
            var selectedSources = new List<ISearchSource>();
            if (ActiveSource == null)
            {
                selectedSources.AddRange(Sources);
            }
            else
            {
                selectedSources.Add(ActiveSource);
            }
            return selectedSources;
        }

        private void SetPropertyChanged<T>(T value, ref T field, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(value, field))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
