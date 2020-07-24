using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class CardDeck : Control
    {
        public CardDeck()
        {
            Cards = new ObservableCollection<OverlayCard>();
            DefaultStyleKey = typeof(CardDeck);

            NavigationCards = new List<OverlayCard>();

            Cards.CollectionChanged += Cards_CollectionChanged;
        }

        private void UpdateProperties()
        {
            TopCard = Cards.Where(card => card.IsOpen).LastOrDefault();
            NavigationCards = Cards.Where(card => card.IsOpen && card != TopCard).ToList();

            ComputedVisibility = TopCard == null ? Visibility.Collapsed : Visibility.Visible;
            ComputedNavigationVisibility = NavigationCards.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

            Visibility = TopCard == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach(var item in e.NewItems.OfType<OverlayCard>())
                {
                    item.IsOpenChanged += Item_IsOpenChanged;
                }
            }
            
            if (e.OldItems != null)
            {
                foreach(var item in e.OldItems.OfType<OverlayCard>())
                {
                    item.IsOpenChanged -= Item_IsOpenChanged;
                }
            }
            UpdateProperties();
        }

        private void Item_IsOpenChanged(object sender, EventArgs e)
        {
            UpdateProperties();
        }

        public ObservableCollection<OverlayCard> Cards
        {
            get { return (ObservableCollection<OverlayCard>)GetValue(CardsProperty); }
            set { SetValue(CardsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cards.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardsProperty =
            DependencyProperty.Register("Cards", typeof(ObservableCollection<OverlayCard>), typeof(CardDeck), new PropertyMetadata(null));

        public OverlayCard TopCard
        {
            get { return (OverlayCard)GetValue(TopCardProperty); }
            set { SetValue(TopCardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopCard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopCardProperty =
            DependencyProperty.Register("TopCard", typeof(OverlayCard), typeof(CardDeck), new PropertyMetadata(null));

        public List<OverlayCard> NavigationCards
        {
            get { return (List<OverlayCard>)GetValue(NavigationCardsProperty); }
            set { SetValue(NavigationCardsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationCards.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationCardsProperty =
            DependencyProperty.Register("NavigationCards", typeof(List<OverlayCard>), typeof(CardDeck), new PropertyMetadata(null));




        public DataTemplate NavigationItemTemplate
        {
            get { return (DataTemplate)GetValue(NavigationItemTemplateProperty); }
            set { SetValue(NavigationItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationItemTemplateProperty =
            DependencyProperty.Register("NavigationItemTemplate", typeof(DataTemplate), typeof(CardDeck), new PropertyMetadata(null));




        public Style NavigationContainerStyle
        {
            get { return (Style)GetValue(NavigationContainerStyleProperty); }
            set { SetValue(NavigationContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationContainerStyleProperty =
            DependencyProperty.Register("NavigationContainerStyle", typeof(Style), typeof(CardDeck), new PropertyMetadata(null));




        public Style CardPresenterStyle
        {
            get { return (Style)GetValue(CardPresenterStyleProperty); }
            set { SetValue(CardPresenterStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardPresenterStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardPresenterStyleProperty =
            DependencyProperty.Register("CardPresenterStyle", typeof(Style), typeof(CardDeck), new PropertyMetadata(null));




        public CornerRadius CardCornerRadius
        {
            get { return (CornerRadius)GetValue(CardCornerRadiusProperty); }
            set { SetValue(CardCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardCornerRadiusProperty =
            DependencyProperty.Register("CardCornerRadius", typeof(CornerRadius), typeof(CardDeck), new PropertyMetadata(new CornerRadius(0)));




        public Visibility ComputedVisibility
        {
            get { return (Visibility)GetValue(ComputedVisibilityProperty); }
            set { SetValue(ComputedVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComputedVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputedVisibilityProperty =
            DependencyProperty.Register("ComputedVisibility", typeof(Visibility), typeof(CardDeck), new PropertyMetadata(Visibility.Collapsed));




        public Visibility ComputedNavigationVisibility
        {
            get { return (Visibility)GetValue(ComputedNavigationVisibilityProperty); }
            set { SetValue(ComputedNavigationVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComputedNavigationVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputedNavigationVisibilityProperty =
            DependencyProperty.Register("ComputedNavigationVisibility", typeof(Visibility), typeof(CardDeck), new PropertyMetadata(Visibility.Collapsed));


    }
}
