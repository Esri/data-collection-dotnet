using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    public class NewFeatureViewModel : BaseViewModel
    {
        private IEnumerable<FeatureTemplateViewModel> _allTemplates;
        public IEnumerable<FeatureTemplateViewModel> AvailableTemplates
        {
            get => _allTemplates;
            private set
            {
                _allTemplates = value;
            }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand;

        public NewFeatureViewModel(Map sourceMap, ICommand closeCommand)
        {
            _closeCommand = closeCommand;
            List<FeatureTemplateViewModel> templates = new List<FeatureTemplateViewModel>();
            var candidateLayers = sourceMap.OperationalLayers.OfType<FeatureLayer>().Where(layer => layer.FeatureTable is ArcGISFeatureTable);

            foreach(var layer in candidateLayers)
            {
                var table = layer.FeatureTable as ArcGISFeatureTable;
                // Add explicit feature templates
                templates.AddRange(table.FeatureTemplates.Select(template => new FeatureTemplateViewModel(template, layer)));
                // Add templates for each feature type
                templates.AddRange(table.FeatureTypes.SelectMany(featureType => featureType.Templates).Select(template => new FeatureTemplateViewModel(template, layer)));
            }
            AvailableTemplates = templates;
        }
    }

    public class FeatureTemplateViewModel : BaseViewModel
    {
        private FeatureTemplate _template;
        private FeatureLayer _layer;
        private ImageSource _iconImageSource;
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource IconImageSource
        {
            get => _iconImageSource;
            set
            {
                if (_iconImageSource != value)
                {
                    _iconImageSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public FeatureTemplate Template { get => _template; set
            {
                if (value != _template)
                {
                    _template = value;
                    OnPropertyChanged();
                }
            } 
        }

        public FeatureLayer Layer { get => _layer; set
            {
                if (value != _layer)
                {
                    _layer = value;
                    OnPropertyChanged();
                }
            } }


        public FeatureTemplateViewModel(FeatureTemplate template, FeatureLayer sourceLayer)
        {
            _template = template;
            _layer = sourceLayer;
            _ = UpdateImageSource();
        }

        private async Task UpdateImageSource()
        {
            IsLoading = true;
            try
            {
                ArcGISFeature tempFeature = (Layer.FeatureTable as ArcGISFeatureTable).CreateFeature(Template);
                Popup popup = new Popup(tempFeature, null);
                var symbol = await popup.Symbol.CreateSwatchAsync(192);
                var imageSource = await symbol.ToImageSourceAsync();
                IconImageSource = imageSource;
            }
            catch (Exception)
            {
                // Ignore
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
