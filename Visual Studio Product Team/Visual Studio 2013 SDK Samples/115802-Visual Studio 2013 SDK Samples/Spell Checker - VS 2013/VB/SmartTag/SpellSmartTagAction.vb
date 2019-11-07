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
	''' Smart tag actions for spelling suggestions.
	''' </summary>
	Friend Class SpellSmartTagAction
		Implements ISmartTagAction
		#Region "private data"
		Private _span As ITrackingSpan
		Private _replaceWith As String
		#End Region

		''' <summary>
		''' Constructor for spelling suggestions smart tag actions.
		''' </summary>
		''' <param name="span">Word to replace.</param>
		''' <param name="replaceWith">Text to replace misspelled word with.</param>
		''' <param name="enabled">Enable/disable this action.</param>
		Public Sub New(ByVal span As ITrackingSpan, ByVal replaceWith As String, ByVal enabled As Boolean)
			_span = span
			_replaceWith = replaceWith
		End Sub
		#Region "ISmartTagAction"
		''' <summary>
		''' Display text.
		''' </summary>
		Public ReadOnly Property DisplayText() As String Implements ISmartTagAction.DisplayText
			Get
				Return _replaceWith
			End Get
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
			_span.TextBuffer.Replace(_span.GetSpan(_span.TextBuffer.CurrentSnapshot), _replaceWith)
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
