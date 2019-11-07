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

' ****************************************************************************
' Copyright (c) Microsoft Corporation.  All rights reserved.
' ****************************************************************************

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.Composition
Imports System.Linq
Imports System.Threading
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Operations
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace HighlightWordSample


    ''' <summary>
    ''' Derive from TextMarkerTag, in case anyone wants to consume
    ''' just the HighlightWordTags by themselves.
    ''' </summary>
    Public Class HighlightWordTag
        Inherits TextMarkerTag

        Public Sub New()
            MyBase.New("blue")
        End Sub
    End Class

    ''' <summary>
    ''' This tagger will provide tags for every word in the buffer that
    ''' matches the word currently under the cursor.
    ''' </summary>
    Public Class HighlightWordTagger
        Implements ITagger(Of HighlightWordTag)

        Private privateView As ITextView
        Private Property View As ITextView
            Get
                Return privateView
            End Get
            Set(ByVal value As ITextView)
                privateView = value
            End Set
        End Property

        Private privateSourceBuffer As ITextBuffer
        Private Property SourceBuffer As ITextBuffer
            Get
                Return privateSourceBuffer
            End Get
            Set(ByVal value As ITextBuffer)
                privateSourceBuffer = value
            End Set
        End Property

        Private privateTextSearchService As ITextSearchService
        Private Property TextSearchService As ITextSearchService
            Get
                Return privateTextSearchService
            End Get
            Set(ByVal value As ITextSearchService)
                privateTextSearchService = value
            End Set
        End Property

        Private privateTextStructureNavigator As ITextStructureNavigator
        Private Property TextStructureNavigator As ITextStructureNavigator
            Get
                Return privateTextStructureNavigator
            End Get
            Set(ByVal value As ITextStructureNavigator)
                privateTextStructureNavigator = value
            End Set
        End Property

        Private updateLock As New Object

        ' The current set of words to highlight
        Private privateWordSpans As NormalizedSnapshotSpanCollection
        Private Property WordSpans As NormalizedSnapshotSpanCollection
            Get
                Return privateWordSpans
            End Get
            Set(ByVal value As NormalizedSnapshotSpanCollection)
                privateWordSpans = value
            End Set
        End Property

        Private privateCurrentWord? As SnapshotSpan
        Private Property CurrentWord As SnapshotSpan?
            Get
                Return privateCurrentWord
            End Get
            Set(ByVal value? As SnapshotSpan)
                privateCurrentWord = value
            End Set
        End Property

        ' The current request, from the last cursor movement or view render
        Private privateRequestedPoint As SnapshotPoint
        Private Property RequestedPoint As SnapshotPoint
            Get
                Return privateRequestedPoint
            End Get
            Set(ByVal value As SnapshotPoint)
                privateRequestedPoint = value
            End Set
        End Property

        Public Sub New(ByVal view As ITextView, ByVal sourceBuffer As ITextBuffer, ByVal textSearchService As ITextSearchService, ByVal textStructureNavigator As ITextStructureNavigator)
            Me.View = view

            Me.SourceBuffer = sourceBuffer
            Me.TextSearchService = textSearchService
            Me.TextStructureNavigator = textStructureNavigator

            Me.WordSpans = New NormalizedSnapshotSpanCollection
            Me.CurrentWord = Nothing

            ' Subscribe to both change events in the view - any time the view is updated
            ' or the caret is moved, we refresh our list of highlighted words.
            AddHandler Me.View.Caret.PositionChanged, AddressOf CaretPositionChanged
            AddHandler Me.View.LayoutChanged, AddressOf ViewLayoutChanged
        End Sub

        Private Sub ViewLayoutChanged(ByVal sender As Object, ByVal e As TextViewLayoutChangedEventArgs)
            ' If a new snapshot wasn't generated, then skip this layout
            If e.NewViewState.EditSnapshot IsNot e.OldViewState.EditSnapshot Then
                UpdateAtCaretPosition(View.Caret.Position)
            End If
        End Sub

        Private Sub CaretPositionChanged(ByVal sender As Object, ByVal e As CaretPositionChangedEventArgs)
            UpdateAtCaretPosition(e.NewPosition)
        End Sub

        Private Sub UpdateAtCaretPosition(ByVal caretPoisition As CaretPosition)
            Dim point? As SnapshotPoint = caretPoisition.Point.GetPoint(SourceBuffer, caretPoisition.Affinity)

            If Not point.HasValue Then
                Return
            End If

            ' If the new cursor position is still within the current word (and on the same snapshot),
            ' we don't need to check it.
            If CurrentWord.HasValue AndAlso CurrentWord.Value.Snapshot Is View.TextSnapshot AndAlso point.Value.Position >= CurrentWord.Value.Start.Position AndAlso point.Value.Position <= CurrentWord.Value.End.Position Then
                Return
            End If

            RequestedPoint = point.Value
            ThreadPool.QueueUserWorkItem(AddressOf UpdateWordAdornments)
        End Sub

        Private Sub UpdateWordAdornments(ByVal threadContext As Object)
            Dim currentRequest As SnapshotPoint = RequestedPoint
            Dim wordSpans As New List(Of SnapshotSpan)

            ' Find all words in the buffer like the one the caret is on
            Dim word As TextExtent = TextStructureNavigator.GetExtentOfWord(currentRequest)
            Dim foundWord As Boolean = True

            ' If we've selected something not worth highlighting, we might have
            ' missed a "word" by a little bit
            If Not WordExtentIsValid(currentRequest, word) Then
                ' Before we retry, make sure it is worthwhile
                If word.Span.Start <> currentRequest OrElse currentRequest = currentRequest.GetContainingLine().Start OrElse Char.IsWhiteSpace((currentRequest - 1).GetChar()) Then
                    foundWord = False
                Else
                    ' Try again, one character previous.  If the caret is at the end of a word, then
                    ' this will pick up the word we are at the end of.
                    word = TextStructureNavigator.GetExtentOfWord(currentRequest - 1)

                    ' If we still aren't valid the second time around, we're done
                    If Not WordExtentIsValid(currentRequest, word) Then
                        foundWord = False
                    End If
                End If
            End If

            If Not foundWord Then
                ' If we couldn't find a word, just clear out the existing markers
                SynchronousUpdate(currentRequest, New NormalizedSnapshotSpanCollection, Nothing)
                Return
            End If

            Dim currentWord As SnapshotSpan = word.Span

            ' If this is the same word we currently have, we're done (e.g. caret moved within a word).
            If Me.CurrentWord.HasValue AndAlso currentWord = Me.CurrentWord Then
                Return
            End If

            ' Find the new spans
            Dim findData As New FindData(currentWord.GetText(), currentWord.Snapshot)
            findData.FindOptions = FindOptions.WholeWord Or FindOptions.MatchCase

            wordSpans.AddRange(TextSearchService.FindAll(findData))

            ' If we are still up-to-date (another change hasn't happened yet), do a real update
            If currentRequest = RequestedPoint Then
                SynchronousUpdate(currentRequest, New NormalizedSnapshotSpanCollection(wordSpans), currentWord)
            End If
        End Sub

        ''' <summary>
        ''' Determine if a given "word" should be highlighted
        ''' </summary>
        Private Shared Function WordExtentIsValid(ByVal currentRequest As SnapshotPoint, ByVal word As TextExtent) As Boolean
            Return word.IsSignificant AndAlso currentRequest.Snapshot.GetText(word.Span).Any(Function(c) Char.IsLetter(c))
        End Function

        ''' <summary>
        ''' Perform a synchronous update, in case multiple background threads are running
        ''' </summary>
        Private Sub SynchronousUpdate(ByVal currentRequest As SnapshotPoint, ByVal newSpans As NormalizedSnapshotSpanCollection, ByVal newCurrentWord? As SnapshotSpan)
            SyncLock updateLock
                If currentRequest <> RequestedPoint Then
                    Return
                End If

                WordSpans = newSpans
                CurrentWord = newCurrentWord

                Dim tempEvent = TagsChangedEvent
                If tempEvent IsNot Nothing Then
                    tempEvent(Me, New SnapshotSpanEventArgs(New SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)))
                End If
            End SyncLock
        End Sub

        Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of HighlightWordTag)) Implements ITagger(Of HighlightWordTag).GetTags
            Dim tags As New List(Of TagSpan(Of HighlightWordTag))

            If Me.CurrentWord Is Nothing Then
                Return Nothing
            End If

            ' Hold on to a "snapshot" of the word spans and current word, so that we maintain the same
            ' collection throughout
            Dim currentWord As SnapshotSpan = Me.CurrentWord.Value
            Dim wordSpans As NormalizedSnapshotSpanCollection = Me.WordSpans

            If spans.Count = 0 OrElse Me.WordSpans.Count = 0 Then
                Return Nothing
            End If

            ' If the requested snapshot isn't the same as the one our words are on, translate our spans
            ' to the expected snapshot
            If spans(0).Snapshot IsNot wordSpans(0).Snapshot Then
                wordSpans = New NormalizedSnapshotSpanCollection(wordSpans.Select(Function(span) span.TranslateTo(spans(0).Snapshot, SpanTrackingMode.EdgeExclusive)))
                currentWord = currentWord.TranslateTo(spans(0).Snapshot, SpanTrackingMode.EdgeExclusive)
            End If

            ' First, yield back the word the cursor is under (if it overlaps)
            ' Note that we'll yield back the same word again in the wordspans collection;
            ' the duplication here is expected.
            If spans.OverlapsWith(New NormalizedSnapshotSpanCollection(currentWord)) Then
                tags.Add(New TagSpan(Of HighlightWordTag)(currentWord, New HighlightWordTag))
            End If

            ' Second, yield all the other words in the file
            For Each span As SnapshotSpan In NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans)
                tags.Add(New TagSpan(Of HighlightWordTag)(span, New HighlightWordTag))
            Next span

            Return tags
        End Function

        Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of HighlightWordTag).TagsChanged

    End Class

    <Export(GetType(IViewTaggerProvider)),
    ContentType("text"),
    TagType(GetType(HighlightWordTag))>
    Public Class HighlightWordTaggerProvider
        Implements IViewTaggerProvider

        Private privateTextSearchService As ITextSearchService
        <Import()>
        Friend Property TextSearchService As ITextSearchService
            Get
                Return privateTextSearchService
            End Get
            Set(ByVal value As ITextSearchService)
                privateTextSearchService = value
            End Set
        End Property

        Private privateTextStructureNavigatorSelector As ITextStructureNavigatorSelectorService
        <Import()>
        Friend Property TextStructureNavigatorSelector As ITextStructureNavigatorSelectorService
            Get
                Return privateTextStructureNavigatorSelector
            End Get
            Set(ByVal value As ITextStructureNavigatorSelectorService)
                privateTextStructureNavigatorSelector = value
            End Set
        End Property

        Public Function CreateTagger(Of T As ITag)(ByVal textView As ITextView, ByVal buffer As ITextBuffer) As ITagger(Of T) Implements IViewTaggerProvider.CreateTagger
            ' Only provide highlighting on the top-level buffer
            If textView.TextBuffer IsNot buffer Then
                Return Nothing
            End If

            Dim textStructureNavigator As ITextStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer)

            Return TryCast(New HighlightWordTagger(textView, buffer, TextSearchService, textStructureNavigator), ITagger(Of T))
        End Function

    End Class
End Namespace
