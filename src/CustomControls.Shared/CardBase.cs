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
    public class CardBase : UserControl
    {
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CardBase), new PropertyMetadata(false, HandleIsOpenChanged));

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
            get { return (CardState)GetValue(CardStateProperty); }
            set { SetValue(CardStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardStateProperty =
            DependencyProperty.Register("CardState", typeof(CardState), typeof(CardBase), new PropertyMetadata(CardState.Minimized, HandleCardStateChanged));



        public ModernMapPanel ParentPanel
        {
            get { return (ModernMapPanel)GetValue(ParentPanelProperty); }
            set { SetValue(ParentPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentPanelProperty =
            DependencyProperty.Register("ParentPanel", typeof(ModernMapPanel), typeof(CardBase), new PropertyMetadata(null));

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
