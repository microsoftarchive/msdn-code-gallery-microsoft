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
    class MockTerm : ISearchTerm
    {
        readonly string _text;
        readonly int _severity;
        readonly string _class;
        readonly string _comment;
        List<IExclusion> _exclusions = new List<IExclusion>();
        readonly ITermTable _table;
        readonly string _recommended;

        public MockTerm(string text, int severity, string termClass, string comment, string recommended, ITermTable table)
        {
            _text = text;
            _severity = severity;
            _class = termClass;
            _comment = comment;
            _table = table;
            _recommended = recommended;
        }

        public void AddExclusion(string exclusion)
        {
            _exclusions.Add(new MockExclusion(exclusion, this));
        }

        #region ISearchTerm Members

        public string Text
        {
            get { return _text; }
        }

        public int Severity
        {
            get { return _severity; }
        }

        public string Class
        {
            get { return _class; }
        }

        public string Comment
        {
            get { return _comment; }
        }

        public string RecommendedTerm
        {
            get { return _recommended; }
        }

        public IEnumerable<IExclusion> Exclusions
        {
            get { return _exclusions; }
        }

        public ITermTable Table
        {
            get { return _table; }
        }

        #endregion
    }
}
