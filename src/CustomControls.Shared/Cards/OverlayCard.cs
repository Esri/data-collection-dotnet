using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if __UWP__
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

#if __WPF__
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Cards
{
    public class OverlayCard : ContentControl
    {

        public OverlayCard() : base()
        {
            DefaultStyleKey = typeof(OverlayCard);

            BottomAccessories = new ObservableCollection<UIElement>();
            TopAccessories = new ObservableCollection<UIElement>();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(OverlayCard), new PropertyMetadata(null));



        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(OverlayCard), new PropertyMetadata(null));



        public string PrimarySubtitle
        {
            get { return (string)GetValue(PrimarySubtitleProperty); }
            set { SetValue(PrimarySubtitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimarySubtitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimarySubtitleProperty =
            DependencyProperty.Register(nameof(PrimarySubtitle), typeof(string), typeof(OverlayCard), new PropertyMetadata(null));




        public string SecondarySubtitle
        {
            get { return (string)GetValue(SecondarySubtitleProperty); }
            set { SetValue(SecondarySubtitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondarySubtitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondarySubtitleProperty =
            DependencyProperty.Register(nameof(SecondarySubtitle), typeof(string), typeof(OverlayCard), new PropertyMetadata(null));



        public ObservableCollection<UIElement> BottomAccessories
        {
            get { return (ObservableCollection<UIElement>)GetValue(BottomAccessoriesProperty); }
            private set { SetValue(BottomAccessoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomAccessories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomAccessoriesProperty =
            DependencyProperty.Register(nameof(BottomAccessories), typeof(ObservableCollection<UIElement>), typeof(OverlayCard), new PropertyMetadata(null));




        public ObservableCollection<UIElement> TopAccessories
        {
            get { return (ObservableCollection<UIElement>)GetValue(TopAccessoriesProperty); }
            private set { SetValue(TopAccessoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopAccessories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopAccessoriesProperty =
            DependencyProperty.Register(nameof(TopAccessories), typeof(ObservableCollection<UIElement>), typeof(OverlayCard), new PropertyMetadata(null));



        public Brush BarBackground
        {
            get { return (Brush)GetValue(BarBackgroundProperty); }
            set { SetValue(BarBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarBackgroundProperty =
            DependencyProperty.Register(nameof(BarBackground), typeof(Brush), typeof(OverlayCard), new PropertyMetadata(null));


#if __WPF__
        // TODO figure out hiding base member warning
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(OverlayCard), new PropertyMetadata(null));

#elif __UWP__
        // NOTE: this is primarily necessary because some supported UWP versions support corner radius, others don't.
        // Dependency property is conditionally registered in a static constructor above
        // TODO: test on old windows and determine if this is actually working

        public CornerRadius CornerRadiusCompat
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty;

        static OverlayCard()
        {
            if (!Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent(nameof(ContentControl), nameof(CornerRadius))){
                CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(OverlayCard), new PropertyMetadata(null));
            }
        }
#endif


        public double PrimaryBarHeight
        {
            get { return (double)GetValue(PrimaryBarHeightProperty); }
            set { SetValue(PrimaryBarHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimaryBarHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryBarHeightProperty =
            DependencyProperty.Register("PrimaryBarHeight", typeof(double), typeof(OverlayCard), new PropertyMetadata(40.0));



        public double SecondaryBarHeight
        {
            get { return (double)GetValue(SecondaryBarHeightProperty); }
            set { SetValue(SecondaryBarHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondaryBarHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryBarHeightProperty =
            DependencyProperty.Register("SecondaryBarHeight", typeof(double), typeof(OverlayCard), new PropertyMetadata(30.0));



        public Style TitleTextStyle
        {
            get { return (Style)GetValue(TitleTextStyleProperty); }
            set { SetValue(TitleTextStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleTextStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTextStyleProperty =
            DependencyProperty.Register("TitleTextStyle", typeof(Style), typeof(OverlayCard), new PropertyMetadata(null));




        public Style SubtitleTextStyle
        {
            get { return (Style)GetValue(SubtitleTextStyleProperty); }
            set { SetValue(SubtitleTextStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubtitleTextStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtitleTextStyleProperty =
            DependencyProperty.Register("SubtitleTextStyle", typeof(Style), typeof(OverlayCard), new PropertyMetadata(null));


    }
}
