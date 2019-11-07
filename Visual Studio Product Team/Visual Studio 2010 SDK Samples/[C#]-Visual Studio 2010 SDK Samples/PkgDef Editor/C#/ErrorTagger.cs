//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;

namespace PkgDefLanguage
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("pkgdef")]
    [TagType(typeof(ErrorTag))]
    internal sealed class ErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IBufferTagAggregatorFactoryService _aggregatorFactory = null;

        [Import(typeof(Microsoft.VisualStudio.Shell.SVsServiceProvider))]
        internal IServiceProvider _serviceProvider = null;

        [Import] 
        ITextDocumentFactoryService _textDocumentFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate() { return new ErrorTagger(buffer, _aggregatorFactory, _serviceProvider, _textDocumentFactory) as ITagger<T>; };
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
        }
    }

    /// <summary>
    /// Translate PkgDefTokenTags into ErrorTags and Error List items
    /// </summary>
    internal sealed class ErrorTagger : ITagger<ErrorTag>, IDisposable
    {
        ITagAggregator<PkgDefTokenTag> _aggregator;

        ITextBuffer _buffer;

        ErrorListProvider _errorProvider;

        ITextDocument _document;

        internal ErrorTagger(ITextBuffer buffer, IBufferTagAggregatorFactoryService aggregatorFactory, IServiceProvider serviceProvider, ITextDocumentFactoryService textDocumentFactory )
        {
            _buffer = buffer;

            _aggregator = aggregatorFactory.CreateTagAggregator<PkgDefTokenTag>(buffer);

            if (!textDocumentFactory.TryGetTextDocument(_buffer, out _document))
                _document = null;

            _errorProvider = new ErrorListProvider(serviceProvider);

            ReparseFile(null, EventArgs.Empty);

            BufferIdleEventUtil.AddBufferIdleEventListener(_buffer, ReparseFile);
        }

        public void Dispose()
        {
            if (_errorProvider != null)
            {
                _errorProvider.Tasks.Clear();
                _errorProvider.Dispose();
            }
            BufferIdleEventUtil.RemoveBufferIdleEventListener(_buffer, ReparseFile);
        }

        /// <summary>
        /// Find the Error tokens in the set of all tokens and create an ErrorTag for each
        /// </summary>
        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                if (tagSpan.Tag.type == PkgDefLanguageTokens.Error)
                {
                    var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                    PkgDefErrorTag tag = tagSpan.Tag as PkgDefErrorTag;
                    yield return new TagSpan<ErrorTag>(tagSpans[0], new ErrorTag("error", tag.message));
                }
            }
        }

#pragma warning disable 67
        // the Classifier tagger is translating buffer change events into TagsChanged events, so we don't have to
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore

        /// <summary>
        /// Updates the Error List by clearing our items and adding any errors found in the current set of tags
        /// </summary>
        void ReparseFile(object sender, EventArgs args)
        {
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            NormalizedSnapshotSpanCollection spans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(snapshot, 0, snapshot.Length));

            _errorProvider.Tasks.Clear();

            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                if (tagSpan.Tag.type == PkgDefLanguageTokens.Error)
                {
                    var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                    PkgDefErrorTag tag = tagSpan.Tag as PkgDefErrorTag;
                    AddErrorTask(tagSpans[0], tag);
                }
            }
        }

        /// <summary>
        /// Add one task to the Error List based on the given tag
        /// </summary>
        private void AddErrorTask(SnapshotSpan span, PkgDefErrorTag tag)
        {
            if (_errorProvider != null)
            {
                ErrorTask task = new ErrorTask();
                task.CanDelete = true;
                if (_document != null)
                    task.Document = _document.FilePath;
                task.ErrorCategory = TaskErrorCategory.Error;
                task.Text = tag.message;
                task.Line = span.Start.GetContainingLine().LineNumber;
                task.Column = span.Start.Position - span.Start.GetContainingLine().Start.Position;

                task.Navigate += new EventHandler(task_Navigate);

                _errorProvider.Tasks.Add(task);
            }
        }

        /// <summary>
        /// Callback method attached to each of our tasks in the Error List
        /// </summary>
        void task_Navigate(object sender, EventArgs e)
        {
            ErrorTask error = sender as ErrorTask;

            if (error != null)
            {

                error.Line += 1;
                error.Column += 1;
                _errorProvider.Navigate(error, new Guid(EnvDTE.Constants.vsViewKindCode));
                error.Column -= 1;
                error.Line -= 1;
            }
        }
    }
}
