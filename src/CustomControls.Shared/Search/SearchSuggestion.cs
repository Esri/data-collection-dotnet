using Esri.ArcGISRuntime.Tasks.Geocoding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class SearchSuggestion
    {
        public string DisplayTitle { get; }
        public string DisplaySusbtitle { get; }
        public ISearchSource OwningSource { get; }
        public SuggestResult SuggestResult { get; }

        public SearchSuggestion(string title, string subtitle, ISearchSource owner, SuggestResult result)
        {
            DisplayTitle = title;
            DisplaySusbtitle = subtitle;
            OwningSource = owner;
            SuggestResult = result;
        }
    }
}
