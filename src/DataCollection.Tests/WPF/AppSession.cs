using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Tests.WPF
{
    public class AppSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string AppId = @"C:\Projects\data-collection-dotnet\src\DataCollection.WPF\bin\Debug\DataCollection.WPF.exe";


        internal static WindowsDriver<WindowsElement> session;

        public static void Setup(TestContext context)
        {
            // Launch application if it is not yet launched
            if (session == null)
            {
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", AppId);
                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(session);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1.5));
            }
        }

        public static void TearDown()
        {
            // Close the application and delete the session
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }

        protected static void ConfirmDeleteDialog()
        {
            var windowHandles= session.WindowHandles;
            session.SwitchTo().Window(windowHandles[0]);
            session.FindElementByName("Delete").Click();
            session.SwitchTo().Window(windowHandles[1]);
        }
    }
}
