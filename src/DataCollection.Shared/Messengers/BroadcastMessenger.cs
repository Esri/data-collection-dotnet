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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers
{
    /// <summary>
    /// Messenger class to handle communication between classes when app settings change
    /// </summary>
    class BroadcastMessenger
    {
        private static BroadcastMessenger _instance;

        /// <summary>
        /// Gets the instance of the <see cref="BroadcastMessenger"/> singleton
        /// </summary>
        public static BroadcastMessenger Instance
        {
            get
            {
                return _instance ?? (_instance = new BroadcastMessenger());
            }
        }

        public event EventHandler<BroadcastMessengerEventArgs> BroadcastMessengerValueChanged;

        /// <summary>
        /// Event handler for when a setting is updated
        /// </summary>
        public void RaiseBroadcastMessengerValueChanged(object o, BroadcastMessageKey broadcastMessageKey)
        {
            BroadcastMessengerValueChanged?.Invoke(this, new BroadcastMessengerEventArgs()
            {
                Args = new KeyValuePair<BroadcastMessageKey, object>(broadcastMessageKey, o)
            });
        }

        /// <summary>
        /// Awaitable task that handles the need for a response
        /// </summary>
        public Task<object> AwaitMessageValueChanged(BroadcastMessageKey key)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            Instance.BroadcastMessengerValueChanged += handler;

            void handler(object o, BroadcastMessengerEventArgs e)
            {
                if (e.Args.Key == key)
                {
                    Instance.BroadcastMessengerValueChanged -= handler;
                    Instance.RaiseBroadcastMessengerValueChanged(o, key);
                    taskCompletionSource.TrySetResult(e.Args.Value);
                }
            }

            return taskCompletionSource.Task;
        }
    }
}
