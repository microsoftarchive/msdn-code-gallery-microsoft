'***************************************************************************
' Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'***************************************************************************
Imports System.Text
Imports System.IO
Imports System.ComponentModel
Imports System.Globalization

Namespace Microsoft.VsTemplateDesigner
	Public Interface IViewModel
		ReadOnly Property TemplateData() As VSTemplateTemplateData
		ReadOnly Property TemplateContent() As VSTemplateTemplateContent

		Property Name() As String
		Property Description() As String
		Property Icon() As String
		Property ProjectType() As String
		Property ProjectSubType() As String
		Property DefaultName() As String
		Property TemplateID() As String
		Property GroupID() As String
		Property SortOrder() As String
		Property LocationField() As VSTemplateTemplateDataLocationField
		Property LocationFieldMRUPrefix() As String
		Property PreviewImage() As String
		Property WizardAssembly() As String
		Property WizardClassName() As String
		Property WizardData() As String

		Property ProvideDefaultName() As Boolean
		Property CreateNewFolder() As Boolean
		Property PromptForSaveOnCreation() As Boolean
		Property Hidden() As Boolean
		Property SupportsMasterPage() As Boolean
		Property SupportsCodeSeparation() As Boolean
		Property SupportsLanguageDropDown() As Boolean

		Property DesignerDirty() As Boolean
		ReadOnly Property IsNameEnabled() As Boolean
		ReadOnly Property IsDescriptionEnabled() As Boolean
		ReadOnly Property IsIconEnabled() As Boolean
		ReadOnly Property IsLocationFieldSpecified() As Boolean

		Event ViewModelChanged As EventHandler
		Sub DoIdle()
		Sub Close()

		Sub OnSelectChanged(ByVal p As Object)
	End Interface
End Namespace