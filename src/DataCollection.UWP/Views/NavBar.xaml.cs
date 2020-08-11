using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
