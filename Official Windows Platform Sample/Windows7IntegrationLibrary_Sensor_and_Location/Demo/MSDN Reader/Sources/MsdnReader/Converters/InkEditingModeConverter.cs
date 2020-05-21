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
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace MsdnReader
{
    /// <summary>
    /// Converts InkEditingMode to bool - the output is true if the expect mode is the same as the  actual mode
    /// </summary>
    public class InkEditingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                InkCanvasEditingMode expectedMode = (InkCanvasEditingMode)parameter;
                InkCanvasEditingMode currentMode = (InkCanvasEditingMode)value;

                // If the current EditingMode is the mode which menuitem is expecting, return true for IsChecked.
                if (currentMode == expectedMode)
                {
                    return true;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}