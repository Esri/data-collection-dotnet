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

using ControlzEx.Native;
using ControlzEx.Standard;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Implements the responsive custom titlebar
    /// </summary>
    public partial class TitleBar
    {
        public TitleBar()
        {
            InitializeComponent();

            // Do work that can only be done when the titlebar is loaded
            Loaded += TitleBarLoaded;
        }

        /// <summary>
        /// Updates the minimize icon once this loads, which is when <see cref="ThisWindow"/> becomes available.
        /// </summary>
        private void TitleBarLoaded(object sender, RoutedEventArgs e)
        {
            // Minimize icon can only be updated once the window is available.
            if (ThisWindow != null) {
                UpdateMinimizeIcon();
            }

            Loaded -= TitleBarLoaded;
        }

        /// <summary>
        /// Holds a reference to the window.
        /// </summary>
        private MainWindow _window;

        /// <summary>
        /// Gets the window associated with this view.
        /// </summary>
        private MainWindow ThisWindow => _window ?? (_window = Window.GetWindow(this) as MainWindow);

        /// <summary>
        /// Implement responsive behavior by setting properties according to the new rendered size.
        /// </summary>
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

        /// <summary>
        /// Update the minimize/maximize button based on the window's current state
        /// </summary>
        internal void MainWindow_StateChanged(object sender, EventArgs e) => UpdateMinimizeIcon();

        /// <summary>
        /// Handle minimize button press
        /// </summary>
        private void OnMinimizeWindow(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(ThisWindow);

        /// <summary>
        /// Handles maximize/minimize button presses
        /// </summary>
        private void OnMaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(ThisWindow);
            }
            else if (ThisWindow.WindowState == WindowState.Normal)
            {
                SystemCommands.MaximizeWindow(ThisWindow);
            }
        }

        /// <summary>
        /// Updates the minimize/maximize button based on the window's current state
        /// </summary>
        private void UpdateMinimizeIcon()
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

        /// <summary>
        /// Handle close button clicks.
        /// </summary>
        private void OnCloseWindow(object sender, RoutedEventArgs e) => ThisWindow.Close();

        /// <summary>
        /// Corrects popup behavior with toggle button.
        /// </summary>
        private void ToggleButton_MouseEnter(object sender, MouseEventArgs e) => UserPopup.StaysOpen = OfflinePopup.StaysOpen = OnlinePopup.StaysOpen = AppInfoPopup.StaysOpen = true;

        /// <summary>
        /// Corrects popup behavior with toggle button.
        /// </summary>
        private void ToggleButton_MouseLeave(object sender, MouseEventArgs e) => UserPopup.StaysOpen = OfflinePopup.StaysOpen = OnlinePopup.StaysOpen = AppInfoPopup.StaysOpen = false;

        #region Custom window management based on Fluent Ribbon's RibbonWindow implementation
        // Code based on Fluent.Ribbon's implementation
        // Source: https://github.com/fluentribbon/Fluent.Ribbon/blob/ad5c70ffc120b6676394e68438b4c2fc04e5ba90/Fluent.Ribbon/Controls/WindowSteeringHelperControl.cs 
        //     and https://github.com/fluentribbon/Fluent.Ribbon/blob/ad5c70ffc120b6676394e68438b4c2fc04e5ba90/Fluent.Ribbon/Helpers/WindowSteeringHelper.cs
        // MIT license: Copyright (c) 2009-2018 Bastian Schmidt, Degtyarev Daniel, Rikker Serg (https://github.com/fluentribbon/Fluent.Ribbon)

        /// <summary>
        /// Show system menu on right click in titlebar.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            e.Handled = true;
            #pragma warning disable 618
            ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenu(ThisWindow, e);
            #pragma warning restore 618
        }

        /// <summary>
        /// Defines the CriticalHandlePropertyInfo.
        /// </summary>
        private static readonly PropertyInfo CriticalHandlePropertyInfo = typeof(Window).GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Defines the EmptyObjectArray.
        /// </summary>
        private static readonly object[] EmptyObjectArray = new object[0];

        /// <summary>
        /// Handle mouse clicks on titlebar
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                e.Handled = true;

                ThisWindow.VerifyAccess();
#pragma warning disable 618
                // for the touch usage
                UnsafeNativeMethods.ReleaseCapture();

                var criticalHandle = (IntPtr)CriticalHandlePropertyInfo.GetValue(ThisWindow, EmptyObjectArray);

                NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
                NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
#pragma warning restore 618
            }
            else if (e.ClickCount == 2 && ThisWindow.ResizeMode == ResizeMode.CanResize || ThisWindow.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                e.Handled = true;

                if (ThisWindow.WindowState == WindowState.Normal)
                {
                    SystemCommands.MaximizeWindow(ThisWindow);
                }
                else
                {
                    SystemCommands.RestoreWindow(ThisWindow);
                }
            }
        }
        #endregion
    }
}
