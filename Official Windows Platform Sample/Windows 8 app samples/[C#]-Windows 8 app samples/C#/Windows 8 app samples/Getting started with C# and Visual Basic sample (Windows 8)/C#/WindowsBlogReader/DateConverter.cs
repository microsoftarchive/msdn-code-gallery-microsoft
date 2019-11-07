//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************


using System;
using Windows.Globalization.DateTimeFormatting;

namespace WindowsBlogReader
{
    public class DateConverter : Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Value cannot be null.");

            if (!typeof(DateTime).Equals(value.GetType()))
                throw new ArgumentException("Value must be of type DateTime.", "value");

            DateTime dt = (DateTime)value;

            if (parameter == null)
            {
                // Date "7/27/2011 9:30:59 AM" returns "7/27/2011"
                return DateTimeFormatter.ShortDate.Format(dt);
            }
            else if ((string)parameter == "day")
            {
                // Date "7/27/2011 9:30:59 AM" returns "27"
                DateTimeFormatter dateFormatter = new DateTimeFormatter("{day.integer(2)}");
                return dateFormatter.Format(dt);
            }
            else if ((string)parameter == "month")
            {
                // Date "7/27/2011 9:30:59 AM" returns "JUL"
                DateTimeFormatter dateFormatter = new DateTimeFormatter("{month.abbreviated(3)}");
                return dateFormatter.Format(dt).ToUpper();
            }
            else if ((string)parameter == "year")
            {
                // Date "7/27/2011 9:30:59 AM" returns "2011"
                DateTimeFormatter dateFormatter = new DateTimeFormatter("{year.full}");
                return dateFormatter.Format(dt);
            }
            else
            {
                // Requested format is unknown. Return in the original format.
                return dt.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return Windows.UI.Xaml.DependencyProperty.UnsetValue;
        }
    }
}
