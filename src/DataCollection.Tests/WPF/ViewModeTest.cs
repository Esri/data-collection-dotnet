using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Tests.WPF
{
    [TestClass]
    public class ViewModeTest : AppSession
    {
        /// <summary>
        /// Test case 2.1
        /// Tests that the identify returned the proper feature attributes
        /// </summary>
        [TestMethod]
        public void TestIdentifyAttributesPresent()
        {
            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Assert.IsTrue(textBoxes.Count == 9);
            Assert.AreEqual("DOWNTOWN", textBoxes[0].Text);
            Assert.AreEqual("Fair", textBoxes[2].Text);
            Assert.AreEqual("9.00", textBoxes[3].Text);
            Assert.AreEqual("4.00", textBoxes[6].Text);
            Assert.AreEqual("Cutout", textBoxes[7].Text);
            Assert.AreEqual("No high voltage", textBoxes[8].Text);
            Assert.IsTrue(detailsView.FindElementByName("Neighborhood").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Address").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Condition").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("DBH").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Planted By").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Planted Date").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Site Width").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Site Type").Displayed);
            Assert.IsTrue(detailsView.FindElementByName("Wires").Displayed);
        }

        /// <summary>
        /// Test case 2.2
        /// Tests that the identify returned the proper Destination (Species) related table info
        /// </summary>
        [TestMethod]
        public void TestIdentifyDestinationRelationshipsPresent()
        {
            var container = session.FindElementByAccessibilityId("DetailsView");
            Assert.IsTrue(container.FindElementByName("Common Name").Displayed);
            Assert.IsTrue(container.FindElementByName("maple, red").Displayed);
            Assert.IsTrue(container.FindElementByName("Scientific Name").Displayed);
            Assert.IsTrue(container.FindElementByName("Acer rubrum").Displayed);
        }
        
        /// <summary>
        /// Test case 2.2
        /// Tests expanding Destination (Species) related table info to see details
        /// </summary>
        [TestMethod]
        public void TestDestinationRelationshipsDetailsPresent()
        {
            var container = session.FindElementByAccessibilityId("DetailsView");
            var buttons = container.FindElementsByClassName("Button");
            session.Mouse.Click(buttons[0].Coordinates);
            var relationshipContainer = session.FindElementByAccessibilityId("DestinationRelationshipUserControl");
            var textBoxes = relationshipContainer.FindElementsByClassName("TextBox");

            Assert.IsTrue(session.FindElementByAccessibilityId("DestinationRelationshipUserControl").Displayed);
            Assert.IsTrue(session.FindElementByName("Species").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Common Name").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Scientific Name").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Functional Type").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Family").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Genus").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Edible").Displayed);
            Assert.IsTrue(textBoxes.Count == 6);
            Assert.AreEqual("maple, red", textBoxes[0].Text);
            Assert.AreEqual("Acer rubrum", textBoxes[1].Text);
            Assert.AreEqual("Broadleaf Deciduous", textBoxes[2].Text);
            Assert.AreEqual("Sapindaceae", textBoxes[3].Text);
            Assert.AreEqual("Acer", textBoxes[4].Text);
            Assert.AreEqual("no", textBoxes[5].Text);

            session.Mouse.Click(session.FindElementByAccessibilityId("CloseDestinationRelationshipButton").Coordinates);
        }

        /// <summary>
        /// Test case 2.3
        /// Tests that the identify returned the proper Origin (Inspections) related table info
        /// </summary>
        [TestMethod]
        public void TestIdentifyOriginRelationshipsPresent()
        {
            var container = session.FindElementByAccessibilityId("DetailsView");

            Assert.IsTrue(container.FindElementByName("Inspection").Displayed);
            Assert.IsTrue(container.FindElementByName("Collected Date").Displayed);
            Assert.IsTrue(container.FindElementByName("4/17/2019").Displayed);
            Assert.IsTrue(container.FindElementByName("Condition").Displayed);
            Assert.IsTrue(container.FindElementByName("Fair").Displayed);
            Assert.IsTrue(container.FindElementByName("DBH").Displayed);
            Assert.IsTrue(container.FindElementByName("9.00").Displayed);
            Assert.IsTrue(container.FindElementByName("Collected Date").Displayed);
            Assert.IsTrue(container.FindElementByName("4/1/2019").Displayed);
            Assert.IsTrue(container.FindElementByName("Condition").Displayed);
            Assert.IsTrue(container.FindElementByName("Poor").Displayed);
            Assert.IsTrue(container.FindElementByName("DBH").Displayed);
            Assert.IsTrue(container.FindElementByName("6.00").Displayed);
        }

        /// <summary>
        /// Test case 2.3 & 2.5
        /// Tests expanding Origin (Inspections) related table info to see details
        /// </summary>
        [TestMethod]
        public void TestOriginRelationshipsDetailsPresent()
        {
            // get identify popup and expand the first Inspection
            var container = session.FindElementByAccessibilityId("DetailsView");
            session.Mouse.MouseMove(container.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 16; i++)
                session.Keyboard.SendKeys(Keys.ArrowDown);
            var buttons = container.FindElementsByClassName("Button");
            session.Mouse.Click(buttons[2].Coordinates);
            var relationshipContainer = session.FindElementByAccessibilityId("OriginRelationshipUserControl");
            var textBoxes = relationshipContainer.FindElementsByClassName("TextBox");

            Assert.IsTrue(session.FindElementByAccessibilityId("OriginRelationshipUserControl").Displayed);
            Assert.IsTrue(session.FindElementByName("Inspection").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Collected Date").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Condition").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("DBH").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Collected By").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Notes").Displayed);
            Assert.IsTrue(textBoxes.Count == 5);
            Assert.AreEqual("4/17/2019", textBoxes[0].Text);
            Assert.AreEqual("Fair", textBoxes[1].Text);
            Assert.AreEqual("9.00", textBoxes[2].Text);

            //expand the second inspection
            session.Mouse.Click(buttons[3].Coordinates);
            relationshipContainer = session.FindElementByAccessibilityId("OriginRelationshipUserControl");
            textBoxes = relationshipContainer.FindElementsByClassName("TextBox");

            Assert.IsTrue(session.FindElementByAccessibilityId("OriginRelationshipUserControl").Displayed);
            Assert.IsTrue(session.FindElementByName("Inspection").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Collected Date").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Condition").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("DBH").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Collected By").Displayed);
            Assert.IsTrue(relationshipContainer.FindElementByName("Notes").Displayed);
            Assert.IsTrue(textBoxes.Count == 5);
            Assert.AreEqual("4/1/2019", textBoxes[0].Text);
            Assert.AreEqual("Poor", textBoxes[1].Text);
            Assert.AreEqual("6.00", textBoxes[2].Text);

            session.Mouse.Click(session.FindElementByAccessibilityId("CloseOriginRelationshipButton").Coordinates);
        }

        /// <summary>
        /// Test case 2.4
        /// Tests that the Condition and DBH of the last inspection match the Condition and DBH of the tree
        /// </summary>
        [TestMethod]
        public void TestLastInspectionInformsTree()
        {
            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");
            var textBlocks = detailsView.FindElementsByClassName("TextBlock");
            Assert.AreEqual(textBlocks[17].Text, textBoxes[2].Text); // "Fair"
            Assert.AreEqual(textBlocks[19].Text, textBoxes[3].Text); // "9.00"
        }




        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

            var mapView = session.FindElementByAccessibilityId("MapView");
            session.Mouse.MouseMove(mapView.Coordinates, 435, 314);
            Thread.Sleep(5000);
            session.Mouse.DoubleClick(null);
            session.Mouse.DoubleClick(null);
            session.Mouse.Click(null);
            Thread.Sleep(3000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
            TearDown();
        }
    }
}
