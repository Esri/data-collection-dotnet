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
        /// <summary>
        /// Defines the _appVersion.
        /// </summary>
        private string _appVersion;

        /// <summary>
        /// Defines the _runtimeVersion.
        /// </summary>
        private string _runtimeVersion;

        /// <summary>
        /// Defines the _launchLicenseInfoCommand.
        /// </summary>
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
        /// Gets a value indicating whether a link to license info should be shown in the UI.
        /// </summary>
        public bool ShowLicenseInfo { get => Settings.Default.ShowLicenseInfo ?? false; }

        /// <summary>
        /// Gets a value indicating whether ShowAppInfo.
        /// </summary>
        public bool ShowAppInfo { get => ShowAppVersion || ShowRuntimeVersion || ShowLicenseInfo; }

        /// <summary>
        /// Gets a formatted string including the app's name, platform, and version.
        /// </summary>
        public string AppVersion
        {
            get
            {
                if (_appVersion == null)
                {

                    var appName = Resources.GetString("TitleBar_Name_App");
                    var platform = "";
#if NET_CORE
                    platform = "WPF (.NET Core)";
#elif WPF
                    platform = "WPF (.NET Framework)";
#elif NETFX_CORE
                    platform = "UWP";
#endif

                    var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    _appVersion = $"{appName}\n{platform}\n{version}";
                }
                return _appVersion;
            }
        }

        /// <summary>
        /// Gets the version of Runtime used by the app.
        /// </summary>
        public string RuntimeVersion
        {
            get
            {
                if (_runtimeVersion == null)
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
                    _runtimeVersion = $"ArcGIS Runtime {productVersion} ({build})";
                }
                return _runtimeVersion;
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
                            var url = Resources.GetString("AppInfo_LicenseLink");
                            _ = Windows.System.Launcher.LaunchUriAsync(new Uri(url));
#else
                            ProcessStartInfo psi = new ProcessStartInfo(Resources.GetString("AppInfo_LicenseLink")) { UseShellExecute = true };
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
    }
}
