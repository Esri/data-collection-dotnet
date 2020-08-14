using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    public sealed partial class NavBar : UserControl
    {
        public NavBar()
        {
            this.InitializeComponent();
        }

        public ModernMapPanel ParentPanel
        {
            get { return (ModernMapPanel)GetValue(ParentPanelProperty); }
            set { SetValue(ParentPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register("ParentPanel", typeof(ModernMapPanel), typeof(NavBar), new PropertyMetadata(null));
    }
}
