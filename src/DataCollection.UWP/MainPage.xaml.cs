using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP
{
    /// <summary>
    /// A map page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
	    {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the view-model that provides mapping capabilities to the view
        /// </summary>
        public MainViewModel MainViewModel { get; } = new MainViewModel();

        public AuthViewModel AuthViewModel { get; } = new AuthViewModel(
                                                        Settings.Default.WebmapURL,
                                                        Settings.Default.ArcGISOnlineURL,
                                                        Settings.Default.AppClientID,
                                                        Settings.Default.RedirectURL,
                                                        Settings.Default.OAuthRefreshToken);

        private void BladeView_BladeClosed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.BladeItem e)
        {
            if (e.Name == "IdentifiedFeatureBlade")
                MainViewModel.IdentifiedFeatureViewModel = null;
            else if (e.Name == "OriginRelationshipBlade")
                MainViewModel.IdentifiedFeatureViewModel.SelectedOriginRelationship = null;
            else if (e.Name == "DestinationRelationshipBlade")
                MainViewModel.IdentifiedFeatureViewModel.SelectedDestinationRelationship = null;
        }
    }
}