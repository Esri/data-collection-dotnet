/*******************************************************************************
  * Copyright 2020 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties
{
    public interface ISettings
    {
        string AddressAttribute { get; set; }
        string AppClientID { get; set; }
        string ArcGISOnlineURL { get; set; }
        string AuthenticatedUserName { get; set; }
        string ConnectivityMode { get; set; }
        string CurrentOfflineSubdirectory { get; set; }
        int DefaultZoomScale { get; set; }
        string GeocodeUrl { get; set; }
        string InspectionConditionAttribute { get; set; }
        string InspectionDBHAttribute { get; set; }
        int MaxIdentifyResultsPerLayer { get; set; }
        string NeighborhoodAttribute { get; set; }
        string NeighborhoodNameField { get; set; }
        string NeighborhoodOperationalLayerId { get; set; }
        string OAuthRefreshToken { get; set; }
        string OfflineLocatorPath { get; set; }
        string PopupExpressionForSubtitle { get; set; }
        string RedirectURL { get; set; }
        string SyncDate { get; set; }
        string TreeConditionAttribute { get; set; }
        string TreeDatasetWebmapUrl { get; set; }
        string TreeDBHAttribute { get; set; }
        string WebmapURL { get; set; }
        bool? ShowRuntimeVersion { get; set; }
        bool? ShowAppVersion { get; set; }
        bool? ShowLicenseInfo { get; set; }
        string LicenseInfoLink { get; set; }
    }
}