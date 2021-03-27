using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views.Cards
{
    /// <summary>
    /// Interaction logic for NewFeatureCard.xaml
    /// </summary>
    public partial class NewFeatureCard : CardBase
    {
        public NewFeatureCard()
        {
            InitializeComponent();
        }



        public NewFeatureViewModel ViewModel
        {
            get { return (NewFeatureViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(NewFeatureViewModel), typeof(NewFeatureCard), new PropertyMetadata(null));


    }
}
