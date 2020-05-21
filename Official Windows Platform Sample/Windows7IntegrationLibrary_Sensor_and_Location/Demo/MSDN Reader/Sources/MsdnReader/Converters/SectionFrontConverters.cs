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
using System.Globalization;
using Microsoft.SceReader.Data;

namespace MsdnReader
{
    public class BestStoryImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Story story = value as Story;
            if (story != null)
            {
                if (story.ImageReferenceCollection != null)
                {
                    foreach (ImageReference imageRef in story.ImageReferenceCollection)
                    {
                        if ( (imageRef.LargestImageData.Width / imageRef.LargestImageData.Height) > 1.2)
                        {
                            return imageRef;
                        }
                    }
                }
            }



            return null;

           

        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }

    }


}