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

using System;
#if __UWP__
using Windows.UI.Xaml.Data;
using Culture = System.String;
using Windows.Foundation;
#else
using System.Windows.Data;
using Culture = System.Globalization.CultureInfo;
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Converters
{
    /// <summary>
    /// Converter either X or Y from a point object, applying an offset if specified.
    /// </summary>
    public class PointToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Returns the X or Y component of a point, applying an offset if specified.
        /// </summary>
        /// <param name="value">Point to extract the X or Y component from</param>
        /// <param name="parameter">Defines which component to extract and which offset to apply, in the format X|5, Y|10. Offset can be omitted</param>
        public object Convert(object value, Type targetType, object parameter, Culture language)
        {
            if (parameter is string positionString && value is Point screenPoint)
            {
                string position = null;
                double offset = 0;
                var parts = positionString.Split('|');

                if (parts.Length > 0)
                {
                    position = parts[0];
                }
                if (parts.Length > 1)
                {
                    double.TryParse(parts[1], out offset);
                }
                switch (position)
                {
                    case "Top":
                    case "Y":
                        return screenPoint.Y + offset;
                    case "Left":
                    case "X":
                        return screenPoint.X + offset;
                }
            }
            return value;
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, Culture language) => throw new NotImplementedException();
    }
}
