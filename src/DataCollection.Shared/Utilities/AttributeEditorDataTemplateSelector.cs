//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
#if WPF
using System.Windows;
using System.Windows.Controls;
#elif NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities
{
    /// <summary>
    /// Helper class to select the correct DataTemplate based on the field's data type
    /// </summary>
    public class AttributeEditorDataTemplateSelector : DataTemplateSelector
    {
#if WPF
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (element != null && item != null && item is FieldContainer popupFieldValue)
            {
                if (popupFieldValue.OriginalField.FieldType == FieldType.Date)
                {
                    return element.FindResource("DateTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.Domain != null && popupFieldValue.OriginalField.Domain is CodedValueDomain)
                {
                    return element.FindResource("CodedValueDomainTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.Domain != null && popupFieldValue.OriginalField.Domain is RangeDomain<int>)
                {
                    return element.FindResource("IntegerRangeDomainTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.Domain != null && popupFieldValue.OriginalField.Domain is RangeDomain<float>)
                {
                    return element.FindResource("DoubleRangeDomainTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.FieldType == FieldType.Float32 || popupFieldValue.OriginalField.FieldType == FieldType.Float64)
                {
                    return element.FindResource("DoubleTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.FieldType == FieldType.Int16 || popupFieldValue.OriginalField.FieldType == FieldType.Int32)
                {
                    return element.FindResource("IntTemplate") as DataTemplate;
                }
                else
                {
                    return element.FindResource("StringTemplate") as DataTemplate;
                }
            }
            return null;
        }
#elif NETFX_CORE
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (element != null && item != null && item is FieldContainer popupFieldValue)
            {
                if (popupFieldValue.OriginalField.FieldType == FieldType.Date)
                {
                    return element.FindName("DateTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.Domain != null && popupFieldValue.OriginalField.Domain is CodedValueDomain)
                {
                    return element.FindName("CodedValueDomainTemplate") as DataTemplate;
                }
                else if ((popupFieldValue.OriginalField.Domain != null && popupFieldValue.OriginalField.Domain is RangeDomain<int>) ||
                    (popupFieldValue.OriginalField.Domain != null && popupFieldValue.OriginalField.Domain is RangeDomain<float>))
                {
                    return element.FindName("RangeDomainTemplate") as DataTemplate;
                }
                else if (popupFieldValue.OriginalField.FieldType == FieldType.Float32 || popupFieldValue.OriginalField.FieldType == FieldType.Float64 ||
                    popupFieldValue.OriginalField.FieldType == FieldType.Int16 || popupFieldValue.OriginalField.FieldType == FieldType.Int32)
                {
                    return element.FindName("NumberTemplate") as DataTemplate;
                }
                else
                {
                    return element.FindName("StringTemplate") as DataTemplate;
                }
            }
            return null;
        }
#endif
    }
}
