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

using System;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers
{
    /// <summary>
    /// Messenger class to handle communication between the viewmodels and the dialog box displayed when app requires user input
    /// </summary>
    internal class UserPromptMessenger
    {
        private static UserPromptMessenger _instance;

        /// <summary>
        /// Gets the instance of the <see cref="UserPromptMessenger"/> singleton
        /// </summary>
        public static UserPromptMessenger Instance
        {
            get
            {
                return _instance ?? (_instance = new UserPromptMessenger());
            }
        }

        public event EventHandler<UserPromptMessageChangedEventArgs> MessageValueChanged;

        /// <summary>
        /// Event handler for when the messages is updated
        /// </summary>
        public void RaiseMessageValueChanged(string messageTitle, string message, bool isError, string stackTrace = null, 
            string affirmativeActionButtonContent = null, string negativeActionButtonContent = null)
        {
            MessageValueChanged?.Invoke(this, new UserPromptMessageChangedEventArgs()
            {
                Message = message,
                StackTrace = stackTrace,
                IsError = isError,
                MessageTitle = messageTitle,
                AffirmativeActionButtonContent = affirmativeActionButtonContent,
                NegativeActionButtonContent = negativeActionButtonContent
            });
        }

        public event EventHandler<UserPromptResponseChangedEventArgs> ResponseValueChanged;

        /// <summary>
        /// Event handler for when the response is updated
        /// </summary>
        public void RaiseResponseValueChanged(bool response)
        {
            ResponseValueChanged?.Invoke(this, new UserPromptResponseChangedEventArgs()
            {
                Response = response
            });
        }

        /// <summary>
        /// Awaitable task that handles the need for a user response
        /// </summary>
        public Task<bool> AwaitConfirmation(string messageTitle, string message, bool isError, string stackTrace = null,
            string affirmativeActionButtonContent = null, string negativeActionButtonContent = null, bool shouldCancel = false)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            Instance.ResponseValueChanged += handler;

            Instance.RaiseMessageValueChanged(messageTitle, message, isError, stackTrace, affirmativeActionButtonContent, negativeActionButtonContent);

            void handler(object o, UserPromptResponseChangedEventArgs e)
            {
                {
                    Instance.ResponseValueChanged -= handler;
                    if (e.Response)
                    {
                        taskCompletionSource.TrySetResult(e.Response);
                    }
                    else if (shouldCancel)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                }
            }
            return taskCompletionSource.Task;
        }
    }
}
