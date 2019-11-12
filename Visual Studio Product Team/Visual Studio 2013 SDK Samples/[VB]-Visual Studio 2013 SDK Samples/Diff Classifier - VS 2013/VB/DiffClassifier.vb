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

' Copyright (c) Microsoft Corporation
' All rights reserved

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Classification

Namespace DiffClassifier

    Public Class DiffClassifier
        Implements IClassifier

        Private _classificationTypeRegistry As IClassificationTypeRegistryService

        Friend Sub New(ByVal registry As IClassificationTypeRegistryService)
            Me._classificationTypeRegistry = registry
        End Sub

        ''' <summary>
        ''' Classify the given spans, which, for diff files, classifies
        ''' a line at a time.
        ''' </summary>
        ''' <param name="span">The span of interest in this projection buffer.</param>
        ''' <returns>The list of <see cref="ClassificationSpan"/> as contributed by the source buffers.</returns>
        Public Function GetClassificationSpans(ByVal span As SnapshotSpan) As IList(Of ClassificationSpan) Implements IClassifier.GetClassificationSpans

            Dim snapshot As ITextSnapshot = span.Snapshot
            Dim spans As New List(Of ClassificationSpan)
            If snapshot.Length = 0 Then
                Return spans
            End If

            Dim startno As Integer = span.Start.GetContainingLine().LineNumber
            Dim endno As Integer = (span.End - 1).GetContainingLine().LineNumber

            For i As Integer = startno To endno

                Dim line As ITextSnapshotLine = snapshot.GetLineFromLineNumber(i)
                Dim type As IClassificationType = Nothing
                Dim text As String = line.Snapshot.GetText(New SnapshotSpan(line.Start, Math.Min(4, line.Length))) ' We only need the first 4

                If text.StartsWith("!", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.changed")
                ElseIf text.StartsWith("---", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.header")
                ElseIf text.StartsWith("-", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.removed")
                ElseIf text.StartsWith("<", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.removed")
                ElseIf text.StartsWith("@@", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.patchline")
                ElseIf text.StartsWith("+++", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.header")
                ElseIf text.StartsWith("+", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.added")
                ElseIf text.StartsWith(">", StringComparison.Ordinal) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.added")
                ElseIf text.StartsWith("***", StringComparison.Ordinal) Then
                    If i < 2 Then
                        type = _classificationTypeRegistry.GetClassificationType("diff.header")
                    Else
                        type = _classificationTypeRegistry.GetClassificationType("diff.infoline")
                    End If
                ElseIf text.Length > 0 AndAlso (Not Char.IsWhiteSpace(text.Chars(0))) Then
                    type = _classificationTypeRegistry.GetClassificationType("diff.infoline")
                End If

                If type IsNot Nothing Then
                    spans.Add(New ClassificationSpan(line.Extent, type))
                End If

            Next i

            Return spans
        End Function

        Public Event ClassificationChanged As EventHandler(Of ClassificationChangedEventArgs) Implements IClassifier.ClassificationChanged

    End Class
End Namespace
