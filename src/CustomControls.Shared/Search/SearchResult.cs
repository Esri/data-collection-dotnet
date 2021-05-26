using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class SearchResult
    {
        public string Title { get; }
        public string Subtitle { get; }
        public ISearchSource OwningSource { get; }
        public GeoElement GeoElement { get; }
        public Viewpoint SelectionViewpoint { get; }

        public SearchResult(string title, string subtitle, ISearchSource owner, GeoElement geoElement, Viewpoint viewpoint)
        {
            Title = title;
            Subtitle = subtitle;
            OwningSource = owner;
            GeoElement = geoElement;
            SelectionViewpoint = viewpoint;
        }
    }
}
