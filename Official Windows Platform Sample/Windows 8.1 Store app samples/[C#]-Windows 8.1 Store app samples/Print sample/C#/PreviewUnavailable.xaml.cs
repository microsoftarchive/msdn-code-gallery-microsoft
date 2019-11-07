// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PrintSample
{
    public sealed partial class PreviewUnavailable : Page
    {
        public PreviewUnavailable()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Preview unavailable page constructor
        /// </summary>
        /// <param name="paperSize">The printing page paper size</param>
        /// <param name="printableSize">The usable/"printable" area on the page</param>
        public PreviewUnavailable(Size paperSize, Size printableSize)
            :this()
        {
            page.Width = paperSize.Width;
            page.Height = paperSize.Height;

            printablePage.Width = printableSize.Width;
            printablePage.Height = printableSize.Height;
        }
    }
}
