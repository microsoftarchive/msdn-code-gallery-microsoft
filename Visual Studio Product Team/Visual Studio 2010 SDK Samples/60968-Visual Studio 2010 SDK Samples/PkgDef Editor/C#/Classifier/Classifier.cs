//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace PkgDefLanguage
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("pkgdef")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class PkgDefClassifierProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate() { return new PkgDefClassifier(buffer, aggregatorFactory, ClassificationTypeRegistry) as ITagger<T>; };
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
        }
    }

    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    internal sealed class PkgDefClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<PkgDefTokenTag> _aggregator;
        IDictionary<PkgDefLanguageTokens, IClassificationType> _pkgdefTypes;

        internal PkgDefClassifier(ITextBuffer buffer,
                                  IBufferTagAggregatorFactoryService aggregatorFactory, 
                                  IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = aggregatorFactory.CreateTagAggregator<PkgDefTokenTag>(buffer);

            _aggregator.TagsChanged += new EventHandler<TagsChangedEventArgs>(_aggregator_TagsChanged);

            // create mapping from token types to classification types
            _pkgdefTypes = new Dictionary<PkgDefLanguageTokens, IClassificationType>();
            _pkgdefTypes[PkgDefLanguageTokens.Token] = typeService.GetClassificationType("pkgdef.token");
            _pkgdefTypes[PkgDefLanguageTokens.Guid] = typeService.GetClassificationType("pkgdef.guid");
            _pkgdefTypes[PkgDefLanguageTokens.Comment] = typeService.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            _pkgdefTypes[PkgDefLanguageTokens.Key] = typeService.GetClassificationType("pkgdef.key");
            _pkgdefTypes[PkgDefLanguageTokens.ValueName] = typeService.GetClassificationType("pkgdef.value.name");
            _pkgdefTypes[PkgDefLanguageTokens.ValueString] = typeService.GetClassificationType("pkgdef.value.string");
            _pkgdefTypes[PkgDefLanguageTokens.ValueBinary] = typeService.GetClassificationType(PredefinedClassificationTypeNames.Number);
            _pkgdefTypes[PkgDefLanguageTokens.ValueType] = typeService.GetClassificationType(PredefinedClassificationTypeNames.Keyword);
        }

        void _aggregator_TagsChanged(object sender, TagsChangedEventArgs e)
        {
            var temp = this.TagsChanged;
            if (temp != null)
            {
                NormalizedSnapshotSpanCollection spans = e.Span.GetSpans(_buffer.CurrentSnapshot);
                if (spans.Count > 0)
                {
                    SnapshotSpan span = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End);
                    temp(this, new SnapshotSpanEventArgs(span));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Translate each PkgDefTokenTag to an appropriate ClassificationTag
        /// </summary>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                if (_pkgdefTypes.ContainsKey(tagSpan.Tag.type))
                {
                    var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                    yield return
                        new TagSpan<ClassificationTag>(tagSpans[0],
                                                   new ClassificationTag(_pkgdefTypes[tagSpan.Tag.type]));
                }
            }
        }
    }
}
