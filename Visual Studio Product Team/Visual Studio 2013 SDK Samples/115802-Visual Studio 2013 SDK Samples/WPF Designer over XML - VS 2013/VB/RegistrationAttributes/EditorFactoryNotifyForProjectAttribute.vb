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

Imports System.Globalization

Namespace Microsoft.VisualStudio.Shell
	''' <summary>
	''' This attribute adds a File Extension for a Project System so that the Project
	''' will call IVsEditorFactoryNotify methods when an item of this type is added 
	''' or renamed.
	''' </summary>
	''' <remarks>
	''' For example:
	'''   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\Projects\
	'''		{F184B08F-C81C-45F6-A57F-5ABD9991F28F}\FileExtensions\.addin]
	'''			"EditorFactoryNotify"="{FA3CD31E-987B-443A-9B81-186104E8DAC1}"
	''' </remarks>
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := True, Inherited := True), System.Runtime.InteropServices.ComVisibleAttribute(False)>
	Public NotInheritable Class EditorFactoryNotifyForProjectAttribute
		Inherits RegistrationAttribute
		#Region "Fields"

		Private projectType_Renamed As Guid

		Private factoryType_Renamed As Guid

		Private fileExtension_Renamed As String
		#End Region

		#Region "Constructors"
		''' <summary>
		''' Creates a new ProvideEditorFactoryNotifyForProject attribute to register a 
		''' file extension with a project. 
		''' </summary>
		''' <param name="projectType">The type of project; can be a Type, a GUID or a string representation of a GUID</param>
		''' <param name="factoryType">The type of factory; can be a Type, a GUID or a string representation of a GUID</param>
		''' <param name="fileExtension">The file extension the EditorFactoryNotify wants to handle</param>
		Public Sub New(ByVal projectType As Object, ByVal fileExtension As String, ByVal factoryType As Object)
			If factoryType Is Nothing Then
				Throw New ArgumentNullException("factoryType", "Factory type can not be null.")
			End If
			If projectType Is Nothing Then
				Throw New ArgumentNullException("projectType", "Project type can not be null.")
			End If

			Me.fileExtension_Renamed = fileExtension

			' figure out what type of object they passed in and get the GUID from it
			If TypeOf factoryType Is String Then
				Me.factoryType_Renamed = New Guid(factoryType.ToString())
			ElseIf TypeOf factoryType Is Type Then
				Me.factoryType_Renamed = (CType(factoryType, Type)).GUID
			ElseIf TypeOf factoryType Is Guid Then
				Me.factoryType_Renamed = CType(factoryType, Guid)
			Else
				Throw New ArgumentException("Parameter is expected to be an instance of the type 'Type' or 'Guid'.", "factoryType")
			End If

			' figure out what type of object they passed in and get the GUID from it
			If TypeOf projectType Is String Then
				Me.projectType_Renamed = New Guid(projectType.ToString())
			ElseIf TypeOf projectType Is Type Then
				Me.projectType_Renamed = (CType(projectType, Type)).GUID
			ElseIf TypeOf projectType Is Guid Then
				Me.projectType_Renamed = CType(projectType, Guid)
			Else
				Throw New ArgumentException("Parameter is expected to be an instance of the type 'Type' or 'Guid'.", "projectType")
			End If
		End Sub
		#End Region

		#Region "Properties"
		''' <summary>
		''' Get the Guid representing the type of the editor factory
		''' </summary>
		'public Guid FactoryType
		Public ReadOnly Property FactoryType() As Object
			Get
				Return factoryType_Renamed
			End Get
		End Property

		''' <summary>
		''' Get the Guid representing the project type
		''' </summary>
		Public ReadOnly Property ProjectType() As Object
			Get
				Return projectType_Renamed
			End Get
		End Property

		''' <summary>
		''' Get or Set the extension of the XML files that support this view
		''' </summary>
		Public ReadOnly Property FileExtension() As String
			Get
				Return fileExtension_Renamed
			End Get
		End Property

		''' <summary>
		''' Extention path within the registration context
		''' </summary>
		Private ReadOnly Property ProjectFileExtensionPath() As String
			Get
                Return String.Format(CultureInfo.InvariantCulture, "Projects\{0}\FileExtensions\{1}",
                                     projectType_Renamed.ToString("B"), fileExtension_Renamed)
			End Get
		End Property
		#End Region

		#Region "Methods"
		''' <summary>
		''' Called to register this attribute with the given context.  The context
		''' contains the location where the registration information should be placed.
		''' It also contains other information such as the type being registered and path information.
		''' </summary>
		''' <param name="context">Given context to register in</param>
		Public Overrides Sub Register(ByVal context As RegistrationContext)
			If context Is Nothing Then
				Throw New ArgumentNullException("context")
			End If

            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture,
                                                "EditorFactoryNoftifyForProject: {0} Extension = {1}" & vbLf,
                                                projectType_Renamed.ToString(), fileExtension_Renamed))

			Using childKey As Key = context.CreateKey(ProjectFileExtensionPath)
				childKey.SetValue("EditorFactoryNotify", factoryType_Renamed.ToString("B"))
			End Using
		End Sub

		''' <summary>
		''' Unregister this file extension.
		''' </summary>
		''' <param name="context">Given context to unregister from</param>
		Public Overrides Sub Unregister(ByVal context As RegistrationContext)
			If context IsNot Nothing Then
				context.RemoveKey(ProjectFileExtensionPath)
			End If
		End Sub
		#End Region
	End Class
End Namespace
