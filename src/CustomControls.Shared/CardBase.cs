/*******************************************************************************
  * Copyright 2020 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  https://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using System.ComponentModel;

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
        /// <summary>
        /// Backing field for <see cref="ToggleStateCommand"/>.
        /// </summary>
        private ToggleCommand _toggleCommand;

        /// <summary>
        /// Gets or sets this card's open/closed state.
        /// </summary>
        public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        /// <summary>
        /// Enables binding to the open/closed state of this card.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(CardBase), new PropertyMetadata(false, HandleIsOpenChanged));

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> when the open/closed state changes.
        /// </summary>
        /// <param name="dpo">The <see cref="DependencyObject"/>.</param>
        /// <param name="dpcea">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void HandleIsOpenChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea) => (dpo as CardBase)?.PropertyChanged?.Invoke(dpo, new PropertyChangedEventArgs(nameof(IsOpen)));

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> when the minimize/maximize state changes.
        /// </summary>
        /// <param name="dpo">The <see cref="DependencyObject"/>.</param>
        /// <param name="dpcea">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void HandleCardStateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs dpcea) => (dpo as CardBase)?.PropertyChanged?.Invoke(dpo, new PropertyChangedEventArgs(nameof(CardState)));

        /// <summary>
        /// Gets or sets this card's minimize/maximize state.
        /// </summary>
        public CardState CardState { get => (CardState)GetValue(CardStateProperty); set => SetValue(CardStateProperty, value); }

        /// <summary>
        /// Enables binding to the minimize/maximize state of the card.
        /// </summary>
        public static readonly DependencyProperty CardStateProperty =
            DependencyProperty.Register(nameof(CardState), typeof(CardState), typeof(CardBase), new PropertyMetadata(CardState.Minimized, HandleCardStateChanged));

        /// <summary>
        /// Reference to parent panel is needed for binding convenience.
        /// </summary>
        public ModernMapPanel ParentPanel { get => (ModernMapPanel)GetValue(ParentPanelProperty); set => SetValue(ParentPanelProperty, value); }

        /// <summary>
        /// Enables binding to the parent panel.
        /// </summary>
        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register(nameof(ParentPanel), typeof(ModernMapPanel), typeof(CardBase), new PropertyMetadata(null));

        /// <summary>
        /// Command for toggling this cards minimize/maximize state.
        /// </summary>
        public ToggleCommand ToggleStateCommand => _toggleCommand ?? (_toggleCommand = new ToggleCommand(this));
    }
}
