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
Imports Microsoft.VisualStudio.Text.Tagging

Namespace IntraTextAdornmentSample

	''' <summary>
	''' Helper class for translating given tags into intra-text adornments.
	''' </summary>
	Friend MustInherit Class IntraTextAdornmentTagTransformer(Of TDataTag As ITag, TAdornment As UIElement)
		Inherits IntraTextAdornmentTagger(Of TDataTag, TAdornment)
		Implements IDisposable

		Protected ReadOnly _dataTagger As ITagAggregator(Of TDataTag)

		Protected Sub New(ByVal view As IWpfTextView, ByVal dataTagger As ITagAggregator(Of TDataTag))

			MyBase.New(view)
			_dataTagger = dataTagger

			AddHandler _dataTagger.TagsChanged, AddressOf HandleDataTagsChanged

		End Sub

        Protected Overrides Function GetAdorenmentData(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of Tuple(Of SnapshotSpan, TDataTag))

            Dim result As New List(Of Tuple(Of SnapshotSpan, TDataTag))()

            If spans.Count = 0 Then
                Return result
            End If

            Dim snapshot As ITextSnapshot = spans(0).Snapshot

            For Each dataTagSpan As IMappingTagSpan(Of TDataTag) In _dataTagger.GetTags(spans)

                Dim dataTagSpans As NormalizedSnapshotSpanCollection = dataTagSpan.Span.GetSpans(snapshot)

                ' Ignore data tags that are split by projection.
                ' This is theoretically possible but unlikely in current scenarios.
                If dataTagSpans.Count <> 1 Then
                    Continue For
                End If

                result.Add(Tuple.Create(dataTagSpans(0), dataTagSpan.Tag))
            Next dataTagSpan

            Return result

        End Function

		Private Sub HandleDataTagsChanged(ByVal sender As Object, ByVal args As TagsChangedEventArgs)

			Dim changedSpans = args.Span.GetSpans(_view.TextBuffer.CurrentSnapshot)
			InvalidateSpans(changedSpans)

		End Sub

		Public Overridable Sub Dispose() Implements IDisposable.Dispose

			_dataTagger.Dispose()

		End Sub

	End Class

End Namespace
