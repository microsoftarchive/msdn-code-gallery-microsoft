/****************************** Module Header ******************************\
 * Module Name:  DoubleToMoneyString.cs
 * Project:      CSWindowsStoreAppFlightDataFilter
 * Copyright (c) Microsoft Corporation.
 * 
 * DoubleToMoneyString converter. 
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

namespace FlightDataFilter.Converters
{
    public class DoubleToMoneyString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double price = (double)value;
            return string.Format("{0:C2}", price);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
