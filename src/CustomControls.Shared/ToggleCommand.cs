/*******************************************************************************
  * Copyright 2020 Esri
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
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls
{
    /// <summary>
    /// Command that toggles the associated <see cref="CardBase"/>'s minimize/maximize state.
    /// </summary>
    public class ToggleCommand : ICommand
    {
        /// <summary>
        /// Raised when <see cref="CanExecute(object)"/> changes. This event will not be raised.
        /// </summary>
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        private CardBase _cardBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleCommand"/> class.
        /// </summary>
        /// <param name="cb">The associated <see cref="CardBase"/> whose minimize/maximize state will be toggled.</param>
        public ToggleCommand(CardBase cb)
        {
            _cardBase = cb;
        }

        /// <summary>
        /// Always returns true.
        /// </summary>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Toggles the associated card's minimize/maximize state.
        /// </summary>
        /// <param name="parameter">Ignored.</param>
        public void Execute(object parameter)
        {
            if (_cardBase.CardState == CardState.Maximized)
            {
                _cardBase.CardState = CardState.Minimized;
            }
            else
            {
                _cardBase.CardState = CardState.Maximized;
            }
        }
    }
}
