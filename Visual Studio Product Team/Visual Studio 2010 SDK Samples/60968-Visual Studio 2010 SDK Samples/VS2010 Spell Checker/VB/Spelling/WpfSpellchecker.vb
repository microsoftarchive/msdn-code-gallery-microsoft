
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
Imports System.IO
Imports Microsoft.VisualStudio.Text

Namespace Microsoft.VisualStudio.Language.Spellchecker
    ''' <summary>
    ''' Spell checking provider based on WPF spell checker
    ''' </summary>
    <Export(GetType(ISpellingDictionaryService))> _
    Friend Class SpellingDictionaryService
        Implements ISpellingDictionaryService
#Region "Private data"
        Private _ignoreWords As New SortedSet(Of String)()
        Private _ignoreWordsFile As String
#End Region

#Region "Cached span"
        Private Class CachedSpan
            Public Sub New(ByVal start As Integer, ByVal length As Integer, ByVal suggestions As IEnumerable(Of String))
                Me.Start = start
                Me.Length = length
                Me.Suggestions = suggestions
            End Sub

            Private _start As Integer
            Public Property Start() As Integer
                Get
                    Return _start
                End Get
                Private Set(ByVal value As Integer)
                    _start = value
                End Set
            End Property
            Private _length As Integer
            Public Property Length() As Integer
                Get
                    Return _length
                End Get
                Private Set(ByVal value As Integer)
                    _length = value
                End Set
            End Property
            Private _suggestions As IEnumerable(Of String)
            Public Property Suggestions() As IEnumerable(Of String)
                Get
                    Return _suggestions
                End Get
                Private Set(ByVal value As IEnumerable(Of String))
                    _suggestions = value
                End Set
            End Property
        End Class

        Private Class CachedSpanList
            Inherits List(Of CachedSpan)
            Public Sub New()
                HitCount = 0
            End Sub

            Public Property HitCount() As UInteger
        End Class

        Private Class SpanCache
            Private m_cleanupCount As ULong = 0
            Private m_spans As New Dictionary(Of String, CachedSpanList)()
            Public Sub New()
                MaxCacheCount = 10000
                CleanupRate = MaxCacheCount * CType(10, UInteger)
            End Sub

            Public Function TryGetValue(ByVal key As String, <System.Runtime.InteropServices.Out()> ByRef value As CachedSpanList) As Boolean
                Dim result As Boolean = m_spans.TryGetValue(key, value)
                If result Then
                    value.HitCount += CType(1, UInteger)
                End If
                Return result
            End Function


            Default Public Property Item(ByVal key As String) As CachedSpanList
                Get
                    Dim value As CachedSpanList = m_spans(key)
                    value.HitCount += CType(1, UInteger)
                    Return value
                End Get
                Set(ByVal value As CachedSpanList)
                    m_spans(key) = value
                    CleanupCache()
                End Set
            End Property

            Public Sub Clear()
                m_spans.Clear()
            End Sub


            Public Property MaxCacheCount() As UInteger

            Public Property CleanupRate() As UInteger

            Private Sub CleanupCache()
                If Function() As ULong
                       m_cleanupCount += CType(1, ULong)
                       Return m_cleanupCount
                   End Function() > CleanupRate Then
                    m_cleanupCount = 0
                    If m_spans.Count > MaxCacheCount Then
                        Dim removedStep As UInteger = CType((CUInt(m_spans.Count)) / (MaxCacheCount \ 2), UInteger)
                        Dim index As UInteger = 0
                        For Each i In m_spans
                            If i.Value.HitCount < 2 OrElse index Mod removedStep <> 0 Then
                                m_spans.Remove(i.Key)
                            End If
                            index += CType(1, UInteger)
                            removedStep = CType((removedStep + 1), UInteger)
                        Next i
                    End If

                End If
            End Sub
        End Class

#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for SpellingDictionaryService
        ''' </summary>
        Public Sub New()
            Dim localFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\VisualStudio\10.0\SpellChecker")
            If Not Directory.Exists(localFolder) Then
                Directory.CreateDirectory(localFolder)
            End If
            _ignoreWordsFile = Path.Combine(localFolder, "Dictionary.txt")

            LoadIgnoreDictionary()
        End Sub
#End Region

#Region "ISpellingDictionaryService"

        ''' <summary>
        ''' Adds given word to the dictionary.
        ''' </summary>
        ''' <param name="word">The word to add to the dictionary.</param>
        Public Sub AddWordToDictionary(ByVal word As String) Implements ISpellingDictionaryService.AddWordToDictionary
            If (Not String.IsNullOrEmpty(word)) AndAlso (Not _ignoreWords.Contains(word)) Then
                SyncLock _ignoreWords
                    _ignoreWords.Add(word)
                End SyncLock

                ' Add this word to the dictionary file.
                Using writer As New StreamWriter(_ignoreWordsFile, True)
                    writer.WriteLine(word)
                End Using

                ' Notify listeners.
                RaiseSpellingChangedEvent(word)
            End If
        End Sub

        Public Function IsWordInDictionary(ByVal word As String) As Boolean Implements ISpellingDictionaryService.IsWordInDictionary
            SyncLock _ignoreWords
                Return _ignoreWords.Contains(word)
            End SyncLock
        End Function

        Public Event DictionaryUpdated As EventHandler(Of SpellingEventArgs) Implements ISpellingDictionaryService.DictionaryUpdated

#End Region

#Region "Helpers"

        Private Sub RaiseSpellingChangedEvent(ByVal word As String)
            Dim temp = DictionaryUpdatedEvent
            If temp IsNot Nothing Then
                RaiseEvent DictionaryUpdated(Me, New SpellingEventArgs(word))
            End If
        End Sub

        Private Sub LoadIgnoreDictionary()
            If File.Exists(_ignoreWordsFile) Then
                _ignoreWords.Clear()
                Using reader As New StreamReader(_ignoreWordsFile)
                    Dim word As String
                    word = reader.ReadLine()
                    Do While Not String.IsNullOrEmpty(word)
                        _ignoreWords.Add(word)
                        word = reader.ReadLine()
                    Loop
                End Using
            End If
        End Sub
#End Region
    End Class
End Namespace