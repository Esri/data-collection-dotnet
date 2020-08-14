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
    public class PointToDoubleConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, Culture language)
        {
            throw new NotImplementedException();
        }
    }
}
