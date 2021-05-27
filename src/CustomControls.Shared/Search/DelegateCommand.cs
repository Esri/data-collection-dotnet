using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public class DelegateCommand : ICommand
    {
        private Action _action;
        public DelegateCommand(Action inputAction)
        {
            _action = inputAction;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }
}
