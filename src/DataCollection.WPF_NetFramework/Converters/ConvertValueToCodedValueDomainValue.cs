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
using System.Linq;
using Esri.ArcGISRuntime.Data;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.WPF.Converters
{
    /// <summary>
    /// Converts a field value representing a coded value domain value to and from a CodedValue object.
    /// </summary>
    class ConvertValueToCodedValueDomainValue : IMultiValueConverter
    {
        /// <summary>
        /// Takes in a list of CodedValue objects and a code and returns the CodedValue object represented by that code
        /// </summary>
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] is the list of all the CodedValue objects available for that field
            // values[1] is the code for the actual CodedValue of the field 
            if (values[0] != null && values[0] is IReadOnlyList<CodedValue> && values[1] != null)
            {
                var CodedValues = values[0] as IReadOnlyList<CodedValue>;
                return CodedValues.Where(x => x.Code.ToString() == values[1].ToString()).Select(x => x).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Takes in a CodedValue object and returns the code for the object
        /// </summary>
        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // object[2] is an array of 2 objects because ConvertBack expects to return a multivalue
            // but only the actual code is needed so the first value is set to null
            if (value != null && value is CodedValue codedValue)
            {
                return new object[2] { null, codedValue.Code };
            }
            return new object[2] { null, null };
        }
    }
}
