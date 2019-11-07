
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
Imports Microsoft.VisualStudio.Text

Namespace Microsoft.VisualStudio.Language.Spellchecker
    Public Class SpellingEventArgs
        Inherits EventArgs
        Public Sub New(ByVal word As String)
            Me.Word = word
        End Sub

        ''' <summary>
        ''' Word placed in the Dictionary.
        ''' </summary>
        Private _word As String
        Public Property Word() As String
            Get
                Return _word
            End Get
            Private Set(ByVal value As String)
                _word = value
            End Set
        End Property
    End Class

    Public Interface ISpellingDictionaryService
        ''' <summary>
        ''' Add the given word to the dictionary, so that it will no longer show up as an
        ''' incorrect spelling.
        ''' </summary>
        ''' <param name="word">The word to add to the dictionary.</param>
        Sub AddWordToDictionary(ByVal word As String)

        ''' <summary>
        ''' Check the ignore dictionary for the given word.
        ''' </summary>
        ''' <param name="word"></param>
        ''' <returns></returns>
        Function IsWordInDictionary(ByVal word As String) As Boolean

        ''' <summary>
        ''' Raised when a new word is added to the dictionary, with the word
        ''' that was added.
        ''' </summary>
        Event DictionaryUpdated As EventHandler(Of SpellingEventArgs)
    End Interface
End Namespace