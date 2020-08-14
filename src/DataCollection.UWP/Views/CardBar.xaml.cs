using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    public sealed partial class CardBar : UserControl
    {
        public CardBar()
        {
            this.InitializeComponent();
        }
         public string IconGeometry
        {
            get { return (string)GetValue(IconGeometryProperty); }
            set { SetValue(IconGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register("IconGeometry", typeof(string), typeof(CardBar), new PropertyMetadata(null));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CardBar), new PropertyMetadata(null));




        public CardBase OwningCard
        {
            get { return (CardBase)GetValue(OwningCardProperty); }
            set { SetValue(OwningCardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OwningCard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwningCardProperty =
            DependencyProperty.Register("OwningCard", typeof(CardBase), typeof(CardBar), new PropertyMetadata(null));



        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(CardBar), new PropertyMetadata(null));




        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CardBar), new PropertyMetadata(null));


    }
}
