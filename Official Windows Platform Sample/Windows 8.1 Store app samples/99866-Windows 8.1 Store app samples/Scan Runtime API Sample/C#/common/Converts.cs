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
using Windows.UI.Xaml;

namespace SDKTemplate.Common
{
    public  class UIntToVisibilityConverter  : Windows.UI.Xaml.Data.IValueConverter
    {
        virtual public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            return ((int)value) > 0? Visibility.Visible : Visibility.Collapsed;
        }

        virtual public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException("UIntToVisibilityConverter::ConvertBack not implemented");
        }
    }

    public class OutputHeightConverter : Windows.UI.Xaml.Data.IValueConverter
    {
        virtual public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            // Return 85% of the input value
            return ((double)(value) * 0.85);
        }

        virtual public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException("OutputHeightConverter::ConvertBack not implemented");
        }
    }
}
