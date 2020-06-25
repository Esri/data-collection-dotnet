using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CustomControls
{
    public partial class FloatingMapOverlay : Control
    {
        public FloatingMapOverlay()
        {
            DefaultStyleKey = typeof(FloatingMapOverlay);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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

        /// <summary>
        /// Height of the navigation and command areas on the cards
        /// </summary>
        public double BarHeight { get; set; }

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

        #endregion 
    }
}
