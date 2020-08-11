using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.MapCards
{
    public sealed partial class TocCard : CardBase
    {
        public TocCard()
        {
            this.InitializeComponent();
        }

        public MainViewModel MainViewModel
        {
            get { return (MainViewModel)base.GetValue(MainViewModelProperty); }
            set { base.SetValue(MainViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(TocCard), new PropertyMetadata(null));

        //public bool IsOpen
        //{
        //    get { return (bool)GetValue(IsOpenProperty); }
        //    set { SetValue(IsOpenProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsOpenProperty =
        //    DependencyProperty.Register("IsOpen", typeof(bool), typeof(TocCard), new PropertyMetadata(false, HandleIsOpenChanged));

        //public event PropertyChangedEventHandler PropertyChanged;

        //private static void HandleIsOpenChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea)
        //{
        //    (dpo as TocCard)?.PropertyChanged?.Invoke(dpo, new PropertyChangedEventArgs(nameof(IsOpen)));
        //}

    }
}
