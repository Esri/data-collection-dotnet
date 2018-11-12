using System.Resources;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties
{
    /// <summary>
    /// Resources class handles getting resources from the Resources files
    /// </summary>
    internal static class Resources
    {
#if NETFX_CORE
        private static readonly Windows.ApplicationModel.Resources.ResourceLoader s_resource = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse("Esri.ArcGISRuntime.ExampleApps/Resources");

        public static string GetString(string name)
        {
            return s_resource.GetString(name);
        }
#else
        private static ResourceManager s_resourceManager;

        private static ResourceManager ResourceManager
        {
            get
            {
                if (s_resourceManager == null)
                {
#if WPF
                    s_resourceManager = new ResourceManager("Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Properties.Resources", typeof(Resources).Assembly);
#endif
                }

                return s_resourceManager;
            }
        }

        /// <summary>
        /// Method called to get a specific resource
        /// </summary>
        public static string GetString(string name)
        {
            return ResourceManager.GetString(name);
        }
#endif
    }
}