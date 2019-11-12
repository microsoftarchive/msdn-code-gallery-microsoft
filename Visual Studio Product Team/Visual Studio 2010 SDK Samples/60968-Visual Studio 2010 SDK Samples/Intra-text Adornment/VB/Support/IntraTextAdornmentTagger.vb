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
	''' Helper class for interspersing adornments into text.
	''' </summary>
	''' <remarks>
	''' To avoid an issue around intra-text adornment support and its interaction with text buffer changes,
	''' this tagger reacts to text and color tag changes with a delay. It waits to send out its own TagsChanged
	''' event until the WPF Dispatcher is running again and it takes care to report adornments
	''' that are consistent with the latest sent TagsChanged event by storing that particular snapshot
	''' and using it to query for the data tags.
	''' </remarks>
	Friend MustInherit Class IntraTextAdornmentTagger(Of TData As ITag, TAdornment As UIElement)
		Implements ITagger(Of IntraTextAdornmentTag)

		Protected ReadOnly _view As IWpfTextView
		Private _adornmentCache As New Dictionary(Of SnapshotSpan, TAdornment)
		Private private_snapshot As ITextSnapshot
		Protected Property _snapshot As ITextSnapshot
			Get
				Return private_snapshot
			End Get
			Private Set(ByVal value As ITextSnapshot)
				private_snapshot = value
			End Set
		End Property
		Private ReadOnly _invalidatedSpans As New List(Of SnapshotSpan)

		Protected Sub New(ByVal view As IWpfTextView)

			_view = view
			_snapshot = view.TextBuffer.CurrentSnapshot

			AddHandler _view.LayoutChanged, AddressOf HandleLayoutChanged
			AddHandler _view.TextBuffer.Changed, AddressOf HandleBufferChanged

		End Sub

		Protected MustOverride Function CreateAdornment(ByVal dataTag As TData) As TAdornment
		Protected MustOverride Sub UpdateAdornment(ByVal adornment As TAdornment, ByVal dataTag As TData)
		Protected MustOverride Function GetAdorenmentData(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of Tuple(Of SnapshotSpan, TData))

		Private Sub HandleBufferChanged(ByVal sender As Object, ByVal args As TextContentChangedEventArgs)

			Dim editedSpans = args.Changes.Select(Function(change) New SnapshotSpan(args.After, change.NewSpan)).ToList()
			InvalidateSpans(editedSpans)

		End Sub

		Protected Sub InvalidateSpans(ByVal spans As IList(Of SnapshotSpan))

			SyncLock _invalidatedSpans

				Dim wasEmpty As Boolean = _invalidatedSpans.Count = 0
				_invalidatedSpans.AddRange(spans)

				If wasEmpty AndAlso _invalidatedSpans.Count > 0 Then
					_view.VisualElement.Dispatcher.BeginInvoke(New Action(AddressOf AsyncUpdate))
				End If

			End SyncLock

		End Sub

		Private Sub AsyncUpdate()

			' Store the snapshot that we're now current with and send an event
			' for the text that has changed.
			If _snapshot IsNot _view.TextBuffer.CurrentSnapshot Then

				_snapshot = _view.TextBuffer.CurrentSnapshot

				Dim translatedAdornmentCache As New Dictionary(Of SnapshotSpan, TAdornment)

				For Each keyValuePair In _adornmentCache
					translatedAdornmentCache.Add(keyValuePair.Key.TranslateTo(_snapshot, SpanTrackingMode.EdgeExclusive), keyValuePair.Value)
				Next keyValuePair

				_adornmentCache = translatedAdornmentCache

			End If

			Dim translatedSpans As List(Of SnapshotSpan)
			SyncLock _invalidatedSpans

				translatedSpans = _invalidatedSpans.Select(Function(s) s.TranslateTo(_snapshot, SpanTrackingMode.EdgeInclusive)).ToList()
				_invalidatedSpans.Clear()

			End SyncLock

			If translatedSpans.Count = 0 Then
				Return
			End If

			Dim start = translatedSpans.Select(Function(span) span.Start).Min()
			Dim [end] = translatedSpans.Select(Function(span) span.End).Max()


			RaiseTagsChanged(New SnapshotSpan(start, [end]))

		End Sub

		Protected Sub RaiseTagsChanged(ByVal span As SnapshotSpan)

			Dim handler = Me.TagsChangedEvent
			If handler IsNot Nothing Then
				handler(Me, New SnapshotSpanEventArgs(span))
			End If

		End Sub

		Private Sub HandleLayoutChanged(ByVal sender As Object, ByVal e As TextViewLayoutChangedEventArgs)

			Dim visibleSpan As SnapshotSpan = _view.TextViewLines.FormattedSpan

			' Filter out the adornments that are no longer visible.
			Dim toRemove As New List(Of SnapshotSpan)(
			    From keyValuePair In _adornmentCache
			    Where (Not keyValuePair.Key.TranslateTo(visibleSpan.Snapshot, SpanTrackingMode.EdgeExclusive).IntersectsWith(visibleSpan))
			    Select keyValuePair.Key)

			For Each span In toRemove
				_adornmentCache.Remove(span)
			Next span

		End Sub


		' Produces tags on the snapshot that the tag consumer asked for.
		Public Overridable Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of IntraTextAdornmentTag)) Implements ITagger(Of IntraTextAdornmentTag).GetTags

            Dim result As New List(Of ITagSpan(Of IntraTextAdornmentTag))

			If spans Is Nothing OrElse spans.Count = 0 Then
                Return result
            End If

			' Translate the request to the snapshot that this tagger is current with.

			Dim requestedSnapshot As ITextSnapshot = spans(0).Snapshot

			Dim translatedSpans = New NormalizedSnapshotSpanCollection(spans.Select(Function(span) span.TranslateTo(_snapshot, SpanTrackingMode.EdgeExclusive)))

			' Grab the adornments.
			For Each tagSpan In GetAdornmentTagsOnSnapshot(translatedSpans)

				' Translate each adornment to the snapshot that the tagger was asked about.
				Dim span As SnapshotSpan = tagSpan.Span.TranslateTo(requestedSnapshot, SpanTrackingMode.EdgeExclusive)
                Dim affinity As Nullable(Of PositionAffinity)  ' Affinity is needed only for zero-length adornments.
                If span.Length = 0 Then
                    affinity = PositionAffinity.Successor
                Else
                    affinity = Nothing
                End If

				Dim tag As New IntraTextAdornmentTag(tagSpan.Tag.Adornment, tagSpan.Tag.RemovalCallback, affinity)
                result.Add(New TagSpan(Of IntraTextAdornmentTag)(span, tag))

			Next tagSpan

            Return result

		End Function

		' Produces tags on the snapshot that this tagger is current with.
        Private Function GetAdornmentTagsOnSnapshot(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of TagSpan(Of IntraTextAdornmentTag))

            Dim result As New List(Of TagSpan(Of IntraTextAdornmentTag))

            If spans.Count = 0 Then
                Return result
            End If

            Dim snapshot As ITextSnapshot = spans(0).Snapshot

            Debug.Assert(snapshot Is _snapshot)

            ' Since WPF UI objects have state (like mouse hover or animation) and are relatively expensive to create and lay out,
            ' this code tries to reuse controls as much as possible.
            ' The controls are stored in _adornmentCache between the calls.

            ' Mark which adornments fall inside the requested spans with Keep=false
            ' so that they can be removed from the cache if they no longer correspond to data tags.
            Dim toRemove As New HashSet(Of SnapshotSpan)
            For Each ar In _adornmentCache
                If spans.IntersectsWith(New NormalizedSnapshotSpanCollection(ar.Key)) Then
                    toRemove.Add(ar.Key)
                End If
            Next ar

            For Each spanDataPair In GetAdorenmentData(spans)

                ' Look up the corresponding adornment or create one if it's new.
                Dim adornment As TAdornment = Nothing
                Dim snapshotSpan As SnapshotSpan = spanDataPair.Item1
                Dim adornmentData As TData = spanDataPair.Item2
                If _adornmentCache.TryGetValue(snapshotSpan, adornment) Then

                    UpdateAdornment(adornment, adornmentData)
                    toRemove.Remove(snapshotSpan)

                Else

                    adornment = CreateAdornment(adornmentData)

                    ' Get the adornment to measure itself. Its DesiredSize property is used to determine
                    ' how much space to leave between text for this adornment.
                    ' Note: If the size of the adornment changes, the line will be reformatted to accommodate it.
                    ' Note: Some adornments may change size when added to the view's visual tree due to inherited
                    ' dependency properties that affect layout. Such options can include SnapsToDevicePixels,
                    ' UseLayoutRounding, TextRenderingMode, TextHintingMode, and TextFormattingMode. Making sure
                    ' that these properties on the adornment match the view's values before calling Measure here
                    ' can help avoid the size change and the resulting unnecessary re-format.
                    adornment.Measure(New Size(Double.PositiveInfinity, Double.PositiveInfinity))

                    _adornmentCache.Add(snapshotSpan, adornment)

                End If

                result.Add(New TagSpan(Of IntraTextAdornmentTag)(snapshotSpan, New IntraTextAdornmentTag(adornment, Nothing, Nothing)))

            Next spanDataPair

            For Each snapshotSpan In toRemove
                _adornmentCache.Remove(snapshotSpan)
            Next snapshotSpan

            Return result

        End Function

		Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of IntraTextAdornmentTag).TagsChanged

	End Class

End Namespace
