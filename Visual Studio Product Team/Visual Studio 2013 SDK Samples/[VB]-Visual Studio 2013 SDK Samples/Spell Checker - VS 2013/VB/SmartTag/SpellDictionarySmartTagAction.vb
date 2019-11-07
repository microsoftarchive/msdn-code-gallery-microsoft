
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
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Text
Imports System.Collections.ObjectModel

Namespace Microsoft.VisualStudio.Language.Spellchecker
    ''' <summary>
    ''' Smart tag action for adding new words to the dictionary.
    ''' </summary>
    Friend Class SpellDictionarySmartTagAction
        Implements ISmartTagAction
#Region "Private data"
        Private _span As ITrackingSpan
        Private _dictionary As ISpellingDictionaryService
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for SpellDictionarySmartTagAction.
        ''' </summary>
        ''' <param name="span">Word to add to dictionary.</param>
        ''' <param name="dictionary">The dictionary (used to ignore the word).</param>
        ''' <param name="displayText">Text to show in the context menu for this action.</param>
        Public Sub New(ByVal span As ITrackingSpan, ByVal dictionary As ISpellingDictionaryService, ByVal displayText As String)
            _span = span
            _dictionary = dictionary
            Me.DisplayText = displayText
        End Sub
#End Region

#Region "ISmartTagAction implementation"
        ''' <summary>
        ''' Text to display in the context menu.
        ''' </summary>

        Public ReadOnly Property IDisplayText() As String Implements ISmartTagAction.DisplayText
            Get
                Return DisPlayText
            End Get
        End Property

        Private _displayText As String
        Public Property DisPlayText() As String
            Get
                Return _displayText
            End Get
            Private Set(ByVal value As String)
                _displayText = value
            End Set
        End Property

        ''' <summary>
        ''' Icon to place next to the display text.
        ''' </summary>
        Public ReadOnly Property Icon() As ImageSource Implements ISmartTagAction.Icon
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' This method is executed when action is selected in the context menu.
        ''' </summary>
        Public Sub Invoke() Implements ISmartTagAction.Invoke
            _dictionary.AddWordToDictionary(_span.GetText(_span.TextBuffer.CurrentSnapshot))
        End Sub

        ''' <summary>
        ''' Enable/disable this action.
        ''' </summary>
        Public ReadOnly Property IsEnabled() As Boolean Implements ISmartTagAction.IsEnabled
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Action set to make sub menus.
        ''' </summary>
        Public ReadOnly Property ActionSets() As ReadOnlyCollection(Of SmartTagActionSet) Implements ISmartTagAction.ActionSets
            Get
                Return Nothing
            End Get
        End Property
#End Region
    End Class
End Namespace