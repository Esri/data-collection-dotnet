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

using System;
using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Utils
{
    public class SourceController : DependencyObject
    {
        private WeakReference<WebBrowser> _webBrowserWeakRef;
        private bool _isWebBrowserNavigatingEventFiring = false;
        private bool _isWebBrowserNavigatingExecuting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceController"/> class.
        /// </summary>
        public SourceController() { }

        /// <summary>
        /// Web Browser setter
        /// </summary>
        internal void SetWebBrowser(WebBrowser webBrowser)
        {
            WebBrowser = webBrowser;
        }

        /// <summary>
        /// Gets or sets the WebBrowser
        /// </summary>
        private WebBrowser WebBrowser
        {
            get
            {
                WebBrowser webBrowser = null;
                _webBrowserWeakRef?.TryGetTarget(out webBrowser);
                return webBrowser;
            }
            set
            {
                if (WebBrowser != null)
                    WebBrowser.Navigating -= WebBrowser_Navigating;

                if (_webBrowserWeakRef == null)
                    _webBrowserWeakRef = new WeakReference<WebBrowser>(value);
                else
                    _webBrowserWeakRef.SetTarget(value);

                if (value != null)
                    value.Navigating += WebBrowser_Navigating;
            }
        }

        /// <summary>
        /// Invoked when the browser is navigating to a different page
        /// </summary>
        private void WebBrowser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (!_isWebBrowserNavigatingExecuting)
            {
                _isWebBrowserNavigatingEventFiring = true;
                UriSource = e.Uri;
                _isWebBrowserNavigatingEventFiring = false;
            }
        }

        /// <summary>
        /// Identifies the <see cref="UriSourceProperty"/> property
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(SourceController), 
            new PropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// Invoked when the UriSource property changes
        /// </summary>
        private static void OnSourceChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is Uri && (dependency as SourceController).WebBrowser != null && !(dependency as SourceController)._isWebBrowserNavigatingEventFiring)
            {
                (dependency as SourceController)._isWebBrowserNavigatingExecuting = true;
                (dependency as SourceController).WebBrowser.Source = (Uri)args.NewValue;
                (dependency as SourceController)._isWebBrowserNavigatingExecuting = false;
            }
        }

        /// <summary>
        /// Gets or sets the Source property
        /// </summary>
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
    }
}
