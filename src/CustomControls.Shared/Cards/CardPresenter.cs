using System;
using System.Collections.Generic;
using System.Text;


#if __WPF__

using System.Windows;
using System.Windows.Controls;
#elif __UWP__
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Cards
{
    public class CardPresenter : ContentControl
    {
        public CardPresenter() : base()
        {
            DefaultStyleKey = typeof(CardPresenter);
        }

        public OverlayCard Card
        {
            get { return (OverlayCard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Card.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(OverlayCard), typeof(CardPresenter), new PropertyMetadata(null));
 #if __WPF__
        // TODO figure out hiding base member warning
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CardPresenter), new PropertyMetadata(new CornerRadius(0)));

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

        static CardPresenter()
        {
            if (!Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent(nameof(ContentControl), nameof(CornerRadius)))
            {
                CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CardPresenter), new PropertyMetadata(null));
            }
        }
#endif



        public Style TopBarStyle
        {
            get { return (Style)GetValue(TopBarStyleProperty); }
            set { SetValue(TopBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopBarStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopBarStyleProperty =
            DependencyProperty.Register("TopBarStyle", typeof(Style), typeof(CardPresenter), new PropertyMetadata(null));



        public Style SubtitleBarStyle
        {
            get { return (Style)GetValue(SubtitleBarStyleProperty); }
            set { SetValue(SubtitleBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubtitleBarStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtitleBarStyleProperty =
            DependencyProperty.Register("SubtitleBarStyle", typeof(Style), typeof(CardPresenter), new PropertyMetadata(null));




        public Style BottomBarStyle
        {
            get { return (Style)GetValue(BottomBarStyleProperty); }
            set { SetValue(BottomBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomBarStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomBarStyleProperty =
            DependencyProperty.Register("BottomBarStyle", typeof(Style), typeof(CardPresenter), new PropertyMetadata(null));



        public Style TitleTextStyle
        {
            get { return (Style)GetValue(TitleTextStyleProperty); }
            set { SetValue(TitleTextStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleTextStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTextStyleProperty =
            DependencyProperty.Register("TitleTextStyle", typeof(Style), typeof(CardPresenter), new PropertyMetadata(null));




        public Style SubtitleTextStyle
        {
            get { return (Style)GetValue(SubtitleTextStyleProperty); }
            set { SetValue(SubtitleTextStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubtitleTextStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtitleTextStyleProperty =
            DependencyProperty.Register("SubtitleTextStyle", typeof(Style), typeof(CardPresenter), new PropertyMetadata(null));



    }
}
