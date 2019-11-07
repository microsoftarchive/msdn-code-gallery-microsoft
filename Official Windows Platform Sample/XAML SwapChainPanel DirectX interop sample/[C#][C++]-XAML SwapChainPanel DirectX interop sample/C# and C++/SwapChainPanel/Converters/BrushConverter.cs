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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SwapChainPanel
{
    /// <summary>
    /// An <see cref="IValueConverter"/> that converts between <see cref="Color"/> and <see cref="SolidColorBrush"/>.
    /// </summary>
    class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush((value is Color) ? (Color)value : Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is SolidColorBrush) ? (value as SolidColorBrush).Color : Colors.Black;
        }
    }
}
