/*******************************************************************************
  * Copyright 2018 Esri
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

using Esri.ArcGISRuntime.Mapping.Popups;
using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for AttachmentsView.xaml
    /// </summary>
    public partial class AttachmentsView : UserControl
    {
        public AttachmentsView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Creates a PopupManager property
        /// </summary>
        public static readonly DependencyProperty PopupManagerProperty = DependencyProperty.Register(
            "PopupManager", typeof(PopupManager), typeof(AttachmentsView), new PropertyMetadata(null, OnPopupManagerChanged));

        /// <summary>
        /// Invoked when the  ViewPoint value has changed
        /// </summary>
        private static void OnPopupManagerChanged(DependencyObject bindable, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PopupManager)
            {
                var x = e.NewValue;
            }
        }

        public PopupManager PopupManager
        {
            get { return GetValue(PopupManagerProperty) as PopupManager; }
            set { SetValue(PopupManagerProperty, value); }
        }
    }
}
