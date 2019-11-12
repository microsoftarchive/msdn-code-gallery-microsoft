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

' Copyright (c) Microsoft Corporation.  All rights reserved.
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Imports System.Collections.ObjectModel
Imports System.ComponentModel.Composition
Imports System.Windows.Threading
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace Microsoft.VisualStudio.Language.Spellchecker
	Friend Class SpellSmartTag
		Inherits SmartTag
		Public Sub New(ByVal actionSets As ReadOnlyCollection(Of SmartTagActionSet))
			MyBase.New(SmartTagType.Factoid, actionSets)
		End Sub
	End Class

	''' <summary>
	''' Tagger for Spelling smart tags.
	''' </summary>
	Friend Class SpellSmartTagger
		Implements ITagger(Of SpellSmartTag), IDisposable
		#Region "Private Fields"

		Private _buffer As ITextBuffer
		Private _dictionary As ISpellingDictionaryService
		Private _misspellingAggregator As ITagAggregator(Of IMisspellingTag)
		Private disposed As Boolean = False
		Friend Const SpellingErrorType As String = "Spelling Error Smart Tag"
		#End Region

		#Region "MEF Imports / Exports"

		<Export(GetType(IViewTaggerProvider)), ContentType("text"), TagType(GetType(Microsoft.VisualStudio.Language.Intellisense.SmartTag))> _
		Friend Class SpellSmartTaggerProvider
			Implements IViewTaggerProvider
			#Region "MEF Imports"
			<Import> _
			Private Dictionary As ISpellingDictionaryService = Nothing

			<Import> _
			Friend TagAggregatorFactory As IBufferTagAggregatorFactoryService = Nothing
			#End Region

			#Region "ITaggerProvider"
			Public Function CreateTagger(Of T As ITag)(ByVal textView As ITextView, ByVal buffer As ITextBuffer) As ITagger(Of T) Implements IViewTaggerProvider.CreateTagger
				If buffer Is Nothing Then
					Throw New ArgumentNullException("buffer")
				End If
				If textView Is Nothing Then
					Throw New ArgumentNullException("textView")
				End If

				' Make sure we only tagging top buffer
				If buffer IsNot textView.TextBuffer Then
					Return Nothing
				End If

				Dim misspellingAggregator = TagAggregatorFactory.CreateTagAggregator(Of IMisspellingTag)(buffer)
                Return TryCast(New SpellSmartTagger(buffer, Dictionary, misspellingAggregator), ITagger(Of T))

			End Function
			#End Region
		End Class
		#End Region

		Public Sub New(ByVal buffer As ITextBuffer, ByVal dictionary As ISpellingDictionaryService, ByVal misspellingAggregator As ITagAggregator(Of IMisspellingTag))
			_buffer = buffer
			_dictionary = dictionary
			_misspellingAggregator = misspellingAggregator

            AddHandler _misspellingAggregator.TagsChanged, Sub(sender, args)
                                                               For Each span In args.Span.GetSpans(_buffer)
                                                                   RaiseTagsChangedEvent(span)
                                                               Next span
                                                           End Sub
		End Sub
		

		#Region "ITagger<SpellSmartTag> Members"
		''' <summary>
		''' Returns tags on demand.
		''' </summary>
		''' <param name="spans">Spans collection to get tags for.</param>
		''' <returns>Squiggle tags in provided spans.</returns>
		Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of SpellSmartTag)) Implements ITagger(Of SpellSmartTag).GetTags
            Dim list As New List(Of ITagSpan(Of SpellSmartTag))
            If spans.Count = 0 Then
                Return list
            End If

			Dim snapshot As ITextSnapshot = spans(0).Snapshot

			For Each misspelling In _misspellingAggregator.GetTags(spans)
				Dim misspellingSpans = misspelling.Span.GetSpans(snapshot)
				If misspellingSpans.Count <> 1 Then
					Continue For
				End If

				Dim errorSpan As SnapshotSpan = misspellingSpans(0)
                list.Add(New TagSpan(Of SpellSmartTag)(errorSpan, New SpellSmartTag(GetSmartTagActions(errorSpan, misspelling.Tag.Suggestions))))
            Next misspelling

            Return list
		End Function

		Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of SpellSmartTag).TagsChanged
		#End Region

		#Region "IDisposable"
		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

		Private Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposed Then
				If disposing Then
					_misspellingAggregator.Dispose()
					_misspellingAggregator = Nothing
				End If

				disposed = True
			End If
		End Sub

		#End Region

		#Region "Helpers"
		Private Function GetSmartTagActions(ByVal errorSpan As SnapshotSpan, ByVal suggestions As IEnumerable(Of String)) As ReadOnlyCollection(Of SmartTagActionSet)
			Dim smartTagSets As New List(Of SmartTagActionSet)()

			Dim trackingSpan As ITrackingSpan = errorSpan.Snapshot.CreateTrackingSpan(errorSpan, SpanTrackingMode.EdgeExclusive)

			' Add spelling suggestions (if there are any)
			Dim actions As New List(Of ISmartTagAction)()

			For Each suggestion In suggestions
				actions.Add(New SpellSmartTagAction(trackingSpan, suggestion, True))
			Next suggestion

			If actions.Count > 0 Then
				smartTagSets.Add(New SmartTagActionSet(actions.AsReadOnly()))
			End If

			' Add Dictionary operations (ignore all)
			Dim dictionaryActions As New List(Of ISmartTagAction)()
			dictionaryActions.Add(New SpellDictionarySmartTagAction(trackingSpan, _dictionary, "Ignore All"))
			smartTagSets.Add(New SmartTagActionSet(dictionaryActions.AsReadOnly()))

			Return smartTagSets.AsReadOnly()

		End Function

		#End Region

		#Region "Event handlers"

		Private Sub RaiseTagsChangedEvent(ByVal subjectSpan As SnapshotSpan)
			Dim handler As EventHandler(Of SnapshotSpanEventArgs) = Me.TagsChangedEvent
			If handler IsNot Nothing Then
				handler(Me, New SnapshotSpanEventArgs(subjectSpan))
			End If
		End Sub

		#End Region
	End Class
End Namespace
