// /*******************************************************************************
//  * Copyright 2020 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    /// <summary>
    /// ViewModel for the app's about page.
    /// </summary>
    public class AppInfoViewModel : BaseViewModel
    {
        private string _appVersionNamePart;
        private string _appVersionNumberPart;
        private string _runtimeVersionNamePart;
        private string _runtimeVersionNumberPart;
        private string _runtimeVersionBuildPart;
        private ICommand _launchLicenseInfoCommand;

        /// <summary>
        /// Gets a value indicating if the version of ArcGIS Runtime should be shown in the UI.
        /// </summary>
        public bool ShowRuntimeVersion { get => Settings.Default.ShowRuntimeVersion ?? false; }

        /// <summary>
        /// Gets a value indicating if the version of the app should be shown in the UI.
        /// </summary>
        public bool ShowAppVersion { get => Settings.Default.ShowAppVersion ?? false; }

        /// <summary>
        /// Gets a value indicating if a link to license info should be shown in the UI.
        /// </summary>
        public bool ShowLicenseInfo { get => Settings.Default.ShowLicenseInfo ?? false; }

        /// <summary>
        /// Gets a value indicating whether to show the app's about page.
        /// </summary>
        public bool ShowAppInfo { get => ShowAppVersion || ShowRuntimeVersion || ShowLicenseInfo; }

        /// <summary>
        /// Gets a string with the app's name.
        /// </summary>
        public string AppVersionNamePart
        {
            get
            {
                if (_appVersionNamePart == null)
                {
                    _appVersionNamePart = Resources.GetString("TitleBar_Name_App");
                }
                return _appVersionNamePart;
            }
        }

        /// <summary>
        /// Gets a string with the app's UI platform.
        /// </summary>
        public string AppVersionPlatformPart
        {
            get
            {
#if NET_CORE
                return "WPF (.NET Core)";
#elif WPF
                return "WPF (.NET Framework)";
#elif NETFX_CORE
                return "UWP";
#else
                return "Unknown";
#endif
            }
        }

        /// <summary>
        /// Gets a string with the app's version number.
        /// </summary>
        public string AppVersionNumberPart
        {
            get
            {
                if (_appVersionNumberPart == null)
                {
                    _appVersionNumberPart = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                return _appVersionNumberPart;
            }
        }

        /// <summary>
        /// Gets the name of the ArcGIS Runtime SDK in use.
        /// </summary>
        public string RuntimeVersionNamePart
        {
            get
            {
                if (_runtimeVersionNamePart == null)
                {
                    PopulateRuntimeVersion();
                }
                return _runtimeVersionNamePart;
            }
        }

        /// <summary>
        /// Gets the ArcGIS Runtime product version.
        /// </summary>
        public string RuntimeVersionNumberPart
        {
            get
            {
                if (_runtimeVersionNumberPart == null)
                {
                    PopulateRuntimeVersion();
                }
                return _runtimeVersionNumberPart;
            }
        }

        /// <summary>
        /// Gets the ArcGIS Runtime build number.
        /// </summary>
        public string RuntimeVersionBuildPart
        {
            get
            {
                if (_runtimeVersionBuildPart == null)
                {
                    PopulateRuntimeVersion();
                }
                return _runtimeVersionBuildPart;
            }
        }

        /// <summary>
        /// Gets the command that launches the license info page.
        /// </summary>
        public ICommand LaunchLicenseInfoCommand
        {
            get
            {
                return _launchLicenseInfoCommand ?? (_launchLicenseInfoCommand = new DelegateCommand(
                    (param) =>
                    {
                        try
                        {
#if NETFX_CORE
                            var url = Settings.Default.LicenseInfoLink;
                            _ = Windows.System.Launcher.LaunchUriAsync(new Uri(url));
#else
                            ProcessStartInfo psi = new ProcessStartInfo(Settings.Default.LicenseInfoLink) { UseShellExecute = true };
                            Process.Start(psi);
#endif
                        }
                        catch (Exception ex)
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(Resources.GetString("Error_LaunchURLFailed"),
                                ex.Message, true, ex.StackTrace);
                        }
                    }));
            }
        }
        /// <summary>
        /// Populates version fields by inspection of the ArcGIS Runtime DLLs.
        /// </summary>
        private void PopulateRuntimeVersion()
        {
#if NETFX_CORE
            var version = FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.Installed­Location.Path, "RuntimeCoreNet.dll"));
#else
            var runtimeTypeInfo = typeof(ArcGISRuntimeEnvironment).GetTypeInfo();
            var version = FileVersionInfo.GetVersionInfo(runtimeTypeInfo.Assembly.Location);
#endif
            var buildparts = version.FileVersion.Split('.');
            var build = buildparts[buildparts.Length - 1];
            var productVersion = version.ProductVersion;

            if (productVersion.EndsWith(build))
            {
                productVersion = productVersion.Substring(0, productVersion.Length - (build.Length + 1));
            }

            _runtimeVersionBuildPart = build;
            _runtimeVersionNamePart = version.ProductName;
            _runtimeVersionNumberPart = productVersion;
        }
    }
}
