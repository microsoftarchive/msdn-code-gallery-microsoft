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
    class MockTermTable : ITermTable
    {
        List<ISearchTerm> _termList = new List<ISearchTerm>();
        string _file;

        public MockTermTable(string sourceFile)
        {
            _file = sourceFile;
        }

        public void AddSearchTerm(ISearchTerm term)
        {
            _termList.Add(term);
        }

        #region ITermTable Members

        public string SourceFile
        {
            get { return _file; }
        }

        public IEnumerable<ISearchTerm> Terms
        {
            get { return _termList; }
        }

        #endregion
    }
}
