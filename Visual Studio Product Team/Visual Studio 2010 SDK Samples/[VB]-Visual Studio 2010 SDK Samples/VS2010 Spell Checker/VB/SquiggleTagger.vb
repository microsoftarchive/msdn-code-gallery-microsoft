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
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace Microsoft.VisualStudio.Language.Spellchecker
	''' <summary>
	''' Squiggle tag for misspelled words.
	''' </summary>
	Friend Class SpellSquiggleTag
		Inherits ErrorTag
		Public Sub New(ByVal squiggleType As String, ByVal toolTipContent As Object)
			MyBase.New(squiggleType, toolTipContent)
		End Sub
	End Class

	''' <summary>
	''' Tagger for Spelling squiggles.
	''' </summary>
	Friend Class SquiggleTagger
		Implements ITagger(Of IErrorTag), IDisposable
		#Region "Private Fields"
		Private _buffer As ITextBuffer
		Private _misspellingAggregator As ITagAggregator(Of IMisspellingTag)
		Private disposed As Boolean = False
		Friend Const SpellingErrorType As String = "Spelling Error"
		#End Region

		#Region "MEF Imports / Exports"

		''' <summary>
		''' Defines colors for the spelling squiggles.
		''' </summary>
		<Export(GetType(EditorFormatDefinition)), Name(SquiggleTagger.SpellingErrorType), Order(After := Priority.High), UserVisible(True)> _
		Friend Class SpellingErrorClassificationFormatDefinition
			Inherits EditorFormatDefinition
			Public Sub New()
				Me.ForegroundBrush = New SolidColorBrush(Color.FromRgb(149, 23, 149))
				Me.BackgroundCustomizable = False
				Me.DisplayName = "Spelling Error"
			End Sub
		End Class

		''' <summary>
		''' MEF connector for the Spell checker squiggles.
		''' </summary>
		<Export(GetType(IViewTaggerProvider)), ContentType("text"), TagType(GetType(SpellSquiggleTag))> _
		Friend Class SquiggleTaggerProvider
			Implements IViewTaggerProvider
            Private _tagAggregatorFactory As IBufferTagAggregatorFactoryService
            <Import(GetType(IBufferTagAggregatorFactoryService))> _
            Friend Property TagAggregatorFactory() As IBufferTagAggregatorFactoryService
                Get
                    Return _tagAggregatorFactory
                End Get
                Set(ByVal value As IBufferTagAggregatorFactoryService)
                    _tagAggregatorFactory = value
                End Set
            End Property

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

                Return TryCast(New SquiggleTagger(buffer, TagAggregatorFactory.CreateTagAggregator(Of IMisspellingTag)(buffer)), ITagger(Of T))
			End Function
			#End Region
		End Class
		#End Region

		#Region "Constructors"
		Public Sub New(ByVal buffer As ITextBuffer, ByVal misspellingAggregator As ITagAggregator(Of IMisspellingTag))
			_buffer = buffer
			_misspellingAggregator = misspellingAggregator
            AddHandler _misspellingAggregator.TagsChanged, Sub(sender, args)
                                                               For Each span As SnapshotSpan In args.Span.GetSpans(_buffer)
                                                                   RaiseTagsChangedEvent(span)
                                                               Next span
                                                           End Sub
		End Sub


		#End Region

		#Region "ITagger<SpellSquiggleTag> Members"
		''' <summary>
		''' Returns tags on demand.
		''' </summary>
		''' <param name="spans">Spans collection to get tags for.</param>
		''' <returns>Squiggle tags in provided spans.</returns>
		Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of IErrorTag)) Implements ITagger(Of IErrorTag).GetTags
            Dim list As New List(Of ITagSpan(Of IErrorTag))
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
                list.Add(New TagSpan(Of IErrorTag)(errorSpan, New SpellSquiggleTag(SquiggleTagger.SpellingErrorType, errorSpan.GetText())))
            Next misspelling
            Return list
		End Function

		Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of IErrorTag).TagsChanged
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
				End If

				disposed = True
			End If
		End Sub
		#End Region

		#Region "Event Handlers"
		Private Sub SpellingChangedHandler(ByVal sender As Object, ByVal e As SpellingEventArgs)
			Dim snapshot As ITextSnapshot = _buffer.CurrentSnapshot
			RaiseTagsChangedEvent(New SnapshotSpan(snapshot, New Span(0, snapshot.Length)))
		End Sub
		#End Region

		#Region "Helpers"
		Private Sub RaiseTagsChangedEvent(ByVal subjectSpan As SnapshotSpan)
			Dim handler As EventHandler(Of SnapshotSpanEventArgs) = Me.TagsChangedEvent
			If handler IsNot Nothing Then
				handler(Me, New SnapshotSpanEventArgs(subjectSpan))
			End If
		End Sub

		#End Region
	End Class
End Namespace
