/*******************************************************************************
  * Copyright 2018 Esri
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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using System;
using System.IO;
using System.Xml.Serialization;
using static System.Environment;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties
{
    // Singleton class to provide access to settings from Configuration.xml
    [Serializable()]
    public class Settings
    {
        private static Settings _instance;

        // set the path on disk for the settings file
        private static string _settingsPath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData), "ArcGISRuntime", "ExampleApps", "DataCollection", "DataCollectionSettings.xml");

        /// <summary>
        /// Default instance of the <see cref="Settings"/> class
        /// </summary>
        public static Settings Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();

                    // if settings file doesn't exist on disk, make a new one and save it
                    if (!File.Exists(_settingsPath))
                    {
                        // get settings file shipped with the app
#if WPF
                        var streamPath = "Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Properties.Configuration.xml";
#endif
                        // create stream and deserialize into a Settings object
                        var stream = typeof(Settings).Assembly.GetManifestResourceStream(streamPath);
                        _instance = DeserializeSettings(stream);

                        // serialize to save the new settings xml file
                        SerializeSettings(_instance);
                    }
                    else
                    {
                        // open settins file 
                        var settingsFile = File.Open(_settingsPath, FileMode.Open);

                        // deserialize settings file into AppSettings object
                        _instance = DeserializeSettings(settingsFile);
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        private Settings()
        {
            // fires when any of the specified settings values change
            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += (s, l) =>
            {
                if (l.Args.Key == BroadcastMessageKey.ConnectivityMode)
                {
                    ConnectivityMode = l.Args.Value?.ToString();
                }
                else if (l.Args.Key == BroadcastMessageKey.DownloadPath)
                {
                    DownloadPath = l.Args.Value?.ToString();
                }
                else if (l.Args.Key == BroadcastMessageKey.OAuthRefreshToken)
                {
                    OAuthRefreshToken = l.Args.Value?.ToString();
                }
                else if (l.Args.Key == BroadcastMessageKey.SyncDate)
                {
                    SyncDate = l.Args.Value?.ToString();
                }

                SerializeSettings(_instance);
            };
        }

        [XmlElement("ArcGISOnlineURL")]
        public string ArcGISOnlineURL { get; set; }

        [XmlElement("WebmapURL")]
        public string WebmapURL { get; set; }

        [XmlElement("GeocodeUrl")]
        public string GeocodeUrl { get; set; }

        [XmlElement("DefaultZoomScale")]
        public int DefaultZoomScale { get; set; }

        [XmlElement("AppClientID")]
        public string AppClientID { get; set; }

        [XmlElement("RedirectURL")]
        public string RedirectURL { get; set; }

        // The settings below are speficic to the Tree Survey dataset and workflow 
        [XmlElement("TreeDatasetWebmapUrl")]
        public string TreeDatasetWebmapUrl { get; set; }

        [XmlElement("OfflineLocatorPath")]
        public string OfflineLocatorPath { get; set; }

        [XmlElement("NeighborhoodOperationalLayerId")]
        public string NeighborhoodOperationalLayerId { get; set; }

        [XmlElement("NeighborhoodNameField")]
        public string NeighborhoodNameField { get; set; }

        [XmlElement("NeighborhoodAttribute")]
        public string NeighborhoodAttribute { get; set; }

        [XmlElement("AddressAttribute")]
        public string AddressAttribute { get; set; }

        [XmlElement("TreeConditionAttribute")]
        public string TreeConditionAttribute { get; set; }

        [XmlElement("InspectionConditionAttribute")]
        public string InspectionConditionAttribute { get; set; }

        [XmlElement("TreeDBHAttribute")]
        public string TreeDBHAttribute { get; set; }

        [XmlElement("InspectionDBHAttribute")]
        public string InspectionDBHAttribute { get; set; }

        // The settings below are set by the app and should not be changed manually
        [XmlElement("OAuthRefreshToken")]
        public string OAuthRefreshToken { get; set; }

        [XmlElement("ConnectivityMode")]
        public string ConnectivityMode { get; set; }

        [XmlElement("DownloadPath")]
        public string DownloadPath { get; set; }

        [XmlElement("SyncDate")]
        public string SyncDate { get; set; }

        /// <summary>
        /// Serializes Settings object and saves it to the settings file
        /// </summary>
        private static void SerializeSettings(Settings instance)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            // open settings file for edit
            var fileinfo = new FileInfo(_settingsPath);
            if (!fileinfo.Directory.Exists)
                fileinfo.Directory.Create();
            var settingsFile = fileinfo.Exists ?
                File.Open(_settingsPath, FileMode.Truncate) :
                File.Create(_settingsPath);

            try
            {
                // serialize file
                serializer.Serialize(settingsFile, instance);
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("SerializationError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);
            }
            finally
            {
                // close settings file once it's been written 
                settingsFile.Close();
            }
        }

        /// <summary>
        /// Deserializes file into Settings object
        /// </summary>
        private static Settings DeserializeSettings(object o)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            if (o is Stream stream)
            {
                try
                {
                    return (Settings)serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(
                        Resources.GetString("DeserializationError_Title"),
                        ex.Message,
                        true,
                        ex.StackTrace);
                    return null;
                }
                finally
                {
                    stream.Close();
                }
            }
            return null;
        }
    }
}
