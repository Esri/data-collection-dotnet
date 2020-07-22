using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views.Cards
{
    /// <summary>
    /// Interaction logic for TocCard.xaml
    /// </summary>
    public partial class TocCard : UserControl
    {
        public TocCard()
        {
            InitializeComponent();
            DataContext = this;
        }



        public MapAccessoryViewModel MapAccessoryViewModel
        {
            get { return (MapAccessoryViewModel)GetValue(MapAccessoryViewModelProperty); }
            set { SetValue(MapAccessoryViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapAccessoryViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapAccessoryViewModelProperty =
            DependencyProperty.Register("MapAccessoryViewModel", typeof(MapAccessoryViewModel), typeof(TocCard), new PropertyMetadata(null));


    }
}
