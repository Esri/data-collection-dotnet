using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardBar.xaml
    /// </summary>
    public partial class CardBar
    {
        public CardBar()
        {
            InitializeComponent();
        }

        public object IconGeometry
        {
            get => GetValue(IconGeometryProperty);
            set => SetValue(IconGeometryProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register("IconGeometry", typeof(object), typeof(CardBar), new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CardBar), new PropertyMetadata(null));
        
        public CardBase OwningCard
        {
            get => (CardBase)GetValue(OwningCardProperty);
            set => SetValue(OwningCardProperty, value);
        }

        // Using a DependencyProperty as the backing store for OwningCard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwningCardProperty =
            DependencyProperty.Register("OwningCard", typeof(CardBase), typeof(CardBar), new PropertyMetadata(null));
        
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(CardBar), new PropertyMetadata(null));
        
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CardBar), new PropertyMetadata(null));
    }
}
