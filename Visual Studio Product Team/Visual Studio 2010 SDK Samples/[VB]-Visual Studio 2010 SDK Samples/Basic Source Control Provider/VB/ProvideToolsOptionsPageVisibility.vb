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
Imports System.Globalization
Imports MsVsShell = Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider
	''' <summary>
	''' This attribute registers the visibility of a Tools/Options property page.
	''' While Microsoft.VisualStudio.Shell allow registering a tools options page 
	''' using the ProvideOptionPageAttribute attribute, currently there is no better way
	''' of declaring the options page visibility, so a custom attribute needs to be used.
	''' </summary>
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := True, Inherited := True)> _
	Public NotInheritable Class ProvideToolsOptionsPageVisibility
		Inherits MsVsShell.RegistrationAttribute
		Private _categoryName As String = Nothing
		Private _pageName As String = Nothing
		Private _commandUIGuid As Guid

		''' <summary>
		''' </summary>
		Public Sub New(ByVal categoryName As String, ByVal pageName As String, ByVal commandUIGuid As String)
			_categoryName = categoryName
			_pageName = pageName
			_commandUIGuid = New Guid(commandUIGuid)
		End Sub

		''' <summary>
		''' The programmatic name for this category (non localized).
		''' </summary>
		Public ReadOnly Property CategoryName() As String
			Get
				Return _categoryName
			End Get
		End Property

		''' <summary>
		''' The programmatic name for this page (non localized).
		''' </summary>
		Public ReadOnly Property PageName() As String
			Get
				Return _pageName
			End Get
		End Property

		''' <summary>
		''' Get the command UI guid controlling the visibility of the page.
		''' </summary>
		Public ReadOnly Property CommandUIGuid() As Guid
			Get
				Return _commandUIGuid
			End Get
		End Property

		Private ReadOnly Property RegistryPath() As String
			Get
				Return String.Format(CultureInfo.InvariantCulture, "ToolsOptionsPages\{0}\{1}\VisibilityCmdUIContexts", CategoryName, PageName)
			End Get
		End Property

		''' <summary>
		'''     Called to register this attribute with the given context.  The context
		'''     contains the location where the registration inforomation should be placed.
		'''     It also contains other information such as the type being registered and path information.
		''' </summary>
		Public Overrides Sub Register(ByVal context As RegistrationContext)
            ' Write to the context's log what we are about to do.
			context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "Opt.Page Visibility:" & Constants.vbTab & "{0}\{1}, {2}" & Constants.vbLf, CategoryName, PageName, CommandUIGuid.ToString("B")))

			' Create the visibility key.
			Using childKey As Key = context.CreateKey(RegistryPath)
				' Set the value for the command UI guid.
				childKey.SetValue(CommandUIGuid.ToString("B"), 1)
			End Using
		End Sub

		''' <summary>
		''' Unregister this visibility entry.
		''' </summary>
		Public Overrides Sub Unregister(ByVal context As RegistrationContext)
			context.RemoveValue(RegistryPath, CommandUIGuid.ToString("B"))
		End Sub
	End Class
End Namespace
