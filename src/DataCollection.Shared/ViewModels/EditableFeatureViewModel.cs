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

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Mapping.Popups;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public abstract class EditableFeatureViewModel : FeatureViewModel
    {
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

        /// <summary>
        /// Discards edits performed on a related record 
        /// </summary>
        internal async Task<bool> DiscardChanges()
        {
            if (PopupManager.HasEdits())
            {
                bool confirmCancelEdits = false;
                // wait for response from the user if the truly want to cancel the edit operation
                UserPromptMessenger.Instance.ResponseValueChanged += handler;

                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("DiscardEditsConfirmation_Title"),
                    Resources.GetString("DiscardEditsConfirmation_Message"),
                    false,
                    null,
                    Resources.GetString("DiscardButton_Content"));

                void handler(object o, UserPromptResponseChangedEventArgs e)
                {
                    {
                        UserPromptMessenger.Instance.ResponseValueChanged -= handler;
                        if (e.Response)
                        {
                            confirmCancelEdits = true;
                        }
                    }
                }

                if (!confirmCancelEdits)
                {
                    return false;
                }
            }

            // cancel the edits if the PopupManager doesn't have any edits or if the user chooses to
            EditViewModel.CancelEdits(PopupManager);
            EditViewModel = null;

            // reload the attachments to discard any changes that the user may have done to the Attachments list
            await AttachmentsViewModel.LoadAttachments();
            return true;
        }
    }
}
