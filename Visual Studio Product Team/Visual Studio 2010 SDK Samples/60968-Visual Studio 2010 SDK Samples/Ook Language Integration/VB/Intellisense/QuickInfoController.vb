'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
' Copyright (c) Microsoft Corporation.  All rights reserved.
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Utilities

Namespace VSLTK.Intellisense

    Friend Class TemplateQuickInfoController
        Implements IIntellisenseController

        Private _textView As ITextView
        Private _subjectBuffers As IList(Of ITextBuffer)
        Private _componentContext As TemplateQuickInfoControllerProvider

        Private _session As IQuickInfoSession

        Friend Sub New(ByVal textView As ITextView, ByVal subjectBuffers As IList(Of ITextBuffer), ByVal componentContext As TemplateQuickInfoControllerProvider)
            _textView = textView
            _subjectBuffers = subjectBuffers
            _componentContext = componentContext

            AddHandler _textView.MouseHover, AddressOf OnTextViewMouseHover
        End Sub

        Public Sub ConnectSubjectBuffer(ByVal subjectBuffer As ITextBuffer) Implements IIntellisenseController.ConnectSubjectBuffer

        End Sub

        Public Sub DisconnectSubjectBuffer(ByVal subjectBuffer As ITextBuffer) Implements IIntellisenseController.DisconnectSubjectBuffer

        End Sub

        Public Sub Detach(ByVal textView As ITextView) Implements IIntellisenseController.Detach
            If _textView Is textView Then
                RemoveHandler _textView.MouseHover, AddressOf OnTextViewMouseHover
                _textView = Nothing
            End If
        End Sub

        Private Sub OnTextViewMouseHover(ByVal sender As Object, ByVal e As MouseHoverEventArgs)
            Dim point? As SnapshotPoint = Me.GetMousePosition(New SnapshotPoint(_textView.TextSnapshot, e.Position))

            If point IsNot Nothing Then
                Dim triggerPoint As ITrackingPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive)
                ' Find the broker for this buffer
                If Not _componentContext.QuickInfoBroker.IsQuickInfoActive(_textView) Then
                    _session = _componentContext.QuickInfoBroker.CreateQuickInfoSession(_textView, triggerPoint, True)
                    _session.Start()
                End If
            End If
        End Sub

        Private Function GetMousePosition(ByVal topPosition As SnapshotPoint) As SnapshotPoint?
            ' Map this point down to the appropriate subject buffer.
            Return _textView.BufferGraph.MapDownToFirstMatch(topPosition, PointTrackingMode.Positive, Function(snapshot) _subjectBuffers.Contains(snapshot.TextBuffer), PositionAffinity.Predecessor)
        End Function

    End Class
End Namespace