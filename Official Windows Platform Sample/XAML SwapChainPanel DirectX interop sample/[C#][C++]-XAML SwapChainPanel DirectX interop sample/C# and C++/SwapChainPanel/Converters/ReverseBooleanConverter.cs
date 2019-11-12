//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SwapChainPanel
{
    /// <summary>
    /// An <see cref="IValueConverter"/> that negates a <see cref="Boolean"/> value.
    /// </summary>
    class ReverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool) ? !(bool)value : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is bool) ? !(bool)value : false;
        }

    }
}
