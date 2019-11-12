Imports System.Text
Imports Microsoft.VisualStudio.Language.Intellisense
Imports System.Collections.ObjectModel
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Tagging
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Utilities

Namespace OokLanguage

    <Export(GetType(ICompletionSourceProvider)),
    ContentType("ook!"),
    Name("ookCompletion")>
    Friend Class OokCompletionSourceProvider
        Implements ICompletionSourceProvider

        Public Function TryCreateCompletionSource(ByVal textBuffer As ITextBuffer) As ICompletionSource Implements ICompletionSourceProvider.TryCreateCompletionSource
            Return New OokCompletionSource(textBuffer)
        End Function

    End Class

    Friend Class OokCompletionSource
        Implements ICompletionSource

        Private _buffer As ITextBuffer
        Private _disposed As Boolean = False

        Public Sub New(ByVal buffer As ITextBuffer)
            _buffer = buffer
        End Sub

        Public Sub AugmentCompletionSession(ByVal session As ICompletionSession, ByVal completionSets As IList(Of CompletionSet)) Implements ICompletionSource.AugmentCompletionSession
            If _disposed Then
                Throw New ObjectDisposedException("OokCompletionSource")
            End If

            Dim completions As New List(Of Completion) From {New Completion("Ook!"), New Completion("Ook."), New Completion("Ook?")}
            Dim snapshot As ITextSnapshot = _buffer.CurrentSnapshot
            Dim triggerPoint = CType(session.GetTriggerPoint(snapshot), SnapshotPoint)

            If triggerPoint = Nothing Then
                Return
            End If

            Dim line = triggerPoint.GetContainingLine()
            Dim start As SnapshotPoint = triggerPoint

            Do While start > line.Start AndAlso Not Char.IsWhiteSpace((start - 1).GetChar())
                start -= 1
            Loop

            Dim applicableTo = snapshot.CreateTrackingSpan(New SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive)
            completionSets.Add(New CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty(Of Completion)()))
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _disposed = True
        End Sub

    End Class
End Namespace