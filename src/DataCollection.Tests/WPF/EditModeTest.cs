using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Tests.WPF
{
    [TestClass]
    public class EditModeTest : AppSession
    {

        /// <summary>
        /// Test case 4.0
        /// Tests that clicking the Add Feature button causes the screen to show the tree adding workflow
        /// Current location is available
        /// Add Tree and Elipsis buttons are not available
        /// Identify isn't available
        /// </summary>
        [TestMethod]
        public void TestAddFeatureButtonDisplaysProperControls()
        {
            session.FindElementByAccessibilityId("AddFeatureButton").Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
            Assert.IsTrue(session.FindElementByAccessibilityId("CurrentLocationButton").Displayed);
            Assert.IsFalse(session.FindElementByAccessibilityId("AddFeatureButton").Displayed);
            Assert.IsFalse(session.FindElementByAccessibilityId("toggleMenu").Displayed);

            var mapView = session.FindElementByAccessibilityId("MapView");
            session.Mouse.MouseMove(mapView.Coordinates, 435, 314);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            session.Mouse.Click(null);
            Thread.Sleep(3000);

            Assert.IsFalse(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            session.FindElementByAccessibilityId("CancelAddFeatureButton").Click();
        }

        /// <summary>
        /// Test case 4.1
        /// Tests starting to add a feature then canceling
        /// </summary>
        [TestMethod]
        public void TestCancelAddingFeature()
        {
            session.FindElementByAccessibilityId("AddFeatureButton").Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
            session.FindElementByAccessibilityId("CancelAddFeatureButton").Click();
            Assert.IsFalse(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
        }

        /// <summary>
        /// Test case 4.1
        /// Tests adding a feature without saving
        /// </summary>
        [TestMethod]
        public void TestAddingFeatureNoSave()
        {
            ZoomToTestArea();

            session.FindElementByAccessibilityId("AddFeatureButton").Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
            session.FindElementByAccessibilityId("SaveNewFeatureButton").Click();
            Thread.Sleep(10000);
            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Assert.AreEqual("DOWNTOWN", textBoxes[0].Text);
            Assert.IsTrue(textBoxes[1].Text.Contains("SW YAMHILL ST"));

            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.1 & 4.2
        /// Tests saving added feature and deleting it
        /// </summary>
        [TestMethod]
        public void TestSavingAddedFeatureAndDeletingIt()
        {
            ZoomToTestArea();

            session.FindElementByAccessibilityId("AddFeatureButton").Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
            session.FindElementByAccessibilityId("SaveNewFeatureButton").Click();
            Thread.Sleep(10000);
            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Assert.AreEqual("DOWNTOWN", textBoxes[0].Text);
            Assert.IsTrue(textBoxes[1].Text.Contains("SW YAMHILL ST"));

            session.Mouse.Click(session.FindElementByAccessibilityId("SaveFeatureEditsButton")?.Coordinates);
            Thread.Sleep(5000);
            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveFeatureEditsButton").Displayed);
            Assert.IsTrue(session.FindElementByAccessibilityId("DeleteFeatureButton").Displayed);

            //clean up
            session.Mouse.Click(session.FindElementByAccessibilityId("DeleteFeatureButton")?.Coordinates);
            ConfirmDeleteDialog();
        }

        private void ZoomToTestArea()
        {
            var mapView = session.FindElementByAccessibilityId("MapView");
            session.Mouse.MouseMove(mapView.Coordinates, 470, 314);
            Thread.Sleep(5000);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            Thread.Sleep(5000);

        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

            //var mapView = session.FindElementByAccessibilityId("MapView");
            //session.Mouse.MouseMove(mapView.Coordinates, 435, 314);
            //Thread.Sleep(5000);
            //session.Mouse.DoubleClick(null);
            //session.Mouse.DoubleClick(null);
            //session.Mouse.DoubleClick(null);
            //session.Mouse.DoubleClick(null);
            //session.Mouse.DoubleClick(null);
            //session.Mouse.MouseMove(mapView.Coordinates, 470, 314);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
            TearDown();
        }
    }
}
