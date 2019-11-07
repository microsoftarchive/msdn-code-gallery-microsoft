//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Classification;

namespace ToDoGlyphFactory
{
    /// <summary>
    /// Empty ToDoTag class.
    /// </summary>
    internal class ToDoTag : IGlyphTag { }

    /// <summary>
    /// This class implements ITagger for ToDoTag.  It is responsible for creating
    /// ToDoTag TagSpans, which our GlyphFactory will then create glyphs for.
    /// </summary>
    internal class ToDoTagger : ITagger<ToDoTag>
    {
        #region Private members
        private IClassifier _aggregator;
        private const string _searchText = "todo";
        #endregion

        #region Constructors
        internal ToDoTagger(IClassifier aggregator)
        {
            _aggregator = aggregator;
        }
        #endregion

        #region ITagger<ToDoTag> Members

        /// <summary>
        /// This method creates ToDoTag TagSpans over a set of SnapshotSpans.
        /// </summary>
        /// <param name="spans">A set of spans we want to get tags for.</param>
        /// <returns>The list of ToDoTag TagSpans.</returns>
        IEnumerable<ITagSpan<ToDoTag>> ITagger<ToDoTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan span in spans)
            {
                // Look at each classification span inside the requested span
                foreach (ClassificationSpan classification in _aggregator.GetClassificationSpans(span))
                {
                    // If the classification is a comment...
                    if (classification.ClassificationType.Classification.ToLower().Contains("comment"))
                    {
                        // Look for the word "todo" in the comment.  If it is found then create a new ToDoTag TagSpan
                        int index = classification.Span.GetText().ToLower().IndexOf(_searchText);
                        if (index != -1)
                        {
                            yield return new TagSpan<ToDoTag>(new SnapshotSpan(classification.Span.Start + index, _searchText.Length),
                                                              new ToDoTag());
                        }
                    }
                }
            }
        }

#pragma warning disable 67
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67
        #endregion
    }
}
