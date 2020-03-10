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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.UWP.Converters
{
    class SelectedPivotIndexToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a Pivot's selected index to a visibility, returning <value>Visible</value> if
        /// selected index matches the parameter.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int inputIndex)
            {
                if (parameter is string targetValue && int.TryParse(targetValue, out var targetIndex))
                {
                    if (inputIndex == targetIndex)
                    {
                        return Visibility.Visible;
                    }
                }
            }
            
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
