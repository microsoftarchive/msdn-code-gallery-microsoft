'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner.Properties
Imports System.Globalization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    Delegate Sub MatchFoundCallback(ByVal term As ISearchTerm, ByVal line As Integer, ByVal column As Integer, ByVal lineText As String, ByVal warning As String)

    Friend Class MatchFinder
        ''' <summary>
        ''' Creates a match finder for the specified set of term tables.
        ''' </summary>
        ''' <param name="termTables">The set of term tables containing the terms to search for.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>termTables</c> is null.</exception>
        Public Sub New(ByVal termTables As IEnumerable(Of ITermTable))
            If termTables Is Nothing Then
                Throw New ArgumentNullException("termTables")
            End If

            _termIndex = CreateSortedTermIndex(termTables)
            _exclusionIndex = CreateSortedExclusionIndex(_termIndex)
        End Sub

        ''' <summary>
        ''' Processes the next character in the sequence.
        ''' </summary>
        ''' <param name="c">The next character to process.</param>
        ''' <param name="callback">The callback for search hits that are found.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>callback</c> is null.</exception>
        ''' <remarks>
        ''' As a result of this processing, one or more matches may be resolved.  For each match
        ''' that is resolved, <c>callback</c> is called.  The call will specify the following
        ''' arguments:
        '''     <c>term</c>: non-null
        '''     <c>line</c>: zero-based line number
        '''     <c>column</c>: zero-based column number
        '''     <c>lineText</c>: non-null, non-empty line text
        '''     <c>warning</c>: null in most cases, otherwise the text of a warning if one is relevant
        ''' </remarks>
        Public Sub AnalyzeNextCharacter(ByVal c As Char, ByVal callback As MatchFoundCallback)
            UpdateLineAndColumn(c)
            UpdatePartialMatchesWithNextChar(c)

            ' If this is the first character being processed or the previous character was a
            ' separator, we will look for new matches and exclusions now.
            If _lastChar = Char.MinValue OrElse IsSeparator(_lastChar) Then
                FindNewTermsAndExclusionsStartingWith(c)
            End If

            DiscardMarkedItems()

            SendAndDiscardConfirmedMatches(callback)

            DiscardMarkedItems()

            _secondToLastChar = _lastChar
            _lastChar = c
        End Sub

        ''' <summary>
        ''' Signals that the end of input has been reached.  Any pending term matches and
        ''' exclusion matches will be resolved now, and either confirmed or discarded.
        ''' </summary>
        ''' <param name="callback">The callback for any matches that are found.</param>
        Public Sub Finish(ByVal callback As MatchFoundCallback)
            For Each match As TermMatch In PartiallyMatchedTerms
                match.LineCompleted(_currentLineText)
            Next match

            SendAndDiscardConfirmedMatches(callback)
        End Sub

        ''' <summary>
        ''' Resets this match finder to a "clean" state, by removing all cached data and setting
        ''' line and column coordinates to zero.
        ''' </summary>
        Public Sub Reset()
            _currentColumn = -1
            _currentLine = 0
            _lastChar = Char.MinValue
            _secondToLastChar = Char.MinValue
            _partialMatches.Clear()
        End Sub

