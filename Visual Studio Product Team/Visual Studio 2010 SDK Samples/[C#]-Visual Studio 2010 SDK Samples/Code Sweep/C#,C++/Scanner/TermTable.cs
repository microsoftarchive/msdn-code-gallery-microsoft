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
using System.Xml;
using System.Globalization;

namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
{
    class TermTable : ITermTable
    {
        readonly string _filePath;
        readonly List<ISearchTerm> _terms = new List<ISearchTerm>();

        public TermTable(string filePath)
        {
            _filePath = filePath;

            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            foreach (XmlNode node in document.SelectNodes("xmldata/PLCKTT/Lang/Term"))
            {
                string text = node.Attributes["Term"].InnerText;
                int severity = Int32.Parse(node.Attributes["Severity"].InnerText, CultureInfo.InvariantCulture);
                string termClass = node.Attributes["TermClass"].InnerText;
                string comment = node.SelectSingleNode("Comment").InnerText;

                string recommended = null;
                XmlNode recommendedNode = node.SelectSingleNode("RecommendedTerm");
                if (recommendedNode != null)
                {
                    recommended = recommendedNode.InnerText;
                }

                SearchTerm term = new SearchTerm(this, text, severity, termClass, comment, recommended);

                foreach (XmlNode exclusionNode in node.SelectNodes("Exclusion"))
                {
                    term.AddExclusion(exclusionNode.InnerText);
                }
                foreach (XmlNode exclusionNode in node.SelectNodes("ExclusionContext"))
                {
                    term.AddExclusion(exclusionNode.InnerText);
                }

                _terms.Add(term);
            }

            if (_terms.Count == 0)
            {
                throw new ArgumentException("The file did not specify a valid term table.", "filePath");
            }
        }

        #region ITermTable Members

        public string SourceFile
        {
            get { return _filePath; }
        }

        public IEnumerable<ISearchTerm> Terms
        {
            get { return _terms; }
        }

        #endregion
    }
}
