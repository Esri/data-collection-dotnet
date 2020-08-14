using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
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

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardBar.xaml
    /// </summary>
    public partial class CardBar : UserControl
    {
        public CardBar()
        {
            InitializeComponent();
        }

        public object IconGeometry
        {
            get { return (object)GetValue(IconGeometryProperty); }
            set { SetValue(IconGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register("IconGeometry", typeof(object), typeof(CardBar), new PropertyMetadata(null));



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
