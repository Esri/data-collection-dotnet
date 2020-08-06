﻿using ControlzEx.Behaviors;
using ControlzEx.Native;
using ControlzEx.Standard;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        private MainWindow _window;
        private MainWindow ThisWindow => _window ?? (_window = Window.GetWindow(this) as MainWindow);


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            var size = sizeInfo.NewSize;

            if (size.Width < 600)
            {
                TitleIconContainer.Visibility = Visibility.Collapsed;

                Grid.SetRow(CenterButtonContainer, 1);
                Grid.SetColumn(CenterButtonContainer, 0);
                Grid.SetColumnSpan(CenterButtonContainer, 5);
                CenterButtonContainer.HorizontalAlignment = HorizontalAlignment.Center;

                Grid.SetColumn(RightPanel, 3);
                Grid.SetRow(RightPanel, 0);
                Grid.SetColumnSpan(RightPanel, 1);


            }
            else if (size.Width < 900)
            {
                TitleIconContainer.Visibility = Visibility.Collapsed;

                Grid.SetRow(CenterButtonContainer, 0);
                Grid.SetColumn(CenterButtonContainer, 0);
                Grid.SetColumnSpan(CenterButtonContainer, 3);
                CenterButtonContainer.HorizontalAlignment = HorizontalAlignment.Left;

                Grid.SetColumn(RightPanel, 2);
                Grid.SetRow(RightPanel, 0);
                Grid.SetColumnSpan(RightPanel, 1);
            }
            else
            {
                TitleIconContainer.Visibility = Visibility.Visible;
                Grid.SetRow(TitleIconContainer, 0);
                Grid.SetColumn(TitleIconContainer, 0);

                Grid.SetRow(CenterButtonContainer, 0);
                Grid.SetColumn(CenterButtonContainer, 1);
                Grid.SetColumnSpan(CenterButtonContainer, 1);

                Grid.SetColumn(RightPanel, 2);
                Grid.SetRow(RightPanel, 0);
                Grid.SetColumnSpan(RightPanel, 1);


            }
        }

        internal void MainWindow_StateChanged(object sender, EventArgs e)
        {
            UpdateMinizeIcon();
            // Works around weird bug on Windows 10 with multiple, mixed-DPI monitors.
            // Solution inspired by https://www.codesd.com/item/windowstyle-none-and-maximized-window-hack-does-not-work-with-multiple-monitors.html
        }
        private void OnDragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                try
                {
                    if (ThisWindow.WindowState == WindowState.Maximized)
                    {
                        ThisWindow.WindowState = WindowState.Normal;
                    }
                    ThisWindow.DragMove();
                }
                catch (Exception ex)
                {
                    // Ignore
                }
        }

        private void OnMinimizeWindow(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(ThisWindow);
        }
        private void OnMaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                ControlzEx.Windows.Shell.SystemCommands.RestoreWindow(ThisWindow);
            }
            else if (ThisWindow.WindowState == WindowState.Normal)
            {
                ControlzEx.Windows.Shell.SystemCommands.MaximizeWindow(ThisWindow);
            }
        }
        private void UpdateMinizeIcon()
        {
            if (ThisWindow.WindowState == WindowState.Maximized)
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
            ThisWindow.Close();
        }
        #region windowsteering
        // Source: https://github.com/fluentribbon/Fluent.Ribbon/blob/ad5c70ffc120b6676394e68438b4c2fc04e5ba90/Fluent.Ribbon/Controls/WindowSteeringHelperControl.cs 
        //     and https://github.com/fluentribbon/Fluent.Ribbon/blob/ad5c70ffc120b6676394e68438b4c2fc04e5ba90/Fluent.Ribbon/Helpers/WindowSteeringHelper.cs
        // MIT license: Copyright (c) 2009-2018 Bastian Schmidt, Degtyarev Daniel, Rikker Serg (https://github.com/fluentribbon/Fluent.Ribbon)

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (ThisWindow.IsEnabled)
            {
                HandleMouseLeftButtonDown(e, true, true);
            }
        }

        /// <inheritdoc />
        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (ThisWindow.IsEnabled)
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

        private void WorkOnlineButton_MouseEnter(object sender, MouseEventArgs e)
        {
            UserPopup.StaysOpen = true;
            OfflinePopup.StaysOpen = true;
            OnlinePopup.StaysOpen = true;
        }

        private void WorkOnlineButton_MouseLeave(object sender, MouseEventArgs e)
        {
            UserPopup.StaysOpen = false;
            OfflinePopup.StaysOpen = false;
            OnlinePopup.StaysOpen = false;
        }
        private void OfflinePopup_Closed(object sender, EventArgs e)
        {
            (this.DataContext as MainViewModel).IsOfflinePanelOpen = false;
        }

        private void OnlinePopup_Closed(object sender, EventArgs e)
        {
            (this.DataContext as MainViewModel).IsMapStatusPanelOpen = false;
        }

        private void UserPopup_Closed(object sender, EventArgs e)
        {
            (this.DataContext as MainViewModel).IsUserPanelOpen = false;
        }
    }
}