using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;

namespace ControlsTest
{
    [TestFixture]
    public class BasicTests
    {
        [TestCase(0,0)]
        [TestCase(300,300)]
        [TestCase(0,600)]
        [TestCase(600,0)]
        public void NoChildren(double width, double height)
        {
            ModernMapPanel panel = new ModernMapPanel();
            panel.Measure(new Size(width, height));
            panel.Arrange(new Rect(0,0, panel.DesiredSize.Width, panel.DesiredSize.Height));
        }

        [TestCase(MapRole.Accessory)]
        [TestCase(MapRole.Attribution)]
        [TestCase(MapRole.Card)]
        [TestCase(MapRole.CardAppendage)]
        [TestCase(MapRole.ContextCanvas)]
        [TestCase(MapRole.GeoView)]
        [TestCase(MapRole.ModalLightbox)]
        [TestCase(MapRole.Titlebar)]
        public void OneChild(MapRole mapRole)
        {
            Grid parentGrid = new Grid();
            ModernMapPanel panel = new ModernMapPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Stretch;
            panel.VerticalAlignment = VerticalAlignment.Stretch;
            Border onlyChild = new Border();
            panel.Children.Add(onlyChild);
            parentGrid.Children.Add(panel);
            parentGrid.Measure(new Size(300,300));
            parentGrid.Arrange(new Rect(0,0,panel.DesiredSize.Width, panel.DesiredSize.Height));

            ModernMapPanel.SetRole(onlyChild, mapRole);
            parentGrid.Measure(new Size(300,300));
            Assert.That(panel.DesiredSize.Width > 0);
            Assert.That(panel.DesiredSize.Height > 0);
            parentGrid.Arrange(new Rect(0,0,panel.DesiredSize.Width, panel.DesiredSize.Height));
        }
    }
}
