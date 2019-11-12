using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CSAddHintText2Textbox
{

    //****************************** Module Header ******************************\
    //Module Name:    TextInputToVisibilityConverter.cs
    //Project:        CSAddHintText2Textbox
    //Copyright (c) Microsoft Corporation

    // The project illustrates how to check whether a file is in use or not.

    //This source is subject to the Microsoft Public License.
    //See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
    //All other rights reserved.

    //*****************************************************************************/

    public class TextInputToVisibilityConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Test for non-null
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];

                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
