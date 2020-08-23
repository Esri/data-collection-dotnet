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

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Control for showing a graphic, title, expand/collapse button, and close button for a card.
    /// </summary>
    public partial class CardBar
    {
        public CardBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Geometry of the path to show as the card's icon.
        /// </summary>
        /// <remarks>Set either the <see cref="ImageSource"/> or the <see cref="IconGeometry"/>, not both.</remarks>
        public object IconGeometry
        {
            get => GetValue(IconGeometryProperty);
            set => SetValue(IconGeometryProperty, value);
        }

        /// <summary>
        /// Image source to show as the card's icon.
        /// </summary>
        /// <remarks>Set either the <see cref="ImageSource"/> or the <see cref="IconGeometry"/>, not both.</remarks>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Title of the card
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Reference to the card this bar is associated with
        /// </summary>
        public CardBase OwningCard
        {
            get => (CardBase)GetValue(OwningCardProperty);
            set => SetValue(OwningCardProperty, value);
        }

        /// <summary>
        /// Command executed when close button is clicked.
        /// </summary>
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        #region Dependency properties to enable convenient binding
        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register(nameof(IconGeometry), typeof(object), typeof(CardBar), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(CardBar), new PropertyMetadata(null));
        
        public static readonly DependencyProperty OwningCardProperty =
            DependencyProperty.Register(nameof(OwningCard), typeof(CardBase), typeof(CardBar), new PropertyMetadata(null));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(CardBar), new PropertyMetadata(null));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CardBar), new PropertyMetadata(null));
        #endregion
    }
}
