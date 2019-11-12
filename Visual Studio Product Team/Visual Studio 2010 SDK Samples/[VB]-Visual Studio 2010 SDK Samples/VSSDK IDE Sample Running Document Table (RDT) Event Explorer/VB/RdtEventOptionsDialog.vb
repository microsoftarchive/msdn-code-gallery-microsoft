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
	''' The Tool/Options dialog page that filters the RDT event display.
	''' This class owns the singleton instance of the options (rdteOptions)
	''' available via automation.
	''' </summary>
	<ClassInterface(ClassInterfaceType.AutoDual), Guid("7F389730-D552-414a-9C43-161B07CBFED4")> _
	Public Class RdtEventOptionsDialog
		Inherits DialogPage
		' Dialog pages are cached singleton instances.
        Private fldRdteOptions As Options = New Options()

		<Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
		Public ReadOnly Property RdteOptions() As IOptions
			Get
                Return CType(fldRdteOptions, IOptions)
			End Get
		End Property
		''' <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.AutomationObject"]' />
		''' <devdoc>
		'''     The object the dialog page is going to browse.  The
		'''     default returns "this", but you can change it to
		'''     browse any object you want.
		''' </devdoc>
		<Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
		Public Overrides ReadOnly Property AutomationObject() As Object
			Get
				 Return RdteOptions
			End Get
		End Property
	End Class
End Namespace
