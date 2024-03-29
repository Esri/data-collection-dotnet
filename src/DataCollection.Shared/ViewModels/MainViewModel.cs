﻿/*******************************************************************************
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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utils;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
#if WPF
using static System.Environment;
#elif NETFX_CORE
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _webMapURL;
        private int _defaultZoomScale;
        private string _downloadPathRoot;
        private bool _isBusyWaiting = false;
        private bool _isAddingFeature = false;
        private string _busyWaitingMessage;
#if WPF
        private static string _localFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
#elif NETFX_CORE
        private static string _localFolder = ApplicationData.Current.LocalFolder.Path;
#elif DOT_NET_CORE_TEST
        private static string _localFolder = Environment.GetEnvironmentVariable("LocalAppData");
#else
        // will throw if another platform is added without handling this 
        throw new NotImplementedException();
#endif

        // Needed for workaround that prevents deleting maps taken offline.
        private string _currentOfflineSubdirectory;

        public MainViewModel()
        {
            AppInfoViewModel = new AppInfoViewModel();

            _webMapURL = Settings.Default.WebmapURL;
            _downloadPathRoot = Path.Combine(_localFolder,
                typeof(Settings).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                typeof(Settings).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title,
                "MMPK");

            _currentOfflineSubdirectory = Settings.Default.CurrentOfflineSubdirectory;

            // create download directory if it doesn't exist
            if (!Directory.Exists(_downloadPathRoot))
            {
                Directory.CreateDirectory(_downloadPathRoot);
            }

            // TODO - set _currentOfflineSubdirectory
            DeleteOldMapPackages();

            AuthViewModel = new AuthViewModel(Settings.Default.WebmapURL,
                                                Settings.Default.ArcGISOnlineURL,
                                                Settings.Default.AppClientID,
                                                Settings.Default.RedirectURL,
                                                Settings.Default.AuthenticatedUserName,
                                                Settings.Default.OAuthRefreshToken);

            ConnectivityMode = Settings.Default.ConnectivityMode == "Online" ? ConnectivityMode.Online : ConnectivityMode.Offline;
            _defaultZoomScale = Settings.Default.DefaultZoomScale;

            // set the download path and connectivity mode as they change
            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += async (s, l) =>
            {
                if (l.Args.Key == BroadcastMessageKey.ConnectivityMode && ConnectivityMode != (ConnectivityMode)l.Args.Value)
                {
                    ConnectivityMode = (ConnectivityMode)l.Args.Value;
                }
            };

            // Initialize the identify controller
            IdentifyController = new IdentifyController();
            IdentifyController.IdentifyCompleted += IdentifyController_IdentifyCompleted;

            // Update the map when the selected identify result changes
            IdentifyResultViewModel.PropertyChanged += IdentifyResultViewModel_PropertyChanged;

            MapAccessoryViewModel = new MapAccessoryViewModel(this);

            // call method to retrieve map based on app state, connection and offline file availability
            GetMap().ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    MapViewModel = new MapViewModel(t.Result, ConnectivityMode, _defaultZoomScale);
                }
            });
        }

        private void IdentifyResultViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IdentifyResultViewModel.CurrentlySelectedFeature))
            {
                if (IdentifyResultViewModel.CurrentlySelectedFeature != null)
                {
                    MapViewModel.SelectFeature(IdentifyResultViewModel.CurrentlySelectedFeature.Feature);
                }
                else if (IdentifyResultViewModel.IdentifiedFeatures?.Any() ?? false)
                {
                    MapViewModel.ClearSelection();
                    IdentifyResultViewModel.IdentifiedFeatures.ForEach(f => MapViewModel.SelectFeature(f.Feature, false));
                }
            }
            else if (e.PropertyName == nameof(IdentifyResultViewModel.IdentifiedFeatures))
            {
                if (IdentifyResultViewModel.IdentifiedFeatures == null)
                {
                    MapViewModel.ClearSelection();
                }
                else
                {
                    MapViewModel.ClearSelection();
                    IdentifyResultViewModel.IdentifiedFeatures.ForEach(f => MapViewModel.SelectFeature(f.Feature, false));
                }
            }
        }

        /// <summary>
        /// Attempts to close active feature and relationship views. If <paramref name="clearAllIdentifyResults"/> is true,
        /// or there are exactly one identify results, all identify results will be cleared
        /// </summary>
        /// <returns>True if editors successfully closed, false if the user chose to keep their editing session.</returns>
        public async Task<bool> AttemptCloseEditorsAsync(bool clearAllIdentifyResults = false)
        {
            bool result = true;
            if (IdentifyResultViewModel.CurrentlySelectedFeature != null && !await IdentifyResultViewModel.CurrentlySelectedFeature.ClearRelationships())
            {
                result = false;
            }
            else if (IdentifyResultViewModel.CurrentlySelectedFeature?.EditViewModel != null && !await IdentifyResultViewModel.CurrentlySelectedFeature.DiscardChanges())
            {
                result = false;
            }
            else if (clearAllIdentifyResults || IdentifyResultViewModel.IdentifiedFeatures?.Count == 1)
            {
                IdentifyResultViewModel.ClearResultsCommand.Execute(null);
            }
            else
            {
                IdentifyResultViewModel.ClearSelectionCommand.Execute(null);
            }
            return result;
        }

        private void DeleteOldMapPackages()
        {
            // Delete all of the non-current map packages from previous runs.
            foreach (var directory in new DirectoryInfo(_downloadPathRoot).GetDirectories())
            {
                try
                {
                    if (_currentOfflineSubdirectory == null || directory.FullName != Path.Combine(_downloadPathRoot, _currentOfflineSubdirectory))
                    {
                        directory.Delete(true);
                    }
                }
                catch
                {
                    // Ignore, sometimes there will be old locks/handles
                }
            }
        }

        private ConnectivityMode _connectivityMode;

        /// <summary>
        /// Gets or sets the state of the application (online or offline)
        /// </summary>
        public ConnectivityMode ConnectivityMode
        {
            get { return _connectivityMode; }
            set
            {
                if (_connectivityMode != value)
                {
                    _connectivityMode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the flag whether the app should only allow user to zoom to their location and disable all other actions
        /// </summary>
        private bool _isLocationOnlyMode;

        public bool IsLocationOnlyMode
        {
            get => _isLocationOnlyMode;
            set
            {
                if (_isLocationOnlyMode != value)
                {
                    _isLocationOnlyMode = value;
                    if (_isLocationOnlyMode)
                    {
                        IdentifyResultViewModel.ClearResultsCommand.Execute(null);
                        IdentifyController.IsIdentifyPaused = true;
                    }
                    else
                    {
                        IdentifyController.IsIdentifyPaused = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private MapViewModel _mapViewModel;

        /// <summary>
        /// Gets or sets the view model that handles the map
        /// </summary>
        public MapViewModel MapViewModel
        {
            get { return _mapViewModel; }
            set
            {
                if (_mapViewModel != value)
                {
                    _mapViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private DownloadViewModel _downloadViewModel;

        /// <summary>
        /// Gets or sets the view model that handles the download
        /// </summary>
        public DownloadViewModel DownloadViewModel
        {
            get { return _downloadViewModel; }
            set
            {
                if (_downloadViewModel != value)
                {
                    _downloadViewModel = value;
                    if (value == null)
                    {
                        IsLocationOnlyMode = false;
                    }
                    else
                    {
                        IsLocationOnlyMode = true;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMajorStateChangeAllowed));
                }
            }
        }

        private SyncViewModel _syncViewModel;

        /// <summary>
        /// Gets or sets the view model that handles the download
        /// </summary>
        public SyncViewModel SyncViewModel
        {
            get { return _syncViewModel; }
            set
            {
                if (_syncViewModel != value)
                {
                    _syncViewModel = value;
                    if (value == null)
                    {
                        IsLocationOnlyMode = false;
                    }
                    else
                    {
                        IsLocationOnlyMode = true;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMajorStateChangeAllowed));
                }
            }
        }

        private EditViewModel _editViewModel;

        /// <summary>
        /// Gets or sets the viewmodel for the current edit session
        /// </summary>
        public EditViewModel EditViewModel
        {
            get => _editViewModel;
            set
            {
                if (_editViewModel != value)
                {
                    _editViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private AuthViewModel _authViewModel;

        public AuthViewModel AuthViewModel
        {
            get => _authViewModel;
            set
            {
                if (_authViewModel != value)
                {
                    _authViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private AppInfoViewModel _appInfoViewModel;

        /// <summary>
        /// Gets or sets the viewmodel that manages the app's about page.
        /// </summary>
        public AppInfoViewModel AppInfoViewModel
        {
            get => _appInfoViewModel;
            set
            {
                if (_appInfoViewModel != value)
                {
                    _appInfoViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public IdentifyResultViewModel IdentifyResultViewModel { get; } = new IdentifyResultViewModel();

        private Map _offlineMap;

        /// <summary>
        /// Gets or sets the map used in offline mode
        /// </summary>
        public Map OfflineMap
        {
            get { return _offlineMap; }
            set
            {
                if (_offlineMap != value)
                {
                    _offlineMap = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusyWaiting
        {
            get => _isBusyWaiting;
            set
            {
                if (_isBusyWaiting != value)
                {
                    _isBusyWaiting = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAddingFeature
        {
            get => _isAddingFeature;
            set
            {
                if (_isAddingFeature != value)
                {
                    _isAddingFeature = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BusyWaitingMessage
        {
            get => _busyWaitingMessage;
            set
            {
                if (_busyWaitingMessage != value)
                {
                    _busyWaitingMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date the map was downloaded or synced
        /// </summary>
        public DateTime? SyncDate
        {
            get
            {
                return DateTime.TryParse(Settings.Default.SyncDate, out var syncDate) ? syncDate : (DateTime?)null;
            }
            set
            {
                if (SyncDate != value)
                {
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(value?.ToUniversalTime().ToString("O"), BroadcastMessageKey.SyncDate);
                    OnPropertyChanged();
                }
            }
        }

        private MapAccessoryViewModel _mapAccessoryViewModel;

        public MapAccessoryViewModel MapAccessoryViewModel
        {
            get => _mapAccessoryViewModel;
            set
            {
                if (_mapAccessoryViewModel != value)
                {
                    _mapAccessoryViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Identify Controller
        /// </summary>
        public IdentifyController IdentifyController { get; private set; }

        private ICommand _workOfflineCommand;

        /// <summary>
        /// Gets the command to switch app mode to offline
        /// </summary>
        public ICommand WorkOfflineCommand
        {
            get
            {
                return _workOfflineCommand ?? (_workOfflineCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // Reset UI state
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.ClosePopups);

                        // reset the identify view model so the feature selected is deselected
                        if (!await AttemptCloseEditorsAsync())
                        {
                            return;
                        }

                        // load offline map if it exists
                        // prompt uset to download map if it doesn't exist
                        if (OfflineMap != null)
                        {
                            MapViewModel = new MapViewModel(OfflineMap, ConnectivityMode, _defaultZoomScale);
                            BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(ConnectivityMode.Offline, BroadcastMessageKey.ConnectivityMode);
                        }
                        else
                        {
                            // Remove reference to any existing offline map
                            ReleaseOfflineMap();

                            string newPath = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('/', '+').Replace('\\', '+');
                            BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(newPath, BroadcastMessageKey.DownloadPath);
                            _currentOfflineSubdirectory = Settings.Default.CurrentOfflineSubdirectory;

                            // set up a new DownloadViewModel
                            DownloadViewModel = new DownloadViewModel(MapViewModel.Map, Path.Combine(_downloadPathRoot, _currentOfflineSubdirectory));

                            var taskResult = await BroadcastMessenger.Instance.AwaitMessageValueChanged(BroadcastMessageKey.SyncSucceeded);

                            if ((bool)taskResult)
                            {
                                // set and save download date 
                                SyncDate = DateTime.Now;

                                // set app mode to offline and load the offline map
                                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(ConnectivityMode.Offline, BroadcastMessageKey.ConnectivityMode);

                                var map = await GetMap();

                                if (map != null)
                                {
                                    MapViewModel = new MapViewModel(map, ConnectivityMode, _defaultZoomScale);
                                }
                            }
                            else
                            {
                                // call method to delete the mmpk
                                ReleaseOfflineMap();
                            }

                            // reset DownloadViewModel
                            DownloadViewModel = null;
                        }
                    }));
            }
        }

        private ICommand _workOnlineCommand;

        /// <summary>
        /// Gets the command to switch app mode to online
        /// </summary>
        public ICommand WorkOnlineCommand
        {
            get
            {
                return _workOnlineCommand ?? (_workOnlineCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // Reset UI state
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.ClosePopups);

                        // retrieve active viewpoint from offline map to pass to online map 
                        var activeViewpoint = ((x is Polygon) ? new Viewpoint((Polygon)x) : (Viewpoint)x) ?? null;

                        // reset the identify view model so the feature selected is deselected
                        if (!await AttemptCloseEditorsAsync())
                        {
                            return;
                        }

                        // if online map is unreachable, do not proceed
                        if (!await ConnectivityHelper.IsWebmapAccessible(_webMapURL))
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("DeviceOffline_Title"),
                                Resources.GetString("NoOnlineMode_DeviceOffline_Message"),
                                true);
                            return;
                        }

                        // change app state to online
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(ConnectivityMode.Online, BroadcastMessageKey.ConnectivityMode);

                        // Call method to get the appropriate map for the current app state
                        var map = await GetMap();

                        // Set map to the the new map
                        // This could kick the app back into offline mode if the device is offline
                        if (map != null)
                        {
                            if (activeViewpoint != null)
                            {
                                map.InitialViewpoint = activeViewpoint;
                            }
                            MapViewModel = new MapViewModel(map, ConnectivityMode, _defaultZoomScale);
                        }
                    }));
            }
        }

        private ICommand _deleteOfflineMapCommand;

        /// <summary>
        /// Gets the command to delete the offline map that the user has downloaded
        /// </summary>
        public ICommand DeleteOfflineMapCommand
        {
            get
            {
                return _deleteOfflineMapCommand ?? (_deleteOfflineMapCommand = new DelegateCommand(
                    async (x) =>
                    {
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.ClosePopups);

                        // if online map is unreachable, do not proceed
                        if (!await ConnectivityHelper.IsWebmapAccessible(_webMapURL))
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("DeviceOffline_Title"),
                                Resources.GetString("NoDelete_DeviceOffline_Message"),
                                true);
                            return;
                        }

                        string deleteOfflineMapMessage = Resources.GetString("DeleteMap_Message");

                        // check first to see if the map has unsynced changes
                        foreach (var layer in OfflineMap.OperationalLayers)
                        {
                            if (layer is FeatureLayer featureLayer)
                            {
                                var table = featureLayer.FeatureTable as GeodatabaseFeatureTable;

                                // only one change needs to be found for this condition to be true
                                if (table.Geodatabase.HasLocalEdits())
                                {
                                    deleteOfflineMapMessage = Resources.GetString("DeleteMapWithUnsyncedChanges_Message");
                                    break;
                                }
                            }
                        }

                        // wait for response from the user if they truly want to delete the offline map
                        bool response = await UserPromptMessenger.Instance.AwaitConfirmation(
                            Resources.GetString("DeleteMap_Title"),
                            deleteOfflineMapMessage,
                            false,
                            null,
                            Resources.GetString("DeleteButton_Content"));

                        if (response)
                        {
                            ReleaseOfflineMap();
                        }
                    }));
            }
        }

        private ICommand _syncMapCommand;

        /// <summary>
        /// Gets the command to sync the online and offline maps
        /// </summary>
        public ICommand SyncMapCommand
        {
            get
            {
                return _syncMapCommand ?? (_syncMapCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // if there is no offline map, there is nothing to sync
                        if (OfflineMap == null)
                        {
                            return;
                        }

                        // Reset UI state
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.ClosePopups);

                        // if online map is unreachable, do not proceed
                        if (!await ConnectivityHelper.IsWebmapAccessible(_webMapURL))
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("DeviceOffline_Title"),
                                Resources.GetString("NoSync_DeviceOffline_Message"),
                                true);
                            return;
                        }

                        SyncViewModel = new SyncViewModel(OfflineMap);

                        BroadcastMessenger.Instance.BroadcastMessengerValueChanged += handler;

                        void handler(object o, BroadcastMessengerEventArgs e)
                        {
                            if (e.Args.Key == BroadcastMessageKey.SyncSucceeded)
                            {
                                BroadcastMessenger.Instance.BroadcastMessengerValueChanged -= handler;
                                if ((bool)e.Args.Value)
                                {
                                    // set and save sync date
                                    SyncDate = DateTime.Now;
                                }

                                SyncViewModel = null;
                            }
                        }
                    }));
            }
        }

        private ICommand _clearIdentifiedFeatureCommand;

        /// <summary>
        /// Gets the command to clear the identified feature
        /// </summary>
        public ICommand ClearIdentifiedFeatureCommand
        {
            get
            {
                return _clearIdentifiedFeatureCommand ?? (_clearIdentifiedFeatureCommand = new DelegateCommand(
                    async (x) =>
                    {
                        await AttemptCloseEditorsAsync();
                    }));
            }
        }

        private ICommand _saveNewFeatureCommand;
        private ICommand _startNewFeatureCommand;
        private ICommand _cancelNewFeatureCommand;

        public ICommand StartNewFeatureCommand
        {
            get
            {
                return _startNewFeatureCommand ?? (_startNewFeatureCommand = new DelegateCommand(
                    (x) => { BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.ClosePopups); IsAddingFeature = true; IsLocationOnlyMode = true; }));
            }
        }

        public ICommand CancelNewFeatureCommand
        {
            get
            {
                return _cancelNewFeatureCommand ?? (_cancelNewFeatureCommand = new DelegateCommand(
                    (x) => { IsAddingFeature = false; IsLocationOnlyMode = false; }));
            }
        }

        /// <summary>
        /// Gets the command to save the feature user created
        /// </summary>
        public ICommand SaveNewFeatureCommand
        {
            get
            {
                return _saveNewFeatureCommand ?? (_saveNewFeatureCommand = new DelegateCommand(
                    async (pinpointElement) =>
                    {
                        IsAddingFeature = false;
                        IsLocationOnlyMode = false;
                        // specifying the first points layer as the target of the Add operation
                        // lines and polygons are not supported in this version of the app
                        // adding to multiple layers in not supported in this version of the app
                        try
                        {
                            var eligibleLayer = MapViewModel.Map.OperationalLayers.OfType<FeatureLayer>()
                                                .Where(layer => layer.FeatureTable.GeometryType == GeometryType.Point && layer.FeatureTable.CanAdd())
                                                .First();
                            // create new feature 
                            var feature = eligibleLayer.FeatureTable.CreateFeature();

                            // set feature geometry as the pinpoint's position
                            var frameworkElement = (FrameworkElement)pinpointElement;
                                    
                            MapPoint newFeatureGeometry = null;
#if WPF
                            var point = new Point(frameworkElement.Width / 2, frameworkElement.Height / 2);
                            newFeatureGeometry = MapAccessoryViewModel.MapView.ScreenToLocation(frameworkElement.TranslatePoint(point, MapAccessoryViewModel.MapView));
#elif NETFX_CORE
                            var transform = frameworkElement.TransformToVisual(MapAccessoryViewModel.MapView);
                            // Get the actual center point. CenterPoint here defaults to top left
                            var point = new Point(frameworkElement.CenterPoint.X + frameworkElement.Width / 2, frameworkElement.CenterPoint.Y + frameworkElement.Height / 2);
                            newFeatureGeometry = MapAccessoryViewModel.MapView.ScreenToLocation(transform.TransformPoint(point));
#else
                            throw new NotImplementedException();
#endif

                            // call method to perform custom workflow for the custom tree dataset
#pragma warning disable CS0162 // Unreachable code detected
                            await TreeSurveyWorkflows.PerformNewTreeWorkflow(MapViewModel.Map.OperationalLayers, feature, newFeatureGeometry);

                            // create the feature and its corresponding viewmodel
                            var featureVM = new IdentifiedFeatureViewModel(feature, ConnectivityMode, this)
                            {
                                EditViewModel = new EditViewModel(ConnectivityMode)
                            };
                            featureVM.EditViewModel.CreateFeature(newFeatureGeometry, feature as ArcGISFeature, featureVM.PopupManager);

                            //get relationship information for the newly added feature
                            await featureVM.GetRelationshipInfoForFeature(feature as ArcGISFeature);

                            // Add the feature to the table so that it is visible on the map.
                            await feature.FeatureTable.AddFeatureAsync(feature);

                            IdentifyResultViewModel.SetNewIdentifyResult(new List<IdentifiedFeatureViewModel>() { featureVM });

                            // select the new feature
                            MapViewModel.SelectFeature(feature);

                            return;
#pragma warning restore CS0162 // Unreachable code detected
                        }
                        catch (Exception ex)
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(Resources.GetString("AddFeatureError_Title"), 
                                Resources.GetString("AddFeatureError_Message"), true, ex.StackTrace);
                        }
                    }));
            }
        }

        private ICommand _deleteFeatureCommand;

        /// <summary>
        /// Gets the command to delete the selected feature
        /// </summary>
        public ICommand DeleteFeatureCommand
        {
            get
            {
                return _deleteFeatureCommand ?? (_deleteFeatureCommand = new DelegateCommand(
                    async (commandParameter) =>
                    {
                        // wait for response from the user if they truly want to delete the feature
                        bool deleteConfirmed = await UserPromptMessenger.Instance.AwaitConfirmation(
                            Resources.GetString("DeleteConfirmation_Title"),
                            Resources.GetString("DeleteConfirmation_Message"),
                            false,
                            null,
                            Resources.GetString("DeleteButton_Content"));

                        if (deleteConfirmed)
                        {
                            // determine if the feature is coming from the main feature class or a related table
                            if (commandParameter is IdentifiedFeatureViewModel)
                            {
                                if (await IdentifyResultViewModel.CurrentlySelectedFeature.DeleteFeature())
                                {
                                    IdentifyResultViewModel.ClearResultsCommand.Execute(null);
                                }
                            }
                            else if (commandParameter is OriginRelationshipViewModel)
                            {
                                if (await IdentifyResultViewModel.CurrentlySelectedFeature.SelectedOriginRelationship.DeleteFeature())
                                {
                                    IdentifyResultViewModel.ClearResultsCommand.Execute(null);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.Fail("Binding error, should never get to this point; ensure command parameter is set");
                            }
                        }
                    }));
            }
        }

        /// <summary>
        /// Invoked when Identify operation completes
        /// </summary>
        private async void IdentifyController_IdentifyCompleted(object sender, IdentifyEventArgs e)
        {
            // do not perform the Identify if user is editing
            if (IdentifyResultViewModel.CurrentlySelectedFeature?.EditViewModel != null ||
                IdentifyResultViewModel.CurrentlySelectedFeature?.SelectedOriginRelationship?.EditViewModel != null || IsLocationOnlyMode)
            {
                return;
            }

            // get first identified layer from the collection that matches the app rules
            if (e.LayerResults.Count > 0)
            {
                var identifiedFeatureVMs = new List<IdentifiedFeatureViewModel>();

                // get the closest identified feature to the location user tapped
                foreach (var identifyResult in e.LayerResults.Where(m => m.Popups?.Any() ?? false))
                {
                    // get feature
                    foreach (var popupFeature in identifyResult.Popups.Select(popup => popup.GeoElement).OfType<ArcGISFeature>())
                    {
                        // set the viewmodel for the feature
                        var identifyVM = new IdentifiedFeatureViewModel(popupFeature, ConnectivityMode, this);
                        identifiedFeatureVMs.Add(identifyVM);
                    }
                }
                MapAccessoryViewModel.CloseAccessories();
                IdentifyResultViewModel.SetNewIdentifyResult(identifiedFeatureVMs);
            }
            else
            {
                // Reset selection
                IdentifyResultViewModel.SetNewIdentifyResult(null);
            }
        }

        /// <summary>
        /// Event handler for BroadcastMessage changing
        /// </summary>
        private async void Instance_BroadcastMessengerValueChanged(object sender, BroadcastMessengerEventArgs e)
        {
            try
            {
                // Reload map when AuthenticatedUser is reset to null
                if (e.Args.Key == BroadcastMessageKey.AuthenticatedUser && e.Args.Value == null)
                {
                    var map = await GetMap();

                    if (map != null)
                    {
                        MapViewModel = new MapViewModel(map, ConnectivityMode, _defaultZoomScale);
                    }
                }
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);
            }
        }

        /// <summary>
        /// Gets or sets the mobile map package used for the offline map
        /// </summary>
        internal MobileMapPackage Mmpk { get; private set; }

        /// <summary>
        /// Methos to retrieve the map based on app state
        /// </summary>
        private async Task<Map> GetMap()
        {
            // get mmpk if it exists and if not already loaded
            if (Mmpk == null)
            {
                try
                {
                    Mmpk = await MobileMapPackage.OpenAsync(Path.Combine(_downloadPathRoot, _currentOfflineSubdirectory));
                    OfflineMap = Mmpk.Maps.FirstOrDefault();
                }
                catch
                {
                    // if offline map is not retrievable, switch app to online mode
                    ConnectivityMode = ConnectivityMode.Online;
                }
            }

            if (ConnectivityMode == ConnectivityMode.Online)
            {
                if (await ConnectivityHelper.IsWebmapAccessible(_webMapURL))
                {
                    return new Map(new Uri(_webMapURL));
                }
                else
                {
                    ConnectivityMode = ConnectivityMode.Offline;
                }
            }

            if (ConnectivityMode == ConnectivityMode.Offline)
            {
                if (OfflineMap != null)
                {
                    return OfflineMap;
                }
                else
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("DeviceOffline_Title"),
                    Resources.GetString("NoMap_DeviceOffline_Message"),
                    true);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets whether the app is in a state where the user should be allowed to sign out, delete the map, go online or offline, or sync.
        /// </summary>
        public bool IsMajorStateChangeAllowed => DownloadViewModel == null && SyncViewModel == null;

        /// <summary>
        /// Method to prepare the offline map package for future deletion
        /// </summary>
        private void ReleaseOfflineMap()
        {
            // switch app to Online mode
            if (ConnectivityMode == ConnectivityMode.Offline)
            {
                WorkOnlineCommand.Execute(MapViewModel.AreaOfInterest);
            }

            // remove the offline map
            OfflineMap = null;

            // close the mmpk and set it to null
            if (Mmpk != null)
            {
                Mmpk.Close();
                Mmpk = null;
            }

            // clear the sync date
            SyncDate = null;

            // try deleting the folder
            try
            {
                if (_currentOfflineSubdirectory != null)
                {
                    string currentPath = Path.Combine(_downloadPathRoot, _currentOfflineSubdirectory);
                    Directory.Delete(currentPath, true);
                }
            }
            catch
            {
                // Ignore, it will be deleted later.
            }
            finally
            {
                _currentOfflineSubdirectory = null;
                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(null, BroadcastMessageKey.DownloadPath);
            }
        }
    }
}
