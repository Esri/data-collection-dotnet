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
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Offline;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    class DownloadViewModel : BaseViewModel
    {
        public DownloadViewModel(Map map, string downloadPath)
        {
            // set the download path as it changes
            BroadcastMessenger.Instance.BroadcastMessengerValueChanged += (s, l) =>
            {
                if (l.Args.Key == BroadcastMessageKey.DownloadPath && DownloadPath != l.Args.Value?.ToString())
                {
                    DownloadPath = l.Args.Value?.ToString();
                }
            };

            Map = map;
            DownloadPath = downloadPath;
            IsAwaitingDownload = true;
        }

        /// <summary>
        /// Gets or sets the map 
        /// </summary>
        internal Map Map { get; }

        /// <summary>
        /// Gets or sets the download path for the mmpk
        /// </summary>
        public string DownloadPath { get; private set; }

        /// <summary>
        /// Gets or sets the GenerateOfflineMapJob
        /// </summary>
        internal GenerateOfflineMapJob GenerateOfflineMapJob { get; private set; }

        private int _progress = 0;

        /// <summary>
        /// Gets or sets the progress for the download
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value != _progress && Enumerable.Range(1,100).Contains(value))
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAwaitingDownload;

        /// <summary>
        /// Gets or sets the property that 
        /// </summary>
        public bool IsAwaitingDownload
        {
            get { return _isAwaitingDownload; }
            set
            {
                _isAwaitingDownload = value;
                OnPropertyChanged();
            }
        }

        private ICommand _downloadMapCommand;

        /// <summary>
        /// Gets the command to initiate the map download for offline use
        /// </summary>
        public ICommand DownloadMapCommand
        {
            get
            {
                return _downloadMapCommand ?? (_downloadMapCommand = new DelegateCommand(
                    async (x) =>
                    {
                        IsAwaitingDownload = false;
                        // call the download method and perform download
                        await DownloadPackageAsync(((Polygon)x).Extent);
                    }));
            }
        }

        private ICommand _cancelDownloadCommand;

        /// <summary>
        /// Gets the command to cancel the offline map download
        /// </summary>
        public ICommand CancelDownloadCommand
        {
            get
            {
                return _cancelDownloadCommand ?? (_cancelDownloadCommand = new DelegateCommand(
                    (x) =>
                    {
                        GenerateOfflineMapJob.Cancel();
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(false, Models.BroadcastMessageKey.SyncSucceeded);
                    }));
            }
        }

        /// <summary>
        /// Method that handles downloading the map into an offline map package
        /// </summary>
        private async Task DownloadPackageAsync(Envelope extent)
        {
            var syncTask = await OfflineMapTask.CreateAsync(Map);

            try
            {
                // set extent based on screen
                var parameters = await syncTask.CreateDefaultGenerateOfflineMapParametersAsync(extent);

                // set the job to generate the offline map
                GenerateOfflineMapJob = syncTask.GenerateOfflineMap(parameters, DownloadPath);

                // update the progress property when progress changes
                GenerateOfflineMapJob.ProgressChanged +=
                    (s, e) =>
                    {
                        Progress = GenerateOfflineMapJob.Progress;
                    };

                // listen for job changed events
                GenerateOfflineMapJob.JobChanged +=
                    async (sender, args) =>
                    {
                        // If the job succeeded, check for layer and table errors
                        // if job fails, get the error
                        if (GenerateOfflineMapJob.Status == JobStatus.Succeeded)
                        {
                            var result = await GenerateOfflineMapJob.GetResultAsync();

                            // download has succeeded and there were no errors, return
                            if (result.LayerErrors.Count == 0 && result.TableErrors.Count == 0)
                            {
                                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(true, BroadcastMessageKey.SyncSucceeded);
                            }
                            else
                            {
                                // if there were errors, create the error message to display to the user
                                var stringBuilder = new StringBuilder();

                                foreach (var error in result.LayerErrors)
                                {
                                    stringBuilder.AppendLine(error.Value.Message);
                                }

                                foreach (var error in result.TableErrors)
                                {
                                    stringBuilder.AppendLine(error.Value.Message);
                                }

                                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                    Properties.Resources.GetString("DownloadWithErrors_Title"), stringBuilder.ToString()
                                    , true, null);
                                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(true, BroadcastMessageKey.SyncSucceeded);
                            }
                        }
                        else if (GenerateOfflineMapJob.Status == JobStatus.Failed)
                        {
                            // if the job failed but not due to user cancellation, display the error 
                            if (GenerateOfflineMapJob.Error != null &&
                            GenerateOfflineMapJob.Error.Message != "User canceled: Job canceled.")
                            {
                                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                                Properties.Resources.GetString("DownloadFailed_Title"), GenerateOfflineMapJob.Error.Message, true, GenerateOfflineMapJob.Error.StackTrace);
                            }
                            BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(false, BroadcastMessageKey.SyncSucceeded);
                        }
                    };

                // begin download job
                GenerateOfflineMapJob.Start();
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                           null, ex.Message, true, ex.StackTrace);
            }
        }
    }
}
