// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Doto
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
            VisibleValue = true;
        }

        public bool VisibleValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value.GetType() != typeof(System.Boolean))
            {
                return Visibility.Collapsed;
            }
            bool interpreted = System.Convert.ToBoolean(value);
            return interpreted == VisibleValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value.GetType() != typeof(Visibility))
            {
                return !VisibleValue;
            }

            Visibility visibility = (Visibility)value;

            return visibility == Visibility.Visible ? VisibleValue : !VisibleValue;
        }
    }
}
