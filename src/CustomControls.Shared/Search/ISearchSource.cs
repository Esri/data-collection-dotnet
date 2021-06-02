using System;
using System.Collections.Generic;
using System.Text;
using Esri.ArcGISRuntime.Geometry;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.UI;
using System.Threading;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public interface ISearchSource
    {
        string DisplayName { get; set; }
        int MaximumResults { get; set; }
        int MaximumSuggestions { get; set; }
        Geometry.Geometry SearchEnvelope { get; set; }
        MapPoint SearchLocation { get; set; }
        Task<IList<SearchSuggestion>> SuggestAsync(string QueryString, CancellationToken? CancellationToken);
        Task<IList<SearchResult>> SearchAsync(string QueryString, Geometry.Geometry Area, CancellationToken? CancellationToken);
        Task<IList<SearchResult>> SearchAsync(SearchSuggestion suggestion, CancellationToken? token);
        void NotifySelected(SearchResult result);
        void NotifyDeselected(SearchResult result);
        Func<SearchResult, CalloutDefinition> CalloutDefinitionProvider { get; }
    }
}
