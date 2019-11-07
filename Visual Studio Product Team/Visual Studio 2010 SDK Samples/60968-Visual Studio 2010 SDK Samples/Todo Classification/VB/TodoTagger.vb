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
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Classification

Namespace ToDoGlyphFactory

    Friend Class ToDoTag
        Implements IGlyphTag
    End Class

    Friend Class ToDoTagger
        Implements ITagger(Of ToDoTag)

        Private Const _searchText As String = "todo"

        Private Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of ToDoTag)) Implements ITagger(Of ToDoTag).GetTags
            Dim tags As New List(Of TagSpan(Of ToDoTag))

            For Each curSpan As SnapshotSpan In spans
                Dim loc As Integer = curSpan.GetText().ToLower().IndexOf(_searchText)
                If loc > -1 Then
                    Dim todoSpan As New SnapshotSpan(curSpan.Snapshot, New Span(curSpan.Start + loc, _searchText.Length))
                    tags.Add(New TagSpan(Of ToDoTag)(todoSpan, New ToDoTag))
                End If
            Next curSpan

            Return tags
        End Function

        Public Event TagsChanged(ByVal sender As Object, ByVal e As Microsoft.VisualStudio.Text.SnapshotSpanEventArgs) Implements Microsoft.VisualStudio.Text.Tagging.ITagger(Of ToDoTag).TagsChanged

    End Class
End Namespace
