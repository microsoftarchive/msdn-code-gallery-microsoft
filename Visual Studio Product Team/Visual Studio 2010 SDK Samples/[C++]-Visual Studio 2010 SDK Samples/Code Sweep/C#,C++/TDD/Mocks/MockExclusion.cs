/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockExclusion : IExclusion
    {
        string _text;
        ISearchTerm _term;

        public MockExclusion(string text, ISearchTerm term)
        {
            _text = text;
            _term = term;
        }

        #region IExclusion Members

        public string Text
        {
            get { return _text; }
        }

        public ISearchTerm Term
        {
            get { return _term; }
        }

        #endregion
    }
}
