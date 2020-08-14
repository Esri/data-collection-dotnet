using Windows.UI.Xaml;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Cards
{
    public sealed partial class BookmarksCard
    {
        public BookmarksCard()
        {
            this.InitializeComponent();
        }

        public MainViewModel MainViewModel
        {
            get => (MainViewModel)GetValue(MainViewModelProperty);
            set => SetValue(MainViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for MainViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(BookmarksCard), new PropertyMetadata(null));
    }
}
