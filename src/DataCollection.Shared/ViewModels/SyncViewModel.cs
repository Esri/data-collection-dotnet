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
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Offline;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    class SyncViewModel : BaseViewModel
    {
        public SyncViewModel(Map map)
        {
            SyncMap(map).ContinueWith(t =>{});
        }

        /// <summary>
        /// Gets or sets the OfflineMapSyncJob
        /// </summary>
        internal OfflineMapSyncJob OfflineMapSyncJob { get; private set; }

        private int _progress = 0;

        /// <summary>
        /// Gets or sets the progress for the sync
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value != _progress)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        private ICommand _cancelSyncCommand;

        /// <summary>
        /// Gets the command to cancel the offline map download
        /// </summary>
        public ICommand CancelSyncCommand
        {
            get
            {
                return _cancelSyncCommand ?? (_cancelSyncCommand = new DelegateCommand(
                    (x) =>
                    {
                        try
                        {
                            OfflineMapSyncJob.Cancel();
                        }
                        catch { }
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(false, Models.BroadcastMessageKey.SyncSucceeded);
                    }));
            }
        }

        /// <summary>
        /// Method that handles syncing the offline and online maps
        /// </summary>
        private async Task SyncMap(Map map)
        {
            var syncTask = await OfflineMapSyncTask.CreateAsync(map);

            // set parameters for sync
            var taskParams = new OfflineMapSyncParameters()
            {
                SyncDirection = SyncDirection.Bidirectional,
                RollbackOnFailure = true
            };

            try
            {
                // set the job to perform the sync
                OfflineMapSyncJob = syncTask.SyncOfflineMap(taskParams);

                // update the progress property when progress changes
                OfflineMapSyncJob.ProgressChanged +=
                    (s, e) =>
                    {
                        Progress = OfflineMapSyncJob.Progress;
                    };

                // listen for job changed events
                OfflineMapSyncJob.JobChanged += (s, e) =>
                {
                    // if sync succeeds, raise success message
                    if (OfflineMapSyncJob.Status == JobStatus.Succeeded)
                    {
                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(true, Models.BroadcastMessageKey.SyncSucceeded);
                    }
                    else if (OfflineMapSyncJob.Status == JobStatus.Failed)
                    {
                        // if the job failed display the error, unless the user has cancelled it on purpose
                        if (OfflineMapSyncJob.Error != null && OfflineMapSyncJob.Error.Message != "User canceled: Job canceled.")
                        {
                            UserPromptMessenger.Instance.RaiseMessageValueChanged(
                            null, OfflineMapSyncJob.Error.Message, true, OfflineMapSyncJob.Error.StackTrace);
                        }

                        BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(false, Models.BroadcastMessageKey.SyncSucceeded);
                    }
                };

                // begin sync job
                OfflineMapSyncJob.Start();
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                           null, ex.Message, true, ex.StackTrace);

                BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(false, Models.BroadcastMessageKey.SyncSucceeded);
            }
        }
    }
}
