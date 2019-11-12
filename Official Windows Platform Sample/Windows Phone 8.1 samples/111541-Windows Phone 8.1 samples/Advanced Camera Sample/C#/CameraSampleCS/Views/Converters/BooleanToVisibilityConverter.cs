// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="BooleanToVisibilityConverter.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.Views.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// This class converts between <see cref="Visibility"/> and <see cref="bool"/> values.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts the <see cref="bool"/> value to the proper <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">
        /// An optional parameter to be used in the converter logic.<br/>
        /// If it's equal to the "<c>not</c>" string, the conversion result is inverted.
        /// </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// <paramref name="value"/> as a <see cref="Visibility"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool && (bool)value;

            return BooleanToVisibilityConverter.ApplyModificator(flag, parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts the <see cref="Visibility"/> value to the proper <see cref="bool"/> value.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">
        /// An optional parameter to be used in the converter logic.<br/>
        /// If it's equal to the "<c>not</c>" string, the conversion result is inverted.
        /// </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// <paramref name="value"/> as a <see cref="bool"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && BooleanToVisibilityConverter.ApplyModificator((Visibility)value == Visibility.Visible, parameter);
        }

        /// <summary>
        /// Applies the modification <paramref name="parameter"/> to the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Boolean value to modify.</param>
        /// <param name="parameter">Modification parameter.</param>
        /// <returns>
        /// If the <paramref name="parameter"/> is equal to "<c>not</c>", returns the reversed <paramref name="value"/>;
        /// otherwise, just returns the <paramref name="value"/>.
        /// </returns>
        private static bool ApplyModificator(bool value, object parameter)
        {
            return string.Equals("not", (string)parameter, StringComparison.OrdinalIgnoreCase) ? !value : value;
        }
    }
}
