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
using System.Printing;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Xps;
using Microsoft.SceReader.Data.Feed;
using System.Xml.XPath;
using Microsoft.SceReader.Converters;
using System.Windows.Data;
using System.Globalization;
using Microsoft.SceReader.Data;
using Microsoft.SceReader;

namespace MsdnReader
{
    public class MsdnPrintManager : PrintManager
    {
        #region Protected Methods

        /// <summary>
        /// Performs processing of story Nitf document after it is fetched
        /// </summary>
        protected override void OnGetXmlDocumentCompleted(GetXmlDocumentCompletedEventArgs e)
        {
            MsdnToPrintableStoryDocumentConverter printConverter = new MsdnToPrintableStoryDocumentConverter();
            printConverter.ConversionCompleted += ConversionCompleted;
            Story story = ((DataRequest)e.UserState).UserState as Story;
            printConverter.ConvertAsync(e.Document, story, ServiceProvider.ConverterManager.FlowDocumentStyleProvider, PrintTarget);
        }        

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for conversion completed event from print document converter
        /// </summary>
        private void ConversionCompleted(object sender, ConversionCompletedEventArgs e)
        {
            if (e.UserState == PrintTarget)
            {
                MsdnToPrintableStoryDocumentConverter printConverter = sender as MsdnToPrintableStoryDocumentConverter;
                if (printConverter != null)
                {
                    printConverter.ConversionCompleted -= ConversionCompleted;
                    OnConversionCompleted(e);
                }
            }
        }

        #endregion
    }
}