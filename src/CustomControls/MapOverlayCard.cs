using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace CustomControls
{
    public class MapOverlayCard : ContentControl
    {
        public MapOverlayCard() : base()
        {
            SetValue(ButtonsProperty, new ObservableCollection<Button>());
        }

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public string TertiarySubtitle { get => (string)GetValue(TertiarySubtitleProperty); set => SetValue(TertiarySubtitleProperty, value); }
        public bool UserCloseable { get => (bool)GetValue(UserCloseableProperty); set => SetValue(UserCloseableProperty, value); }
        public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public ObservableCollection<Button> Buttons { get => (ObservableCollection<Button>)GetValue(ButtonsProperty); set => SetValue(ButtonsProperty, value); }
        public System.Windows.Input.ICommand CloseCommand { get => (System.Windows.Input.ICommand)GetValue(CloseCommandProperty); set => SetValue(CloseCommandProperty, value); }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MapOverlayCard), new PropertyMetadata(null, HandlePropertyChanged));
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(MapOverlayCard), new PropertyMetadata(null, HandlePropertyChanged));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(MapOverlayCard), new PropertyMetadata(null, HandlePropertyChanged));
        public static readonly DependencyProperty TertiarySubtitleProperty = DependencyProperty.Register(nameof(TertiarySubtitle), typeof(string), typeof(MapOverlayCard), new PropertyMetadata(null, HandlePropertyChanged));
        public static readonly DependencyProperty UserCloseableProperty = DependencyProperty.Register(nameof(UserCloseable), typeof(bool), typeof(MapOverlayCard), new PropertyMetadata(true, HandlePropertyChanged));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(MapOverlayCard), new PropertyMetadata(true, HandlePropertyChanged));
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(nameof(Buttons), typeof(ObservableCollection<Button>), typeof(MapOverlayCard), new PropertyMetadata(new ObservableCollection<Button>()));

        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(nameof(CloseCommand), typeof(System.Windows.Input.ICommand), typeof(MapOverlayCard), new PropertyMetadata(null));

        public Action OnRefreshAction;

        private static void HandlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is MapOverlayCard card)
            {
                card.OnRefreshAction?.Invoke();
            }
        }
    }
}
