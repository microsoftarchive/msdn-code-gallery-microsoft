Imports System.Text
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Text

Namespace ToDoGlyphFactory

    Friend Class ToDoClassifier
        Implements IClassifier

        Private _classificationType As IClassificationType
        Private _tagger As ITagAggregator(Of ToDoTag)

        Friend Sub New(ByVal tagger As ITagAggregator(Of ToDoTag), ByVal todoType As IClassificationType)
            _tagger = tagger
            _classificationType = todoType
        End Sub

        Public Function GetClassificationSpans(ByVal span As SnapshotSpan) As IList(Of ClassificationSpan) Implements IClassifier.GetClassificationSpans
            Dim classifiedSpans As IList(Of ClassificationSpan) = New List(Of ClassificationSpan)
            Dim tags = _tagger.GetTags(span)

            For Each tagSpan As IMappingTagSpan(Of ToDoTag) In tags
                Dim todoSpan As SnapshotSpan = tagSpan.Span.GetSpans(span.Snapshot).First()
                classifiedSpans.Add(New ClassificationSpan(todoSpan, _classificationType))
            Next tagSpan

            Return classifiedSpans
        End Function

        Public Event ClassificationChanged(ByVal sender As Object, ByVal e As Microsoft.VisualStudio.Text.Classification.ClassificationChangedEventArgs) Implements Microsoft.VisualStudio.Text.Classification.IClassifier.ClassificationChanged

    End Class
End Namespace
