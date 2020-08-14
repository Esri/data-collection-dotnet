using Windows.UI.Xaml;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Overlays
{
    public sealed partial class IdentifyIndicatorOverlay
    {
        public IdentifyIndicatorOverlay()
        {
            this.InitializeComponent();
        }


        public MainViewModel MainViewModel
        {
            get { return (MainViewModel)GetValue(MainViewModelProperty); }
            set { SetValue(MainViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(IdentifyIndicatorOverlay), new PropertyMetadata(null));


    }
}
