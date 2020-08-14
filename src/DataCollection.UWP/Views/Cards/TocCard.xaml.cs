using Windows.UI.Xaml;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Cards
{
    public sealed partial class TocCard
    {
        public TocCard()
        {
            this.InitializeComponent();
        }

        public MainViewModel MainViewModel
        {
            get => (MainViewModel)GetValue(MainViewModelProperty);
            set => SetValue(MainViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for MainViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(TocCard),
                new PropertyMetadata(null));
    }
}
