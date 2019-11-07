
Imports System.Text
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Text
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Utilities
Imports System.Threading
Imports System.Windows.Threading

Namespace Microsoft.VisualStudio.Language.Spellchecker
    <Export(GetType(ITaggerProvider)), ContentType("text"), TagType(GetType(IMisspellingTag))> _
    Friend NotInheritable Class SpellingTaggerProvider
        Implements ITaggerProvider
        <Import()> _
        Private AggregatorFactory As IBufferTagAggregatorFactoryService = Nothing

        <Import()> _
        Private SpellingDictionary As ISpellingDictionaryService = Nothing

        Public Function CreateTagger(Of T As ITag)(ByVal buffer As ITextBuffer) As ITagger(Of T) Implements ITaggerProvider.CreateTagger
            Return buffer.Properties.GetOrCreateSingletonProperty(Function()
                                                                      Return CType(New SpellingTagger(buffer, AggregatorFactory.CreateTagAggregator(Of NaturalTextTag)(buffer), SpellingDictionary), ITagger(Of T))
                                                                  End Function)
        End Function
    End Class

    Friend Class MisspellingTag
        Implements IMisspellingTag
        Public Sub New(ByVal span As SnapshotSpan, ByVal suggestions As IEnumerable(Of String))
            Me.Span = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive)
            Me.Suggestions = suggestions
        End Sub

        Private _privateSpan As ITrackingSpan
        Public Property Span() As ITrackingSpan
            Get
                Return _privateSpan
            End Get
            Private Set(ByVal value As ITrackingSpan)
                _privateSpan = value
            End Set
        End Property

        Public ReadOnly Property ISuggestions() As IEnumerable(Of String) Implements IMisspellingTag.Suggestions
            Get
                Return Suggestions
            End Get
        End Property

        Public Property Suggestions() As IEnumerable(Of String)

        Public Function ToTagSpan(ByVal snapshot As ITextSnapshot) As ITagSpan(Of IMisspellingTag)
            Return New TagSpan(Of IMisspellingTag)(Span.GetSpan(snapshot), Me)
        End Function
    End Class

    Friend NotInheritable Class SpellingTagger
        Implements ITagger(Of IMisspellingTag)
        Private _buffer As ITextBuffer
        Private _naturalTextTagger As ITagAggregator(Of NaturalTextTag)
        Private _dispatcher As Dispatcher
        Private _dictionary As ISpellingDictionaryService

        Private _dirtySpans As List(Of SnapshotSpan)
        Private _dirtySpanLock As New Object()
        Private _misspellings As List(Of MisspellingTag)

        Private _updateThread As Thread

        Private _timer As DispatcherTimer

        Public Sub New(ByVal buffer As ITextBuffer, ByVal naturalTextTagger As ITagAggregator(Of NaturalTextTag), ByVal dictionary As ISpellingDictionaryService)
            _buffer = buffer
            _naturalTextTagger = naturalTextTagger
            _dispatcher = Dispatcher.CurrentDispatcher
            _dictionary = dictionary

            _dirtySpans = New List(Of SnapshotSpan)()
            _misspellings = New List(Of MisspellingTag)()

            AddHandler _buffer.Changed, AddressOf BufferChanged
            AddHandler _naturalTextTagger.TagsChanged, AddressOf NaturalTagsChanged
            AddHandler _dictionary.DictionaryUpdated, AddressOf DictionaryUpdated

            ' To start with, the entire buffer is dirty
            ' Split this into chunks, so we update pieces at a time
            Dim snapshot As ITextSnapshot = _buffer.CurrentSnapshot

            For Each line In snapshot.Lines
                AddDirtySpan(line.Extent)
            Next line
        End Sub

        Private Sub NaturalTagsChanged(ByVal sender As Object, ByVal e As TagsChangedEventArgs)
            Dim dirtySpans As NormalizedSnapshotSpanCollection = e.Span.GetSpans(_buffer.CurrentSnapshot)

            If dirtySpans.Count = 0 Then
                Return
            End If

            Dim dirtySpan As New SnapshotSpan(_buffer.CurrentSnapshot, dirtySpans(0).Start, dirtySpans(dirtySpans.Count - 1).End)

            AddDirtySpan(dirtySpan)

            Dim temp = TagsChangedEvent
            If temp IsNot Nothing Then
                temp(Me, New SnapshotSpanEventArgs(dirtySpan))
            End If
        End Sub

        Private Sub DictionaryUpdated(ByVal sender As Object, ByVal e As SpellingEventArgs)
            Dim currentMisspellings As List(Of MisspellingTag) = _misspellings
            Dim snapshot As ITextSnapshot = _buffer.CurrentSnapshot

            For Each misspelling In currentMisspellings
                Dim span As SnapshotSpan = misspelling.Span.GetSpan(snapshot)

                If span.GetText() = e.Word Then
                    AddDirtySpan(span)
                End If
            Next misspelling
        End Sub

        Private Sub BufferChanged(ByVal sender As Object, ByVal e As TextContentChangedEventArgs)
            Dim snapshot As ITextSnapshot = e.After

            For Each change In e.Changes
                Dim changedSpan As New SnapshotSpan(snapshot, change.NewSpan)

                Dim startLine = changedSpan.Start.GetContainingLine()
                Dim endLine = If((startLine.EndIncludingLineBreak < changedSpan.End), changedSpan.End.GetContainingLine(), startLine)

                AddDirtySpan(New SnapshotSpan(startLine.Start, endLine.End))
            Next change
        End Sub

