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

using System;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers
{
    /// <summary>
    /// Messenger class to handle communication between the viewmodels and the dialog box displayed when app requires user input
    /// </summary>
    internal class BusyWaitingMessenger
    {
        private static BusyWaitingMessenger _instance;

        /// <summary>
        /// Gets the instance of the <see cref="UserPromptMessenger"/> singleton
        /// </summary>
        public static BusyWaitingMessenger Instance
        {
            get
            {
                return _instance ?? (_instance = new BusyWaitingMessenger());
            }
        }

        public event EventHandler<WaitStatusChangedEventArgs> WaitStatusChanged;

        /// <summary>
        /// Event handler for when the messages is updated
        /// </summary>
        public void SetBusy(bool isBusy, string message)
        {
            WaitStatusChanged?.Invoke(this, new WaitStatusChangedEventArgs
            {
                IsBusy = isBusy,
                Message =  message
            });
        }
    }
}
