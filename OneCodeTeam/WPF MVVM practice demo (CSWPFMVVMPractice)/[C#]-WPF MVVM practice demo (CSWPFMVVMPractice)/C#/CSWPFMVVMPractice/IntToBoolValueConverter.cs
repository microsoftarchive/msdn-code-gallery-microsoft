/****************************** Module Header ******************************\
*   Module Name:  <IntToBoolValueConverter.cs>
*   Project:	<CSWPFMVVMPractice>
*   Copyright (c) Microsoft Corporation.
* 
*  As the name implies, this IntToBoolValueConverter class converts an int value to a bool value. 
*  This converter is used by the MenuItem on the main window that indicate the dimension of 
*  the game.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Windows.Data;

namespace CSWPFMVVMPractice
{  
    public class IntToBoolValueConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Convert.ToInt32(value) == Convert.ToInt32(parameter.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
