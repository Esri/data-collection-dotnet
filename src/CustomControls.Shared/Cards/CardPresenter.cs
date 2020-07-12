using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;


#if __UWP__
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
#elif __WPF__
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Cards
{
    public class CardPresenter : ContentControl
    {
        public CardPresenter() : base()
        {
            DefaultStyleKey = typeof(CardPresenter);
            Cards = new ObservableCollection<OverlayCard>();
            NavigationStackCards = new ObservableCollection<OverlayCard>();

            Cards.CollectionChanged += Cards_CollectionChanged;
            this.Unloaded += CardPresenter_Unloaded;
        }

        private void CardPresenter_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach(var card in Cards)
            {
                card.IsEnabledChanged -= Item_IsEnabledChanged;
            }
            Cards.CollectionChanged -= Cards_CollectionChanged;
            // TODO - is this needed?
            this.Unloaded -= CardPresenter_Unloaded;
        }

        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems?.Count != null && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems.OfType<OverlayCard>())
                {
                    item.IsEnabledChanged -= Item_IsEnabledChanged;
                }

            }
            if (e.NewItems?.Count != null && e.NewItems.Count > 0)
            {
                foreach (var item in e.NewItems.OfType<OverlayCard>())
                {
                    item.IsEnabledChanged += Item_IsEnabledChanged;
                }
            }
            UpdateProperties();
        }

        private void Item_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            TopChild = Cards.Where(card => card.IsEnabled).LastOrDefault();
            var visibleCards = Cards.Where(card => card.IsEnabled);
            var navCards = visibleCards.Take(visibleCards.Count() - 1);

            // TODO - be smarter about this.
            NavigationStackCards.Clear();
            foreach(var card in navCards)
            {
                NavigationStackCards.Add(card);
            }

            PresenterVisibility = TopChild != null ? Visibility.Visible : Visibility.Collapsed;
            NavigationVisibility = NavigationStackCards.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        public ObservableCollection<OverlayCard> Cards
        {
            get { return (ObservableCollection<OverlayCard>)GetValue(CardsProperty); }
            set { SetValue(CardsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cards.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardsProperty =
            DependencyProperty.Register("Cards", typeof(ObservableCollection<OverlayCard>), typeof(CardPresenter), new PropertyMetadata(new ObservableCollection<OverlayCard>()));

        public OverlayCard TopChild
        {
            get { return (OverlayCard)GetValue(TopChildProperty); }
            set { SetValue(TopChildProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopChild.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopChildProperty =
            DependencyProperty.Register("TopChild", typeof(OverlayCard), typeof(CardPresenter), new PropertyMetadata(null));

        public ObservableCollection<OverlayCard> NavigationStackCards
        {
            get { return (ObservableCollection<OverlayCard>)GetValue(NavigationStackCardsProperty); }
            set { SetValue(NavigationStackCardsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationStackCards.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationStackCardsProperty =
            DependencyProperty.Register("NavigationStackCards", typeof(ObservableCollection<OverlayCard>), typeof(CardPresenter), new PropertyMetadata(new ObservableCollection<OverlayCard>()));

        public Visibility PresenterVisibility
        {
            get { return (Visibility)GetValue(PresenterVisibilityProperty); }
            set { SetValue(PresenterVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresenterVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresenterVisibilityProperty =
            DependencyProperty.Register("PresenterVisibility", typeof(Visibility), typeof(CardPresenter), new PropertyMetadata(Visibility.Collapsed));



        public Visibility NavigationVisibility
        {
            get { return (Visibility)GetValue(NavigationVisibilityProperty); }
            set { SetValue(NavigationVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationVisibilityProperty =
            DependencyProperty.Register("NavigationVisibility", typeof(Visibility), typeof(CardPresenter), new PropertyMetadata(Visibility.Collapsed));



        public Style NavigationBarStyle
        {
            get { return (Style)GetValue(NavigationBarStyleProperty); }
            set { SetValue(NavigationBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationBarStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationBarStyleProperty =
            DependencyProperty.Register("NavigationBarStyle", typeof(Style), typeof(CardPresenter), new PropertyMetadata(null));
    }
}
