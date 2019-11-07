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
    [Export(typeof(IClassifierProvider))]
    [ContentType("code")]
    internal class ToDoClassifierProvider : IClassifierProvider
    {

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("todo")]
        internal ClassificationTypeDefinition ToDoClassificationType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService _tagAggregatorFactory = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            IClassificationType classificationType = ClassificationRegistry.GetClassificationType("todo");

            var tagAggregator = _tagAggregatorFactory.CreateTagAggregator<ToDoTag>(buffer);
            return new ToDoClassifier(tagAggregator, classificationType);

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "todo")]
    [Name("ToDoText")]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    internal sealed class ToDoFormat : ClassificationFormatDefinition
    {
        public ToDoFormat()
        {
            this.DisplayName = "ToDo Text"; //human readable version of the name
            this.BackgroundOpacity = 1;
            this.BackgroundColor = Colors.Orange;
            this.ForegroundColor = Colors.MidnightBlue;
        }
    }
}
