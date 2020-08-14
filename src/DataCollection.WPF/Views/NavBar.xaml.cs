using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for NavBar.xaml
    /// </summary>
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
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
