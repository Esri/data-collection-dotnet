using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class SearchResult : INotifyPropertyChanged
    {
        private ImageSource _uiImage;
        private RuntimeImage _rtImage;
        private CalloutDefinition _calloutDefinition;

        public string Title { get; }
        public string Subtitle { get; }
        public ISearchSource OwningSource { get; }
        public GeoElement GeoElement { get; }
        public Viewpoint SelectionViewpoint { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchResult(string title, string subtitle, ISearchSource owner, GeoElement geoElement, Viewpoint viewpoint)
        {
            Title = title;
            Subtitle = subtitle;
            OwningSource = owner;
            GeoElement = geoElement;
            SelectionViewpoint = viewpoint;
        }

        // TODO - cache these operations; multiple roundtrips to core for images is not fast
        public ImageSource MarkerImage
        {
            get
            {
                if (_uiImage != null)
                {
                    return _uiImage;
                }
                else if (GeoElement is Graphic graphic && graphic.Symbol != null)
                {
                    _ = GetImage(graphic.Symbol);
                }
                else if (GeoElement is Feature feature && feature.FeatureTable?.Layer is FeatureLayer featureLayer
                    && featureLayer.Renderer?.GetSymbol(GeoElement) is Symbol symbol)
                {
                    _ = GetImage(symbol);
                }
                return null;
            }
        }

        private bool _imageRequestFlag;

        private async Task GetImage(Symbol symbol)
        {
            if (_imageRequestFlag) return;
            _imageRequestFlag = true;
            var swatch = await symbol.CreateSwatchAsync();
            _uiImage = await swatch.ToImageSourceAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MarkerImage)));
        }

        public CalloutDefinition CalloutDefinition
        {
            get
            {
                if (_calloutDefinition != null)
                {
                    return _calloutDefinition;
                }
                if (OwningSource?.CalloutDefinitionProvider != null)
                {
                    _calloutDefinition = OwningSource?.CalloutDefinitionProvider(this);
                    return _calloutDefinition;
                }
                _calloutDefinition = new CalloutDefinition(GeoElement);
                _calloutDefinition.Text = this.Title;
                _calloutDefinition.DetailText = this.Subtitle;
                _calloutDefinition.Tag = this;
                return _calloutDefinition;
            }
        }
    }
}
