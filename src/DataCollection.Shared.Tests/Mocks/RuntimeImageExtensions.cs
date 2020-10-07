using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Tests.Mocks
{
    public static class RuntimeImageExtensions
    {
        public static Task<ImageSource> ToImageSourceAsync(this RuntimeImage image) => null;
        public static Task<RuntimeImage> ToRuntimeImageAsync(this ImageSource image) => null;
    }
}
