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
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
#if NETFX_CORE
using Windows.ApplicationModel;
using Windows.Security.Credentials;
#elif WPF
using System.Security.Cryptography;
#endif


namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private string _oAuthRefreshToken;
        private string _webmapURL;
        private string _arcGISOnlineURL;
        private string _appClientID;
        private string _redirectURL;
        private string _userName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthViewModel"/> class.
        /// </summary>
        public AuthViewModel(string webmapURL, string arcGISOnlineURL, string appClientID, string redirectURL, string userName, string oAuthRefreshToken)
        {
            _webmapURL = webmapURL;
            _arcGISOnlineURL = arcGISOnlineURL;
            _appClientID = appClientID;
            _redirectURL = redirectURL;
            _userName = userName;
            _oAuthRefreshToken = oAuthRefreshToken;

            // Set up authentication manager to handle signing in
            UpdateAuthenticationManager();

            // test if refresh token is available and sign the user in
            if (!string.IsNullOrEmpty(_oAuthRefreshToken))
            {
                // if device is online, sign user in automatically
                ConnectivityHelper.IsWebmapAccessible(_webmapURL).ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        SignInCommand.Execute(null);
                    }
                });
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
                if (_authenticatedUser != value)
                {
                    _authenticatedUser = value;
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(AuthenticatedUser, BroadcastMessageKey.AuthenticatedUser);
                    OnPropertyChanged();
                }
            }
        }

        private ICommand _signOutCommand;

        /// <summary>
        /// Gets the command to sign the user out
        /// </summary>
        public ICommand SignOutCommand
        {
            get
            {
                return _signOutCommand ?? (_signOutCommand = new DelegateCommand(
                    (x) =>
                    {
                        // clear credentials
                        foreach (var credential in AuthenticationManager.Current.Credentials)
                        {
                            AuthenticationManager.Current.RemoveCredential(credential);
                        }

                        // clear authenticated user property
                        AuthenticatedUser = null;

                        // clear the refresh token
                        _oAuthRefreshToken = null;
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
                    }));
            }
        }

        private ICommand _signInCommand;

        /// <summary>
        /// Gets the command to sign the user in
        /// </summary>
        public ICommand SignInCommand
        {
            get
            {
                return _signInCommand ?? (_signInCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // if device is not online, do not proceed
                        if (!await ConnectivityHelper.IsWebmapAccessible(_webmapURL))
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("DeviceOffline_Title"),
                                Resources.GetString("NoSignIn_DeviceOffline_Message"),
                                true);
                            return;
                        }

                        // Create connection to Portal and provide credential
                        try
                        {
                            var portal = await ArcGISPortal.CreateAsync(new Uri(_arcGISOnlineURL), true);
                        }
                        catch (Exception ex)
                        {
                            // if the token has expired, delete it and leave the user signed out
                            if (ex.Message == "refresh_token expired")
                            {
                                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
                            }

                            // show error unless the user cancelled the authentication
                            else if (ex.Message != Resources.GetString("OperationCancelled"))
                            {
                                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                    Resources.GetString("SignInUnsuccessful_Title"),
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
            foreach (var cred in AuthenticationManager.Current.Credentials)
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

            // if no refresh token, call to generate credentials
            // otherwise if a refresh token exists, sign user in using the refresh token
            var credential = string.IsNullOrEmpty(_oAuthRefreshToken) ?
                await CreateNewCredential(info) :
                await CreateCredentialFromRefreshToken(info);

            // add credential to the authentication manager singleton instance to be used in the app
            AuthenticationManager.Current.AddCredential(credential);

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
                                Resources.GetString("SignInUnsuccessful_Title"),
                                ex.Message,
                                true,
                                ex.StackTrace);
            }

            // Save refresh token if it has changed. Encrypt if necessary
            if (credential.OAuthRefreshToken != _oAuthRefreshToken)
            {
                if (!string.IsNullOrEmpty(credential.OAuthRefreshToken))
                {
                    StoreToken(credential.OAuthRefreshToken, credential.UserName);
                }
                else
                {
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
                }
            }

            return credential;
        }

        /// <summary>
        /// Methos to create a new credential given a refresh token exists
        /// </summary>
        private async Task<OAuthTokenCredential> CreateCredentialFromRefreshToken(CredentialRequestInfo info)
        {
            // set up credential using the refresh token
            try
            {
                var credential = new OAuthTokenCredential()
                {
                    ServiceUri = info.ServiceUri,
                    OAuthRefreshToken = GetToken(_oAuthRefreshToken),
                    GenerateTokenOptions = info.GenerateTokenOptions
                };

                await credential.RefreshTokenAsync();
                return credential;
            }
            catch
            {
                // if using the refresh token fails, clear the token 
                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.OAuthRefreshToken);
            }

            return null;
        }

        /// <summary>
        /// Method to create a new credential
        /// </summary>
        private async Task<OAuthTokenCredential> CreateNewCredential(CredentialRequestInfo info)
        {
            // HACK: portal endpoints that do not contain "sharing/rest" generate ArcGISTokenCredential instead of OAuthTokenCredential
            // Forcing sign in into ArcGIS online if "sharing/rest" not in the service uri
            var serviceUri = info.ServiceUri.ToString().Contains("sharing/rest") ? info.ServiceUri : new Uri(_arcGISOnlineURL);

            // AuthenticationManager will handle challenging the user for credentials
            var credential = await AuthenticationManager.Current.GenerateCredentialAsync(
            serviceUri,
            info.GenerateTokenOptions) as OAuthTokenCredential;
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
                AuthenticationManager.Current.RegisterServer(portalServerInfo);

                // Create a new ChallengeHandler that uses a method in this class to challenge for credentials
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CreateCredentialAsync);
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(Resources.GetString("AuthError_Title"), ex.Message, true, ex.StackTrace);
            }
        }

        /// <summary>
        /// Retrieve refresh token from the appropriate password storage
        /// This varies based on platform
        /// </summary>
        private string GetToken(string tokenFromSettings)
        {
#if WPF
            // decrypt refresh token retrieved from the config file
            var byteToken = ProtectedData.Unprotect(
                    Convert.FromBase64String(tokenFromSettings),
                    null,
                    DataProtectionScope.CurrentUser);
            return System.Text.Encoding.Unicode.GetString(byteToken);

#elif NETFX_CORE
            // retrieve refresh token from Windows' password vault
            var vault = new PasswordVault();
            try
            {
                // throws if no match found
                var credential = vault.Retrieve(Package.Current.DisplayName, _userName);
                credential.RetrievePassword();
                return credential.Password;
            }
            catch
            {
                return null;
            }
#else
            // will throw if another platform is added without handling this 
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Store refresh token in the appropriate password storage 
        /// This varies based on platform
        /// </summary>
        private void StoreToken(string refreshToken, string userName)
        {
#if WPF
            // encrypt refresh token to be stored in the app's config file
            var token = ProtectedData.Protect(
                    System.Text.Encoding.Unicode.GetBytes(refreshToken),
                    null,
                    DataProtectionScope.CurrentUser);
            BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(Convert.ToBase64String(token), BroadcastMessageKey.OAuthRefreshToken);

#elif NETFX_CORE
            // store refresh token for the user, or update it if it already exists
            var vault = new PasswordVault();

            try
            {
                // throws if no match found
                var credential = vault.Retrieve(Package.Current.DisplayName, userName);
                credential.RetrievePassword();

                // remove and re-add credential if the password has changed
                // just changing the password doesn't work, changes are not persisted between app sessions
                if (credential.Password != refreshToken)
                {
                    vault.Remove(credential);
                    vault.Add(new PasswordCredential(
                        Package.Current.DisplayName, userName, refreshToken));
                }
            }
            catch
            {
                vault.Add(new PasswordCredential(
                    Package.Current.DisplayName, userName, refreshToken));
            }
            finally
            {
                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged("Stored in vault", BroadcastMessageKey.OAuthRefreshToken);
            }
#else
            // will throw if another platform is added without handling this 
            throw new NotImplementedException();
#endif
        }
    }
}

