/*******************************************************************************
  * Copyright 2019 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  https://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.ViewModels;
using Esri.ArcGISRuntime.Security;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views.Overlays
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class SignInWindow : UserControl
    {
        private const int ProportionalityConstant = 48000; // constant of inverse proportionality between dpi and browser height
        private const double WidthHeightRatio = 1.4; // calculated ideal ratio of width to height
        public SignInWindow()
        {
            InitializeComponent();

            // set the AuthorizeHandler for the authentication manager
            AuthenticationManager.Current.OAuthAuthorizeHandler = Resources["SignInWindowViewModel"] as SignInWindowViewModel;
            
            // Calculate and set browser size based on dpi
            SetBrowserSize();
        }

        /// <summary>
        /// Method to calculate the browser size based on the screen dpi
        /// This ensures that the sign in screen always displays in the correct proportions
        /// </summary>
        private void SetBrowserSize()
        {
            try
            {
                // get screen dpi
                var dpiProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
                var dpi = (int)dpiProperty?.GetValue(null, null);

                // calculate and det browser size based on dpi
                WebBrowser.Height = ProportionalityConstant / dpi;
                WebBrowser.Width = WebBrowser.Height * WidthHeightRatio;
            }
            catch
            {
                // if there's an error, default to 500 x 700 browser size
                WebBrowser.Height = 500;
                WebBrowser.Width = 700;
            }
        }
    }
}