#Region "Private Members"

        ''' <summary>
        ''' Abstract base class for pending term and exclusion matches.
        ''' </summary>
        Private MustInherit Class MatchBase
            Public MustOverride Sub AddChar(ByVal c As Char)

            Public Sub MarkForDiscard()
                _discardPending = True
            End Sub

            Public ReadOnly Property DiscardPending() As Boolean
                Get
                    Return _discardPending
                End Get
            End Property

#Region "Private Members"

            Private _discardPending As Boolean = False

#End Region
        End Class

        ''' <summary>
        ''' A pending term match in progress.
        ''' </summary>
        Private Class TermMatch
            Inherits MatchBase
            Public Sub New(ByVal finder As MatchFinder, ByVal term As MatchableTerm, ByVal line As Integer, ByVal column As Integer)
                _finder = finder
                _term = term
                _line = line
                _column = column
            End Sub

            Public ReadOnly Property Term() As MatchableTerm
                Get
                    Return _term
                End Get
            End Property

            Public ReadOnly Property Line() As Integer
                Get
                    Return _line
                End Get
            End Property

            Public ReadOnly Property Column() As Integer
                Get
                    Return _column
                End Get
            End Property

            Public ReadOnly Property LineText() As String
                Get
                    Return _lineText
                End Get
            End Property

            ''' <summary>
            ''' Signals that the end of the line on which this term appears has been found.
            ''' </summary>
            ''' <param name="lineText">The full text of the line.</param>
            Public Sub LineCompleted(ByVal lineText As String)
                If _waitingForLineEnd Then
                    _lineText = lineText
                    _waitingForLineEnd = False
                End If

                ' Indicate we're no longer waiting for a separator to indicate end-of-word.
                _nextCharMustBeSeparator = False

                If IsMatchedAndConfirmed Then
                    _finder.RemoveAllMatchesInRangeExceptOne(Line, Column, Line, Column + Term.Term.Text.Length - 1, Me)
                End If
            End Sub

            Public Overrides Sub AddChar(ByVal c As Char)
                If (Not DiscardPending) Then
                    If IsMatched Then
                        ' The term was already matched.  If we're now watching for the end-of-word
                        ' separator, see if this is it.
                        If _nextCharMustBeSeparator Then
                            If MatchFinder.IsSeparator(c) Then
                                _nextCharMustBeSeparator = False
                            Else
                                ' The character following the term match was not a separator, so
                                ' this match is not valid.  Discard it.
                                MarkForDiscard()
                            End If
                        End If
                    Else
                        ' This match is still in progress.  See if the current character matches
                        ' the text we're looking for.
                        If Char.ToLowerInvariant(Term.Term.Text(_matchedChars)) = Char.ToLowerInvariant(c) Then
                            _matchedChars += 1
                            If IsMatched Then
                                _nextCharMustBeSeparator = True
                            End If
                        Else
                            MarkForDiscard()
                        End If
                    End If

                    ' If this term is now matched, discard all other pending matches that overlap
                    ' its span.
                    If IsMatchedAndConfirmed Then
                        _finder.RemoveAllMatchesInRangeExceptOne(Line, Column, Line, Column + Term.Term.Text.Length - 1, Me)
                    End If
                End If
            End Sub

            Public ReadOnly Property IsMatchedAndConfirmed() As Boolean
                Get
                    Return IsMatched AndAlso (Not HasPartiallyMatchedExclusions) AndAlso (Not _waitingForLineEnd) AndAlso (Not IsWaitingOnPreviousMatches) AndAlso Not IsWaitingOnSeparator
                End Get
            End Property

            Public ReadOnly Property IsMatched() As Boolean
                Get
                    Return (Not DiscardPending) AndAlso _matchedChars = Term.Term.Text.Length
                End Get
            End Property

#Region "Private Members"

            Private ReadOnly _term As MatchableTerm
            Private ReadOnly _line As Integer
            Private ReadOnly _column As Integer
            Private _lineText As String
            Private _matchedChars As Integer = 0
            Private _finder As MatchFinder
            Private _nextCharMustBeSeparator As Boolean = False
            Private _waitingForLineEnd As Boolean = True

            Private ReadOnly Property HasPartiallyMatchedExclusions() As Boolean
                Get
                    For Each match As ExclusionMatch In _finder.PartiallyMatchedExclusions
                        If match.Excludes(Me) Then
                            Return True
                        End If
                    Next match

                    Return False
                End Get
            End Property

            ''' <summary>
            ''' This match cannot be confirmed until all matches that began earlier have been
            ''' discarded, since they take precedence and they may invalidate this match when they
            ''' are confirmed.  This property indicates whether any earlier matches are still
            ''' active.
            ''' </summary>
            Private ReadOnly Property IsWaitingOnPreviousMatches() As Boolean
                Get
                    For Each match As TermMatch In _finder.PartiallyMatchedTerms
                        If match Is Me Then
                            ' This is the first match.
                            Return False
                        Else
                            If (Not match.DiscardPending) Then
                                Return True
                            End If
                        End If
                    Next match

                    Throw New InvalidOperationException("This match is not in the list; should be impossible.")
                End Get
            End Property

            Private ReadOnly Property IsWaitingOnSeparator() As Boolean
                Get
                    Return _nextCharMustBeSeparator
                End Get
            End Property

