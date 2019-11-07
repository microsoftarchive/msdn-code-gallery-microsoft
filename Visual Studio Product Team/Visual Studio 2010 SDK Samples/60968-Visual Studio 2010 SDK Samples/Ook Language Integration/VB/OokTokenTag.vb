' Copyright (c) Microsoft Corporation
' All rights reserved

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace OokLanguage


    <Export(GetType(ITaggerProvider)),
    ContentType("ook!"),
    TagType(GetType(OokTokenTag))>
    Friend NotInheritable Class OokTokenTagProvider
        Implements ITaggerProvider

        Public Function CreateTagger(Of T As ITag)(ByVal buffer As ITextBuffer) As ITagger(Of T) Implements ITaggerProvider.CreateTagger
            Return TryCast(New OokTokenTagger(buffer), ITagger(Of T))
        End Function

    End Class

    Public Class OokTokenTag
        Implements ITag

        Private privatetype As OokTokenTypes
        Public Property type As OokTokenTypes
            Get
                Return privatetype
            End Get
            Private Set(ByVal value As OokTokenTypes)
                privatetype = value
            End Set
        End Property

        Public Sub New(ByVal type As OokTokenTypes)
            Me.type = type
        End Sub

    End Class

    Friend NotInheritable Class OokTokenTagger
        Implements ITagger(Of OokTokenTag)

        Private _buffer As ITextBuffer
        Private _ookTypes As IDictionary(Of String, OokTokenTypes)

        Friend Sub New(ByVal buffer As ITextBuffer)
            _buffer = buffer
            _ookTypes = New Dictionary(Of String, OokTokenTypes)
            _ookTypes("ook!") = OokTokenTypes.OokExclaimation
            _ookTypes("ook.") = OokTokenTypes.OokPeriod
            _ookTypes("ook?") = OokTokenTypes.OokQuestion
        End Sub

        Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of OokTokenTag)) Implements ITagger(Of OokTokenTag).GetTags
            Dim tags As New List(Of TagSpan(Of OokTokenTag))

            For Each curSpan As SnapshotSpan In spans
                Dim containingLine As ITextSnapshotLine = curSpan.Start.GetContainingLine()
                Dim curLoc As Integer = containingLine.Start.Position
                Dim tokens() As String = containingLine.GetText().ToLower().Split(" "c)

                For Each ookToken As String In tokens
                    If _ookTypes.ContainsKey(ookToken) Then
                        Dim tokenSpan = New SnapshotSpan(curSpan.Snapshot, New Span(curLoc, ookToken.Length))
                        If tokenSpan.IntersectsWith(curSpan) Then
                            tags.Add(New TagSpan(Of OokTokenTag)(tokenSpan, New OokTokenTag(_ookTypes(ookToken))))
                        End If
                    End If
                    'add an extra char location because of the space
                    curLoc += ookToken.Length + 1
                Next ookToken
            Next curSpan

            Return tags
        End Function

        Public Event TagsChanged(ByVal sender As Object, ByVal e As Microsoft.VisualStudio.Text.SnapshotSpanEventArgs) Implements Microsoft.VisualStudio.Text.Tagging.ITagger(Of OokTokenTag).TagsChanged

    End Class

End Namespace
