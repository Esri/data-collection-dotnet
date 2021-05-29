using Esri.ArcGISRuntime.Tasks.Geocoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class SuggestionComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (x is SuggestResult r1 && y is SuggestResult r2)
            {
                return r1.IsCollection == r2.IsCollection && r1.Label == r2.Label;
            }
            else if (x is GeocodeResult gr1 && y is GeocodeResult gr2)
            {
                // TODO - is this enough?
                return gr1.DisplayLocation == gr2.DisplayLocation && gr1.Label == gr2.Label;
            }
            return false;
        }

        public int GetHashCode(T obj)
        {
            if (obj is SuggestResult sr)
            {
                return sr.Label.GetHashCode() * (sr.IsCollection ? 1 : -1);
            }
            else if (obj is GeocodeResult gr)
            { 
                return gr.DisplayLocation.GetHashCode();
            }
            return obj?.GetHashCode() ?? 0;
        }
    }
}
