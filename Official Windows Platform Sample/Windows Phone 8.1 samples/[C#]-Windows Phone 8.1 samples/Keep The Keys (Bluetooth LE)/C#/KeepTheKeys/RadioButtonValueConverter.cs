using System;

using AlertLevel = KeepTheKeysCommon.AlertLevel;
using DependencyProperty = Windows.UI.Xaml.DependencyProperty;

namespace KeepTheKeys
{
    // This class converts between a value of an enumerated type and a boolean suitable for a binding
    // from the IsChecked property of a RadioButton to an enum-typed property of an object.
    // The converter parameter should be the name of the enum value for that RadioButton.

    public class RadioButtonValueConverter<EnumType> : Windows.UI.Xaml.Data.IValueConverter
        where EnumType : struct
    {
        // Convert from enum value to boolean. Return true iff value == converter parameter.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(Enum.Parse(typeof(EnumType), (string)parameter));
        }

        // Convert from bool to enum value. If the input is true, return the parameter value.
        // Otherwise return DependencyProperty.UnsetValue, which means that the value will not
        // be propagated from the UI control to the bound property.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return Enum.Parse(typeof(EnumType), (string)parameter);
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
    public class AlertLevelRadioButtonValueConverter : RadioButtonValueConverter<AlertLevel> { }
}
