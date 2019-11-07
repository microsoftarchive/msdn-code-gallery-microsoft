/****************************** Module Header ******************************\
 * Module Name:  BoolToStringConverter.cs
 * Project:      CSUnvsAppEnumRadioButton
 * Copyright (c) Microsoft Corporation.
 * 
 * This is converter of Boolean type and String type
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using Windows.UI.Xaml.Data;

namespace CSUnvsAppEnumRadioButton.Common
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "Male" : "Female";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string sex = value as string;

            if (sex == "Female")
                return false;
            return true;

        }
    }
}
