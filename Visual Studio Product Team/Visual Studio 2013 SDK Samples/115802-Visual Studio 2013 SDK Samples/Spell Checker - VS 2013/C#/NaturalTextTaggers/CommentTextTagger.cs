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

﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.Language.Spellchecker
{
    /// <summary>
    /// Provides tags for Doc Comment regions
    /// </summary>
    internal class CommentTextTagger : ITagger<NaturalTextTag>
    {
        #region Private Fields
        private ITextBuffer _buffer;
        private IClassifier _classifier;
        #endregion

        #region MEF Imports / Exports
        /// <summary>
        /// MEF connector for the Natural Text Tagger.
        /// </summary>
        [Export(typeof(ITaggerProvider))]
        [ContentType("code")]
        [TagType(typeof(NaturalTextTag))]
        internal class CommentTextTaggerProvider : ITaggerProvider
        {
            #region MEF Imports
            [Import]
            internal IClassifierAggregatorService ClassifierAggregatorService { get; set; }
            #endregion

            #region ITaggerProvider
            public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }

                return new CommentTextTagger(buffer, ClassifierAggregatorService.GetClassifier(buffer)) as ITagger<T>;
            }

            #endregion
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Natural Text Tagger.
        /// </summary>
        /// <param name="buffer">Relevant buffer.</param>
        /// <param name="classifiers">List of all available classifiers.</param>
        public CommentTextTagger(ITextBuffer buffer, IClassifier classifier)
        {
            _buffer = buffer;
            _classifier = classifier;
        }
        #endregion

        #region ITagger<NaturalTextTag> Members
        /// <summary>
        /// Returns tags on demand.
        /// </summary>
        /// <param name="spans">Spans collection to get tags for.</param>
        /// <returns>Tags in provided spans.</returns>
        public IEnumerable<ITagSpan<NaturalTextTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_classifier != null)
            {
                foreach (var snapshotSpan in spans)
                {
                    Debug.Assert(snapshotSpan.Snapshot.TextBuffer == _buffer);
                    foreach (ClassificationSpan classificationSpan in _classifier.GetClassificationSpans(snapshotSpan))
                    {
                        if (classificationSpan.ClassificationType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("comment") ||
                            classificationSpan.ClassificationType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("string"))
                        {
                            yield return new TagSpan<NaturalTextTag>(
                                    classificationSpan.Span,
                                    new NaturalTextTag()
                                    );
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
