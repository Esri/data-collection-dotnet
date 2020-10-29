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

using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Utils
{
    public class WebBrowserExtensions : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="SourceControllerProperty"/> property
        /// </summary>
        public static readonly DependencyProperty SourceControllerProperty =
            DependencyProperty.Register(nameof(SourceController), typeof(SourceController), typeof(WebBrowser), new PropertyMetadata(null, OnSourceControllerChanged));

        /// <summary>
        /// Invoked when the SourceControllerProperty's value has changed
        /// </summary>
        private static void OnSourceControllerChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is SourceController)
                ((SourceController)args.NewValue).SetWebBrowser(dependency as WebBrowser);
        }

        /// <summary>
        /// SourceControllerProperty getter method
        /// </summary>
        public static SourceController GetSourceController(DependencyObject browser)
        {
            return (browser as WebBrowser)?.GetValue(SourceControllerProperty) as SourceController;
        }

        /// <summary>
        /// SourceControllerProperty setter method
        /// </summary>
        public static void SetSourceController(DependencyObject browser, SourceController sourceController)
        {
            (browser as WebBrowser)?.SetValue(SourceControllerProperty, sourceController);
        }
    }
}
