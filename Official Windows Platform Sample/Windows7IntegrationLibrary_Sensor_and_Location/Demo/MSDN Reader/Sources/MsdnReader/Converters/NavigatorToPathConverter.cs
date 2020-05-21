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
using System.Windows.Data;
using Microsoft.SceReader.View;
using Microsoft.SceReader.Data;

namespace MsdnReader
{
    public class NavigatorToPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = String.Empty;
            SectionNavigator nearestSection = value as SectionNavigator;
            if (nearestSection == null)
            {
                StoryNavigator storyNavigator = value as StoryNavigator;
                if (storyNavigator != null)
                {
                    nearestSection = storyNavigator.GetParent() as SectionNavigator;
                }
            }

            // Build a path to the section
            while (nearestSection != null)
            {
                Section section = nearestSection.Content as Section;
                if (String.IsNullOrEmpty(path))
                {
                    // Initialize
                    path = section.Title;
                }
                else
                {
                    path = String.Concat(section.Title, " : ", path);
                }
                nearestSection = nearestSection.GetParent() as SectionNavigator;
            }
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}