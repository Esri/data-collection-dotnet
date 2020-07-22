using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if __UWP__
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
#endif

#if __WPF__
using System.Windows.Media;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.ComponentModel;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Cards
{
#if __WPF__
    [ContentProperty(nameof(Content))]
#elif __UWP__
    [ContentProperty(Name = nameof(Content))]
#endif
    public class OverlayCard : DependencyObject
    {
        public OverlayCard() : base()
        {
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
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(OverlayCard), new PropertyMetadata(string.Empty));

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
            set { SetValue(BottomAccessoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomAccessories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomAccessoriesProperty =
            DependencyProperty.Register(nameof(BottomAccessories), typeof(ObservableCollection<UIElement>), typeof(OverlayCard), new PropertyMetadata(null));

        public ObservableCollection<UIElement> TopAccessories
        {
            get { return (ObservableCollection<UIElement>)GetValue(TopAccessoriesProperty); }
            set { SetValue(TopAccessoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopAccessories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopAccessoriesProperty =
            DependencyProperty.Register(nameof(TopAccessories), typeof(ObservableCollection<UIElement>), typeof(OverlayCard), new PropertyMetadata(null));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(OverlayCard), new PropertyMetadata(null));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpIsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(OverlayCard), new PropertyMetadata(true, HandleIsOpenChanged));

        private static void HandleIsOpenChanged(DependencyObject overlayCard, DependencyPropertyChangedEventArgs dpcea)
        {
            (overlayCard as OverlayCard)?.OnStateChanged();
        }

        internal void OnStateChanged()
        {
            IsOpenChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> IsOpenChanged;
    }
}