/****************************** Module Header ******************************\
* Module Name:      CustomRichTextBox.cs
* Project:                   CSWinFormSearchAndHighlightText
* Copyright(c)          Microsoft Corporation.
* 
* Theclass is used to create custom RichTextBox
* The custom RichTextBox add custom Highlight and ClearHighlight method
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CSWinFormSearchAndHighlightText
{
    /// <summary>
    ///  Custom RichTextBox control
    /// </summary>
    public partial class CustomRichTextBox :RichTextBox
    {
        public CustomRichTextBox():base()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  Search and Highlight all text in RichTextBox Control 
        /// </summary>
        /// <param name="findWhat">Find What</param>
        /// <param name="highlightColor">Highlight Color</param>
        /// <param name="ismatchCase">Is Match Case</param>
        /// <param name="ismatchWholeWord">Is Match Whole Word</param>
        /// <returns></returns>
        public bool Highlight(string findWhat, Color highlightColor, bool ismatchCase, bool ismatchWholeWord)
        {
            // Clear all highlights before searching text again
            ClearHighlight();

            int startSearch = 0;
            //int searchLength = findWhat.Length;
            RichTextBoxFinds findoptions = default(RichTextBoxFinds);

            // Setup the search options.
            if (ismatchCase && ismatchWholeWord)
            {
                findoptions = RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord;
            }
            else if (ismatchCase)
            {
                findoptions = RichTextBoxFinds.MatchCase;
            }
            else if (ismatchWholeWord)
            {
                findoptions = RichTextBoxFinds.WholeWord;
            }
            else
            {
                findoptions = RichTextBoxFinds.None;
            }

            // detect whether search text exists in richtextbox
            bool isfind = false;
            int index = -1;

            // Search text in RichTextBox and highlight them with color.
            while ((index = this.Find(findWhat, startSearch, findoptions)) > -1)
            {
                isfind = true;

                this.SelectionBackColor = highlightColor;
                
                // Continue after the one we searched
                startSearch = index+1;
            }

            // If the text exist in RichTextBox control, then return true, otherwise, return false
            return isfind;
        }

        /// <summary>
        ///  Clear all Highlights 
        /// </summary>
        private void ClearHighlight()
        {
            this.SelectAll();
            this.SelectionBackColor = Color.White;
        }
    }
}
