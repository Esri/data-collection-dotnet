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

using System;

#if __WPF__
using System.Windows.Data;
using CustomCultureInfo = System.Globalization.CultureInfo;
#endif

#if __UWP__
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using CustomCultureInfo = System.String;
#endif
namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls
{
    /// <summary>
    /// Converter designed to facilitate binding against the <see cref="ModernMapPanel.IsCollapsed"/> property
    /// to facilitate responsive layouts.
    /// </summary>
    public class ResponsiveValueConverter : IValueConverter
    {
        /// <summary>
        /// Chooses a value from the content of the parameter string based on the bound boolean.
        /// </summary>
        /// <param name="value">Boolean value.</param>
        /// <param name="targetType">Any object.</param>
        /// <param name="parameter">Intended values to choose between, in the format: 'valueIfTrue|valueIfFalse'.</param>
        /// <remarks>Intended for use with the <see cref="ModernMapPanel.IsCollapsed"/> property.</remarks>
        public object Convert(object value, Type targetType, object parameter, CustomCultureInfo culture)
        {
            if (value is bool isCollapsed)
            {
                if (parameter is string stringParam)
                {
                    var parts = stringParam.Split('|');
                    if (parts.Length > 1)
                    {
                        if (isCollapsed)
                        {
#if __UWP__
                            return XamlBindingHelper.ConvertValue(targetType, parts[0]);
#else
                            return parts[0];
#endif
                        }
                        else
                        {
#if __UWP__
                            return XamlBindingHelper.ConvertValue(targetType, parts[1]);
#else
                            return parts[1];
#endif
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CustomCultureInfo culture) => throw new NotImplementedException();
    }
}
