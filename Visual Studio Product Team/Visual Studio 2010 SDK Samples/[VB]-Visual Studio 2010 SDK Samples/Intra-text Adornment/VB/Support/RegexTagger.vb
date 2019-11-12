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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Tagging

Namespace IntraTextAdornmentSample

	Friend MustInherit Class RegexTagger(Of T As ITag)
		Implements ITagger(Of T)

		Private ReadOnly _matchExpressions As IEnumerable(Of Regex)

		Public Sub New(ByVal buffer As ITextBuffer, ByVal matchExpressions As IEnumerable(Of Regex))

			_matchExpressions = matchExpressions

			AddHandler buffer.Changed, Sub(sender, args) HandleBufferChanged(args)

		End Sub

		#Region "ITagger implementation"

        Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of T)) Implements ITagger(Of T).GetTags

            Dim result As New List(Of ITagSpan(Of T))

            For Each line In GetIntersectingLines(spans)

                Dim text As String = line.GetText()

                For Each regex In _matchExpressions

                    For Each match In regex.Matches(text).Cast(Of Match)()

                        Dim tag As T = TryCreateTagForMatch(match)
                        If tag IsNot Nothing Then

                            Dim span As New SnapshotSpan(line.Start + match.Index, match.Length)
                            result.Add(New TagSpan(Of T)(span, tag))

                        End If

                    Next match

                Next regex

            Next line

            Return result

        End Function

		Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of T).TagsChanged

		#End Region

        Private Function GetIntersectingLines(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITextSnapshotLine)

            Dim result As New List(Of ITextSnapshotLine)

            If spans.Count = 0 Then
                Return result
            End If

            Dim lastVisitedLineNumber As Integer = -1
            Dim snapshot As ITextSnapshot = spans(0).Snapshot
            For Each span In spans

                Dim firstLine As Integer = snapshot.GetLineNumberFromPosition(span.Start)
                Dim lastLine As Integer = snapshot.GetLineNumberFromPosition(span.End)

                For i As Integer = Math.Max(lastVisitedLineNumber, firstLine) To lastLine

                    result.Add(snapshot.GetLineFromLineNumber(i))

                Next i

                lastVisitedLineNumber = lastLine

            Next span

            Return result

        End Function

		''' <summary>
		''' Overridden in the derived implementation to provide a tag for each regular expression match.
		''' If the return value is <c>null</c>, this match will be skipped.
		''' </summary>
		''' <param name="match">The match to create a tag for.</param>
		''' <returns>The tag to return from <see cref="GetTags"/>, if non-<c>null</c>.</returns>
		Protected MustOverride Function TryCreateTagForMatch(ByVal match As Match) As T

		''' <summary>
		''' Handle buffer changes. The default implementation expands changes to full lines and sends out
		''' a <see cref="TagsChanged"/> event for these lines.
		''' </summary>
		''' <param name="args">The buffer change arguments.</param>
		Protected Overridable Sub HandleBufferChanged(ByVal args As TextContentChangedEventArgs)

			If args.Changes.Count = 0 Then
				Return
			End If

			Dim temp = TagsChangedEvent
			If temp Is Nothing Then
				Return
			End If

			' Combine all changes into a single span so that
			' the ITagger<>.TagsChanged event can be raised just once for a compound edit
			' with many parts.

			Dim snapshot As ITextSnapshot = args.After

			Dim start As Integer = args.Changes(0).NewPosition
			Dim [end] As Integer = args.Changes(args.Changes.Count - 1).NewEnd

			Dim totalAffectedSpan As New SnapshotSpan(snapshot.GetLineFromPosition(start).Start, snapshot.GetLineFromPosition([end]).End)

			temp(Me, New SnapshotSpanEventArgs(totalAffectedSpan))

		End Sub

	End Class

End Namespace
