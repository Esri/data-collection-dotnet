using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public static class ExtensionMethods
    {
        public static void AddRange<T>(this ObservableCollection<T> inputCollection, IEnumerable<T> additions)
        {
            foreach(var addition in additions)
            {
                inputCollection.Add(addition);
            }
        }
    }
}
