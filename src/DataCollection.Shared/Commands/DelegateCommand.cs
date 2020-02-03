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
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Commands
{
    /// <summary>
    /// Generic delegate command
    /// </summary>
    internal class DelegateCommand : ICommand
    {
        private Action<object> _method;
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        public DelegateCommand(Action<object> method)
            : this(method, null)
        {
        }

        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class with parameter.
        /// </summary>
        public DelegateCommand(Action<object> method, Predicate<object> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Sets whether command is enabled or not
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) { return true; }
            return _canExecute(parameter);
        }

        /// <summary>
        /// Execute method for the command
        /// </summary>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                throw new ArgumentException("Command is not in an executable state");
            _method.Invoke(parameter);
        }

        /// <summary>
        /// Fires if can execute changes
        /// </summary>
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise can execute changes
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }
    }
}
