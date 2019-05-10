using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities
{
    /// <summary>
    /// Helper class used in multiple parts of the app to test device connectivity
    /// </summary>
    internal static class ConnectivityHelper
    {
        /// <summary>
        /// Test that the url is online and accessible 
        /// </summary>
        internal static async Task<bool> IsWebmapAccessible(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);

                return (response.StatusCode == HttpStatusCode.OK) ? true : false;
            }
            catch
            {
                return false;
            }
        }
    }
}
