'***************************************************************************
' Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'***************************************************************************
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.Win32
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell

Namespace Microsoft.VsTemplateDesigner
	''' <summary>
	''' This is the class that implements the package exposed by this assembly.
	'''
	''' The minimum requirement for a class to be considered a valid package for Visual Studio
	''' is to implement the IVsPackage interface and register itself with the shell.
	''' This package uses the helper classes defined inside the Managed Package Framework (MPF)
	''' to do it: it derives from the Package class that provides the implementation of the 
	''' IVsPackage interface and uses the registration attributes defined in the framework to 
	''' register itself and its components with the shell.
	''' </summary>
	' This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
	' a package.
	' Register the class as a Designer View in cooperation with the Xml Editor
	' And which type of files we want to handle
	' We register that our editor supports LOGVIEWID_Designer logical view
	' We register the XML Editor ("{FA3CD31E-987B-443A-9B81-186104E8DAC1}") as an EditorFactoryNotify
	' object to handle our ".vstemplate" file extension for the following projects:
	' Microsoft Visual Basic Project
	' Microsoft Visual C# Project
	' This attribute is used to register the informations needed to show the this package
	' in the Help/About dialog of Visual Studio.
	' This attribute is needed to let the shell know that this package exposes some menus.
	<PackageRegistration(UseManagedResourcesOnly := True), ProvideXmlEditorChooserDesignerView("VSTemplate", "vstemplate", LogicalViewID.Designer, &H60, DesignerLogicalViewEditor := GetType(EditorFactory), Namespace := "http://schemas.microsoft.com/developer/vstemplate/2005", MatchExtensionAndNamespace := False), ProvideEditorExtension(GetType(EditorFactory), EditorFactory.Extension, &H40, NameResourceID := 106), ProvideEditorLogicalView(GetType(EditorFactory), LogicalViewID.Designer), EditorFactoryNotifyForProject("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}", EditorFactory.Extension, GuidList.guidXmlChooserEditorFactory), EditorFactoryNotifyForProject("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", EditorFactory.Extension, GuidList.guidXmlChooserEditorFactory), InstalledProductRegistration("#110", "#112", "1.0", IconResourceID := 400), ProvideMenuResource("Menus.ctmenu", 1), Guid(GuidList.guidVsTemplateDesignerPkgString)>
	Public NotInheritable Class VsTemplateDesignerPackage
		Inherits Package
		''' <summary>
		''' Default constructor of the package.
		''' Inside this method you can place any initialization code that does not require 
		''' any Visual Studio service because at this point the package object is created but 
		''' not sited yet inside Visual Studio environment. The place to do all the other 
		''' initialization is the Initialize method.
		''' </summary>
		Public Sub New()

		End Sub



		'///////////////////////////////////////////////////////////////////////////
		' Overriden Package Implementation
		#Region "Package Members"

		''' <summary>
		''' Initialization of the package; this method is called right after the package is sited, so this is the place
		''' where you can put all the initilaization code that rely on services provided by VisualStudio.
		''' </summary>
		Protected Overrides Sub Initialize()
			MyBase.Initialize()

			'Create Editor Factory. Note that the base Package class will call Dispose on it.
			MyBase.RegisterEditorFactory(New EditorFactory(Me))

		End Sub
		#End Region

	End Class
End Namespace
