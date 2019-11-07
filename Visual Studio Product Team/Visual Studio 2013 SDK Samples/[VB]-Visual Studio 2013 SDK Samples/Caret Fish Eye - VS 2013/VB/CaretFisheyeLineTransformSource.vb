'***************************************************************************
'
'    Copyright (c) Microsoft Corporation. All rights reserved.
'    This code is licensed under the Visual Studio SDK license terms.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'***************************************************************************
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Formatting

Namespace CaretFisheye

    Public Class CaretFisheyeLineTransformSource
        Implements ILineTransformSource

        Dim _textView As IWpfTextView

        Sub New(ByRef textView As IWpfTextView)

            _textView = textView
            'Sync to changing the caret position.
            AddHandler _textView.Caret.PositionChanged, AddressOf OnCaretChanged

        End Sub

        Private Sub OnCaretChanged(ByVal sender As Object, ByVal e As CaretPositionChangedEventArgs)

            'Did the caret line number change?
            Dim oldPosition As SnapshotPoint = e.OldPosition.BufferPosition
            Dim newPosition As SnapshotPoint = e.NewPosition.BufferPosition

            If _textView.TextSnapshot.GetLineNumberFromPosition(newPosition) <> _textView.TextSnapshot.GetLineNumberFromPosition(oldPosition) Then
                'Yes. Is the caret on a line that has been formatted by the view?
                Dim line As ITextViewLine = _textView.Caret.ContainingTextViewLine
                If line.VisibilityState <> VisibilityState.Unattached Then
                    'Yes. Force the view to redraw so that (top of) the caret line has exactly the same position.
                    _textView.DisplayTextLineContainingBufferPosition(line.Start, line.Top, ViewRelativePosition.Top)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Static class factory that ensures a single instance of the line transform source/view.
        ''' </summary>
        Public Shared Function Create(ByVal view As IWpfTextView) As CaretFisheyeLineTransformSource
            Return view.Properties.GetOrCreateSingletonProperty(Of CaretFisheyeLineTransformSource)(Function() New CaretFisheyeLineTransformSource(view))
        End Function

        Public Function GetLineTransform(ByVal line As ITextViewLine, ByVal yPosition As Double, ByVal placement As ViewRelativePosition) As LineTransform Implements ILineTransformSource.GetLineTransform
            'Vertically compress lines that are far from the caret (based on buffer lines, not view lines).
            Dim caretLineNumber As Integer = _textView.TextSnapshot.GetLineNumberFromPosition(_textView.Caret.Position.BufferPosition)
            Dim lineNumber As Integer = _textView.TextSnapshot.GetLineNumberFromPosition(line.Start)
            Dim delta As Integer = Math.Abs(caretLineNumber - lineNumber)

            Dim scale As Double
            If delta <= 3 Then
                scale = 1.0
            ElseIf delta <= 8 Then
                scale = 1.0 - (CDbl(delta - 3)) * 0.05
            ElseIf delta <= 18 Then
                scale = 0.75 - (CDbl(delta - 8)) * 0.025
            Else
                scale = 0.5
            End If

            Return New LineTransform(0.0, 0.0, scale)
        End Function

    End Class
End Namespace
