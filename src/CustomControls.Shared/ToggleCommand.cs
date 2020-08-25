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
        public event EventHandler CanExecuteChanged;

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
