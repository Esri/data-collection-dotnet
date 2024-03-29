﻿/*******************************************************************************
  * Copyright 2019 Esri
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

using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Esri.ArcGISRuntime.Data;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views.Cards
{
    public sealed partial class FeatureEditorView : UserControl
    {
        public FeatureEditorView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler gets the selected item and sets the tag. The tag is used to create the two way binding
        /// This is a workaround until domains are available on PopupManager
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (((CodedValue)comboBox.SelectedItem).Code.ToString() != comboBox.Tag?.ToString())
                comboBox.Tag = ((CodedValue)comboBox.SelectedItem).Code;
        }

        /// <summary>
        /// Event handler sets the selected item when the combo box loads
        /// This is a workaround until domains are available on PopupManager
        /// </summary>
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.Tag != null)
            {
                var selectedItem = comboBox.Items.FirstOrDefault(i => ((CodedValue)i).Code.ToString() == comboBox.Tag.ToString());
                comboBox.SelectedItem = selectedItem;
            }
        }
    }
}
