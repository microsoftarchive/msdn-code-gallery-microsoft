using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System.Windows.Media;

namespace ToDoGlyphFactory
{
  
    class ToDoClassifier : IClassifier
    {

        IClassificationType _classificationType;
        ITagAggregator<ToDoTag> _tagger;

        internal ToDoClassifier(ITagAggregator<ToDoTag> tagger, IClassificationType todoType)
        {
            _tagger = tagger;
            _classificationType = todoType;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            
            IList<ClassificationSpan> classifiedSpans = new List<ClassificationSpan>();

            var tags = _tagger.GetTags(span);

            foreach (IMappingTagSpan<ToDoTag> tagSpan in tags)
            {
                SnapshotSpan todoSpan = tagSpan.Span.GetSpans(span.Snapshot).First();
                classifiedSpans.Add(new ClassificationSpan(todoSpan, _classificationType));
            }

            return classifiedSpans;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add {}
            remove {}
        }

    }

}
