using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Tests.Mocks
{
    public class GeoViewInputEventArgs
    {
        public bool Handled { get; set;}
        public MapPoint Location { get; set;}
        public Point Position { get; set;}
    }
}
