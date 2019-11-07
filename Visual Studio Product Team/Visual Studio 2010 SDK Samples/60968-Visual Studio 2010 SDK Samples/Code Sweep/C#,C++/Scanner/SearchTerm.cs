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

namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
{
    class SearchTerm : ISearchTerm
    {
        readonly string _text = "";
        readonly ITermTable _table;
        readonly string _class;
        readonly int _severity;
        readonly string _comment;
        readonly string _recommended;
        readonly List<IExclusion> _exclusions = new List<IExclusion>();

        /// <summary>
        /// Initializes the search term with the specified text.
        /// </summary>
        /// <param name="table">The table to which this term belongs.</param>
        /// <param name="text">The text to search for.</param>
        /// <param name="severity">The severity of the term, normally between 1 and 3 inclusive.</param>
        /// <param name="termClass">The class of the term, such as "Geopolitical".</param>
        /// <param name="comment">A descriptive comment for the term.</param>
        /// <param name="recommendedTerm">The recommended replacement; may be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <c>text</c> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <c>text</c> is an empty string.</exception>
        public SearchTerm(ITermTable table, string text, int severity, string termClass, string comment, string recommendedTerm)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length == 0)
            {
                throw new ArgumentException("Empty string not allowed", "text");
            }

            _table = table;
            _text = text;
            _severity = severity;
            _class = termClass;
            _comment = comment;
            _recommended = recommendedTerm;
        }

        public void AddExclusion(string text)
        {
            _exclusions.Add(new Exclusion(text, this));
        }

        #region ISearchTerm Members

        /// <summary>
        /// Gets the text to search for.
        /// </summary>
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

        /// <summary>
        /// Gets the list of phrases containing this term which should be excluded from the results.
        /// </summary>
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
