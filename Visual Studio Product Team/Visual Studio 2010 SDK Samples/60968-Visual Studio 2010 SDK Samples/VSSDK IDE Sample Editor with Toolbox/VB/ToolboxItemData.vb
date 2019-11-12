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

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox

	''' <summary>
	''' This class implements data to be stored in ToolboxItem.
    ''' This class needs to be serializable in order to be passed to the toolbox
    ''' and back.
    '''
    ''' Moreover, this assembly path is required to be on VS probing paths to make
    ''' deserialization successful. See ToolboxItemData.pkgdef.
	''' </summary>
	<Serializable()> _
	Public Class ToolboxItemData
		#Region "Fields"
        Private strContent As String
		#End Region ' Fields

		#Region "Constructors"
		''' <summary>
		''' Overloaded constructor.
		''' </summary>
		''' <param name="sentence">Sentence value.</param>
		Public Sub New(ByVal sentence As String)
            strContent = sentence
		End Sub
		#End Region ' Constructors

		#Region "Properties"
		''' <summary>
		''' Gets the ToolboxItemData Content.
		''' </summary>
		Public ReadOnly Property Content() As String
			Get
                Return strContent
			End Get
		End Property
		#End Region ' Properties
	End Class
End Namespace
