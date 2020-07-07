using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CustomControls
{
    public partial class FloatingMapOverlay : ContentControl
    {
        public FloatingMapOverlay()
        {
            DefaultStyleKey = typeof(FloatingMapOverlay);
            SetValue(CardsProperty, new ObservableCollection<MapOverlayCard>());
            SetValue(PrimaryActionButtonsProperty, new ObservableCollection<Button>());
            AccessoryButtons = new ObservableCollection<ButtonGroup>();
            ViewModel.Cards = Cards;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DataContext = ViewModel;
        }

        #region plain properties
        public double CardWidth { get => (double)GetValue(CardWidthProperty); set => SetValue(CardWidthProperty, value); }
        public AttachPosition CardAttachPosition { get => (AttachPosition)GetValue(CardAttachPositionProperty); set => SetValue(CardAttachPositionProperty, value); }
        public Thickness CardMargin { get => (Thickness)GetValue(CardMarginProperty); set => SetValue(CardMarginProperty, value); }
        public double MaxHeightWhenDockedToBottom { get => (double)GetValue(MaxHeightWhenDockedToBottomProperty); set => SetValue(MaxHeightWhenDockedToBottomProperty, value); }
        public double MinWidthToUndock { get => (double)GetValue(MinWidthToUndockProperty); set => SetValue(MinWidthToUndockProperty, value); }

        public Brush CardBackgroundBrush { get => (Brush)GetValue(CardBackgroundBrushProperty); set => SetValue(CardBackgroundBrushProperty, value); }
        public Brush CardBorderBrush { get => (Brush)GetValue(CardBorderBrushProperty); set => SetValue(CardBorderBrushProperty, value); }
        public Thickness CardBorderThickness { get => (Thickness)GetValue(CardBorderThicknessProperty); set => SetValue(CardBorderThicknessProperty, value); }
        public Brush CardSecondaryBackgroundBrush { get => (Brush)GetValue(CardSecondaryBackgroundBrushProperty); set => SetValue(CardSecondaryBackgroundBrushProperty, value); }

        public Thickness AccessoryMargin { get => (Thickness)GetValue(AccessoryMarginProperty); set => SetValue(AccessoryMarginProperty, value); }

        public Thickness CardElementPadding { get => (Thickness)GetValue(CardElementPaddingProperty); set => SetValue(CardElementPaddingProperty, value); }

        public Visibility CardNavigationAreaVisibility { get => (Visibility)GetValue(CardNavigationAreaVisibilityProperty); set => SetValue(CardNavigationAreaVisibilityProperty, value); }
        public Visibility CardCommandAreaVisibility { get => (Visibility)GetValue(CardCommandAreaVisibilityProperty); set => SetValue(CardCommandAreaVisibilityProperty, value); }

        public GeoView GeoView { get => (GeoView)GetValue(GeoViewProperty); set => SetValue(GeoViewProperty, value); }

        public FloatingMapOverlayViewModel ViewModel { get => (FloatingMapOverlayViewModel)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

        /// <summary>
        /// Height of the navigation and command areas on the cards
        /// </summary>
        public double BarHeight { get => (double)GetValue(BarHeightProperty); set => SetValue(BarHeightProperty, value); }

        public ObservableCollection<MapOverlayCard> Cards { get => (ObservableCollection<MapOverlayCard>)GetValue(CardsProperty); set => SetValue(CardsProperty, value); }

        public UIElement CloseButtonContent { get => (UIElement)GetValue(CloseButtonContentProperty); set => SetValue(CloseButtonContentProperty, value); }

        public ObservableCollection<Button> PrimaryActionButtons { get => (ObservableCollection<Button>)GetValue(PrimaryActionButtonsProperty); set => SetValue(PrimaryActionButtonsProperty, value); }

        public ObservableCollection<ButtonGroup> AccessoryButtons { get => (ObservableCollection<ButtonGroup>)GetValue(AccessoryButtonsProperty); set => SetValue(AccessoryButtonsProperty, value); }

        #endregion

        #region dependency properties
        public static readonly DependencyProperty CardWidthProperty = DependencyProperty.Register(nameof(CardWidth), typeof(double), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardAttachPositionProperty = DependencyProperty.Register(nameof(CardAttachPosition), typeof(AttachPosition), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardMarginProperty = DependencyProperty.Register(nameof(CardMargin), typeof(Thickness), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty MaxHeightWhenDockedToBottomProperty = DependencyProperty.Register(nameof(MaxHeightWhenDockedToBottom), typeof(double), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty MinWidthToUndockProperty = DependencyProperty.Register(nameof(MinWidthToUndock), typeof(double), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardBackgroundBrushProperty = DependencyProperty.Register(nameof(CardBackgroundBrush), typeof(Brush), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardBorderBrushProperty = DependencyProperty.Register(nameof(CardBorderBrush), typeof(Brush), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardBorderThicknessProperty = DependencyProperty.Register(nameof(CardBorderThickness), typeof(Thickness), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardSecondaryBackgroundBrushProperty = DependencyProperty.Register(nameof(CardSecondaryBackgroundBrush), typeof(Brush), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty AccessoryMarginProperty = DependencyProperty.Register(nameof(AccessoryMargin), typeof(Thickness), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardElementPaddingProperty = DependencyProperty.Register(nameof(CardElementPadding), typeof(Thickness), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardNavigationAreaVisibilityProperty = DependencyProperty.Register(nameof(CardNavigationAreaVisibility), typeof(Visibility), typeof(FloatingMapOverlay), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty CardCommandAreaVisibilityProperty = DependencyProperty.Register(nameof(CardCommandAreaVisibility), typeof(Visibility), typeof(FloatingMapOverlay), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty BarHeightProperty = DependencyProperty.Register(nameof(BarHeight), typeof(double), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty GeoViewProperty = DependencyProperty.Register(nameof(GeoView), typeof(GeoView), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty CardsProperty = DependencyProperty.Register(nameof(Cards), typeof(ObservableCollection<MapOverlayCard>), typeof(FloatingMapOverlay), new PropertyMetadata(new ObservableCollection<MapOverlayCard>()));
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(FloatingMapOverlayViewModel), typeof(FloatingMapOverlay), new PropertyMetadata(new FloatingMapOverlayViewModel()));
        public static readonly DependencyProperty CloseButtonContentProperty = DependencyProperty.Register(nameof(CloseButtonContent), typeof(UIElement), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        public static readonly DependencyProperty PrimaryActionButtonsProperty = DependencyProperty.Register(nameof(PrimaryActionButtons), typeof(ObservableCollection<Button>), typeof(FloatingMapOverlay), new PropertyMetadata(null));

        public static readonly DependencyProperty AccessoryButtonsProperty = DependencyProperty.Register(nameof(AccessoryButtons), typeof(ObservableCollection<ButtonGroup>), typeof(FloatingMapOverlay), new PropertyMetadata(null));
        #endregion
    }

    public class FloatingMapOverlayViewModel : INotifyPropertyChanged
    {
        public Visibility CardVisibility
        {
            get => Cards?.Where(CardPredicate)?.Any() == true ? Visibility.Visible : Visibility.Collapsed;
        }
        public Visibility IsNavigationVisible
        {
            get => Cards?.Where(CardPredicate)?.Count() > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public MapOverlayCard TopCard
        {
            get => Cards?.Where(CardPredicate)?.LastOrDefault();
        }

        private ObservableCollection<MapOverlayCard> _cards;

        public string NavigationTitle
        {
            get
            {
                return string.Join(" < ", Cards?.Where(CardPredicate).SkipLast(1).Select(card => card.Title));
            }
        }

        public ObservableCollection<MapOverlayCard> Cards
        {
            get => _cards;
            set
            {
                if (_cards != value)
                {
                    if (_cards != null)
                    {
                        _cards.CollectionChanged -= _cards_CollectionChanged;
                    }

                    _cards = value;

                    UpdateProperties();

                    _cards.CollectionChanged += _cards_CollectionChanged;
                }
            }
        }

        private void _cards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateProperties();

            if (e.OldItems?.Count > 0)
            {
                foreach (var oldCard in e.OldItems.OfType<MapOverlayCard>())
                {
                    oldCard.OnRefreshAction = null;
                }
            }
            
            if (e.NewItems?.Count > 0)
            {
                foreach (var newCard in e.NewItems.OfType<MapOverlayCard>())
                {
                    newCard.OnRefreshAction = UpdateProperties;
                }
            }
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Cards));
            OnPropertyChanged(nameof(NavigationTitle));
            OnPropertyChanged(nameof(TopCard));
            OnPropertyChanged(nameof(IsNavigationVisible));
            OnPropertyChanged(nameof(CardVisibility));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns true if card should be visible
        /// </summary>
        /// <param name="cardToCheck"></param>
        /// <returns></returns>
        private bool CardPredicate(MapOverlayCard cardToCheck)
        {
            return cardToCheck.IsOpen && cardToCheck.Visibility != Visibility.Collapsed;
        }
    }
}
