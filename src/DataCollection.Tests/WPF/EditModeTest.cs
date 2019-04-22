using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
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
            Assert.IsFalse(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            session.FindElementByAccessibilityId("CancelAddFeatureButton").Click();
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
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
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.1
        /// Tests adding a feature without saving
        /// </summary>
        [TestMethod]
        public void TestAddingFeatureNoSave()
        {
            ZoomToAddTreeTestArea();

            session.FindElementByAccessibilityId("AddFeatureButton").Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
            session.FindElementByAccessibilityId("SaveNewFeatureButton").Click();
            Thread.Sleep(10000);
            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Assert.AreEqual("DOWNTOWN", textBoxes[0].Text);
            Assert.IsTrue(textBoxes[1].Text.Contains("SW YAMHILL ST"));

            // clean up
            session.Mouse.Click(session.FindElementByAccessibilityId("CancelFeatureEditsButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.1 & 4.2
        /// Tests saving added feature and deleting it
        /// </summary>
        [TestMethod]
        public void TestSavingAddedFeatureAndDeletingIt()
        {
            ZoomToAddTreeTestArea();

            session.FindElementByAccessibilityId("AddFeatureButton").Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("AddFeatureUserControl").Displayed);
            session.FindElementByAccessibilityId("SaveNewFeatureButton").Click();
            Thread.Sleep(10000);
            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Assert.AreEqual("DOWNTOWN", textBoxes[0].Text);
            Assert.IsTrue(textBoxes[1].Text.Contains("SW YAMHILL ST"));

            // save feature
            session.Mouse.Click(session.FindElementByAccessibilityId("SaveFeatureEditsButton")?.Coordinates);
            Thread.Sleep(5000);
            Assert.IsTrue(session.FindElementByAccessibilityId("IndentifyUserControl").Displayed);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveFeatureEditsButton").Displayed);
            Assert.IsTrue(session.FindElementByAccessibilityId("DeleteFeatureButton").Displayed);

            // delete feature and clean up
            session.Mouse.Click(session.FindElementByAccessibilityId("DeleteFeatureButton")?.Coordinates);
            ConfirmDeleteDialog();
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.3
        /// Edit a feature and confirm that the edit is persisted
        /// </summary>
        [TestMethod]
        public void EditFeature()
        {
            ZoomAndIdentifyFeature();

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Thread.Sleep(2000);

            // edit feature and save
            session.FindElementByAccessibilityId("EditFeatureButton").Click();
            session.Mouse.Click(textBoxes[0].Coordinates);
            session.Keyboard.SendKeys("12");
            session.FindElementByAccessibilityId("SaveFeatureEditsButton").Click();

            Thread.Sleep(2000);
            // Assert edits saved
            Assert.AreEqual("DOWNTOWN12", textBoxes[0].Text);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveFeatureEditsButton").Displayed);

            // undo edits and save
            session.FindElementByAccessibilityId("EditFeatureButton").Click();
            session.Mouse.Click(textBoxes[0].Coordinates);
            session.Keyboard.SendKeys(Keys.Backspace);
            session.Keyboard.SendKeys(Keys.Backspace);
            session.FindElementByAccessibilityId("SaveFeatureEditsButton").Click();

            Thread.Sleep(2000);
            // assert edits undone
            Assert.AreEqual("DOWNTOWN", textBoxes[0].Text);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveFeatureEditsButton").Displayed);

            //clean up
            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.4
        /// Tests that clicking to add an inspection shows the add inspection window
        /// </summary>
        [TestMethod]
        public void TestAddRelatedRecordButtonDisplaysProperControls()
        {
            ZoomAndIdentifyFeature();

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var buttons = detailsView.FindElementsByClassName("Button");

            // scroll down to button
            session.Mouse.MouseMove(detailsView.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 5; i++)
                session.Keyboard.SendKeys(Keys.ArrowDown);

            // click + button to add inspection
            buttons[1].Click();

            Assert.IsTrue(session.FindElementByAccessibilityId("OriginRelationshipUserControl").Displayed);

            //clean up
            session.Mouse.MouseMove(detailsView.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 5; i++)
                session.Keyboard.SendKeys(Keys.ArrowUp);
            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.4
        /// Tests adding a new inspection and deleting it
        /// </summary>
        [TestMethod]
        public void TestAddNewRelatedRecord()
        {
            ZoomAndIdentifyFeature();

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var buttons = detailsView.FindElementsByClassName("Button");

            // scroll down to button
            session.Mouse.MouseMove(detailsView.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 5; i++)
            {
                session.Keyboard.SendKeys(Keys.ArrowDown);
            }

            // click + button to add inspection
            buttons[1].Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("OriginRelationshipUserControl").Displayed);

            // add values and save
            var datePickers = session.FindElementsByClassName("DatePicker");
            session.Mouse.Click(datePickers[0].Coordinates);
            session.Keyboard.SendKeys("4/22/2019");
            var comboBoxes = session.FindElementsByClassName("ComboBox");
            session.Mouse.Click(comboBoxes[0].Coordinates);
            session.Keyboard.SendKeys(Keys.ArrowDown);
            session.Keyboard.SendKeys(Keys.Enter);
            session.FindElementByAccessibilityId("SaveRelatedRecordEditsButton").Click();
            Thread.Sleep(2000);

            // assert new relationship saved
            var relationshipContainer = session.FindElementByAccessibilityId("OriginRelationshipUserControl");
            var textBoxes = relationshipContainer.FindElementsByClassName("TextBox");
            Thread.Sleep(2000);
            Assert.AreEqual("4/22/2019", textBoxes[0].Text);
            Assert.AreEqual("Good", textBoxes[1].Text);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveFeatureEditsButton").Displayed);

            // delete relationship
            session.FindElementByAccessibilityId("DeleteRelatedRecordButton").Click();
            ConfirmDeleteDialog();
            Thread.Sleep(2000);

            // confirm delete
            Assert.IsFalse(session.FindElementByAccessibilityId("OriginRelationshipUserControl").Displayed);

            //clean up
            session.Mouse.MouseMove(detailsView.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 5; i++)
                session.Keyboard.SendKeys(Keys.ArrowUp);
            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.6
        /// Test editing an inspection
        /// </summary>
        [TestMethod]
        public void EditRelatedRecord()
        {
            ZoomAndIdentifyFeature();

            var detailsView = session.FindElementByAccessibilityId("DetailsView");

            // scroll down to button
            session.Mouse.MouseMove(detailsView.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 10; i++)
                session.Keyboard.SendKeys(Keys.ArrowDown);

            // click > button to select inspection
            var buttons = detailsView.FindElementsByClassName("Button");
            buttons[2].Click();
            Assert.IsTrue(session.FindElementByAccessibilityId("OriginRelationshipUserControl").Displayed);

            // edit values and save
            session.FindElementByAccessibilityId("EditRelatedRecordButton").Click();
            var datePickers = session.FindElementsByClassName("DatePicker");
            session.Mouse.Click(datePickers[0].Coordinates);
            for (int i = 0; i < 9; i++)
                session.Keyboard.SendKeys(Keys.Backspace);
            session.Keyboard.SendKeys("4/22/2019");
            var comboBoxes = session.FindElementsByClassName("ComboBox");
            session.Mouse.Click(comboBoxes[0].Coordinates);
            session.Keyboard.SendKeys(Keys.ArrowUp);
            session.Keyboard.SendKeys(Keys.Enter);
            session.FindElementByAccessibilityId("SaveRelatedRecordEditsButton").Click();
            Thread.Sleep(2000);

            // assert edits were saved
            var relationshipContainer = session.FindElementByAccessibilityId("OriginRelationshipUserControl");
            var textBoxes = relationshipContainer.FindElementsByClassName("TextBox");
            Thread.Sleep(2000);
            Assert.AreEqual("4/22/2019", textBoxes[0].Text);
            Assert.AreEqual("Good", textBoxes[1].Text);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveRelatedRecordEditsButton").Displayed);

            // undo edits and save
            session.FindElementByAccessibilityId("EditRelatedRecordButton").Click();
            datePickers = session.FindElementsByClassName("DatePicker");
            session.Mouse.Click(datePickers[0].Coordinates);
            for (int i = 0; i < 9; i++)
                session.Keyboard.SendKeys(Keys.Backspace);
            session.Keyboard.SendKeys("4/17/2019");
            comboBoxes = session.FindElementsByClassName("ComboBox");
            session.Mouse.Click(comboBoxes[0].Coordinates);
            session.Keyboard.SendKeys(Keys.ArrowDown);
            session.Keyboard.SendKeys(Keys.Enter);
            session.FindElementByAccessibilityId("SaveRelatedRecordEditsButton").Click();
            Thread.Sleep(2000);

            // assert edits were undone 
            relationshipContainer = session.FindElementByAccessibilityId("OriginRelationshipUserControl");
            textBoxes = relationshipContainer.FindElementsByClassName("TextBox");
            Thread.Sleep(2000);
            Assert.AreEqual("4/17/2019", textBoxes[0].Text);
            Assert.AreEqual("Fair", textBoxes[1].Text);
            Assert.IsFalse(session.FindElementByAccessibilityId("SaveRelatedRecordEditsButton").Displayed);

            //clean up
            session.Mouse.MouseMove(detailsView.Coordinates);
            session.Mouse.Click(null);
            for (int i = 0; i < 10; i++)
                session.Keyboard.SendKeys(Keys.ArrowUp);
            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        /// <summary>
        /// Test case 4.10
        /// Edit a feature and confirm that the edit is not persisted when an invalid value is entered
        /// </summary>
        [TestMethod]
        public void EditFeatureInvalidValues()
        {
            ZoomAndIdentifyFeature();

            var detailsView = session.FindElementByAccessibilityId("DetailsView");
            var textBoxes = detailsView.FindElementsByClassName("TextBox");

            Thread.Sleep(2000);

            // add invalid value
            session.FindElementByAccessibilityId("EditFeatureButton").Click();
            session.Mouse.Click(textBoxes[0].Coordinates);
            session.Keyboard.SendKeys("123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567123456712345671234567");
            session.FindElementByAccessibilityId("SaveFeatureEditsButton").Click();

            // confirm invalid input detected popup
            session.SwitchTo().Window(session.WindowHandles[0]);
            Assert.IsTrue(session.FindElementByName("Invalid input detected").Displayed);
            session.FindElementByName("OK").Click();
            session.SwitchTo().Window(session.WindowHandles[0]);

            // assert still in edit mode
            Assert.IsTrue(session.FindElementByAccessibilityId("SaveFeatureEditsButton").Displayed);

            //clean up
            session.Mouse.Click(session.FindElementByAccessibilityId("CancelFeatureEditsButton")?.Coordinates);
            ConfirmDiscardDialog();
            session.Mouse.Click(session.FindElementByAccessibilityId("CloseIdentifyButton")?.Coordinates);
            session.Mouse.ContextClick(session.FindElementByAccessibilityId("CurrentLocationButton")?.Coordinates);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }
    }
}
