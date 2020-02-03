/*******************************************************************************
  * Copyright 2019 Esri
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

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models
{
    /// <summary>
    /// Represents the status of the application, whether it's in Online mode or Offline mode. 
    /// It is not strictly dependent on the device being connected. The app can be in Offline mode even if the device has a network connection
    /// </summary>
    public enum ConnectivityMode
    {
        Online, 
        Offline
    }
}
