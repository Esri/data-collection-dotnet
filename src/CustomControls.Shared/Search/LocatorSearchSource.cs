using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class LocatorSearchSource : ISearchSource
    {
        private string _displayName;
        private int _maxResults = 6;
        private int _maxSuggestions  = 6;
        private LocatorTask _locator;

        public string DisplayName { get => _displayName; set => SetPropertyChanged(value, ref _displayName); }
        public int MaximumResults { get => _maxResults; set => SetPropertyChanged(value, ref _maxResults); }
        public int MaximumSuggestions { get => _maxSuggestions; set => SetPropertyChanged(value, ref _maxSuggestions); }
        public LocatorTask Locator { get => _locator; }
        public GeocodeParameters GeocodeParameters { get; }
        public SuggestParameters SuggestParameters { get; }
        public Geometry.Geometry SearchEnvelope { get => GeocodeParameters.SearchArea; set => GeocodeParameters.SearchArea = SuggestParameters.SearchArea = value; }
        public MapPoint SearchLocation { get => GeocodeParameters.PreferredSearchLocation; set => GeocodeParameters.PreferredSearchLocation = SuggestParameters.PreferredSearchLocation = value; }

        public Func<SearchResult, CalloutDefinition> CalloutDefinitionProvider { get; set; } = null;

        public LocatorSearchSource(LocatorTask locator)
        {
            _locator = locator;
            _displayName = _locator?.LocatorInfo?.Name;
            GeocodeParameters = new GeocodeParameters();
            SuggestParameters = new SuggestParameters();
            _ = EnsureLoaded();
        }

        public async Task<IList<SearchResult>> SearchAsync(string QueryString, Geometry.Geometry geometry)
        {
            await EnsureLoaded();

            var result = await _locator.GeocodeAsync(QueryString, GeocodeParameters);

            return result.Select(r => GeocodeResultToSearchResult(r)).ToList();
        }

        public async Task<IList<SearchResult>> SearchAsync(SearchSuggestion suggestion)
        {
            await EnsureLoaded();
            var result = await _locator.GeocodeAsync(suggestion.SuggestResult, GeocodeParameters);
            return result.Select(r => GeocodeResultToSearchResult(r)).ToList();
        }

        public async Task<IList<SearchSuggestion>> SuggestAsync(string QueryString)
        {
            await EnsureLoaded();
            var suggestions = await _locator.SuggestAsync(QueryString, SuggestParameters);
            return suggestions.Select(s => new SearchSuggestion(s.Label, null, this, s)).ToList();
        }

        private async Task EnsureLoaded()
        {
            if (_locator.LoadStatus == LoadStatus.Loaded)
            {
                return;
            }

            try
            {
            await _locator.RetryLoadAsync();

            }
            catch (Exception ex)
            {

            }

            if (!string.IsNullOrWhiteSpace(_locator?.LocatorInfo?.Name))
            {
                this.DisplayName = _locator?.LocatorInfo?.Name;
            }
            // World Geocoder is annoying and has a description but not a name, because
            else if (!string.IsNullOrWhiteSpace(_locator?.LocatorInfo?.Description))
            {
                this.DisplayName = _locator?.LocatorInfo?.Description;
            }

            // Add any compatible attributes
            var desiredAttributes = new [] {"LongLabel", "Type"};
            if (_locator?.LocatorInfo?.ResultAttributes?.Any() ?? false)
            {
                foreach(var attr in desiredAttributes)
                {
                    if (_locator.LocatorInfo.ResultAttributes.Where(at => at.Name == attr).Any())
                    {
                        GeocodeParameters.ResultAttributeNames.Add(attr);
                    }
                }
            }
            else
            {
                GeocodeParameters.ResultAttributeNames.Add("*");
            }
        }

        private SearchResult GeocodeResultToSearchResult(GeocodeResult r)
        {
            var symbol = SymbolForResult(r);
            string subtitle = $"Match percent: {r.Score}";
            if (r.Attributes.ContainsKey("LongLabel"))
            {
                subtitle = r.Attributes["LongLabel"].ToString();;
            }

            return new SearchResult(r.Label, subtitle, this, new Graphic(r.DisplayLocation, r.Attributes, symbol), new Mapping.Viewpoint(r.Extent));
        }

        private Symbol SymbolForResult(GeocodeResult r)
        {
            // Based on list from: https://developers.arcgis.com/rest/geocode/api-reference/geocoding-category-filtering.htm

            // TODO = use offline symbols

            if (r.Attributes.ContainsKey("Type"))
            {
                // Food
                if (r.Attributes["Type"].ToString().Contains("Food"))
                {
                    return new PictureMarkerSymbol(new Uri("https://static.arcgis.com/images/Symbols/SafetyHealth/FireStation.png"))
                    {
                        Height = 24,
                        Width = 24
                    };
                }
                // Coffee
                if (r.Attributes["Type"].ToString().Contains("Coffee"))
                {
                    return new PictureMarkerSymbol(new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/Recreation/FeatureServer/0/images/e82f744ebb069bb35b234b3fea46deae"))
                    {
                        Height = 24,
                        Width = 24
                    };
                }
            }
            var symbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 8);

            return symbol;
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

        public async Task<IList<SearchResult>> SearchAsync(string QueryString)
        {
            await EnsureLoaded();
            var result = await _locator.GeocodeAsync(QueryString, GeocodeParameters);
            return result.Select(r => GeocodeResultToSearchResult(r)).ToList();
        }

        public void NotifySelected(SearchResult result)
        {
            // This space intentionally left blank.
        }

        public void NotifyDeselected(SearchResult result)
        {
            // This space intentionally left blank.
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
