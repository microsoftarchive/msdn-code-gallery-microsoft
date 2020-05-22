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
using Microsoft.SceReader.Converters;
using System.Windows.Documents;
using System.Xml.XPath;
using Microsoft.SceReader.Data;
using System.IO;
using System.Windows;

namespace MsdnReader
{
    public class MsdnStoryToFlowDocumentConverter : StoryDocumentConverter, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is XPathDocument))
            {
                throw new ArgumentException("Story FlowDocument is created only from XPathDocument");
            }

            return Convert((XPathDocument)value, parameter as Story);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Public Methods

        public virtual FlowDocument Convert(XPathDocument document, Story story)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            //if(something in the doc signifies that this is an MSDN Magazine article)
            MsdnMagazineDocumentToFlowDocumentConverter magazineConverter = new MsdnMagazineDocumentToFlowDocumentConverter();
            return magazineConverter.Convert(document, story);

            //else use the other converter (ie MsdnLibraryDocumentToFlowDocumentConverter)

            //XPathNavigator navigator = document.CreateNavigator();
            //XPathNavigator msdn = GetMsdnNavigator(navigator);
            //if (msdn == null)
            //{
            //    NitfToFlowDocumentConverter conv = new NitfToFlowDocumentConverter();
            //    return conv.Convert(document, styleProvider);                
            //    //throw new InvalidDataException("Root element missing");
            //}

            //return GetFlowDocumentFromNavigator(msdn, null, styleProvider);
        }

        #endregion

    }
}