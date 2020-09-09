using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ControlsTest
{
    [TestFixture]
    public class RenderTests
    {
        /// <summary>
        /// Tests verify that for various sizes, the panel arranges the titlebar and map properly.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [TestCase(50,100)]
        [TestCase(100,100)]
        [TestCase(300,300)]
        public void TitleBarOnly(int width, int height)
        {
            var titlebarHeight = 25;
            Grid parentGrid = new Grid();
            ModernMapPanel panel = new ModernMapPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Stretch;
            panel.VerticalAlignment = VerticalAlignment.Stretch;

            Border titleBorder = new Border();
            titleBorder.Background = new SolidColorBrush(Colors.Red);
            titleBorder.Height = titlebarHeight;
            Border geoviewBorder = new Border();
            geoviewBorder.Background = new SolidColorBrush(Colors.Blue);

            ModernMapPanel.SetRole(titleBorder, MapRole.Titlebar);
            ModernMapPanel.SetRole(geoviewBorder, MapRole.GeoView);

            parentGrid.Children.Add(panel);

            panel.Children.Add(titleBorder);
            panel.Children.Add(geoviewBorder);

            panel.Measure(new Size(width, height));
            panel.Arrange(new Rect(0,0,panel.DesiredSize.Width, panel.DesiredSize.Height));

            // Render
            var bmpx = ControlToBitmap(panel, width, height);

            Assert.AreEqual(bmpx.GetPixel(0,0), System.Drawing.Color.FromArgb(255, 255, 0, 0));
            Assert.AreEqual(bmpx.GetPixel(0, height - 1), System.Drawing.Color.FromArgb(255, 0, 0, 255));
            Assert.AreEqual(bmpx.GetPixel(width / 2, 0), System.Drawing.Color.FromArgb(255, 255, 0, 0));
            Assert.AreEqual(bmpx.GetPixel(width / 2, titlebarHeight / 2), System.Drawing.Color.FromArgb(255, 255, 0, 0));
            Assert.AreEqual(bmpx.GetPixel(width / 2, titlebarHeight + 1), System.Drawing.Color.FromArgb(255, 0, 0, 255));
        }

        [TestCase(1,1)]
        [TestCase(100,100)]
        [TestCase(50,100)]
        [TestCase(300,300)]
        [TestCase(30, 100)]
        [TestCase(100,30)]
        public void AccessoryPositioning(int width, int height)
        {
            var titlebarHeight = 50;
            var accessoryWidth = 50;
            var accessoryHeight = 100;
           
            var mapColor = System.Drawing.Color.FromArgb(255, 0, 0, 255);
            var titleColor = System.Drawing.Color.FromArgb(255, 0, 255, 0);
            var accessoryColor = System.Drawing.Color.FromArgb(255, 255, 0, 0);

            var container = new Border();
            container.Background = System.Drawing.Color.Black.AsBrush();

            ModernMapPanel panel = new ModernMapPanel();
            container.Child = panel;

            var titlebar = new Border();
            titlebar.Background = titleColor.AsBrush();
            titlebar.Height = titlebarHeight;
            ModernMapPanel.SetRole(titlebar, MapRole.Titlebar);
            panel.Children.Add(titlebar);

            var geoview = new Border();
            geoview.Background = mapColor.AsBrush();
            ModernMapPanel.SetRole(geoview, MapRole.GeoView);
            panel.Children.Add(geoview);

             var accessory = new Border();
            accessory.Background = accessoryColor.AsBrush();
            accessory.Width = accessoryWidth;
            accessory.Height = accessoryHeight;
            accessory.HorizontalAlignment = HorizontalAlignment.Right;
            accessory.VerticalAlignment = VerticalAlignment.Top;
            ModernMapPanel.SetRole(accessory, MapRole.Accessory);
            panel.Children.Add(accessory);

            // Render panel
            container.Measure(new Size(width, height));
            container.Arrange(new Rect(0, 0, container.DesiredSize.Width, container.DesiredSize.Height));
            var bmp = ControlToBitmap(container, width, height);
            // title bar is at top and sized correctly
            Assert.AreEqual(titleColor, bmp.GetPixel(0, Math.Min(height - 1, titlebarHeight - 1)));
            // title bar fills width
            Assert.AreEqual(titleColor, bmp.GetPixel(width - 1, Math.Min(height - 1, titlebarHeight - 1)));
            if (height > titlebarHeight && width > accessoryWidth)
            {
                // bottom left corner is map color
                Assert.AreEqual(mapColor, bmp.GetPixel(0, height-1));
                if (height > titlebarHeight + accessoryHeight)
                {
                    // bottom right corner is map color
                    Assert.AreEqual(mapColor, bmp.GetPixel(width - 1, height - 1));
                }
            }
            // accessory is on the right
            if (height > titlebarHeight)
            {
                // Accessory starts immediately after title bar on the right
                Assert.AreEqual(accessoryColor, bmp.GetPixel(width - 2, titlebarHeight + 2));

                // Accessory is sized appropriately
                if (accessoryWidth < width)
                {
                    Assert.AreEqual(mapColor, bmp.GetPixel(0, titlebarHeight + 1));
                    Assert.AreEqual(mapColor, bmp.GetPixel(width - accessoryWidth - 1, titlebarHeight + 1));
                    Assert.AreEqual(accessoryColor, bmp.GetPixel(width - accessoryWidth + 1, titlebarHeight + 1));
                    Assert.AreEqual(accessoryColor, bmp.GetPixel(width - accessoryWidth + 1, Math.Min(titlebarHeight + accessoryHeight - 1, height - 1)));
                }
            }
        }

        private static System.Drawing.Bitmap ControlToBitmap(UIElement control, int width, int height)
        {
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(control);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            encoder.Save(stream);
            return new System.Drawing.Bitmap(stream, false);
        }
    }
}
