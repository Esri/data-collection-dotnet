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
        private int _maxSuggestions = 6;
        private int _minResults = 1;
        private int _minSuggestions = 6;
        private LocatorTask _locator;

        private SymbolStyle _esriStyle;

        public string DisplayName { get => _displayName; set => SetPropertyChanged(value, ref _displayName); }
        public int MaximumResults { get => _maxResults; set => SetPropertyChanged(value, ref _maxResults); }
        public int MaximumSuggestions { get => _maxSuggestions; set => SetPropertyChanged(value, ref _maxSuggestions); }
        public int RepeatSearchResultThreshold { get => _minResults; set => SetPropertyChanged(value, ref _minResults); }
        public int RepeatSuggestResultThreshold { get => _minSuggestions; set => SetPropertyChanged(value, ref _minSuggestions); }
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
            GeocodeParameters = new GeocodeParameters { MaxResults = this.MaximumResults };
            SuggestParameters = new SuggestParameters { MaxResults = this.MaximumSuggestions };
            _ = EnsureLoaded();
        }

        public async Task<IList<SearchResult>> SearchAsync(string QueryString, Geometry.Geometry geometry, CancellationToken? token)
        {
            await EnsureLoaded();

            token?.ThrowIfCancellationRequested();

            // Search with explicit area constraint
            if (geometry != null)
            {
                var tempParams = CopyGeocodeParameters();
                tempParams.PreferredSearchLocation = null;
                tempParams.SearchArea = geometry;
                if (token.HasValue)
                {
                    var areaSearchResult = await _locator.GeocodeAsync(QueryString, tempParams, token.Value);
                    return await ResultToSearchResult(areaSearchResult.ToList());
                }
                else
                {
                    var areaSearchResult = await _locator.GeocodeAsync(QueryString, tempParams);
                    return await ResultToSearchResult(areaSearchResult.Take(MaximumResults).ToList());
                }
            }

            // Search using just the center, no constraint
            var centerParams = CopyGeocodeParameters();
            if (centerParams.PreferredSearchLocation == null && centerParams.SearchArea != null)
            {
                if (centerParams.SearchArea is MapPoint mp)
                {
                    centerParams.PreferredSearchLocation = mp;
                    centerParams.SearchArea = null;
                }
                else if (!centerParams.SearchArea.Extent.IsEmpty && centerParams.SearchArea.Extent.GetCenter() is MapPoint newCenter)
                {
                    centerParams.PreferredSearchLocation = newCenter;
                    centerParams.SearchArea = null;
                }
            }
            else if (centerParams.PreferredSearchLocation != null)
            {
                centerParams.SearchArea = null;
            }
            var comparer = new SuggestionComparer<GeocodeResult>();

            var otherResults = token.HasValue ? await _locator.GeocodeAsync(QueryString, centerParams, token.Value)
                                              : await _locator.GeocodeAsync(QueryString, centerParams);
            if (otherResults.Count() >= RepeatSearchResultThreshold)
            {
                return await ResultToSearchResult(otherResults.Take(MaximumResults).ToList());
            }

            // Repeat with all spatial constraints removed
            var nonSpatialParams = CopyGeocodeParameters();
            nonSpatialParams.SearchArea = null;
            nonSpatialParams.PreferredSearchLocation = null;
            var otherResults2 = token.HasValue ? await _locator.GeocodeAsync(QueryString, nonSpatialParams, token.Value)
                                               : await _locator.GeocodeAsync(QueryString, nonSpatialParams);
            var fullResults = otherResults.Union(otherResults2, comparer);
            return await ResultToSearchResult(fullResults.Take(MaximumResults).ToList());
        }

        // TODO = abstract away commonalities between SearchAsync with suggestion and string
        public async Task<IList<SearchResult>> SearchAsync(SearchSuggestion suggestion, CancellationToken? token)
        {
            await EnsureLoaded();
            // Search using just the center, no constraint
            var centerParams = CopyGeocodeParameters();
            if (centerParams.PreferredSearchLocation == null && centerParams.SearchArea != null)
            {
                if (centerParams.SearchArea is MapPoint mp)
                {
                    centerParams.PreferredSearchLocation = mp;
                    centerParams.SearchArea = null;
                }
                else if (!centerParams.SearchArea.Extent.IsEmpty && centerParams.SearchArea.Extent.GetCenter() is MapPoint newCenter)
                {
                    centerParams.PreferredSearchLocation = newCenter;
                    centerParams.SearchArea = null;
                }
            }
            else if (centerParams.PreferredSearchLocation != null)
            {
                centerParams.SearchArea = null;
            }

            var centerResults = token.HasValue ? await _locator.GeocodeAsync(suggestion.SuggestResult, centerParams, token.Value)
                                               : await _locator.GeocodeAsync(suggestion.SuggestResult, centerParams);

            if (centerResults.Count() >= RepeatSearchResultThreshold)
            {
                return await ResultToSearchResult(centerResults.Take(MaximumResults).ToList());
            }

            // Repeat with all spatial constraints removed
            var nonSpatialParams = CopyGeocodeParameters();
            nonSpatialParams.SearchArea = null;
            nonSpatialParams.PreferredSearchLocation = null;
            var fullR = token.HasValue ? await _locator.GeocodeAsync(suggestion.SuggestResult, nonSpatialParams, token.Value)
                                       : await _locator.GeocodeAsync(suggestion.SuggestResult, nonSpatialParams);
            var fullResults = centerResults.Union(fullR);
            return await ResultToSearchResult(fullResults.Take(MaximumResults).ToList());
        }

        public async Task<IList<SearchSuggestion>> SuggestAsync(string QueryString, CancellationToken? token)
        {
            await EnsureLoaded();

            // Remove area constraint
            var centerOnlyParams = CopySuggestParameters();
            if (centerOnlyParams.PreferredSearchLocation != null)
            {
                centerOnlyParams.SearchArea = null;
            }
            else if (centerOnlyParams.PreferredSearchLocation == null && centerOnlyParams.SearchArea != null)
            {
                if (centerOnlyParams.SearchArea is MapPoint mp)
                {
                    centerOnlyParams.PreferredSearchLocation = mp;
                    centerOnlyParams.SearchArea = null;
                }
                else if (!centerOnlyParams.SearchArea.Extent.IsEmpty)
                {
                    centerOnlyParams.PreferredSearchLocation = centerOnlyParams.SearchArea.Extent.GetCenter();
                    centerOnlyParams.SearchArea = null;
                }
                // TODO = optimize so that searches aren't repeated if searchArea is empty
            }
            var comparer = new SuggestionComparer<SuggestResult>();

            var cenerResults = token.HasValue ? await _locator.SuggestAsync(QueryString, centerOnlyParams, token.Value)
                                              : await _locator.SuggestAsync(QueryString, centerOnlyParams);

            if (cenerResults.Count() >= RepeatSuggestResultThreshold)
            {
                return cenerResults.Take(MaximumSuggestions).Select(s => new SearchSuggestion(s.Label, null, this, s)).ToList();
            }

            // Remove remaining constraints
            var unconstrainedParams = CopySuggestParameters();
            unconstrainedParams.PreferredSearchLocation = null;
            unconstrainedParams.SearchArea = null;
            var allResults = token.HasValue ? await _locator.SuggestAsync(QueryString, unconstrainedParams, token.Value)
                                            : await _locator.SuggestAsync(QueryString, unconstrainedParams);

            var unconstrainedResults = cenerResults.Union(allResults, comparer);
            return unconstrainedResults.Take(MaximumSuggestions).Select(s => new SearchSuggestion(s.Label, null, this, s)).ToList();
        }

        private async Task EnsureLoaded()
        {
            if (_esriStyle == null)
            {
                _esriStyle = await SymbolStyle.OpenAsync(styleName: "Esri2DPointSymbolsStyle", portal: null);
            }
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
            var desiredAttributes = new[] { "LongLabel", "Type" };
            if (_locator?.LocatorInfo?.ResultAttributes?.Any() ?? false)
            {
                foreach (var attr in desiredAttributes)
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

        private async Task<SearchResult> GeocodeResultToSearchResult(GeocodeResult r)
        {
            var symbol = await SymbolForResult(r);
            string subtitle = $"Match percent: {r.Score}";
            if (r.Attributes.ContainsKey("LongLabel"))
            {
                subtitle = r.Attributes["LongLabel"].ToString(); ;
            }

            return new SearchResult(r.Label, subtitle, this, new Graphic(r.DisplayLocation, r.Attributes, symbol), new Mapping.Viewpoint(r.Extent));
        }

        private async Task<Symbol> SymbolForResult(GeocodeResult r)
        {
            if (r.Attributes.ContainsKey("Type"))
            {
                var typeAttrs = r.Attributes["Type"];
                var firstResult = await _esriStyle.GetSymbolAsync(new[] { typeAttrs.ToString().Replace(' ', '-').ToLower() });
                if (firstResult != null)
                {
                    return firstResult;
                }
                var symbParams = await _esriStyle.GetDefaultSearchParametersAsync();
                symbParams.Names.Clear();
                symbParams.Names.Add(typeAttrs.ToString());
                symbParams.NamesStrictlyMatch = false;
                var symbolResult = await _esriStyle.SearchSymbolsAsync(symbParams);

                if (symbolResult.Any())
                {
                    return await symbolResult.First().GetSymbolAsync();
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
            return await ResultToSearchResult(result);
        }

        private async Task<IList<SearchResult>> ResultToSearchResult(IReadOnlyList<GeocodeResult> input)
        {
            var results = await Task.WhenAll(input.Select(i => GeocodeResultToSearchResult(i)));

            return results.ToList();
        }

        public void NotifySelected(SearchResult result)
        {
            // This space intentionally left blank.
        }

        public void NotifyDeselected(SearchResult result)
        {
            // This space intentionally left blank.
        }

        private GeocodeParameters CopyGeocodeParameters()
        {
            GeocodeParameters newParams = new GeocodeParameters()
            {
                CountryCode = GeocodeParameters.CountryCode,
                IsForStorage = GeocodeParameters.IsForStorage,
                MaxResults = GeocodeParameters.MaxResults,
                MinScore = GeocodeParameters.MinScore,
                OutputLanguage = GeocodeParameters.OutputLanguage,
                OutputSpatialReference = GeocodeParameters.OutputSpatialReference,
                PreferredSearchLocation = GeocodeParameters.PreferredSearchLocation,
                SearchArea = GeocodeParameters.SearchArea,
            };
            foreach (var category in GeocodeParameters.Categories)
            {
                newParams.Categories.Add(category);
            }
            foreach (var attrName in GeocodeParameters.ResultAttributeNames)
            {
                newParams.ResultAttributeNames.Add(attrName);
            }
            return newParams;
        }

        private SuggestParameters CopySuggestParameters()
        {
            SuggestParameters newParameters = new SuggestParameters();
            newParameters.CountryCode = SuggestParameters.CountryCode;
            newParameters.MaxResults = SuggestParameters.MaxResults;
            newParameters.PreferredSearchLocation = SuggestParameters.PreferredSearchLocation;
            newParameters.SearchArea = SuggestParameters.SearchArea;
            foreach (var cat in SuggestParameters.Categories)
            {
                newParameters.Categories.Add(cat);
            }

            return newParameters;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
