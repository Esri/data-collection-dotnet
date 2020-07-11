using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


#if __WPF__
using System.Windows.Controls;
using System.Windows;
#endif

#if __UWP__
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Panels
{
    /// <summary>
    /// Lays out a map with accessories, including a 
    /// </summary>
    public class MapLayoutPanel : Panel
    {


        public double FloatingSheetWidthWhenExpanded
        {
            get { return (double)GetValue(FloatingSheetWidthWhenExpandedProperty); }
            set { SetValue(FloatingSheetWidthWhenExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FloatingSheetWidthWhenExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FloatingSheetWidthWhenExpandedProperty =
            DependencyProperty.Register(nameof(FloatingSheetWidthWhenExpanded), typeof(double), typeof(MapLayoutPanel), new PropertyMetadata(300.0));



        public double WidthBreakpoint
        {
            get { return (double)GetValue(WidthBreakpointProperty); }
            set { SetValue(WidthBreakpointProperty, value); }
        }

        public double PanelSpacing
        {
            get { return (double)GetValue(PanelSpacingProperty); }
            set { SetValue(PanelSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WidthBreakpoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthBreakpointProperty =
            DependencyProperty.Register(nameof(WidthBreakpoint), typeof(double), typeof(MapLayoutPanel), new PropertyMetadata(350.0));

        // Using a DependencyProperty as the backing store for PanelSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelSpacingProperty =
            DependencyProperty.Register(nameof(PanelSpacing), typeof(double), typeof(MapLayoutPanel), new PropertyMetadata(5.0));



        public double CollapsedPanelHeight
        {
            get { return (double)GetValue(CollapsedPanelHeightProperty); }
            set { SetValue(CollapsedPanelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CollapsedPanelHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapsedPanelHeightProperty =
            DependencyProperty.Register(nameof(CollapsedPanelHeight), typeof(double), typeof(MapLayoutPanel), new PropertyMetadata(300.0));



        public static LayoutRole GetLayoutRole(DependencyObject obj)
        {
            return (LayoutRole)obj?.GetValue(LayoutRoleProperty);
        }

        public static void SetLayoutRole(DependencyObject obj, LayoutRole value)
        {
            obj?.SetValue(LayoutRoleProperty, value);
        }

        // Using a DependencyProperty as the backing store for LayoutRole.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutRoleProperty =
            DependencyProperty.RegisterAttached(nameof(LayoutRole), typeof(LayoutRole), typeof(MapLayoutPanel), new PropertyMetadata(LayoutRole.BackgroundView));


        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Padding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(MapLayoutPanel), new PropertyMetadata(new Thickness(5)));


        public static AttachPosition GetAttachPosition(DependencyObject obj)
        {
            return (AttachPosition)obj.GetValue(AttachPositionProperty);
        }

        public static void SetAttachPosition(DependencyObject obj, AttachPosition value)
        {
            obj.SetValue(AttachPositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for AttachPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachPositionProperty =
            DependencyProperty.RegisterAttached(nameof(AttachPosition), typeof(AttachPosition), typeof(MapLayoutPanel), new PropertyMetadata(AttachPosition.Unspecified));

        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(MapLayoutPanel), new PropertyMetadata(false));

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach(var child in Children.OfType<UIElement>())
            {
                child.Measure(availableSize);
            }
            // Using default implementation for now
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // offsets used to arrange panels when multiple children of the same type present
            Dictionary<AttachPosition, double> offsets_x = new Dictionary<AttachPosition, double>();
            Dictionary<AttachPosition, double> offsets_y = new Dictionary<AttachPosition, double>();

            IsCollapsed = finalSize.Width < (double)GetValue(WidthBreakpointProperty);

            double totalPanelWidth = 0;
            // initial measurement round for use later
            foreach(var child in Children.OfType<UIElement>())
            {
                if (GetLayoutRole(child) == LayoutRole.FloatingSheet)
                {
                    totalPanelWidth += child.DesiredSize.Width;
                }
            }

            // NOTE: for now, to keep things simple, controls will be laid out horizontally or vertically only; no wrapping is applied on overflow

            foreach(var child in Children.OfType<UIElement>())
            {
                switch (GetLayoutRole(child))
                {
                    case LayoutRole.BackgroundView:
                    case LayoutRole.TransientOverlay:
                        child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                        break;
                    case LayoutRole.FloatingAccessory:
                        switch (GetAttachPosition(child))
                        {
                            case AttachPosition.BottomLeft:
                                if (!offsets_x.ContainsKey(AttachPosition.BottomLeft))
                                {
                                    offsets_x[AttachPosition.BottomLeft] = 0;
                                    offsets_y[AttachPosition.BottomLeft] = 0;
                                    // TODO - account for hidden panel
                                    if (IsCollapsed)
                                    {
                                        offsets_y[AttachPosition.BottomLeft] = (double)GetValue(CollapsedPanelHeightProperty);
                                    }
                                    else
                                    {
                                        offsets_x[AttachPosition.BottomLeft] = totalPanelWidth;
                                    }
                                }
                                child.Arrange(new Rect(offsets_x[AttachPosition.BottomLeft], finalSize.Height - child.DesiredSize.Height - offsets_y[AttachPosition.BottomLeft], child.DesiredSize.Width, child.DesiredSize.Height));
                                offsets_x[AttachPosition.BottomLeft] += child.DesiredSize.Width;
                                break;
                            case AttachPosition.BottomRight:
                                if (!offsets_x.ContainsKey(AttachPosition.BottomRight))
                                {
                                    offsets_x[AttachPosition.BottomRight] = 0;
                                    offsets_y[AttachPosition.BottomRight] = 0;
                                    // TODO - account for hidden panel
                                    if (IsCollapsed)
                                    {
                                        offsets_y[AttachPosition.BottomRight] = (double)GetValue(CollapsedPanelHeightProperty);
                                    }
                                }
                                child.Arrange(new Rect(finalSize.Width - offsets_x[AttachPosition.BottomRight] - child.DesiredSize.Width, finalSize.Height - child.DesiredSize.Height - offsets_y[AttachPosition.BottomRight], child.DesiredSize.Width, child.DesiredSize.Height));
                                offsets_x[AttachPosition.BottomRight] += child.DesiredSize.Width;
                                break;
                            case AttachPosition.TopLeft:
                                if (!offsets_x.ContainsKey(AttachPosition.TopLeft))
                                {
                                    offsets_x[AttachPosition.TopLeft] = 0;
                                    offsets_y[AttachPosition.TopLeft] = 0;
                                    // TODO - account for hidden panel
                                    if (IsCollapsed)
                                    {
                                        
                                    }
                                    else
                                    {
                                        offsets_x[AttachPosition.TopLeft] = totalPanelWidth;
                                    }
                                }
                                child.Arrange(new Rect(offsets_x[AttachPosition.TopLeft], offsets_y[AttachPosition.TopLeft], child.DesiredSize.Width, child.DesiredSize.Height));

                                offsets_x[AttachPosition.TopLeft] += child.DesiredSize.Width;
                                break;
                            case AttachPosition.TopRight:
                                if (!offsets_x.ContainsKey(AttachPosition.TopRight))
                                {
                                    offsets_x[AttachPosition.TopRight] = 0;
                                    offsets_y[AttachPosition.TopRight] = 0;
                                }
                                child.Arrange(new Rect(finalSize.Width - offsets_x[AttachPosition.TopRight] - child.DesiredSize.Width, offsets_y[AttachPosition.TopRight], child.DesiredSize.Width, child.DesiredSize.Height));

                                offsets_y[AttachPosition.TopRight] += child.DesiredSize.Height;
                                break;
                        }
                        break;
                    case LayoutRole.FloatingSheet:
                        FrameworkElement feChild = child as FrameworkElement;

                        if (IsCollapsed)
                        {
                            if (feChild != null)
                            {
                                feChild.HorizontalAlignment = HorizontalAlignment.Stretch;
                                feChild.Width = double.NaN;
                            }
                            var collapsedHeight = (double)GetValue(CollapsedPanelHeightProperty);
                            child.Arrange(new Rect(0, finalSize.Height - collapsedHeight - child.DesiredSize.Height, finalSize.Width, collapsedHeight));
                        }
                        else
                        {
                            if (feChild != null)
                            {
                                feChild.HorizontalAlignment = HorizontalAlignment.Stretch;
                                feChild.Width = (double)GetValue(FloatingSheetWidthWhenExpandedProperty);
                            }
                            
                            child.Arrange(new Rect(0, 0, child.DesiredSize.Width, finalSize.Height));
                        }
                        break;
                }
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
