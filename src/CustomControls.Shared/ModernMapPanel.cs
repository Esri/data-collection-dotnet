using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
#if __WPF__
using System.Windows.Controls;
using System.Windows;
#elif __UWP__
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls
{
    public class ModernMapPanel : Panel
    {
        public ModernMapPanel()
        {
            NavigationTitles = new ObservableCollection<string>();
        }

        
        public double ExpandedWidthMinimum
        {
            get { return (double)GetValue(ExpandedWidthMinimumProperty); }
            set { SetValue(ExpandedWidthMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandedWidthMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandedWidthMinimumProperty =
            DependencyProperty.Register(nameof(ExpandedWidthMinimum), typeof(double), typeof(ModernMapPanel), new PropertyMetadata(600.0));

        public double ExpandedCardWidth
        {
            get { return (double)GetValue(ExpandedCardWidthProperty); }
            set { SetValue(ExpandedCardWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandedCardWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandedCardWidthProperty =
            DependencyProperty.Register(nameof(ExpandedCardWidth), typeof(double), typeof(ModernMapPanel), new PropertyMetadata(350.0));

        public double CollapsedCardHeight
        {
            get { return (double)GetValue(CollapsedCardHeightProperty); }
            set { SetValue(CollapsedCardHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CollapsedCardHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapsedCardHeightProperty =
            DependencyProperty.Register(nameof(CollapsedCardHeight), typeof(double), typeof(ModernMapPanel), new PropertyMetadata(350.0));

        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(ModernMapPanel), new PropertyMetadata(false));

        public ObservableCollection<string> NavigationTitles
        {
            get { return (ObservableCollection<string>)GetValue(NavigationTitlesProperty); }
            private set { SetValue(NavigationTitlesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationTitles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationTitlesProperty =
            DependencyProperty.Register(nameof(NavigationTitles), typeof(ObservableCollection<string>), typeof(ModernMapPanel), new PropertyMetadata(null));

        public static MapRole GetRole(DependencyObject depObj) => (MapRole)depObj?.GetValue(RoleProperty);

        public static void SetRole(DependencyObject depObj, MapRole value) => depObj?.SetValue(RoleProperty, value);

        // Using a DependencyProperty as the backing store for Role.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(MapRole), typeof(ModernMapPanel), new PropertyMetadata(MapRole.Accessory));

        public static string GetTitle(DependencyObject depObj) => (string)depObj?.GetValue(TitleProperty);

        public static void SetTitle(DependencyObject depObj, string value) => depObj?.SetValue(TitleProperty, value);

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(string), typeof(ModernMapPanel), new PropertyMetadata(null));

        public UIElement TopCard
        {
            get { return (UIElement)GetValue(TopCardProperty); }
            set { SetValue(TopCardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for uIElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopCardProperty =
            DependencyProperty.Register(nameof(TopCard), typeof(UIElement), typeof(ModernMapPanel), new PropertyMetadata(null));

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width < ExpandedWidthMinimum)
            {
                IsCollapsed = true;
            }
            UIElement titlebar = null;
            UIElement geoview = null;
            UIElement canvas = null;
            UIElement adorner = null;
            UIElement attribution = null;
            List<UIElement> cards = new List<UIElement>();
            List<UIElement> lightBoxes = new List<UIElement>();
            FrameworkElement bottomRightAccessory = null;
            FrameworkElement topRightAccessory = null;
            FrameworkElement topLeftAccessory = null;
            FrameworkElement bottomLeftAccessory = null;

            foreach (UIElement element in Children)
            {
                if (element is UIElement child)
                {
                    if (child is CardBase card)
                    {
                        card.PropertyChanged -= Child_PropertyChanged;
                        card.PropertyChanged += Child_PropertyChanged;
                    }

                    switch (GetRole(child))
                    {
                        case MapRole.Titlebar:
                            titlebar = child;
                            break;
                        case MapRole.GeoView:
                            geoview = child;
                            break;
                        case MapRole.ContextCanvas:
                            canvas = child;
                            break;
                        case MapRole.Attribution:
                            attribution = child;
                            break;
                        case MapRole.CardAdorner:
                            adorner = child;
                            break;
                        case MapRole.Card:
                            cards.Add(child);
                            break;
                        case MapRole.Accessory:
                            if (child is FrameworkElement fe)
                            {
                                if (fe.HorizontalAlignment == HorizontalAlignment.Right)
                                {
                                    if (fe.VerticalAlignment == VerticalAlignment.Top)
                                    {
                                        topRightAccessory = fe;
                                    }
                                    else
                                    {
                                        bottomRightAccessory = fe;
                                    }
                                }
                                else
                                {
                                    if (fe.VerticalAlignment == VerticalAlignment.Top)
                                    {
                                        topLeftAccessory = fe;
                                    }
                                    else
                                    {
                                        bottomLeftAccessory = fe;
                                    }
                                }
                            }

                            break;
                        case MapRole.ModalLightbox:
                            lightBoxes.Add(child);
                            break;
                    }
                }
            }

            double actualCardWidth = 0;
            UpdateCardModel(cards);

            if (TopCard != null && !IsCollapsed)
            {
                actualCardWidth = ExpandedCardWidth;
            }

            double titleBarBottomY = 0;

            // 1. Place titlebar
            if (titlebar != null)
            {
                titlebar.Measure(new Size(availableSize.Width, availableSize.Height));
                titleBarBottomY = titlebar.DesiredSize.Height;
            }
            // 2. Place map
            if (geoview != null)
            {
                geoview.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
            }
            // 3. Place context overlay
            if (canvas != null)
            {
                canvas.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
            }
            // 4. Place cardadorner
            if (adorner != null && NavigationTitles.Any())
            {
                adorner.Measure(availableSize);
            }
            // 5. Place topcard
            if (TopCard != null)
            {
                if (IsCollapsed)
                {
                    if (TopCard is CardBase cb && cb.CardState == CardState.Maximized)
                    {
                        TopCard.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
                    }
                    else
                    {
                        TopCard.Measure(new Size(availableSize.Width, CollapsedCardHeight));
                    }
                }
                else
                {
                    TopCard.Measure(new Size(ExpandedCardWidth, availableSize.Height - titleBarBottomY));
                }
            }
            // 6. Place attribution
            if (attribution != null)
            {
                attribution.Measure(availableSize);
                if (TopCard != null && !IsCollapsed)
                {
                    attribution.Measure(new Size(availableSize.Width - actualCardWidth, availableSize.Height));
                }
            }
            double attrHeight = attribution?.DesiredSize.Height ?? 0;
            // 7. Place accessories
            if (bottomRightAccessory != null)
            {
                bottomRightAccessory.Measure(new Size(availableSize.Width - actualCardWidth, availableSize.Height - attrHeight));
            }
            double bottomRightHeight = bottomRightAccessory?.DesiredSize.Height ?? 0;
            double bottomRightWidth = bottomRightAccessory?.DesiredSize.Width ?? 0;
            if (topRightAccessory != null)
            {
                topRightAccessory.Measure(new Size(availableSize.Width - actualCardWidth, availableSize.Height - attrHeight - bottomRightHeight));
            }
            double topRightWidth = topRightAccessory?.DesiredSize.Width ?? 0;
            if (topLeftAccessory != null)
            {
                topLeftAccessory.Measure(new Size(availableSize.Width - actualCardWidth- topRightWidth, availableSize.Height - attrHeight));
            }
            double topLeftHeight = topLeftAccessory?.DesiredSize.Height ?? 0;
            if (bottomLeftAccessory != null)
            {
                bottomLeftAccessory.Measure(new Size(availableSize.Width - actualCardWidth- bottomRightWidth, availableSize.Height - attrHeight - topLeftHeight));
            }

            // 8. Place modal lightbox
            if (lightBoxes.Any())
            {
                foreach (var lb in lightBoxes)
                {
                    lb.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IsCollapsed = finalSize.Width < ExpandedWidthMinimum;

            UIElement titlebar = null;
            UIElement geoview = null;
            UIElement canvas = null;
            UIElement adorner = null;
            UIElement attribution = null;
            List<UIElement> lightBoxes = new List<UIElement>();
            FrameworkElement bottomRightAccessory = null;
            FrameworkElement topRightAccessory = null;
            FrameworkElement topLeftAccessory = null;
            FrameworkElement bottomLeftAccessory = null;

            foreach (UIElement element in Children)
            {
                if (element is UIElement child)
                {
                    switch (GetRole(child))
                    {
                        case MapRole.Titlebar:
                            titlebar = child;
                            break;
                        case MapRole.GeoView:
                            geoview = child;
                            break;
                        case MapRole.ContextCanvas:
                            canvas = child;
                            break;
                        case MapRole.Attribution:
                            attribution = child;
                            break;
                        case MapRole.CardAdorner:
                            adorner = child;
                            break;
                        case MapRole.Card:
                            if (child != TopCard)
                            {
                                // TODO - determine if this is the best way
                                child.Arrange(new Rect(0, 0, 0, 0));
                            }

                            break;
                        case MapRole.Accessory:
                            if (child is FrameworkElement fe)
                            {
                                if (fe.HorizontalAlignment == HorizontalAlignment.Right)
                                {
                                    if (fe.VerticalAlignment == VerticalAlignment.Top)
                                    {
                                        topRightAccessory = fe;
                                    }
                                    else
                                    {
                                        bottomRightAccessory = fe;
                                    }
                                }
                                else
                                {
                                    if (fe.VerticalAlignment == VerticalAlignment.Top)
                                    {
                                        topLeftAccessory = fe;
                                    }
                                    else
                                    {
                                        bottomLeftAccessory = fe;
                                    }
                                }
                            }

                            break;
                        case MapRole.ModalLightbox:
                            lightBoxes.Add(child);
                            break;
                        default:
                            child.Arrange(new Rect(0, 0, 0, 0));
                            break;
                    }
                }
            }

            double titleBarBottomY = 0;
            double cardTopY = 0;
            double mapVisibleAreaBottom = finalSize.Height;
            double mapVisibleAreaLeft = 0;
            double cardBottomY = -1;

            // 1. Place titlebar
            if (titlebar != null)
            {
                titleBarBottomY = titlebar.DesiredSize.Height;
                titlebar.Arrange(new Rect(0, 0, finalSize.Width, titlebar.DesiredSize.Height));
            }
            // 2. Place map
            if (geoview != null)
            {
                geoview.Arrange(new Rect(0, titleBarBottomY, finalSize.Width, finalSize.Height - titleBarBottomY));
            }
            // 3. Place context overlay
            if (canvas != null)
            {
                canvas.Arrange(new Rect(0, titleBarBottomY, finalSize.Width, finalSize.Height - titleBarBottomY));
            }
            // 4. Place cardadorner
            cardTopY = titleBarBottomY;

            if (TopCard != null && (TopCard as CardBase)?.CardState == CardState.Minimized && IsCollapsed)
                cardTopY = finalSize.Height - CollapsedCardHeight;

            if (adorner != null && NavigationTitles.Any())
            {
                // Move card to accommodate adorner only if needed
                if (cardTopY < titleBarBottomY + adorner.DesiredSize.Height)
                {
                    cardTopY += adorner.DesiredSize.Height;
                }
                if (IsCollapsed)
                {
                    if ((TopCard as CardBase)?.CardState == CardState.Minimized)
                    { 
                        adorner.Arrange(new Rect(0, cardTopY - adorner.DesiredSize.Height, finalSize.Width, adorner.DesiredSize.Height));
                    }
                    else 
                    {
                        adorner.Arrange(new Rect(0, cardTopY - adorner.DesiredSize.Height, finalSize.Width, adorner.DesiredSize.Height));
                        mapVisibleAreaBottom = titleBarBottomY; // TODO maybe hide things in this state
                    }
                }
                else
                {
                    adorner.Arrange(new Rect(0, titleBarBottomY, ExpandedCardWidth, adorner.DesiredSize.Height));
                    mapVisibleAreaLeft = ExpandedCardWidth;
                }
            }
            else if (adorner != null)
            {
                adorner.Arrange(new Rect(0,0,0,0));
            }
            // 5. Place topcard
            if (TopCard != null)
            {
                if (IsCollapsed)
                {
                    TopCard.Arrange(new Rect(0, cardTopY, finalSize.Width, finalSize.Height - cardTopY));
                    mapVisibleAreaBottom = cardTopY - (adorner?.DesiredSize.Height ?? 0);
                }
                else
                {
                    mapVisibleAreaLeft = ExpandedCardWidth;
                    // Note: when in full width state, cards will always take the full height
                    cardBottomY = finalSize.Height;
                    TopCard.Arrange(new Rect(0, cardTopY, ExpandedCardWidth, finalSize.Height - cardTopY));
                }
            }
            double attrLeftX = 0;
            double attrLeftTopY = Math.Max(mapVisibleAreaBottom - (attribution?.DesiredSize.Height ?? 0), titleBarBottomY);
            // 6. Place attribution
            if (attribution != null)
            {
                if (IsCollapsed)
                {
                    attrLeftX = Math.Max(0, finalSize.Width - attribution.DesiredSize.Width);
                    attribution.Arrange(new Rect(attrLeftX, attrLeftTopY, finalSize.Width - attrLeftX, attribution.DesiredSize.Height));
                    attrLeftTopY = mapVisibleAreaBottom - attribution.DesiredSize.Height;
                }
                else
                {
                    attrLeftX = Math.Max(0, finalSize.Width - attribution.DesiredSize.Width);
                    // expand attribution under top card if full height not taken
                    if (attrLeftX < mapVisibleAreaLeft && TopCard != null && cardBottomY > 0 && cardBottomY > finalSize.Height - attribution.DesiredSize.Height)
                    {
                        attrLeftX = mapVisibleAreaLeft;
                    }
                    attribution.Arrange(new Rect(attrLeftX, attrLeftTopY, finalSize.Width - attrLeftX, attribution.DesiredSize.Height));
                }
            }
            // 7. Place accessories
            if (bottomRightAccessory != null)
            {
                double leftEdge = Math.Max(0, finalSize.Width - bottomRightAccessory.DesiredSize.Width);
                double bottomEdge = Math.Min(finalSize.Height - bottomRightAccessory.DesiredSize.Height, mapVisibleAreaBottom - attribution.DesiredSize.Height);
                bottomRightAccessory.Arrange(new Rect(Math.Max(finalSize.Width - bottomRightAccessory.DesiredSize.Width, leftEdge), 
                                                      Math.Max(0, bottomEdge - bottomRightAccessory.DesiredSize.Height),
                                                      Math.Min(finalSize.Width - bottomRightAccessory.DesiredSize.Width, finalSize.Width - leftEdge),
                                                      bottomRightAccessory.DesiredSize.Height));
            }
            if (topRightAccessory != null)
            {
                topRightAccessory.Arrange(new Rect(finalSize.Width - topRightAccessory.DesiredSize.Width,
                                          titleBarBottomY,
                                          topRightAccessory.DesiredSize.Width,
                                          topRightAccessory.DesiredSize.Height));
            }
            if (topLeftAccessory != null)
            {
                double leftEdge = 0;
                if (TopCard != null && !IsCollapsed)
                    leftEdge = ExpandedCardWidth;
                topLeftAccessory.Arrange(new Rect(leftEdge, titleBarBottomY, topLeftAccessory.DesiredSize.Width, topLeftAccessory.DesiredSize.Height));
            }
            if (bottomLeftAccessory != null)
            {
                double leftEdge = 0;
                double bottomOffset = 0;
                if (TopCard != null && !IsCollapsed && attrLeftTopY - cardBottomY < bottomLeftAccessory.DesiredSize.Height)
                {
                    leftEdge = ExpandedCardWidth;
                }
                // bump accessory above attribution if space needed
                if (finalSize.Width - leftEdge - attrLeftX < bottomLeftAccessory.DesiredSize.Width)
                {
                    bottomOffset = attrLeftTopY;
                }
                bottomLeftAccessory.Arrange(new Rect(leftEdge, finalSize.Height - bottomOffset - bottomLeftAccessory.DesiredSize.Height, 
                    bottomLeftAccessory.DesiredSize.Width, bottomLeftAccessory.DesiredSize.Height));
            }

            // 8. Place modal lightbox
            if (lightBoxes.Any())
            {
                foreach (var lb in lightBoxes)
                {
                    lb.Arrange(new Rect(0, titleBarBottomY, finalSize.Width, finalSize.Height - titleBarBottomY));
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        private void UpdateCardModel(List<UIElement> cards)
        {
            NavigationTitles.Clear();
            // Update visibility based on IsOpen if applicable
            foreach(var card in cards.OfType<CardBase>())
            {
                card.Visibility = card.IsOpen ? Visibility.Visible : Visibility.Collapsed;
                card.ParentPanel = this;
            }
            var visibleCards = cards.Where(card => card.Visibility == Visibility.Visible);
            if (!visibleCards.Any())
            {
                TopCard = null;
                NavigationTitles.Clear();
            }
            else if (visibleCards.Count() == 1)
            {
                NavigationTitles.Clear();
                TopCard = visibleCards.First();
            }
            else
            {
                TopCard = visibleCards.Last();
                for (int x = 0; x < visibleCards.Count() - 1; x++)
                {
                    NavigationTitles.Add(GetTitle(visibleCards.ElementAt(x)));
                }
            }

            if (TopCard is CardBase cb)
            {
                ShouldEnableCardMaximize = IsCollapsed && cb.CardState == CardState.Minimized;
                ShouldEnableCardMinimize = IsCollapsed && cb.CardState == CardState.Maximized;
            }
        }
        
        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // When card property changes (note: only property currently is IsOpen), refresh layout
            if (e.PropertyName == nameof(CardBase.IsOpen) || e.PropertyName == nameof(CardBase.CardState))
            {
                InvalidateMeasure();
            }
        }

        public bool ShouldEnableCardMaximize
        {
            get { return (bool)GetValue(ShouldEnableCardMaximizeProperty); }
            set { SetValue(ShouldEnableCardMaximizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShouldEnableCardMaximize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShouldEnableCardMaximizeProperty =
            DependencyProperty.Register(nameof(ShouldEnableCardMaximize), typeof(bool), typeof(ModernMapPanel), new PropertyMetadata(false));



        public bool ShouldEnableCardMinimize
        {
            get { return (bool)GetValue(ShouldEnableCardMinimizeProperty); }
            set { SetValue(ShouldEnableCardMinimizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShouldEnableCardMinimize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShouldEnableCardMinimizeProperty =
            DependencyProperty.Register(nameof(ShouldEnableCardMinimize), typeof(bool), typeof(ModernMapPanel), new PropertyMetadata(false));
    }
}
