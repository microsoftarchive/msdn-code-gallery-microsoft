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
Imports System.Xml.Serialization
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.Runtime.InteropServices
Imports System.ComponentModel


Namespace MyCompany.RdtEventExplorer
	''' <summary>
	''' Interfact that describes the options that control the RDT Explorer grid display.
	''' </summary>
	Public Interface IOptions
		#Region "IVsRunningDocTableEvents# Properties"
		Property OptAfterAttributeChange() As Boolean
		Property OptAfterDocumentWindowHide() As Boolean
		Property OptAfterFirstDocumentLock() As Boolean
		Property OptAfterSave() As Boolean
		Property OptBeforeDocumentWindowShow() As Boolean
		Property OptBeforeLastDocumentUnlock() As Boolean
		Property OptAfterAttributeChangeEx() As Boolean
		Property OptBeforeSave() As Boolean
		Property OptAfterLastDocumentUnlock() As Boolean
		Property OptAfterSaveAll() As Boolean
		Property OptBeforeFirstDocumentLock() As Boolean
		#End Region
		' Return a [this] pointer so that options can be passed as a group via automation.
		ReadOnly Property ContainedOptions() As Options
	End Interface

	''' <summary>
	''' Implementation of IOptions.
	''' </summary>
	<ClassInterface(ClassInterfaceType.AutoDual), Guid("8ACA7448-B10D-4534-B6B6-234331DE58A1")> _
	Public Class Options
		Implements IOptions
		Public Sub New()
		End Sub
		#Region "IVsRunningDocTableEvents# Properties"

        Private fldOptAfterAttributeChange As Boolean = True
        Private fldOptAfterDocumentWindowHide As Boolean = True
        Private fldOptAfterFirstDocumentLock As Boolean = True
        Private fldOptAfterSave As Boolean = True
        Private fldOptBeforeDocumentWindowShow As Boolean = True
        Private fldOptBeforeLastDocumentUnlock As Boolean = True
        Private fldOptAfterAttributeChangeEx As Boolean = True
        Private fldOptBeforeSave As Boolean = True
        Private fldOptAfterLastDocumentUnlock As Boolean = True
        Private fldOptAfterSaveAll As Boolean = True
        Private fldOptBeforeFirstDocumentLock As Boolean = True

		Public Property OptAfterAttributeChange() As Boolean Implements IOptions.OptAfterAttributeChange
			Get
                Return fldOptAfterAttributeChange
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterAttributeChange = value
			End Set
		End Property
		Public Property OptAfterDocumentWindowHide() As Boolean Implements IOptions.OptAfterDocumentWindowHide
			Get
                Return fldOptAfterDocumentWindowHide
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterDocumentWindowHide = value
			End Set
		End Property
		Public Property OptAfterFirstDocumentLock() As Boolean Implements IOptions.OptAfterFirstDocumentLock
			Get
                Return fldOptAfterFirstDocumentLock
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterFirstDocumentLock = value
			End Set
		End Property
		Public Property OptAfterSave() As Boolean Implements IOptions.OptAfterSave
			Get
                Return fldOptAfterSave
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterSave = value
			End Set
		End Property
		Public Property OptBeforeDocumentWindowShow() As Boolean Implements IOptions.OptBeforeDocumentWindowShow
			Get
                Return fldOptBeforeDocumentWindowShow
			End Get
			Set(ByVal value As Boolean)
                fldOptBeforeDocumentWindowShow = value
			End Set
		End Property
		Public Property OptBeforeLastDocumentUnlock() As Boolean Implements IOptions.OptBeforeLastDocumentUnlock
			Get
                Return fldOptBeforeLastDocumentUnlock
			End Get
			Set(ByVal value As Boolean)
                fldOptBeforeLastDocumentUnlock = value
			End Set
		End Property
		Public Property OptAfterAttributeChangeEx() As Boolean Implements IOptions.OptAfterAttributeChangeEx
			Get
                Return fldOptAfterAttributeChangeEx
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterAttributeChangeEx = value
			End Set
		End Property
		Public Property OptBeforeSave() As Boolean Implements IOptions.OptBeforeSave
			Get
                Return fldOptBeforeSave
			End Get
			Set(ByVal value As Boolean)
                fldOptBeforeSave = value
			End Set
		End Property
		Public Property OptAfterLastDocumentUnlock() As Boolean Implements IOptions.OptAfterLastDocumentUnlock
			Get
                Return fldOptAfterLastDocumentUnlock
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterLastDocumentUnlock = value
			End Set
		End Property
		Public Property OptAfterSaveAll() As Boolean Implements IOptions.OptAfterSaveAll
			Get
                Return fldOptAfterSaveAll
			End Get
			Set(ByVal value As Boolean)
                fldOptAfterSaveAll = value
			End Set
		End Property
		Public Property OptBeforeFirstDocumentLock() As Boolean Implements IOptions.OptBeforeFirstDocumentLock
			Get
                Return fldOptBeforeFirstDocumentLock
			End Get
			Set(ByVal value As Boolean)
                fldOptBeforeFirstDocumentLock = value
			End Set
		End Property
		#End Region
		' Return a [this] pointer so that options can be passed as a group via automation.
		Public ReadOnly Property ContainedOptions() As Options Implements IOptions.ContainedOptions
			Get
				Return Me
			End Get
		End Property
	End Class
End Namespace
