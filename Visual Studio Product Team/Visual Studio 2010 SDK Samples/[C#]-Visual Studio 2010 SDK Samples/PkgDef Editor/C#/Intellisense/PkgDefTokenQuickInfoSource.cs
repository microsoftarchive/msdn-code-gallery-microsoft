//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace PkgDefLanguage
{

    [Export(typeof(IQuickInfoSourceProvider))]
    [ContentType("pkgdef")]
    [Name("pkgefTokenQuickInfo")]
    class PkgDefTokenQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        IBufferTagAggregatorFactoryService aggService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new PkgDefTokenQuickInfoSource(textBuffer, aggService.CreateTagAggregator<PkgDefTokenTag>(textBuffer));
        }
    }

    /// <summary>
    /// Provide QuickInfo text for pkgdef string-substitution tokens
    /// </summary>
    class PkgDefTokenQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<PkgDefTokenTag> _aggregator;
        private ITextBuffer _buffer;

        public PkgDefTokenQuickInfoSource(ITextBuffer buffer, ITagAggregator<PkgDefTokenTag> aggregator)
        {
            _aggregator = aggregator;
            _buffer = buffer;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint == null)
                return;

            // find each span that looks like a token and look it up in the dictionary
            foreach (IMappingTagSpan<PkgDefTokenTag> curTag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
            {
                if (curTag.Tag.type == PkgDefLanguageTokens.Token)
                {
                    SnapshotSpan tagSpan = curTag.Span.GetSpans(_buffer).First();
                    if (PkgDefTokenTagger.ValidTokens.Keys.Contains(tagSpan.GetText()))
                    {
                        applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                        quickInfoContent.Add(PkgDefTokenTagger.ValidTokens[tagSpan.GetText()]);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}

