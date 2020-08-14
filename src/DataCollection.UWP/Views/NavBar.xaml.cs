using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using Windows.UI.Xaml;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    public sealed partial class NavBar
    {
        public NavBar()
        {
            this.InitializeComponent();
        }

        public ModernMapPanel ParentPanel
        {
            get => (ModernMapPanel)GetValue(ParentPanelProperty);
            set => SetValue(ParentPanelProperty, value);
        }

        // Using a DependencyProperty as the backing store for ParentPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register("ParentPanel", typeof(ModernMapPanel), typeof(NavBar), new PropertyMetadata(null));
    }
}
