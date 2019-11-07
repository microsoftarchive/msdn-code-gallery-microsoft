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
    class MockScanHit : IScanHit
    {
        readonly string _file;
        readonly int _line;
        readonly int _column;
        readonly ISearchTerm _term;
        readonly string _lineText;
        readonly string _warning;

        public MockScanHit(string file, int line, int column, string lineText, ISearchTerm term, string warning)
        {
            _file = file;
            _line = line;
            _column = column;
            _term = term;
            _lineText = lineText;
            _warning = warning;
        }

        #region IScanHit Members

        public string FilePath
        {
            get { return _file; }
        }

        public int Line
        {
            get { return _line; }
        }

        public int Column
        {
            get { return _column; }
        }

        public string LineText
        {
            get { return _lineText; }
        }

        public ISearchTerm Term
        {
            get { return _term; }
        }

        public string Warning
        {
            get { return _warning; }
        }

        #endregion
    }
}
