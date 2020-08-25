/*******************************************************************************
  * Copyright 2020 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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
    /// <summary>
    /// Layout panel that lays out elements according to their role in a mapping application.
    /// </summary>
    public class ModernMapPanel : Panel
    {
        public ModernMapPanel()
        {
            NavigationTitles = new ObservableCollection<string>();
        }

        /// <summary>
        /// Gets or sets the minimum width needed for the panel to show a full-width view.
        /// </summary>
        public double ExpandedWidthMinimum { get => (double)GetValue(ExpandedWidthMinimumProperty); set => SetValue(ExpandedWidthMinimumProperty, value); }

        /// <summary>
        /// Enables binding the minimum width needed for the panel to show a full-width view.
        /// </summary>
        public static readonly DependencyProperty ExpandedWidthMinimumProperty =
            DependencyProperty.Register(nameof(ExpandedWidthMinimum), typeof(double), typeof(ModernMapPanel), new PropertyMetadata(600.0));

        /// <summary>
        /// Gets or sets the ExpandedCardWidth.
        /// </summary>
        public double ExpandedCardWidth { get => (double)GetValue(ExpandedCardWidthProperty); set => SetValue(ExpandedCardWidthProperty, value); }

        /// <summary>
        /// Enables binding the width of the floating card when the panel is showing a full-width view.
        /// </summary>
        public static readonly DependencyProperty ExpandedCardWidthProperty =
            DependencyProperty.Register(nameof(ExpandedCardWidth), typeof(double), typeof(ModernMapPanel), new PropertyMetadata(350.0));

        /// <summary>
        /// Gets or sets the height of the floating card when the panel is showing a reduced-width view.
        /// </summary>
        public double CollapsedCardHeight { get => (double)GetValue(CollapsedCardHeightProperty); set => SetValue(CollapsedCardHeightProperty, value); }

        /// <summary>
        /// Enables binding the height of the floating card when the panel is showing a reduced-width view.
        /// </summary>
        public static readonly DependencyProperty CollapsedCardHeightProperty =
            DependencyProperty.Register(nameof(CollapsedCardHeight), typeof(double), typeof(ModernMapPanel), new PropertyMetadata(350.0));

        /// <summary>
        /// Returns true if the panel is showing a reduced-width view.
        /// </summary>
        public bool IsCollapsed { get => (bool)GetValue(IsCollapsedProperty); private set => SetValue(IsCollapsedProperty, value); }

        /// <summary>
        /// Enables binding to the width state of the panel, value is true if panel is showing a reduced-width view.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(ModernMapPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets the titles of cards that are eligible to be shown, but that aren't shown because another card is in the foreground.
        /// </summary>
        public ObservableCollection<string> NavigationTitles { get => (ObservableCollection<string>)GetValue(NavigationTitlesProperty); private set => SetValue(NavigationTitlesProperty, value); }

        /// <summary>
        /// Enables binding to the titles of cards that are eligible to be shown but that aren't shown because another card is in the foreground.
        /// </summary>
        public static readonly DependencyProperty NavigationTitlesProperty =
            DependencyProperty.Register(nameof(NavigationTitles), typeof(ObservableCollection<string>), typeof(ModernMapPanel), new PropertyMetadata(null));

        /// <summary>
        /// Gets the layout role of a child.
        /// </summary>
        public static MapRole GetRole(DependencyObject depObj) => (MapRole)depObj?.GetValue(RoleProperty);

        /// <summary>
        /// Sets the layout role of a child.
        /// </summary>
        public static void SetRole(DependencyObject depObj, MapRole value) => depObj?.SetValue(RoleProperty, value);

        /// <summary>
        /// Defines the role attached property for children.
        /// </summary>
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(MapRole), typeof(ModernMapPanel), new PropertyMetadata(MapRole.Accessory));

        /// <summary>
        /// Gets the title of a child.
        /// </summary>
        public static string GetTitle(DependencyObject depObj) => (string)depObj?.GetValue(TitleProperty);

        /// <summary>
        /// Sets the title of a child.
        /// </summary>
        public static void SetTitle(DependencyObject depObj, string value) => depObj?.SetValue(TitleProperty, value);

        /// <summary>
        /// Defines the title attached property for children.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(string), typeof(ModernMapPanel), new PropertyMetadata(null));

        /// <summary>
        /// Gets the topmost card that is eligible for display.
        /// </summary>
        public UIElement TopCard { get => (UIElement)GetValue(TopCardProperty); private set => SetValue(TopCardProperty, value); }

        /// <summary>
        /// Enables binding to the topmost card that is eligible for display.
        /// </summary>
        private static readonly DependencyProperty TopCardProperty =
            DependencyProperty.Register(nameof(TopCard), typeof(UIElement), typeof(ModernMapPanel), new PropertyMetadata(null));

        /// <summary>
        /// Implements the measure step for this panel.
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            // Update the IsCollapsed state
            IsCollapsed = availableSize.Width < ExpandedWidthMinimum;

            // Organize children elements by role
            UIElement titlebar = null;
            UIElement geoview = null;
            UIElement canvas = null;
            UIElement appendage = null;
            UIElement attribution = null;
            var cards = new List<UIElement>();
            var lightBoxes = new List<UIElement>();
            FrameworkElement bottomRightAccessory = null;
            FrameworkElement topRightAccessory = null;
            FrameworkElement topLeftAccessory = null;
            FrameworkElement bottomLeftAccessory = null;

            foreach (var element in Children)
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
                        case MapRole.CardAppendage:
                            appendage = child;
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
            if (TopCard != null)
            {
                foreach (var card in cards)
                {
                    if (card != TopCard)
                    {
                        card.Measure(new Size(0, 0));
                    }
                }
            }

            double titleBarBottomY = 0;

            // 1. Place titlebar
            if (titlebar != null)
            {
                titlebar.Measure(new Size(availableSize.Width, availableSize.Height));
                titleBarBottomY = titlebar.DesiredSize.Height;
            }
            // 2. Place map
            geoview?.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
            // 3. Place context overlay
            canvas?.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
            // 4. Place card appendage
            if (appendage != null && NavigationTitles.Any())
            {
                appendage.Measure(availableSize);
            }
            // 5. Place top card
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
            // Future enhancement: support multiple accessories in each position.
            bottomRightAccessory?.Measure(new Size(availableSize.Width - actualCardWidth, availableSize.Height - attrHeight));
            double bottomRightHeight = bottomRightAccessory?.DesiredSize.Height ?? 0;
            double bottomRightWidth = bottomRightAccessory?.DesiredSize.Width ?? 0;

            topRightAccessory?.Measure(new Size(availableSize.Width - actualCardWidth, availableSize.Height - attrHeight - bottomRightHeight));
            double topRightWidth = topRightAccessory?.DesiredSize.Width ?? 0;

            topLeftAccessory?.Measure(new Size(availableSize.Width - actualCardWidth - topRightWidth, availableSize.Height - attrHeight));
            double topLeftHeight = topLeftAccessory?.DesiredSize.Height ?? 0;
            bottomLeftAccessory?.Measure(new Size(availableSize.Width - actualCardWidth - bottomRightWidth, availableSize.Height - attrHeight - topLeftHeight));

            // 8. Place modal light box
            if (lightBoxes.Any())
            {
                foreach (var lb in lightBoxes)
                {
                    lb.Measure(new Size(availableSize.Width, availableSize.Height - titleBarBottomY));
                }
            }
            // Note: this panel should not be used in scroll viewers or any other context where size could be infinite.
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                return new Size(0, 0);
            }
            return new Size(availableSize.Width, availableSize.Height);
        }

        /// <summary>
        /// Implements the arrange step of the layout cycle.
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IsCollapsed = finalSize.Width < ExpandedWidthMinimum;

            UIElement titlebar = null;
            UIElement geoview = null;
            UIElement canvas = null;
            UIElement appendage = null;
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
                        case MapRole.CardAppendage:
                            appendage = child;
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
            geoview?.Arrange(new Rect(0, titleBarBottomY, finalSize.Width, finalSize.Height - titleBarBottomY));
            // 3. Place context overlay
            canvas?.Arrange(new Rect(0, titleBarBottomY, finalSize.Width, finalSize.Height - titleBarBottomY));
            // 4. Place card appendage
            cardTopY = titleBarBottomY;

            if (TopCard != null && (TopCard as CardBase)?.CardState == CardState.Minimized && IsCollapsed)
                cardTopY = finalSize.Height - CollapsedCardHeight;

            if (appendage != null && NavigationTitles.Any())
            {
                // Move card to accommodate appendage only if needed
                if (cardTopY < titleBarBottomY + appendage.DesiredSize.Height)
                {
                    cardTopY += appendage.DesiredSize.Height;
                }
                if (IsCollapsed)
                {
                    if ((TopCard as CardBase)?.CardState == CardState.Minimized)
                    {
                        appendage.Arrange(new Rect(0, cardTopY - appendage.DesiredSize.Height, finalSize.Width, appendage.DesiredSize.Height));
                    }
                    else
                    {
                        appendage.Arrange(new Rect(0, cardTopY - appendage.DesiredSize.Height, finalSize.Width, appendage.DesiredSize.Height));
                        mapVisibleAreaBottom = titleBarBottomY; // TODO maybe hide things in this state
                    }
                }
                else
                {
                    appendage.Arrange(new Rect(0, titleBarBottomY, ExpandedCardWidth, appendage.DesiredSize.Height));
                    mapVisibleAreaLeft = ExpandedCardWidth;
                }
            }
            else if (appendage != null)
            {
                appendage.Arrange(new Rect(0, 0, 0, 0));
            }
            // 5. Place top card
            if (TopCard != null)
            {
                if (IsCollapsed)
                {
                    TopCard.Arrange(new Rect(0, cardTopY, finalSize.Width, finalSize.Height - cardTopY));
                    mapVisibleAreaBottom = cardTopY - (appendage?.DesiredSize.Height ?? 0);
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
            topRightAccessory?.Arrange(new Rect(finalSize.Width - topRightAccessory.DesiredSize.Width,
                titleBarBottomY,
                topRightAccessory.DesiredSize.Width,
                topRightAccessory.DesiredSize.Height));

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

            // 8. Place modal light box
            if (lightBoxes.Any())
            {
                foreach (var lb in lightBoxes)
                {
                    lb.Arrange(new Rect(0, titleBarBottomY, finalSize.Width, finalSize.Height - titleBarBottomY));
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Applies rules to update the list of navigation titles and select the topmost visible card.
        /// </summary>
        /// <param name="cards">The cards<see cref="IReadOnlyCollection{UIElement}"/>.</param>
        private void UpdateCardModel(IReadOnlyCollection<UIElement> cards)
        {
            NavigationTitles.Clear();
            // Update visibility based on IsOpen if applicable
            foreach (var card in cards.OfType<CardBase>())
            {
                card.Visibility = card.IsOpen ? Visibility.Visible : Visibility.Collapsed;
                card.ParentPanel = this;
            }
            var visibleCards = cards.Where(card => card.Visibility == Visibility.Visible);
            var enumeratedCards = visibleCards as UIElement[] ?? visibleCards.ToArray();
            if (!enumeratedCards.Any())
            {
                TopCard = null;
                NavigationTitles.Clear();
            }
            else if (enumeratedCards.Length == 1)
            {
                NavigationTitles.Clear();
                TopCard = enumeratedCards[0];
            }
            else
            {
                TopCard = enumeratedCards.Last();
                for (int x = 0; x < enumeratedCards.Length - 1; x++)
                {
                    NavigationTitles.Add(GetTitle(enumeratedCards[x]));
                }
            }

            if (TopCard is CardBase cb)
            {
                ShouldEnableCardMaximize = IsCollapsed && cb.CardState == CardState.Minimized;
                ShouldEnableCardMinimize = IsCollapsed && cb.CardState == CardState.Maximized;
            }
        }

        /// <summary>
        /// Handles requesting layout changes when a child property changes.
        /// </summary>
        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // When card property changes refresh layout
            if (e.PropertyName == nameof(CardBase.IsOpen) || e.PropertyName == nameof(CardBase.CardState))
            {
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets whether cards should be able to be maximized. Cards are maximized by default in a full-width view and can't be collapsed.
        /// </summary>
        /// <remarks>
        /// Some layouts are awkward when cards can or can't be maximized, so this and the related dependency property allow the panel to weigh in.
        /// </remarks>
        public bool ShouldEnableCardMaximize { get => (bool)GetValue(ShouldEnableCardMaximizeProperty); set => SetValue(ShouldEnableCardMaximizeProperty, value); }

        /// <summary>
        /// Enables binding to the <see cref="ShouldEnableCardMaximize"/> property.
        /// </summary>
        public static readonly DependencyProperty ShouldEnableCardMaximizeProperty =
            DependencyProperty.Register(nameof(ShouldEnableCardMaximize), typeof(bool), typeof(ModernMapPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets whether cards should be able to be minimized. Cards are maximized by default in a full-width view and can't be collapsed.
        /// </summary>
        /// <remarks>
        /// Some layouts are awkward when cards can or can't be minimized, so this and the related dependency property allow the panel to weigh in.
        /// </remarks>
        public bool ShouldEnableCardMinimize { get => (bool)GetValue(ShouldEnableCardMinimizeProperty); set => SetValue(ShouldEnableCardMinimizeProperty, value); }

        /// <summary>
        /// Enables binding to the <see cref="ShouldEnableCardMaximize"/> property.
        /// </summary>
        public static readonly DependencyProperty ShouldEnableCardMinimizeProperty =
            DependencyProperty.Register(nameof(ShouldEnableCardMinimize), typeof(bool), typeof(ModernMapPanel), new PropertyMetadata(false));
    }
}
