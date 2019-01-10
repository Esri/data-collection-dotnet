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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
#elif WPF
using System.Security.Cryptography;
#endif


namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private string _oAuthRefreshToken;
        private string _WebmapURL;
        private string _arcGISOnlineURL;
        private string _appClientID;
        private string _redirectURL;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthViewModel"/> class.
        /// </summary>
        public AuthViewModel(string webmapURL, string arcGISOnlineURL, string appClientID, string redirectURL, string oAuthRefreshToken)
        {
            _WebmapURL = webmapURL;
            _arcGISOnlineURL = arcGISOnlineURL;
            _appClientID = appClientID;
            _redirectURL = redirectURL;
            _oAuthRefreshToken = oAuthRefreshToken;

            // Set up authentication manager to handle logins
            UpdateAuthenticationManager();

            // test if refresh token is available and login user
            if (!string.IsNullOrEmpty(_oAuthRefreshToken))
            {
                // test that the device is online
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_WebmapURL);
                    HttpWebResponse response;

                    using (response = (HttpWebResponse)request.GetResponse())
                    {
                        // login user if the status code for the web map is OK
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            LoginCommand.Execute(null);
                        }
                    }
                }
                catch { /* Leave the user logged off if device is offline */ }
            }
        }

        private PortalUser _authenticatedUser;

        /// <summary>
        /// Gets or sets the authenticated user for the Portal instance provided
        /// </summary>
        public PortalUser AuthenticatedUser
        {
            get { return _authenticatedUser; }
            set
            {
                _authenticatedUser = value;
                OnPropertyChanged();
            }
        }

        private ICommand _logOutCommand;

        /// <summary>
        /// Gets the command to log out the user
        /// </summary>
        public ICommand LogOutCommand
        {
            get
            {
                return _logOutCommand ?? (_logOutCommand = new DelegateCommand(
                    (x) =>
                    {
                        // clear credentials
                        foreach (var credential in Security.AuthenticationManager.Current.Credentials)
                        {
                            Security.AuthenticationManager.Current.RemoveCredential(credential);
                        }

                        // clear authenticated user property
                        AuthenticatedUser = null;

                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(AuthenticatedUser, BroadcastMessageKey.AuthenticatedUser);

                        // clear the refresh token
                        _oAuthRefreshToken = null;
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
                    }));
            }
        }

        private ICommand _loginCommand;

        /// <summary>
        /// Gets the command to log in the user
        /// </summary>
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // Create connection to Portal and provide credential
                        try
                        {
                            var portal = await ArcGISPortal.CreateAsync(new Uri(_arcGISOnlineURL), true);
                        }
                        catch (Exception ex)
                        {
                            // if the token has expired, delete it and leave the user logged out
                            if (ex.Message == "refresh_token expired")
                            {
                                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
                            }

                            // show error unless the user cancelled the authentication
                            else if (ex.Message != Resources.GetString("OperationCancelled"))
                            {
                                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                    Resources.GetString("LoginUnsuccessful_Title"),
                                    ex.Message,
                                    true,
                                    ex.StackTrace);
                            }
                        }
                        return;
                    }));
            }
        }

        /// <summary>
        /// ChallengeHandler function that will be called whenever access to a secured resource is attempted
        /// </summary>
        private async Task<Credential> CreateCredentialAsync(CredentialRequestInfo info)
        {
            // if credentials are already set, return set values
            foreach (var cred in Security.AuthenticationManager.Current.Credentials)
            {
                if (cred.ServiceUri == new Uri(_arcGISOnlineURL))
                {
                    return cred;
                }
            }

            // Create generate token options if necessary
            if (info.GenerateTokenOptions == null)
            {
                info.GenerateTokenOptions = new GenerateTokenOptions();
            }

            OAuthTokenCredential credential = null;

            // if no refresh token, call to generate credentials
            // otherwise if a refresh token exists, login user using the refresh token
            if (string.IsNullOrEmpty(_oAuthRefreshToken))
            {
                // HACK: portal endpoints that do not contain "sharing/rest" generate ArcGISTokenCredential instead of OAuthTokenCredential
                // Forcing login into ArcGIS online if "sharing/rest" not in the service uri
                var serviceUri = info.ServiceUri.ToString().Contains("sharing/rest") ? info.ServiceUri : new Uri(_arcGISOnlineURL);

                // AuthenticationManager will handle challenging the user for credentials
                credential = await Security.AuthenticationManager.Current.GenerateCredentialAsync(
                serviceUri,
                info.GenerateTokenOptions) as OAuthTokenCredential;
            }
            else
            {
                // unprotect the refresh token
#if WPF
                var byteToken = ProtectedData.Unprotect(
                      Convert.FromBase64String(_oAuthRefreshToken),
                      null,
                      DataProtectionScope.CurrentUser);

                var token = System.Text.Encoding.Unicode.GetString(byteToken);
#elif NETFX_CORE
                var token = await UnprotectRefreshTokenForUWP(_oAuthRefreshToken);
#endif
                // set up credential using the refresh token
                credential = new OAuthTokenCredential()
                {
                    ServiceUri = info.ServiceUri,
                    OAuthRefreshToken = token,
                    GenerateTokenOptions = info.GenerateTokenOptions
                };

                await credential.RefreshTokenAsync();
            }

            // add credential to the authentication manager singleton instance to be used int he app
            Security.AuthenticationManager.Current.AddCredential(credential);

            try
            {
                // Create connection to Portal and provide credential
                var portal = await ArcGISPortal.CreateAsync(new Uri(_arcGISOnlineURL), credential);

                // set authenticated user local property
                AuthenticatedUser = portal.User;
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("LoginUnsuccessful_Title"),
                                ex.Message,
                                true,
                                ex.StackTrace);
            }

            // Save refresh token if it has changed. Encrypt if necessary
            if (credential.OAuthRefreshToken != _oAuthRefreshToken)
            {
                if (!string.IsNullOrEmpty(credential.OAuthRefreshToken))
                {
#if WPF
                    var token = ProtectedData.Protect(
                            System.Text.Encoding.Unicode.GetBytes(credential.OAuthRefreshToken),
                            null,
                            DataProtectionScope.CurrentUser);
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(Convert.ToBase64String(token), BroadcastMessageKey.OAuthRefreshToken);
#elif NETFX_CORE
                    var token = await ProtectRefreshTokenForUWP(credential.OAuthRefreshToken);
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(token, BroadcastMessageKey.OAuthRefreshToken);

#endif

                }
                else
                {
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
                }
            }

            return credential;
        }

        /// <summary>
        /// Set up AuthenticationManager
        /// </summary>
        private void UpdateAuthenticationManager()
        {
            // Define the server information for ArcGIS Online
            var portalServerInfo = new ServerInfo
            {
                ServerUri = new Uri(_arcGISOnlineURL),
                TokenAuthenticationType = TokenAuthenticationType.OAuthAuthorizationCode,
                OAuthClientInfo = new OAuthClientInfo
                {
                    ClientId = _appClientID,
                    RedirectUri = new Uri(_redirectURL)
                },
            };

            try
            {
                // Register the ArcGIS Online server information with the AuthenticationManager
                Security.AuthenticationManager.Current.RegisterServer(portalServerInfo);

                // Create a new ChallengeHandler that uses a method in this class to challenge for credentials
                Security.AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CreateCredentialAsync);
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(Resources.GetString("AuthError_Title"), ex.Message, true, ex.StackTrace);
            }
        }

#if NETFX_CORE
        private async Task<string> ProtectRefreshTokenForUWP(string refreshToken)
        {
            var descriptor = "LOCAL=user";
            var encoding = BinaryStringEncoding.Utf8;

            // Create a DataProtectionProvider object for the specified descriptor.
            var provider = new DataProtectionProvider(descriptor);

            // Encode the plaintext input to a buffer.
            var buffer = CryptographicBuffer.ConvertStringToBinary(refreshToken, encoding);

            // Encrypt the message.
            var token = await provider.ProtectAsync(buffer);

            return token.ToString();
        }

        private async Task<string> UnprotectRefreshTokenForUWP(string token)
        {
            var encoding = BinaryStringEncoding.Utf8;

            // Create a DataProtectionProvider object.
            var provider = new DataProtectionProvider();

            // Encode the plaintext input to a buffer.
            var buffer = CryptographicBuffer.ConvertStringToBinary(token, encoding);

            var unprotectedToken = await provider.UnprotectAsync(buffer);

            return unprotectedToken.ToString();
        }
#endif
    }
}

