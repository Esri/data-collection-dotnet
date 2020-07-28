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
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Utilities;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using ControlzEx.Behaviors;
using Microsoft.Xaml.Behaviors;
using ControlzEx.Native;
using ControlzEx.Standard;
using System.Reflection;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindowChromeBehavior();

            // create event handler for when the message from the viewmodel changes
            UserPromptMessenger.Instance.MessageValueChanged += DialogBoxMessenger_MessageValueChanged;
            BusyWaitingMessenger.Instance.WaitStatusChanged += OnWaitStatusChanged;

            this.StateChanged += MainWindow_StateChanged;
            this.Unloaded += OnUnloaded;

            _mainViewModel = TryFindResource("MainViewModel") as MainViewModel;

            // load settings for the custom tree survey dataset
            LoadTreeSurveySettings();

            UpdateMinizeIcon();
        }

        private void InitializeWindowChromeBehavior()
        {
            var behavior = new WindowChromeBehavior();
            behavior.EnableMinimize = true;
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });

            Interaction.GetBehaviors(this).Add(behavior);
        }



        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResizeBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(MainWindow), new PropertyMetadata(new Thickness(6)));



        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            UpdateMinizeIcon();
            // Works around weird bug on Windows 10 with multiple, mixed-DPI monitors.
            // Solution inspired by https://www.codesd.com/item/windowstyle-none-and-maximized-window-hack-does-not-work-with-multiple-monitors.html
        }

        private void OnWaitStatusChanged(object sender, WaitStatusChangedEventArgs e)
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.IsBusyWaiting = e.IsBusy;
                _mainViewModel.BusyWaitingMessage = e.Message;
            }
        }

        private void InstanceOnBecameBusyWaiting(object sender, EventArgs e)
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.IsBusyWaiting = true;
            }
        }

        /// <summary>
        /// Handles changes in messages coming from the viewmodel by opening a dialog box and prompting the user for answer
        /// </summary>
        private void DialogBoxMessenger_MessageValueChanged(object sender, UserPromptMessageChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DialogBoxWindow dialogBox = new DialogBoxWindow(e.Message, e.MessageTitle, e.IsError, e.StackTrace, e.AffirmativeActionButtonContent, e.NegativeActionButtonContent);
                dialogBox.Closing += DialogBox_Closing;

                dialogBox.ShowDialog();
            });
        }

        /// <summary>
        /// Handles closing the dialog box by raising response value changed event
        /// </summary>
        private void DialogBox_Closing(object sender, CancelEventArgs e)
        {
            UserPromptMessenger.Instance.RaiseResponseValueChanged(((DialogBoxWindow)sender).Response);
        }

        /// <summary>
        /// Loads the settings for the custom workflows required bu the tree survey sample dataset
        /// </summary>
        private void LoadTreeSurveySettings()
        {
            // retrieve settings for the tree dataset specific workflows
            TreeSurveyWorkflows.NeighborhoodNameField = Settings.Default.NeighborhoodNameField;
            TreeSurveyWorkflows.GeocodeUrl = Settings.Default.GeocodeUrl;
            TreeSurveyWorkflows.WebmapURL = Settings.Default.WebmapURL;
            TreeSurveyWorkflows.TreeDatasetWebmapUrl = Settings.Default.TreeDatasetWebmapUrl;
            TreeSurveyWorkflows.OfflineLocatorPath = Settings.Default.OfflineLocatorPath;
            TreeSurveyWorkflows.TreeConditionAttribute = Settings.Default.TreeConditionAttribute;
            TreeSurveyWorkflows.TreeDBHAttribute = Settings.Default.TreeDBHAttribute;
            TreeSurveyWorkflows.InspectionConditionAttribute = Settings.Default.InspectionConditionAttribute;
            TreeSurveyWorkflows.InspectionDBHAttribute = Settings.Default.InspectionDBHAttribute;
            TreeSurveyWorkflows.NeighborhoodOperationalLayerId = Settings.Default.NeighborhoodOperationalLayerId;
            TreeSurveyWorkflows.NeighborhoodAttribute = Settings.Default.NeighborhoodAttribute;
            TreeSurveyWorkflows.AddressAttribute = Settings.Default.AddressAttribute;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from events.
            this.Unloaded += OnUnloaded;
            UserPromptMessenger.Instance.MessageValueChanged += DialogBoxMessenger_MessageValueChanged;
            BusyWaitingMessenger.Instance.WaitStatusChanged += OnWaitStatusChanged;
        }

        private void OnDragMoveWindow(object sender, MouseButtonEventArgs e)
    {
            if (e.ButtonState == MouseButtonState.Pressed)
                try
                {
                    if (this.WindowState == WindowState.Maximized)
                    {
                        this.WindowState = WindowState.Normal;
                    }
                    this.DragMove();
                }
                catch (Exception ex)
                {
                    // Ignore
                }
    }

    private void OnMinimizeWindow(object sender, RoutedEventArgs e)
    {
            SystemCommands.MinimizeWindow(this);
    }
    private void OnMaximizeWindow(object sender, RoutedEventArgs e)
    {
            if (this.WindowState == WindowState.Maximized)
            {
                ControlzEx.Windows.Shell.SystemCommands.RestoreWindow(this);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                ControlzEx.Windows.Shell.SystemCommands.MaximizeWindow(this);
            }
    }
        private void UpdateMinizeIcon()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MinimizePath.Visibility = Visibility.Visible;
                MaximizePath.Visibility = Visibility.Collapsed;
            }
            else
            {
                MinimizePath.Visibility = Visibility.Collapsed;
                MaximizePath.Visibility = Visibility.Visible;
            }
        }
    private void OnCloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
        #region windowsteering
        // Source: https://github.com/fluentribbon/Fluent.Ribbon/blob/ad5c70ffc120b6676394e68438b4c2fc04e5ba90/Fluent.Ribbon/Controls/WindowSteeringHelperControl.cs 
        //     and https://github.com/fluentribbon/Fluent.Ribbon/blob/ad5c70ffc120b6676394e68438b4c2fc04e5ba90/Fluent.Ribbon/Helpers/WindowSteeringHelper.cs
        // MIT license: Copyright (c) 2009-2018 Bastian Schmidt, Degtyarev Daniel, Rikker Serg (https://github.com/fluentribbon/Fluent.Ribbon)

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.IsEnabled)
            {
                HandleMouseLeftButtonDown(e, true, true);
            }
        }

        /// <inheritdoc />
        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (this.IsEnabled)
            {
                ShowSystemMenu(this, e);
            }
        }

        private static readonly PropertyInfo criticalHandlePropertyInfo = typeof(Window).GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly object[] emptyObjectArray = new object[0];

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="e">The mouse event args.</param>
        /// <param name="handleDragMove">Defines if window dragging should be handled.</param>
        /// <param name="handleStateChange">Defines if window state changes should be handled.</param>
        public static void HandleMouseLeftButtonDown(MouseButtonEventArgs e, bool handleDragMove, bool handleStateChange)
        {
            var dependencyObject = e.Source as DependencyObject;

            if (dependencyObject == null)
            {
                return;
            }

            HandleMouseLeftButtonDown(dependencyObject, e, handleDragMove, handleStateChange);
        }

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="dependencyObject">The object which was the source of the mouse event.</param>
        /// <param name="e">The mouse event args.</param>
        /// <param name="handleDragMove">Defines if window dragging should be handled.</param>
        /// <param name="handleStateChange">Defines if window state changes should be handled.</param>
        public static void HandleMouseLeftButtonDown(DependencyObject dependencyObject, MouseButtonEventArgs e, bool handleDragMove, bool handleStateChange)
        {
            var window = Window.GetWindow(dependencyObject);

            if (window == null)
            {
                return;
            }

            if (handleDragMove
                && e.ClickCount == 1)
            {
                e.Handled = true;

                // taken from DragMove internal code
                window.VerifyAccess();

                // for the touch usage
                UnsafeNativeMethods.ReleaseCapture();

                var criticalHandle = (IntPtr)criticalHandlePropertyInfo.GetValue(window, emptyObjectArray);
                // DragMove works too, but not on maximized windows
                NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
                NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            }
            else if (handleStateChange
                && e.ClickCount == 2
                && (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip))
            {
                e.Handled = true;

                if (window.WindowState == WindowState.Normal)
                {
                    ControlzEx.Windows.Shell.SystemCommands.MaximizeWindow(window);
                }
                else
                {
                    ControlzEx.Windows.Shell.SystemCommands.RestoreWindow(window);
                }
            }
        }


        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="dependencyObject">The object which was the source of the mouse event.</param>
        /// <param name="e">The mouse event args.</param>
        public static void ShowSystemMenu(DependencyObject dependencyObject, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(dependencyObject);

            if (window == null)
            {
                return;
            }

            ShowSystemMenu(window, e);
        }

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="window">The window for which the system menu should be shown.</param>
        /// <param name="e">The mouse event args.</param>
        public static void ShowSystemMenu(Window window, MouseButtonEventArgs e)
        {
            e.Handled = true;

            ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenu(window, e);
        }

        /// <summary>
        /// Shows the system menu at <paramref name="screenLocation"/>.
        /// </summary>
        /// <param name="window">The window for which the system menu should be shown.</param>
        /// <param name="screenLocation">The location at which the system menu should be shown.</param>
        public static void ShowSystemMenu(Window window, Point screenLocation)
        {
            ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenu(window, screenLocation);
        }
        #endregion
    }
}
