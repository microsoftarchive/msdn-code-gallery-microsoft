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
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("pkgdef")]
    [Name("pkgdefTokenCompletion")]
    class PkgDefTokenCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new PkgDefTokenCompletionSource(textBuffer);
        }
    }

    /// <summary>
    /// Provide list of valid string-substitution tokens when the user types a '$'
    /// </summary>
    class PkgDefTokenCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;

        public PkgDefTokenCompletionSource(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// Provide a list of completion items that contains the valid string-substitution tokens
        /// </summary>
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            // create a list of completions from the dictionary of valid tokens
            List<Completion> completions = new List<Completion>();
            foreach (KeyValuePair<string,string> token in PkgDefTokenTagger.ValidTokens)
            {
                completions.Add(new Completion(token.Key, token.Key, token.Value, null, null));
            };
            
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint - 1;  // start selection to left of '$'

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

            completionSets.Add(new CompletionSet("PkgDefTokens", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
        }
    }
}

