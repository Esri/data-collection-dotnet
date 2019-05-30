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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.ViewModels
{
    public class SignInWindowViewModel : BaseViewModel, IOAuthAuthorizeHandler
    {
        // Use a TaskCompletionSource to track the completion of the authorization
        private TaskCompletionSource<IDictionary<string, string>> _tcs;

        // URL for the authorization callback result (the redirect URI configured for your application)
        private string _callbackUrl;

        private Uri _webAddress;

        /// <summary>
        /// Gets or sets the web address that the browser will navigate to
        /// </summary>
        public Uri WebAddress
        {
            get => _webAddress;
            set
            {
                if (_webAddress != value)
                {
                    _webAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        private ICommand _navigateCommand;

        /// <summary>
        /// Gets the command fired when the browser navigates
        /// </summary>
        public ICommand NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand = new DelegateCommand(
                    (x) =>
                    {
                        // Check for a response to the callback url
                        const string portalApprovalMarker = "/oauth2/approval";

                        var uri = WebAddress;

                        // If no uri, or an empty url, return
                        if (WebAddress == null || string.IsNullOrEmpty(uri.AbsoluteUri))
                            return;

                        // Check for redirect
                        bool isRedirected = uri.AbsoluteUri.StartsWith(_callbackUrl) ||
                            _callbackUrl.Contains(portalApprovalMarker) && uri.AbsoluteUri.Contains(portalApprovalMarker);

                        // if redirected, success
                        if (isRedirected)
                        {
                            try
                            {
                                // Call a helper function to decode the response parameters
                                var authResponse = DecodeParameters(uri);

                                // Set the result for the task completion source
                                _tcs.SetResult(authResponse);
                            }
                            catch (Exception ex)
                            {
                                _tcs.SetException(ex);
                            }

                            // remove the web address
                            WebAddress = null;
                        }
                    }));
            }
        }

        /// <summary>
        /// Function to handle authorization requests, takes the URIs for the secured service, the authorization endpoint, and the redirect URI
        /// </summary>
        public Task<IDictionary<string, string>> AuthorizeAsync(Uri serviceUri, Uri authorizeUri, Uri callbackUri)
        {
            if (_tcs?.Task.IsCompleted == false)
                throw new Exception("Task in progress");

            _tcs = new TaskCompletionSource<IDictionary<string, string>>();

            // Store the authorization and redirect URLs
            WebAddress = authorizeUri;
            _callbackUrl = callbackUri.AbsoluteUri;

            // Return the task associated with the TaskCompletionSource
            return _tcs.Task;
        }

        /// <summary>
        /// Function to decode the parameters present in the URI to determine the auth response
        /// </summary>
        private static IDictionary<string, string> DecodeParameters(Uri uri)
        {
            // Create a dictionary of key value pairs returned in an OAuth authorization response URI query string
            var answer = string.Empty;

            // Get the values from the URI fragment or query string
            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                answer = uri.Fragment.Substring(1);
            }
            else
            {
                if (!string.IsNullOrEmpty(uri.Query))
                {
                    answer = uri.Query.Substring(1);
                }
            }

            // Parse parameters into key / value pairs
            var keyValueDictionary = new Dictionary<string, string>();
            var keysAndValues = answer.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var kvString in keysAndValues)
            {
                var pair = kvString.Split('=');
                string key = pair[0];
                string value = string.Empty;
                if (key.Length > 1)
                {
                    value = Uri.UnescapeDataString(pair[1]);
                }

                keyValueDictionary.Add(key, value);
            }

            // Return the dictionary of string keys/values
            return keyValueDictionary;
        }
    }
}
