using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Tests.WPF
{
    [TestClass]
    public class BasicsTests : AppSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        /// <summary>
        /// Test case 1.1
        /// App should open be open and MapView control should be present
        /// Title of the map screen should be that of the web map('Portland Tree Survey')
        /// </summary>
        [TestMethod]
        public void TestMapExists()
        {
            Assert.IsNotNull(session.FindElementByAccessibilityId("MapView"));
            Assert.IsTrue(session.FindElementByAccessibilityId("MapView").Displayed);
        }

        /// <summary>
        /// Test case 1.1 
        /// Title of the map screen should be that of the web map('Portland Tree Survey')
        /// </summary>
        [TestMethod]
        public void TestMapTitle()
        {
            Assert.IsTrue(session.FindElementByName("Portland Tree Survey").Displayed);
        }

        /// <summary>
        /// Test case 1.6 
        /// Tap map where no tree exists
        /// Nothing should happen. No feature should be identified
        /// </summary>
        [TestMethod]
        public void TestMapClickNoTree()
        {
            var mapView = session.FindElementByAccessibilityId("MapView");
            session.Mouse.MouseMove(mapView.Coordinates, 435, 10);
            Thread.Sleep(5000);
            session.Mouse.Click(null);
            Thread.Sleep(5000);
            Assert.IsFalse(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 1.6 
        /// Tap map where there is a tree
        /// The feature should be identified and the identified window should display
        /// </summary>
        [TestMethod]
        public void TestMapClickOnTree()
        {
            var mapView = session.FindElementByAccessibilityId("MapView");
            session.Mouse.MouseMove(mapView.Coordinates, 435, 314);
            Thread.Sleep(5000);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            session.Mouse.Click(null);
            Thread.Sleep(5000);

            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
            TearDown();
        }
    }
}
