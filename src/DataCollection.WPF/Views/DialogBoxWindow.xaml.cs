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
using System.Windows;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for DialogBoxWindow.xaml
    /// </summary>
    public partial class DialogBoxWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogBoxWindow"/> class.
        /// </summary>
        public DialogBoxWindow(string message, string messageTitle, bool isError, string stackTrace, string affirmativeActionButtonContent, string negativeActionButtonContent)
        {
            Message = message;
            if (messageTitle == null && isError)
            {
                MessageTitle = Shared.Properties.Resources.GetString("GenericError_Title");
            }
            else
            {
                MessageTitle = messageTitle;
            }
            StackTrace = stackTrace;
            IsError = isError;
            AffirmativeActionButtonContent = affirmativeActionButtonContent ?? Shared.Properties.Resources.GetString("GenericAffirmativeButton_Content");
            NegativeActionButtonContent = negativeActionButtonContent ?? Shared.Properties.Resources.GetString("GenericNegativeButton_Content");
            InitializeComponent();
        }

        /// <summary>
        /// Gets the message to be shown to the user
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the stack trace to be shown to the user
        /// </summary>
        public string StackTrace { get; }

        /// <summary>
        /// Gets the type of message that will be shown to the user
        /// The message type could be Error or Dialog
        /// </summary>
        public bool IsError { get; }

        /// <summary>
        /// Gets the title of the emessage to be displayed to the user
        /// </summary>
        public string MessageTitle { get; }

        /// <summary>
        /// Gets or sets the response received from the user
        /// </summary>
        public bool Response { get; private set; }

        /// <summary>
        /// Gets or sets the content of the affirmative button to be shown to the user
        /// If nothing is speficied, the default is "OK"
        /// </summary>
        public string AffirmativeActionButtonContent { get; private set; }

        /// <summary>
        /// Gets or sets the content of the negative button to be shown to the user
        /// If nothing is speficied, the default is "Cancel"
        /// </summary>
        public string NegativeActionButtonContent { get; private set; }

        private ICommand _dismissDialogBoxCommand;

        /// <summary>
        /// Gets the command to dismiss the dialog box and return the user response
        /// </summary>
        public ICommand DismissDialogBoxCommand
        {
            get
            {
                return _dismissDialogBoxCommand ?? (_dismissDialogBoxCommand = new DelegateCommand(
                    (x) =>
                    {
                        if (x.ToString() == "true")
                        {
                            Response = true;
                        }
                        else
                        {
                            Response = false;
                        }

                        // close the window
                        this.Close();
                    }));
            }
        }
    }
}