#Region "Helpers"

        Private Function GetNaturalLanguageSpansForDirtySpan(ByVal dirtySpan As SnapshotSpan) As NormalizedSnapshotSpanCollection
            If dirtySpan.IsEmpty Then
                Return New NormalizedSnapshotSpanCollection()
            End If

            Dim snapshot As ITextSnapshot = dirtySpan.Snapshot
            Return New NormalizedSnapshotSpanCollection(_naturalTextTagger.GetTags(dirtySpan).SelectMany(Function(tag) tag.Span.GetSpans(snapshot)).Select(Function(s) s.Intersection(dirtySpan)).Where(Function(s) s.HasValue AndAlso (Not s.Value.IsEmpty)).Select(Function(s) s.Value))
        End Function

        Private Sub AddDirtySpan(ByVal span As SnapshotSpan)
            Dim naturalLanguageSpans = GetNaturalLanguageSpansForDirtySpan(span)

            If naturalLanguageSpans.Count = 0 Then
                Return
            End If

            SyncLock _dirtySpanLock
                _dirtySpans.AddRange(naturalLanguageSpans)
                ScheduleUpdate()
            End SyncLock
        End Sub

        Private Sub ScheduleUpdate()
            If _timer Is Nothing Then
                _timer = New DispatcherTimer(DispatcherPriority.ApplicationIdle, _dispatcher) With {.Interval = TimeSpan.FromMilliseconds(500)}

                AddHandler _timer.Tick, Sub(sender, args)
                                            If _updateThread IsNot Nothing AndAlso _updateThread.IsAlive Then
                                                Return
                                            End If
                                            _timer.Stop()
                                            _updateThread = New Thread(AddressOf CheckSpellings) With {.Name = "Spell Check", .Priority = ThreadPriority.Normal}
                                            If Not _updateThread.TrySetApartmentState(ApartmentState.STA) Then
                                                Debug.Fail("Unable to set thread apartment state to STA, things *will* break.")
                                            End If
                                            _updateThread.Start()
                                        End Sub
            End If

            _timer.Stop()
            _timer.Start()
        End Sub

        Private Sub CheckSpellings(ByVal obj As Object)
            Dim textBox As New TextBox()
            textBox.SpellCheck.IsEnabled = True

            Dim dirtySpans As IList(Of SnapshotSpan)

            SyncLock _dirtySpanLock
                dirtySpans = _dirtySpans
                If dirtySpans.Count = 0 Then
                    Return
                End If

                _dirtySpans = New List(Of SnapshotSpan)()
            End SyncLock

            Dim snapshot As ITextSnapshot = _buffer.CurrentSnapshot

            Dim dirty As New NormalizedSnapshotSpanCollection(dirtySpans.Select(Function(span) span.TranslateTo(snapshot, SpanTrackingMode.EdgeInclusive)))

            If dirty.Count = 0 Then
                Debug.Fail("The list of dirty spans is empty when normalized, which shouldn't be possible.")
                Return
            End If

            ' Break up dirty into component pieces, so we produce incremental updates
            For Each dirtySpan In dirty
                Dim currentMisspellings As List(Of MisspellingTag) = _misspellings
                Dim newMisspellings As New List(Of MisspellingTag)()
                Dim dir1 As SnapshotSpan = dirtySpan
                Dim removed As Integer = currentMisspellings.RemoveAll(Function(tag) tag.ToTagSpan(snapshot).Span.OverlapsWith(dir1))
                newMisspellings.AddRange(GetMisspellingsInSpan(dirtySpan, textBox))

                ' Also remove empties
                removed += currentMisspellings.RemoveAll(Function(tag) tag.ToTagSpan(snapshot).Span.IsEmpty)

                ' If anything has been updated, we need to send out a change event
                If newMisspellings.Count <> 0 OrElse removed <> 0 Then
                    currentMisspellings.AddRange(newMisspellings)

                    _dispatcher.Invoke(New Action(Sub()
                                                      _misspellings = currentMisspellings
                                                      Dim temp = TagsChangedEvent
                                                      If temp IsNot Nothing Then
                                                          temp(Me, New SnapshotSpanEventArgs(dir1))
                                                      End If
                                                  End Sub))
                End If
            Next dirtySpan

            SyncLock _dirtySpanLock
                If _dirtySpans.Count <> 0 Then
                    _dispatcher.Invoke(New Action(Sub() ScheduleUpdate()))
                End If
            End SyncLock
        End Sub

        Private Function GetMisspellingsInSpan(ByVal span As SnapshotSpan, ByVal textBox As TextBox) As IEnumerable(Of MisspellingTag)
            Dim list As New List(Of MisspellingTag)
            Dim text As String = span.GetText()

            ' We need to break this up for WPF, because it is *incredibly* slow at checking the spelling
            For i As Integer = 0 To text.Length - 1
                If text.Chars(i) = " "c OrElse text.Chars(i) = ControlChars.Tab OrElse text.Chars(i) = ControlChars.Cr OrElse text.Chars(i) = ControlChars.Lf Then
                    Continue For
                End If

                ' We've found a word (or something), so search for the next piece of whitespace or punctuation to get the entire word span.
                ' However, we will ignore words that are CamelCased, since those are probably not "real" words to begin with.
                Dim [end] As Integer = i
                Dim foundLower As Boolean = False
                Dim ignoreWord As Boolean = False
                Do While [end] < text.Length
                    Dim c As Char = text.Chars([end])

                    If c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf Then
                        Exit Do
                    End If

                    If Not ignoreWord Then
                        Dim isUppercase As Boolean = Char.IsUpper(c)

                        If foundLower AndAlso isUppercase Then
                            ignoreWord = True
                        End If

                        foundLower = Not isUppercase
                    End If
                    [end] += 1
                Loop

                ' Skip this word and move on to the next
                If ignoreWord Then
                    i = [end] - 1
                    Continue For
                End If

                Dim textToParse As String = text.Substring(i, [end] - i)

                ' Now pass these off to WPF
                textBox.Text = textToParse

                Dim nextSearchIndex As Integer = 0
                Dim nextSpellingErrorIndex As Integer = -1

                nextSpellingErrorIndex = textBox.GetNextSpellingErrorCharacterIndex(nextSearchIndex, LogicalDirection.Forward)
                Do While -1 <> nextSpellingErrorIndex
                    Dim spellingError = textBox.GetSpellingError(nextSpellingErrorIndex)
                    Dim length As Integer = textBox.GetSpellingErrorLength(nextSpellingErrorIndex)

                    Dim errorSpan As New SnapshotSpan(span.Snapshot, span.Start + i + nextSpellingErrorIndex, length)

                    If _dictionary.IsWordInDictionary(errorSpan.GetText()) Then
                        spellingError.IgnoreAll()
                    Else
                        list.Add(New MisspellingTag(errorSpan, spellingError.Suggestions.ToArray()))
                    End If

                    nextSearchIndex = nextSpellingErrorIndex + length
                    If nextSearchIndex >= textToParse.Length Then
                        Exit Do
                    End If
                    nextSpellingErrorIndex = textBox.GetNextSpellingErrorCharacterIndex(nextSearchIndex, LogicalDirection.Forward)
                Loop

                ' Move past this word
                i = [end] - 1
            Next i

            Return list
        End Function

#End Region

#Region "Tagging implementation"

        Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of IMisspellingTag)) Implements ITagger(Of IMisspellingTag).GetTags
            Dim list As New List(Of ITagSpan(Of IMisspellingTag))
            If spans.Count = 0 Then
                Return list
            End If

            Dim currentMisspellings As List(Of MisspellingTag) = _misspellings

            If currentMisspellings.Count = 0 Then
                Return list
            End If

            Dim snapshot As ITextSnapshot = spans(0).Snapshot

            For Each misspelling In currentMisspellings
                Dim tagSpan = misspelling.ToTagSpan(snapshot)
                If tagSpan.Span.Length = 0 Then
                    Continue For
                End If

                If spans.IntersectsWith(New NormalizedSnapshotSpanCollection(tagSpan.Span)) Then
                    list.Add(tagSpan)
                End If
            Next misspelling
            Return list
        End Function

        Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of IMisspellingTag).TagsChanged

#End Region
    End Class
End Namespace