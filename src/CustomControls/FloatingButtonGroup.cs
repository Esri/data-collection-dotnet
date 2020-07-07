using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace CustomControls
{
    [ContentProperty(Name = nameof(Elements))]
    public class ButtonGroup : DependencyObject
    {
        public ObservableCollection<UIElement> Elements { get => (ObservableCollection<UIElement>)GetValue(ElementsProperty); set => SetValue(ElementsProperty, value); }
        public Windows.UI.Xaml.Media.Brush Background { get => (Windows.UI.Xaml.Media.Brush)GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }

        public ButtonGroup()
        {
            Elements = new ObservableCollection<UIElement>();
        }

        public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(nameof(Elements), typeof(ObservableCollection<UIElement>), typeof(ButtonGroup), new PropertyMetadata(null));
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Windows.UI.Xaml.Media.Brush), typeof(ButtonGroup), new PropertyMetadata(null));
    }
}
