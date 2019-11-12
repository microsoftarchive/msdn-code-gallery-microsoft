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
Imports System.ComponentModel
Imports System.Collections
Imports System.Drawing
Imports System.Diagnostics
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports MsVsShell = Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	''' <summary>
	''' Summary description for SccProviderOptions.
	''' </summary>
	''' 
	<Guid("B0BAC05D-111E-4a5b-9834-076CB319ED59")> _
	Public Class SccProviderOptions
		Inherits MsVsShell.DialogPage
		Private page As SccProviderOptionsControl = Nothing

		''' <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage".Window]' />
		''' <devdoc>
		'''     The window this dialog page will use for its UI.
		'''     This window handle must be constant, so if you are
		'''     returning a Windows Forms control you must make sure
		'''     it does not recreate its handle.  If the window object
		'''     implements IComponent it will be sited by the 
		'''     dialog page so it can get access to global services.
		''' </devdoc>
		<Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
		Protected Overrides ReadOnly Property Window() As IWin32Window
			Get
				page = New SccProviderOptionsControl()
				page.Location = New Point(0, 0)
				page.OptionsPage = Me
				Return page
			End Get
		End Property

		''' <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnActivate"]' />
		''' <devdoc>
		'''     This method is called when VS wants to activate this
		'''     page.  If the Cancel property of the event is set to true, the page is not activated.
		''' </devdoc>
		Protected Overrides Sub OnActivate(ByVal e As CancelEventArgs)
			Trace.WriteLine(String.Format("In OnActivate"))
			 MyBase.OnActivate(e)
		End Sub

		''' <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnClosed"]' />
		''' <devdoc>
		'''     This event is raised when the page is closed.   
		''' </devdoc>
		Protected Overrides Sub OnClosed(ByVal e As EventArgs)
			Trace.WriteLine(String.Format("In OnClosed"))
			MyBase.OnClosed(e)
		End Sub

		''' <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnDeactivate"]' />
		''' <devdoc>
		'''     This method is called when VS wants to deatviate this
		'''     page.  If true is set for the Cancel property of the event, 
		'''     the page is not deactivated.
		''' </devdoc>
		Protected Overrides Sub OnDeactivate(ByVal e As CancelEventArgs)
			Trace.WriteLine(String.Format("In OnDeactivate"))
			MyBase.OnDeactivate(e)
		End Sub

		''' <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnApply"]' />
		''' <devdoc>
		'''     This method is called when VS wants to save the user's 
		'''     changes then the dialog is dismissed.
		''' </devdoc>
		Protected Overrides Sub OnApply(ByVal e As PageApplyEventArgs)
			Trace.WriteLine(String.Format("In OnApply"))
			Dim messageText As String = Resources.ResourceManager.GetString("ApplyProviderOptions")
			Dim messageCaption As String = Resources.ResourceManager.GetString("ProviderName")

			Dim uiShell As IVsUIShell = CType(GetService(GetType(SVsUIShell)), IVsUIShell)
			Dim clsid As Guid = Guid.Empty
			Dim result As Integer = VSConstants.S_OK
			If uiShell.ShowMessageBox(0, clsid, messageCaption, messageText, String.Empty, 0, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, result) <> VSConstants.S_OK OrElse result <> CInt(Fix(System.Windows.Forms.DialogResult.OK)) Then
				Trace.WriteLine(String.Format("Cancelled the OnApply event"))
				e.ApplyBehavior = ApplyKind.Cancel
			Else
				MyBase.OnApply(e)
			End If
		End Sub
	End Class
End Namespace
