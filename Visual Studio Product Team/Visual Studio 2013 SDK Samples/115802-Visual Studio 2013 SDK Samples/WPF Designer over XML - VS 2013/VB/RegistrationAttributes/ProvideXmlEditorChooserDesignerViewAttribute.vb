'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Text
Imports System.IO

Namespace Microsoft.VisualStudio.Shell
	''' <summary>
	''' Used to provide registration information to the XML Chooser
	''' for a custom XML designer.
	''' </summary>
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := True, Inherited := True)>
	Public NotInheritable Class ProvideXmlEditorChooserDesignerViewAttribute
		Inherits RegistrationAttribute
		Private Const XmlChooserFactory As String = "XmlChooserFactory"
		Private Const XmlChooserEditorExtensionsKeyPath As String = "Editors\{32CC8DFA-2D70-49b2-94CD-22D57349B778}\Extensions"
		Private Const XmlEditorFactoryGuid As String = "{FA3CD31E-987B-443A-9B81-186104E8DAC1}"


		Private name As String
		Private extension As String
		Private defaultLogicalView As Guid
		Private xmlChooserPriority As Integer

		''' <summary>
		''' Constructor for ProvideXmlEditorChooserDesignerViewAttribute.
		''' </summary>
		''' <param name="name">The registry keyName for your XML editor. For example "RESX", "Silverlight", "Workflow", etc...</param>
		''' <param name="extension">The file extension for your custom XML type (e.g. "xaml", "resx", "xsd").</param>
		''' <param name="defaultLogicalViewEditorFactory">A Type, Guid, or String object representing the editor factory for the default logical view.</param>
		''' <param name="xmlChooserPriority">The priority of the extension in the XML Chooser. This value must be greater than the extension's priority value for the XML designer's EditorFactory.</param>
        Public Sub New(ByVal name As String, ByVal extension As String, ByVal defaultLogicalViewEditorFactory As Object,
                       ByVal xmlChooserPriority As Integer)
            If String.IsNullOrWhiteSpace(name) Then
                Throw New ArgumentException("Editor description cannot be null or empty.", "editorDescription")
            End If
            If String.IsNullOrWhiteSpace(extension) Then
                Throw New ArgumentException("Extension cannot be null or empty.", "extension")
            End If
            If defaultLogicalViewEditorFactory Is Nothing Then
                Throw New ArgumentNullException("defaultLogicalViewEditorFactory")
            End If

            Me.name = name
            Me.extension = extension
            Me.defaultLogicalView = TryGetGuidFromObject(defaultLogicalViewEditorFactory)
            Me.xmlChooserPriority = xmlChooserPriority

            Me.CodeLogicalViewEditor = XmlEditorFactoryGuid
            Me.DebuggingLogicalViewEditor = XmlEditorFactoryGuid
            Me.DesignerLogicalViewEditor = XmlEditorFactoryGuid
            Me.TextLogicalViewEditor = XmlEditorFactoryGuid
        End Sub

		Public Overrides Sub Register(ByVal context As RegistrationContext)
			If context Is Nothing Then
				Throw New ArgumentNullException("context")
			End If

			Using xmlChooserExtensions As Key = context.CreateKey(XmlChooserEditorExtensionsKeyPath)
				xmlChooserExtensions.SetValue(extension, xmlChooserPriority)
			End Using

			Using key_Renamed As Key = context.CreateKey(GetKeyName())
				key_Renamed.SetValue("DefaultLogicalView", defaultLogicalView.ToString("B").ToUpperInvariant())
				key_Renamed.SetValue("Extension", extension)

				If Not String.IsNullOrWhiteSpace([Namespace]) Then
					key_Renamed.SetValue("Namespace", [Namespace])
				End If

				If MatchExtensionAndNamespace Then
					key_Renamed.SetValue("Match", "both")
				End If

				If IsDataSet.HasValue Then
					key_Renamed.SetValue("IsDataSet", Convert.ToInt32(IsDataSet.Value))
				End If

				SetLogicalViewMapping(key_Renamed, VSConstants.LOGVIEWID_Debugging, DebuggingLogicalViewEditor)
				SetLogicalViewMapping(key_Renamed, VSConstants.LOGVIEWID_Code, CodeLogicalViewEditor)
				SetLogicalViewMapping(key_Renamed, VSConstants.LOGVIEWID_Designer, DesignerLogicalViewEditor)
				SetLogicalViewMapping(key_Renamed, VSConstants.LOGVIEWID_TextView, TextLogicalViewEditor)
			End Using
		End Sub

        Private Sub SetLogicalViewMapping(ByVal key_Renamed As Key, ByVal logicalView As Guid,
                                          ByVal editorFactory As Object)
            If editorFactory IsNot Nothing Then
                key_Renamed.SetValue(logicalView.ToString("B").ToUpperInvariant(),
                                     TryGetGuidFromObject(editorFactory).ToString("B").ToUpperInvariant())
            End If
        End Sub

		Private Function TryGetGuidFromObject(ByVal guidObject As Object) As Guid
			' figure out what type of object they passed in and get the GUID from it
			If TypeOf guidObject Is String Then
				Return New Guid(CStr(guidObject))
			ElseIf TypeOf guidObject Is Type Then
				Return (CType(guidObject, Type)).GUID
			ElseIf TypeOf guidObject Is Guid Then
				Return CType(guidObject, Guid)
			Else
				Throw New ArgumentException("Could not determine Guid from supplied object.", "guidObject")
			End If
		End Function

		Public Overrides Sub Unregister(ByVal context As RegistrationContext)
			If context Is Nothing Then
				Throw New ArgumentNullException("context")
            End If
            With context

                .RemoveKey(GetKeyName())

                .RemoveValue(XmlChooserEditorExtensionsKeyPath, extension)
                .RemoveKeyIfEmpty(XmlChooserEditorExtensionsKeyPath)
            End With
        End Sub

		Private Function GetKeyName() As String
			Return Path.Combine(XmlChooserFactory, name)
		End Function

		''' <summary>
		''' The XML Namespace used in documents that this editor supports.
		''' </summary>
		Public Property [Namespace]() As String

		''' <summary>
		''' Boolean value indicating whether the XML chooser should match on both the file extension
		''' and the Namespace. If false, the XML chooser will match on either the extension or the 
		''' Namespace.
		''' </summary>
		Public Property MatchExtensionAndNamespace() As Boolean

		''' <summary>
		''' Special value used only by the DataSet designer.
		''' </summary>
		Public Property IsDataSet() As Boolean?

		''' <summary>
		''' The editor factory to associate with the debugging logical view
		''' </summary>
		Public Property DebuggingLogicalViewEditor() As Object

		''' <summary>
		''' The editor factory to associate with the code logical view
		''' </summary>
		Public Property CodeLogicalViewEditor() As Object

		''' <summary>
		''' The editor factory to associate with the designer logical view
		''' </summary>
		Public Property DesignerLogicalViewEditor() As Object

		''' <summary>
		''' The editor factory to associate with the text logical view
		''' </summary>
		Public Property TextLogicalViewEditor() As Object
	End Class
End Namespace
