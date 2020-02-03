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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
#if NETFX_CORE
using System;
using Windows.UI.Core;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels
{
    /// <summary>
    /// Base View Model that all View Models inherit
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Raises the <see cref="BaseViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
#if NETFX_CORE
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
#else
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
#endif
            }
            catch (Exception ex)
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("GenericError_Title"),
                    ex.Message,
                    true,
                    ex.StackTrace);

                // This exception should never happen; if it does, the app is in an unknown state and should terminate.
                Environment.FailFast("Error invoking property change event handler.", ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
