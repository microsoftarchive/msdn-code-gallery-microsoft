// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;

namespace MsdnReader
{
    public class LightValueToDesaturationFactorConverter :IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double desaturationFactor = 1.0;

            // check to see if the incoming value is a double. This value
            // should be bound to the current light value, in lux.
            if (value is double)
            {
                /*
                 * Approximate table of lux values as they map to human perceived values. 
                 * 0     -> 10     Dark
                 * 10    -> 300    Dim Indoor
                 * 300   -> 800    Normal Indoor
                 * 800   -> 10000  Bright Indoor
                 * 10000 -> 30000  Overcast Outdoor
                 * 30000 -> 100000 Direct sunlight
                 */
                double lightLevelInLux = (double)value;
                double lightLevelRangeValue = Math.Log10(lightLevelInLux) / Math.Log10(LightSensorProvider.MaximumSensorReportValue);

                desaturationFactor = Math.Min(1.0 - Math.Pow(lightLevelRangeValue, 2),1.0);
            }

            return desaturationFactor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class LightValueToBrightnessFactorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double brightness = -.5;
            // check to see if the incoming value is a double. This value
            // should be bound to the current light value, in lux.
            if (value is double)
            {
                /*
                 * Approximate table of lux values as they map to human perceived values. 
                 * 0     -> 10     Dark
                 * 10    -> 300    Dim Indoor
                 * 300   -> 800    Normal Indoor
                 * 800   -> 10000  Bright Indoor
                 * 10000 -> 30000  Overcast Outdoor
                 * 30000 -> 100000 Direct sunlight
                 */
                double lightLevelInLux = (double)value;
                double lightLevelRangeValue = Math.Log10(lightLevelInLux) / Math.Log10(LightSensorProvider.MaximumSensorReportValue);

                brightness = Math.Min(0,-lightLevelRangeValue + 0.5);
            }

            return brightness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class LightValueToContrastFactorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double contrast = 0.0;

            // check to see if the incoming value is a double. This value
            // should be bound to the current light value, in lux.
            if (value is double)
            {
                /*
                 * Approximate table of lux values as they map to human perceived values. 
                 * 0     -> 10     Dark
                 * 10    -> 300    Dim Indoor
                 * 300   -> 800    Normal Indoor
                 * 800   -> 10000  Bright Indoor
                 * 10000 -> 30000  Overcast Outdoor
                 * 30000 -> 100000 Direct sunlight
                 */
                double lightLevelInLux = (double)value;
                double lightLevelRangeValue = Math.Log10(lightLevelInLux) / Math.Log10(LightSensorProvider.MaximumSensorReportValue);

                contrast = Math.Pow(lightLevelRangeValue,2);
            }

            return contrast;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Converts the light level (in lux) to a corresponding font size. This enables the UI to bind
    /// to a dependency property light level in lux (LightSensorProvider.LuxValueProperty) to dynamically
    /// update font size.
    /// </summary>
    /// <example>
    /// First, you need to declare the object data provider for the ambient light sensor as well as the 
    /// converter as objects:
    /// 
    /// <ObjectDataProvider x:Key="LightSensorProvider" ObjectType="{x:Type local:LightSensorProvider}"/>
    /// <local:LightLevelToFontSizeConverter x:Key="LightLevelToFontSizeConverter"/>
    /// 
    /// Next, on the font size property that you want to bind to, use the following:
    /// 
    /// FontSize={Binding Source={StaticResource LightSensorProvider}, Path=LuxValue, Converter={StaticResource LightLevelToFontSizeConverter}, ConverterParameter=12pt}
    /// 
    /// This will change a standard 12pt font to be the correct ambient light value.
    /// </example>
    public class LightLevelToFontSizeConverter : IValueConverter
    {
        /// <summary>
        /// Compensate the scale of 0.0->1.0 for a font size such that we are
        /// increasing the font size based on ambient light conditions. This
        /// value is ideally from 0.8 to 1.0, resulting in a font multiplier from
        /// 0.8 to 1.8.
        /// </summary>
        private const double FontSizeMultiplierCompensation = 0.8;

        #region IValueConverter Members
        /// <summary>
        /// Convert an ambient light sensor value (in lux) to a corresponding font size
        /// for the current lighting conditions based on the default font size (stored in the parameter).
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // check to see if the incoming value is a double. This value
            // should be bound to the current light value, in lux.
            if (value is double)
            {
                /*
                 * Approximate table of lux values as they map to human perceived values. 
                 * 0     -> 10     Dark
                 * 10    -> 300    Dim Indoor
                 * 300   -> 800    Normal Indoor
                 * 800   -> 10000  Bright Indoor
                 * 10000 -> 30000  Overcast Outdoor
                 * 30000 -> 100000 Direct sunlight
                 */
                double lightLevelInLux = (double)value;

                // check to see if a converter parameter is provided. If it is, that parameter
                // is the "base" font size value to convert from.
                if (parameter != null)
                {
                    double fontSize = 0.0;

                    if (parameter is double)
                    {
                        // if the font size is a double, then simply cast the double to the font size.
                        fontSize = (double)parameter;
                    }
                    else
                    {
                        // see if we can convert from the parameter value, for example the string "12pt"
                        // will convert to a double fontsize value according to the current DPI settings.
                        FontSizeConverter fontConverter = new FontSizeConverter();
                        if (fontConverter.CanConvertFrom(parameter.GetType()))
                        {
                            fontSize = (double)fontConverter.ConvertFrom(parameter);
                        }
                        else
                        {
                            // convert from a default 12pt font value.
                            fontSize = (double)fontConverter.ConvertFrom("12pt");
                        }
                    }

                    System.Diagnostics.Debug.Assert(fontSize > 0);

                    // the correlation between light values in lux and human perceived brightness are
                    // not linear. To fit these two values, we need to convert the values from lux
                    // into a linear range from 0.0 <-> 1.0. We'll use that value to scale our font size
                    // up to compensate for the light.
                    // The calculation of Math.Log10(LightSensorProvider.MaximumSensorReportValue) is shown
                    // here for clarity's sake. Since this is a constant value, the calculation can be optimized
                    // into a constant.
                    double lightLevelRangeValue = Math.Log10(lightLevelInLux) / Math.Log10(LightSensorProvider.MaximumSensorReportValue);

                    // now that lightLevelRangeValue is between 0.0 <-> 1.0, we can scale the font size.
                    // since we don't want a zero size font for low light levels, we'll compensate by a constant
                    // value (0.8) such that we will adjust the font size parameter (stored in fontSize)
                    // by 80% to 180% (0.8 -> 1.8);

                    return (FontSizeMultiplierCompensation + lightLevelRangeValue) * fontSize;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(
                    false, 
                    "Binding value is not a double.", 
                    "Expect value to be a double specifying the light value in lux.");
            }
            return value;
        }

        /// <summary>
        /// Not supported, as there is no round trip conversion declared for font size.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException(); // There is no round-trip conversion supported for this property.
        }

        #endregion
    }
}