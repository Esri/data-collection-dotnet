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

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utils;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
#if WPF
using System.Windows;
using static System.Environment;
#elif NETFX_CORE
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Storage;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _webMapURL;
        private int _defaultZoomScale;
        private string _downloadPath;
#if WPF
        private static string _localFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
#elif NETFX_CORE
        private static string _localFolder = ApplicationData.Current.LocalFolder.Path;
#else
        // will throw if another platform is added without handling this 
        throw new NotImplementedException();
#endif

        public MainViewModel()
        {
            _webMapURL = Settings.Default.WebmapURL;


            _downloadPath = Path.Combine(_localFolder,
                typeof(Settings).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                typeof(Settings).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title,
                "MMPK");

            // create download directory if it doesn't exist
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }

            ConnectivityMode = Settings.Default.ConnectivityMode == "Online" ? ConnectivityMode.Online : ConnectivityMode.Offline;
            SyncDate = DateTime.TryParse(Settings.Default.SyncDate, out DateTime syncDate) ? syncDate : DateTime.MinValue;
            _defaultZoomScale = Settings.Default.DefaultZoomScale;

            // set the download path and connectivity mode as they change
            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += (s, l) =>
            {
                if (l.Args.Key == BroadcastMessageKey.ConnectivityMode && ConnectivityMode != (ConnectivityMode)l.Args.Value)
                {
                    ConnectivityMode = (ConnectivityMode)l.Args.Value;
                }
            };

            // Initialize the identify controller
            IdentifyController = new IdentifyController();
            IdentifyController.IdentifyCompleted += IdentifyController_IdentifyCompleted;

            // call method to retrieve map based on app state, connection and offline file availability
            GetMap().ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    MapViewModel = new MapViewModel(t.Result, ConnectivityMode, _defaultZoomScale);
                }
            });
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
                _isLocationOnlyMode = value;
                if (_isLocationOnlyMode)
                    IdentifiedFeatureViewModel = null;
                OnPropertyChanged();
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

        private Map _offlineMap;

        /// <summary>
        /// Gets or sets the map used in offline mode
        /// </summary>
        public Map OfflineMap
        {
            get { return _offlineMap; }
            set
            {
                _offlineMap = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _syncDate;

        /// <summary>
        /// Gets or sets the date the map was downloaded or synced
        /// </summary>
        public DateTime? SyncDate
        {
            get { return _syncDate; }
            set
            {
                _syncDate = value;
                OnPropertyChanged();
            }
        }

        private IdentifiedFeatureViewModel _identifiedFeatureViewModel;

        /// <summary>
        /// Gets or sets the view model that handles the identified feature
        /// </summary>
        public IdentifiedFeatureViewModel IdentifiedFeatureViewModel
        {
            get { return _identifiedFeatureViewModel; }
            set
            {
                if (_identifiedFeatureViewModel != value)
                {
                    _identifiedFeatureViewModel = value;
                    OnPropertyChanged();

                    if (value == null && MapViewModel != null)
                    {
                        // clear any selected features on the map
                        MapViewModel.ClearSelection();
                    }
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
                        // reset the identify view model so the feature selected is deselected
                        IdentifiedFeatureViewModel = null;

                        // load offline map if it exists
                        // prompt uset to download map if it doesn't exist
                        if (OfflineMap != null)
                        {
                            MapViewModel = new MapViewModel(OfflineMap, ConnectivityMode, _defaultZoomScale);
                            BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(ConnectivityMode.Offline, BroadcastMessageKey.ConnectivityMode);
                        }
                        else
                        {
                            // first delete any leftover files in the download directory
                            // this happens if the previous delete method failed due to locks on the vtpk
                            await DeleteOfflineMap();

                            // set up a new DownloadViewModel
                            DownloadViewModel = new DownloadViewModel(MapViewModel.Map, _downloadPath);

                            var taskResult = await BroadcastMessenger.Instance.AwaitMessageValueChanged(BroadcastMessageKey.SyncSucceeded);

                            if ((bool)taskResult)
                            {
                                // set and save download date 
                                SyncDate = DateTime.Now;
                                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(SyncDate, BroadcastMessageKey.SyncDate);

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
                                await DeleteOfflineMap();
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
                        // retrieve active viewpoint from offline map to pass to online map 
                        var activeViewpoint = ((x is Polygon) ? new Viewpoint((Polygon)x) : (Viewpoint)x) ?? null;

                        // reset the identify view model so the feature selected is deselected
                        IdentifiedFeatureViewModel = null;

                        // if online map is unreachable, do not proceed
                        if (!IsWebmapAccessible())
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Resources.GetString("DeviceOffline_Title"),
                                Resources.GetString("NoMap_DeviceOffline_Message"),
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
                        // if online map is unreachable, do not proceed
                        if (!IsWebmapAccessible())
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
                            await DeleteOfflineMap();
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
                    (x) =>
                    {
                        // if there is no offline map, there is nothing to sync
                        if (OfflineMap == null)
                        {
                            return;
                        }

                        // if online map is unreachable, do not proceed
                        if (!IsWebmapAccessible())
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
                                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(SyncDate, BroadcastMessageKey.SyncDate);
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
                    (x) =>
                    {
                        IdentifiedFeatureViewModel = null;
                    }));
            }
        }

        private ICommand _saveNewFeatureCommand;

        /// <summary>
        /// Gets the command to save the feature user created
        /// </summary>
        public ICommand SaveNewFeatureCommand
        {
            get
            {
                return _saveNewFeatureCommand ?? (_saveNewFeatureCommand = new DelegateCommand(
                    async (x) =>
                    {
                        // specifying the first points layer as the target of the Add operation
                        // lines and polygons are not supported in this version of the app
                        // adding to multiple layers in not supported in this version of the app
                        foreach (var layer in MapViewModel.Map.OperationalLayers)
                        {
                            // check to see that layer is identifiable based on the app rules
                            if (layer is FeatureLayer && layer.IsIdentifiable())
                            {
                                try
                                {
                                    // create new feature 
                                    var feature = ((FeatureLayer)layer).FeatureTable.CreateFeature();

                                    // set feature geometry as the mapview's center
                                    var newFeatureGeometry = MapViewModel.AreaOfInterest?.TargetGeometry.Extent.GetCenter() as MapPoint;

                                    // call method to perform custom workflow for the custom tree dataset
                                    await TreeSurveyWorkflows.PerformNewTreeWorkflow(MapViewModel.Map.OperationalLayers, feature, newFeatureGeometry);

                                    // create the feature and its corresponding viewmodel
                                    IdentifiedFeatureViewModel = new IdentifiedFeatureViewModel(feature, ConnectivityMode)
                                    {
                                        EditViewModel = new EditViewModel(ConnectivityMode)
                                    };
                                    IdentifiedFeatureViewModel.EditViewModel.CreateFeature(newFeatureGeometry, feature as ArcGISFeature, IdentifiedFeatureViewModel.PopupManager);

                                    //get relationship information for the newly added feature
                                    await IdentifiedFeatureViewModel.GetRelationshipInfoForFeature(feature as ArcGISFeature);

                                    // select the new feature
                                    MapViewModel.SelectFeature(feature);

                                    break;
                                }
                                catch (Exception ex)
                                {
                                    UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                                }
                            }
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
                    async (x) =>
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
                            if (x is IdentifiedFeatureViewModel)
                            {
                                if (await IdentifiedFeatureViewModel.DeleteFeature())
                                {
                                    IdentifiedFeatureViewModel = null;
                                }
                            }
                            else
                            {
                                if (await IdentifiedFeatureViewModel.SelectedOriginRelationship.DeleteFeature())
                                {
                                    IdentifiedFeatureViewModel.SelectedOriginRelationship = null;
                                }
                            }
                        }
                    }));
            }
        }

        private ICommand _cancelEditsCommand;

        /// <summary>
        /// Gets the command to delete the selected feature
        /// </summary>
        public ICommand CancelEditsCommand
        {
            get
            {
                return _cancelEditsCommand ?? (_cancelEditsCommand = new DelegateCommand(
                    async (x) =>
                    {
                        if (x is IdentifiedFeatureViewModel)
                        {
                            if (await IdentifiedFeatureViewModel.DiscardChanges() &&
                            IdentifiedFeatureViewModel.Feature.IsNewFeature())
                            {
                                IdentifiedFeatureViewModel = null;
                            }
                        }
                        else if (x is OriginRelationshipViewModel originRelationshipVM)
                        {
                            var feature = originRelationshipVM?.PopupManager?.Popup?.GeoElement as ArcGISFeature;

                            if (feature != null && await IdentifiedFeatureViewModel.SelectedOriginRelationship.DiscardChanges() && feature.IsNewFeature())
                            {
                                IdentifiedFeatureViewModel.SelectedOriginRelationship = null;
                                // remove viewmodel from collection
                                var originRelationshipVMCollection = (IdentifiedFeatureViewModel.OriginRelationships.FirstOrDefault(o => o.RelationshipInfo == originRelationshipVM.RelationshipInfo)).OriginRelationshipViewModelCollection;
                                originRelationshipVMCollection.Remove(originRelationshipVM);
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
            if (IdentifiedFeatureViewModel?.EditViewModel != null ||
                IdentifiedFeatureViewModel?.SelectedOriginRelationship?.EditViewModel != null || IsLocationOnlyMode)
            {
                return;
            }

            // get first identified layer from the collection that matches the app rules
            if (e.LayerResults.Count > 0)
            {
                // get the first result that meets the app rules
                var identifyLayerResult = e.LayerResults.Where(l => (l.LayerContent as Layer).IsIdentifiable()).FirstOrDefault();

                // get the closest identified feature to the location user tapped
                if (identifyLayerResult?.Popups?.Count() > 0)
                {
                    // get feature
                    var feature = ((IdentifyController)sender).FindNearestGeoElement(identifyLayerResult.GeoElements) as ArcGISFeature;

                    if (feature != null)
                    {
                        // select the identified feature
                        MapViewModel.SelectFeature(feature);
                    }
                    else
                    {
                        return;
                    }

                    // load feature to access its relationship info
                    if (feature.LoadStatus != LoadStatus.Loaded)
                    {
                        await feature.LoadAsync();
                    }

                    // set the viewmodel for the feature
                    IdentifiedFeatureViewModel = new IdentifiedFeatureViewModel(feature, ConnectivityMode);

                    try
                    {
                        // Call method to set up relationship info
                        await IdentifiedFeatureViewModel.GetRelationshipInfoForFeature(feature);
                    }
                    catch (Exception ex)
                    {
                        UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for BroadcastMessage changing
        /// </summary>
        private async void Instance_BroadcastMessengerValueChanged(object sender, BroadcastMessengerEventArgs e)
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

        /// <summary>
        /// Gets or sets the mobile map package used for the offline map
        /// </summary>
        internal MobileMapPackage Mmpk { get; private set; }

        /// <summary>
        /// Methos to retrieve the map based on app state
        /// </summary>
        private async Task<Map> GetMap()
        {
            // test connectivity to online webmap
            // if the device is fully offline, this will throw
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_webMapURL);
                HttpWebResponse response;

                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // force app state to offline if the app cannot find the webmap
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ConnectivityMode = ConnectivityMode.Offline;
                    }
                }
            }
            catch
            {
                ConnectivityMode = ConnectivityMode.Offline;
            }

            // get mmpk if it exists and if not already loaded
            if (Mmpk == null)
            {
                try
                {
                    Mmpk = await MobileMapPackage.OpenAsync(_downloadPath);
                    OfflineMap = Mmpk.Maps.FirstOrDefault();
                }
                catch
                {
                    // if offline map is not retrievable, switch app to online mode
                    ConnectivityMode = ConnectivityMode.Online;
                }
            }

            // Choose online or offline map based on app state         
            return (ConnectivityMode == ConnectivityMode.Online) ?
                new Map(new Uri(_webMapURL)) :
                OfflineMap;
        }

        /// <summary>
        /// Method to delete the mmpk that contails the offline map
        /// </summary>
        private async Task DeleteOfflineMap()
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

            // try deleting the folder
            // if deleting the folder doesn't work, restart the application
            if (Directory.Exists(_downloadPath))
            {
                try
                {
                    var directoryInfo = new DirectoryInfo(_downloadPath);
                    directoryInfo.ClearDirectory();
                }
                catch
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(
                        Resources.GetString("IOException_Title"),
                        Resources.GetString("IOException_Message"),
                        true);

                    // restart the app
#if WPF
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    });
#elif NETFX_CORE
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        CoreApplication.Exit();
                    });
#else
                    // will throw if another platform is added without handling this 
                    throw new NotImplementedException();
#endif
                }
            }
        }

        /// <summary>
        /// Test that the web map used for the app is online and accessible 
        /// </summary>
        private bool IsWebmapAccessible()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_webMapURL);
                HttpWebResponse response;

                using (response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