#End Region
        End Class

        ''' <summary>
        ''' A pending exclusion match in progress.
        ''' </summary>
        Private Class ExclusionMatch
            Inherits MatchBase
            Public Sub New(ByVal finder As MatchFinder, ByVal exclusion As MatchableExclusion, ByVal line As Integer, ByVal column As Integer)
                _finder = finder
                _exclusion = exclusion
                _line = line
                _column = column
            End Sub

            Public Overrides Sub AddChar(ByVal c As Char)
                If (Not IsMatched) Then
                    If Char.ToLowerInvariant(_exclusion.Text(_matchedChars)) = Char.ToLowerInvariant(c) Then
                        _matchedChars += 1
                        If IsMatched Then
                            ' When this exclusion is matched, we need to discard all terms it
                            ' excludes.
                            For Each match As TermMatch In _finder.PartiallyMatchedTerms
                                If Me.Excludes(match) Then
                                    match.MarkForDiscard()
                                End If
                            Next match
                        End If
                    Else
                        MarkForDiscard()
                    End If
                End If
            End Sub

            Public Function Excludes(ByVal match As TermMatch) As Boolean
                Return match.Term Is _exclusion.Term AndAlso MatchFinder.RangeContains(_line, _column, _line, _column + _exclusion.Text.Length - 1, match.Line, match.Column) AndAlso MatchFinder.RangeContains(_line, _column, _line, _column + _exclusion.Text.Length - 1, match.Line, match.Column + match.Term.Term.Text.Length - 1)
            End Function

            Public ReadOnly Property IsMatched() As Boolean
                Get
                    Return _matchedChars = _exclusion.Text.Length
                End Get
            End Property

#Region "Private Members"

            Private ReadOnly _finder As MatchFinder
            Private ReadOnly _exclusion As MatchableExclusion
            Private ReadOnly _line As Integer
            Private ReadOnly _column As Integer
            Private _matchedChars As Integer = 0

#End Region
        End Class

        ''' <summary>
        ''' A wrapper for ISearchTerm that stores a few extra properties we need to do matching
        ''' correctly.
        ''' </summary>
        Private Class MatchableTerm
            Private ReadOnly _term As ISearchTerm
            Private ReadOnly _originalTableIndex As Integer
            Private ReadOnly _originalTermIndex As Integer
            Private _hasDuplicates As Boolean

            Public Sub New(ByVal term As ISearchTerm, ByVal originalTableIndex As Integer, ByVal originalTermIndex As Integer)
                _term = term
                _originalTableIndex = originalTableIndex
                _originalTermIndex = originalTermIndex
            End Sub

            Public ReadOnly Property OriginalTableIndex() As Integer
                Get
                    Return _originalTableIndex
                End Get
            End Property

            Public ReadOnly Property OriginalTermIndex() As Integer
                Get
                    Return _originalTermIndex
                End Get
            End Property

            Public ReadOnly Property Term() As ISearchTerm
                Get
                    Return _term
                End Get
            End Property

            Public Property HasDuplicates() As Boolean
                Get
                    Return _hasDuplicates
                End Get
                Set(ByVal value As Boolean)
                    _hasDuplicates = value
                End Set
            End Property
        End Class

        ''' <summary>
        ''' A wrapper for IExclusion that stores a few extra properties we need to do matching
        ''' correctly.
        ''' </summary>
        Private Class MatchableExclusion
            Private ReadOnly _text As String
            Private ReadOnly _term As MatchableTerm

            Public Sub New(ByVal text As String, ByVal term As MatchableTerm)
                _text = text
                _term = term
            End Sub

            Public ReadOnly Property Text() As String
                Get
                    Return _text
                End Get
            End Property

            Public ReadOnly Property Term() As MatchableTerm
                Get
                    Return _term
                End Get
            End Property
        End Class

        Private ReadOnly _termIndex As List(Of MatchableTerm)
        Private ReadOnly _exclusionIndex As List(Of MatchableExclusion)
        Private ReadOnly _partialMatches As List(Of MatchBase) = New List(Of MatchBase)()
        Private _currentLine As Integer = 0
        Private _currentColumn As Integer = -1
        Private _lastChar As Char = Char.MinValue
        Private _secondToLastChar As Char = Char.MinValue
        Private _currentLineText As String = ""

        Private Const LineFeed As Char = ChrW(10)
        Private Const CarriageReturn As Char = ChrW(13)

        Private Shared Function CompareStringsWithLongestFirst(ByVal x As String, ByVal y As String) As Integer
            Dim comparison As Integer = String.Compare(x, 0, y, 0, Math.Min(x.Length, y.Length), StringComparison.OrdinalIgnoreCase)
            If comparison = 0 Then
                If x.Length > y.Length Then
                    Return -1
                ElseIf x.Length < y.Length Then
                    Return 1
                Else
                    Return 0
                End If
            Else
                Return comparison
            End If
        End Function

        Private Shared Function CreateSortedExclusionIndex(ByVal sortedTermIndex As List(Of MatchableTerm)) As List(Of MatchableExclusion)
            Dim index As New List(Of MatchableExclusion)()

            For Each term As MatchableTerm In sortedTermIndex
                For Each exclusion As IExclusion In term.Term.Exclusions
                    index.Add(New MatchableExclusion(exclusion.Text, term))
                Next exclusion
            Next term

            index.Sort(AddressOf AnonymousMethod1)

            Return index
        End Function
        Private Shared Function AnonymousMethod1(ByVal x As MatchableExclusion, ByVal y As MatchableExclusion) As Integer
            Return CompareStringsWithLongestFirst(x.Text, y.Text)
        End Function

        Private Shared Function CreateSortedTermIndex(ByVal termTables As IEnumerable(Of ITermTable)) As List(Of MatchableTerm)
            Dim index As New List(Of MatchableTerm)()
            Dim tableIndex As Integer = 0

            For Each table As ITermTable In termTables
                If table.Terms IsNot Nothing Then
                    Dim termIndex As Integer = 0
                    For Each term As ISearchTerm In table.Terms
                        If term.Text.Length > 0 Then
                            index.Add(New MatchableTerm(term, tableIndex, termIndex))
                        End If
                        termIndex += 1
                    Next term
                End If
                tableIndex += 1
            Next table

            index.Sort(AddressOf AnonymousMethod2)

            For i As Integer = 0 To index.Count - 1
                If (i > 0 AndAlso String.Compare(index(i).Term.Text, index(i - 1).Term.Text, StringComparison.OrdinalIgnoreCase) = 0) OrElse (i < index.Count - 1 AndAlso String.Compare(index(i).Term.Text, index(i + 1).Term.Text, StringComparison.OrdinalIgnoreCase) = 0) Then
                    index(i).HasDuplicates = True
                End If
            Next i

            Return index
        End Function
        Private Shared Function AnonymousMethod2(ByVal x As MatchableTerm, ByVal y As MatchableTerm) As Integer
            Dim result As Integer = CompareStringsWithLongestFirst(x.Term.Text, y.Term.Text)
            If result = 0 Then
                If x.OriginalTableIndex <> y.OriginalTableIndex Then
                    result = x.OriginalTableIndex - y.OriginalTableIndex
                Else
                    result = x.OriginalTermIndex - y.OriginalTermIndex
                End If
            End If
            Return result
        End Function

        Private Sub SendAndDiscardConfirmedMatches(ByVal callback As MatchFoundCallback)
            For Each match As TermMatch In ConfirmedMatches
                If callback IsNot Nothing Then
                    Dim warning As String = Nothing

                    ' Matches with duplicates will have a warning attached.
                    If match.Term.HasDuplicates Then
                        Dim tableList As New StringBuilder()
                        Dim first As Boolean = True
                        For Each term As MatchableTerm In GetDuplicatesOf(match.Term)
                            If (Not first) Then
                                tableList.Append("; ")
                            End If
                            tableList.Append(term.Term.Table.SourceFile)
                            first = False
                        Next term

                        warning = String.Format(CultureInfo.CurrentUICulture, Resources.DuplicateTermWarning, match.Term.Term.Text, tableList.ToString())
                    End If
                    callback(match.Term.Term, match.Line, match.Column, match.LineText, warning)
                End If
                match.MarkForDiscard()
            Next match
        End Sub

        Private Function GetDuplicatesOf(ByVal sourceTerm As MatchableTerm) As IEnumerable(Of MatchableTerm)
            Dim results As New List(Of MatchableTerm)
            For Each possibleDup As MatchableTerm In _termIndex
                If String.Compare(sourceTerm.Term.Text, possibleDup.Term.Text, StringComparison.OrdinalIgnoreCase) = 0 Then
                    results.Add(possibleDup)
                End If
            Next possibleDup
            Return results
        End Function

        Private Sub DiscardMarkedItems()
            Dim i As Integer = 0
            Do While i < _partialMatches.Count
                If _partialMatches(i).DiscardPending Then
                    _partialMatches.RemoveAt(i)
                    i -= 1
                End If
                i += 1
            Loop
        End Sub

        Private Shared Function IsSeparator(ByVal c As Char) As Boolean
            Return (Char.IsWhiteSpace(c) OrElse Char.IsPunctuation(c) OrElse Char.IsSymbol(c)) AndAlso c <> "_"c
        End Function

        Private Sub UpdateLineAndColumn(ByVal c As Char)
            If StartsNewLine(c) Then
                _currentLine += 1
                _currentColumn = 0

                For Each match As TermMatch In PartiallyMatchedTerms
                    match.LineCompleted(_currentLineText)
                Next match
                _currentLineText = "" & c
            Else
                If c <> CarriageReturn AndAlso c <> LineFeed Then
                    _currentColumn += 1
                    _currentLineText &= c
                End If
            End If
        End Sub

        Private Function StartsNewLine(ByVal c As Char) As Boolean
            If _secondToLastChar = CarriageReturn AndAlso _lastChar = LineFeed Then
                ' <cr><lf>
                Return True
            ElseIf _lastChar = CarriageReturn AndAlso c <> LineFeed Then
                ' <cr> alone
                Return True
            ElseIf _lastChar = LineFeed Then
                ' <lf> alone
                Return True
            Else
                Return False
            End If
        End Function

        Private Sub UpdatePartialMatchesWithNextChar(ByVal c As Char)
            For Each match As MatchBase In PartiallyMatchedTermsAndExclusions
                match.AddChar(c)
            Next match
        End Sub

        Private ReadOnly Property PartiallyMatchedTerms() As IEnumerable(Of TermMatch)
            Get
                Dim results As New List(Of TermMatch)
                For Each match As MatchBase In _partialMatches
                    If TypeOf match Is TermMatch Then
                        results.Add(CType(match, TermMatch))
                    End If
                Next match
                Return results
            End Get
        End Property

        Private ReadOnly Property PartiallyMatchedExclusions() As IEnumerable(Of ExclusionMatch)
            Get
                Dim results As New List(Of ExclusionMatch)
                For Each match As MatchBase In _partialMatches
                    If TypeOf match Is ExclusionMatch Then
                        results.Add(CType(match, ExclusionMatch))
                    End If
                Next match
                Return results
            End Get
        End Property

        Private ReadOnly Property PartiallyMatchedTermsAndExclusions() As IEnumerable(Of MatchBase)
            Get
                Return _partialMatches
            End Get
        End Property

        Private Sub FindNewTermsAndExclusionsStartingWith(ByVal c As Char)
            For Each term As MatchableTerm In TermsStartingWith(c)
                Dim match As New TermMatch(Me, term, _currentLine, _currentColumn)
                _partialMatches.Add(match)
                match.AddChar(c)
            Next term

            For Each exclusion As MatchableExclusion In ExclusionsStartingWith(c)
                Dim match As New ExclusionMatch(Me, exclusion, _currentLine, _currentColumn)
                _partialMatches.Add(match)
                match.AddChar(c)
            Next exclusion
        End Sub
        Private NotInheritable Class AnonymousClass1
            Public ch As Char

            Public Function AnonymousMethod(ByVal term As MatchableTerm) As Integer
                Return AscW(Char.ToLowerInvariant(term.Term.Text(0))) - AscW(Char.ToLowerInvariant(ch))
            End Function
        End Class

        Private Function TermsStartingWith(ByVal c As Char) As IEnumerable(Of MatchableTerm)
            Dim locals As New AnonymousClass1()
            locals.ch = c
            Return BinarySearch(_termIndex, AddressOf locals.AnonymousMethod)
        End Function

        Private NotInheritable Class AnonymousClass2
            Public ch As Char

            Public Function AnonymousMethod(ByVal exclusion As MatchableExclusion) As Integer
                Return AscW(Char.ToLowerInvariant(exclusion.Text(0))) - AscW(Char.ToLowerInvariant(ch))
            End Function
        End Class

        Private Function ExclusionsStartingWith(ByVal c As Char) As IEnumerable(Of MatchableExclusion)
            Dim locals As New AnonymousClass2()
            locals.ch = c
            Return BinarySearch(_exclusionIndex, AddressOf locals.AnonymousMethod)
        End Function

        Private Delegate Function BinarySearchDelegate(Of T)(ByVal t As T) As Integer

        Private Shared Function BinarySearch(Of T)(ByVal index As IList(Of T), ByVal tester As BinarySearchDelegate(Of T)) As IEnumerable(Of T)
            If index.Count > 0 Then
                Return BinarySearch(index, 0, index.Count - 1, tester)
            Else
                Return New List(Of T)()
            End If
        End Function

        ''' <summary>
        ''' Finds all items in the specified range of the specified sorted list for which the
        ''' given tester returns 0.
        ''' </summary>
        Private Shared Function BinarySearch(Of T)(ByVal index As IList(Of T), ByVal first As Integer, ByVal last As Integer, ByVal tester As BinarySearchDelegate(Of T)) As IEnumerable(Of T)
            If index Is Nothing Then
                Throw New ArgumentNullException("index")
            End If
            If tester Is Nothing Then
                Throw New ArgumentNullException("tester")
            End If
            If first < 0 Then
                Throw New ArgumentOutOfRangeException("first")
            End If
            If last >= index.Count OrElse last < first Then
                Throw New ArgumentOutOfRangeException("last")
            End If

            Dim middle As Integer = CInt((first + last) / 2)
            Dim middleTest As Integer = tester(index(middle))

            If middleTest > 0 Then
                If middle > first Then
                    Return BinarySearch(index, first, middle - 1, tester)
                Else
                    Return New List(Of T)()
                End If
            ElseIf middleTest < 0 Then
                If last > middle Then
                    Return BinarySearch(index, middle + 1, last, tester)
                Else
                    Return New List(Of T)()
                End If
            Else
                Return ExtendMatchingRange(index, middle, tester)
            End If
        End Function

        Private Shared Function ExtendMatchingRange(Of T)(ByVal index As IList(Of T), ByVal knownMatchIndex As Integer, ByVal tester As BinarySearchDelegate(Of T)) As IEnumerable(Of T)
            Dim dummyFirst As Integer = 0
            Dim dummyLast As Integer = 0
            Return ExtendMatchingRange(index, knownMatchIndex, tester, dummyFirst, dummyLast)
        End Function

        Private Shared Function ExtendMatchingRange(Of T)(ByVal index As IList(Of T), ByVal knownMatchIndex As Integer, ByVal tester As BinarySearchDelegate(Of T), <System.Runtime.InteropServices.Out()> ByRef first As Integer, <System.Runtime.InteropServices.Out()> ByRef last As Integer) As IEnumerable(Of T)
            If index Is Nothing Then
                Throw New ArgumentNullException("index")
            End If
            If tester Is Nothing Then
                Throw New ArgumentNullException("tester")
            End If
            If knownMatchIndex < 0 OrElse knownMatchIndex >= index.Count Then
                Throw New ArgumentOutOfRangeException("knownMatchIndex")
            End If

            first = knownMatchIndex
            last = knownMatchIndex

            Do While first > 0 AndAlso tester(index(first - 1)) = 0
                first -= 1
            Loop

            Do While last + 1 < index.Count AndAlso tester(index(last + 1)) = 0
                last += 1
            Loop

            Dim result As New List(Of T)()

            For i As Integer = first To last
                result.Add(index(i))
            Next i

            Return result
        End Function

        Private ReadOnly Property ConfirmedMatches() As IEnumerable(Of TermMatch)
            Get
                Dim results As New List(Of TermMatch)
                For Each match As TermMatch In PartiallyMatchedTerms
                    If match.IsMatchedAndConfirmed Then
                        results.Add(match)
                        match.MarkForDiscard()
                    Else
                        If (Not match.DiscardPending) Then
                            Exit For
                        End If
                    End If
                Next match
                Return results
            End Get
        End Property

        Private Sub RemoveAllMatchesInRangeExceptOne(ByVal lineStart As Integer, ByVal columnStart As Integer, ByVal lineEnd As Integer, ByVal columnEnd As Integer, ByVal matchToSave As TermMatch)
            For Each match As TermMatch In PartiallyMatchedTerms
                If Not match Is matchToSave AndAlso RangesOverlap(lineStart, columnStart, lineEnd, columnEnd, match.Line, match.Column, match.Line, match.Column + match.Term.Term.Text.Length - 1) Then
                    match.MarkForDiscard()
                End If
            Next match
        End Sub

        Private Shared Function RangesOverlap(ByVal firstLineStart As Integer, ByVal firstColumnStart As Integer, ByVal firstLineEnd As Integer, ByVal firstColumnEnd As Integer, ByVal secondLineStart As Integer, ByVal secondColumnStart As Integer, ByVal secondLineEnd As Integer, ByVal secondColumnEnd As Integer) As Boolean
            If firstLineEnd < secondLineStart Then
                Return False
            End If

            If secondLineEnd < firstLineStart Then
                Return False
            End If

            ' At this point, we know they at least touch the same line (either firstLineEnd >=
            ' secondLineStart or secondLineEnd >= firstLineStart), but they may not overlap.

            If firstLineEnd = secondLineStart Then
                Return firstColumnEnd >= secondColumnStart
            End If

            If secondLineEnd = firstLineStart Then
                Return secondColumnEnd >= firstColumnStart
            End If

            Return True
        End Function

        Private Shared Function RangeContains(ByVal lineStart As Integer, ByVal columnStart As Integer, ByVal lineEnd As Integer, ByVal columnEnd As Integer, ByVal line As Integer, ByVal column As Integer) As Boolean
            If line < lineStart OrElse line > lineEnd Then
                Return False
            End If

            If line = lineStart AndAlso column < columnStart Then
                Return False
            End If

            If line = lineEnd AndAlso column > columnEnd Then
                Return False
            End If

            Return True
        End Function

#End Region
    End Class
End Namespace
