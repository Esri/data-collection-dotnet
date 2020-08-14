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
using System.ComponentModel;
using System.Windows.Input;
#if __UWP__
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif __WPF__
using System.Windows;
using System.Windows.Controls;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls
{
    /// <summary>
    /// Base class to be used for children of <see cref="ModernMapPanel"/> playing the role of <see cref="MapRole.Card"/>.
    /// </summary>
    public class CardBase : UserControl
    {
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(CardBase), new PropertyMetadata(false, HandleIsOpenChanged));

        public event PropertyChangedEventHandler PropertyChanged;

        private static void HandleIsOpenChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea)
        {
            (dpo as CardBase)?.PropertyChanged?.Invoke(dpo, new PropertyChangedEventArgs(nameof(IsOpen)));
        }

        private static void HandleCardStateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea)
        {
            (dpo as CardBase)?.PropertyChanged?.Invoke(dpo, new PropertyChangedEventArgs(nameof(CardState)));
        }

        public CardState CardState
        {
            get => (CardState)GetValue(CardStateProperty);
            set => SetValue(CardStateProperty, value);
        }

        // Using a DependencyProperty as the backing store for CardState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardStateProperty =
            DependencyProperty.Register(nameof(CardState), typeof(CardState), typeof(CardBase), new PropertyMetadata(CardState.Minimized, HandleCardStateChanged));

        public ModernMapPanel ParentPanel
        {
            get => (ModernMapPanel)GetValue(ParentPanelProperty);
            set => SetValue(ParentPanelProperty, value);
        }

        // Using a DependencyProperty as the backing store for ParentPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register(nameof(ParentPanel), typeof(ModernMapPanel), typeof(CardBase), new PropertyMetadata(null));

        private ToggleCommand _toggleCommand;
        public ToggleCommand ToggleStateCommand => _toggleCommand ?? (_toggleCommand = new ToggleCommand(this));
    }

    public class ToggleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CardBase _cardBase;

        public ToggleCommand(CardBase cb)
        {
            _cardBase = cb;
        }
        public bool CanExecute(object parameter) => true;

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
