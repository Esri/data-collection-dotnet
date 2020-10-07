using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Tests.Mocks
{
    public class Settings : ISettings
    {
        public static ISettings Default => null;
        public string AddressAttribute { get => ""; set { } }
        public string AppClientID { get => ""; set { } }
        public string ArcGISOnlineURL { get => ""; set { } }
        public string AuthenticatedUserName { get => ""; set { } }
        public string ConnectivityMode { get => ""; set { } }
        public string CurrentOfflineSubdirectory { get => ""; set { } }
        public int DefaultZoomScale { get => 0; set { } }
        public string GeocodeUrl { get => ""; set { } }
        public string InspectionConditionAttribute { get => ""; set { } }
        public string InspectionDBHAttribute { get => ""; set { } }
        public int MaxIdentifyResultsPerLayer { get => 8; set { } }
        public string NeighborhoodAttribute { get => ""; set { } }
        public string NeighborhoodNameField { get => ""; set { } }
        public string NeighborhoodOperationalLayerId { get => ""; set { } }
        public string OAuthRefreshToken { get => ""; set { } }
        public string OfflineLocatorPath { get => ""; set { } }
        public string PopupExpressionForSubtitle { get => ""; set { } }
        public string RedirectURL { get => ""; set { } }
        public string SyncDate { get => ""; set { } }
        public string TreeConditionAttribute { get => ""; set { } }
        public string TreeDatasetWebmapUrl { get => ""; set { } }
        public string TreeDBHAttribute { get => ""; set { } }
        public string WebmapURL { get => ""; set { } }
    }
}
