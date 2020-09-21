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
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Views
{
    /// <summary>
    /// A user control that shows title, image/icon, and tools for a card.
    /// </summary>
    public sealed partial class CardBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardBar"/> class.
        /// </summary>
        public CardBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the geometry of the path to show as the card's icon.
        /// </summary>
        public string IconGeometry { get => (string)GetValue(IconGeometryProperty); set => SetValue(IconGeometryProperty, value); }

        /// <summary>
        /// Gets or sets the image source to show as the card's icon.
        /// </summary>
        public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }

        /// <summary>
        /// Gets or sets the Title of the card.
        /// </summary>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>
        /// Gets or sets the card this bar is associated with.
        /// </summary>
        public CardBase OwningCard { get => (CardBase)GetValue(OwningCardProperty); set => SetValue(OwningCardProperty, value); }

        /// <summary>
        /// Gets or sets the command executed when close button is clicked.
        /// </summary>
        public ICommand CloseCommand { get => (ICommand)GetValue(CloseCommandProperty); set => SetValue(CloseCommandProperty, value); }

        /// <summary>
        /// Enables binding the <see cref="IconGeometry"/> property.
        /// </summary>
        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register(nameof(IconGeometry), typeof(string), typeof(CardBar), new PropertyMetadata(null));

        /// <summary>
        /// Enables binding the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(CardBar), new PropertyMetadata(null));

        /// <summary>
        /// Enables binding the <see cref="OwningCard"/> property.
        /// </summary>
        public static readonly DependencyProperty OwningCardProperty =
            DependencyProperty.Register(nameof(OwningCard), typeof(CardBase), typeof(CardBar), new PropertyMetadata(null));

        /// <summary>
        /// Enables binding the <see cref="CloseCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(CardBar), new PropertyMetadata(null));

        /// <summary>
        /// Enables binding the <see cref="ImageSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CardBar), new PropertyMetadata(null));
    }
}
