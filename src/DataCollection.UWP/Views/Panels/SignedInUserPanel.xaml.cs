using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Popups
{
    public sealed partial class SignedInUserPanel : UserControl
    {
        public SignedInUserPanel()
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
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(SignedInUserPanel), new PropertyMetadata(null));


    }
}
