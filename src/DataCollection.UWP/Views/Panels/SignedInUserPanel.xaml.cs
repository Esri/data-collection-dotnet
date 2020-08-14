using Windows.UI.Xaml;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;


namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Panels
{
    public sealed partial class SignedInUserPanel
    {
        public SignedInUserPanel()
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
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(SignedInUserPanel), new PropertyMetadata(null));
    }
}
