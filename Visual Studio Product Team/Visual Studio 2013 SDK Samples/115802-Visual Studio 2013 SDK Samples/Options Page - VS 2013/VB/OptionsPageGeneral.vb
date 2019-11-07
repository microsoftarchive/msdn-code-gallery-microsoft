'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
    ''' <summary>
    ''' Extends the standard dialog functionality for implementing ToolsOptions pages, 
    ''' with support for the Visual Studio automation model, Windows Forms, and state 
    ''' persistence through the Visual Studio settings mechanism.
    ''' </summary>
    <Guid(GuidStrings.GuidPageGeneral)>
    Public Class OptionsPageGeneral
        Inherits DialogPage

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the String type custom option value.
        ''' </summary>
        ''' <remarks>This value is shown in the options page.</remarks>
        <Category("String Options"), Description("My string option")>
        Public Property OptionString As String = "Hello World"

        ''' <summary>
        ''' Gets or sets the integer type custom option value.
        ''' </summary>
        ''' <remarks>This value is shown in the options page.</remarks>
        <Category("Integer Options"), Description("My integer option")>
        Public Property OptionInteger As Integer = 567

        ''' <summary>
        ''' Gets or sets the Size type custom option value.
        ''' </summary>
        ''' <remarks>This value is shown in the options page.</remarks>
        <Category("Expandable Options"), Description("My Expandable option")>
        Public Property CustomSize As Size = New Size(50, 50)

#End Region ' Properties

#Region "Event Handlers"

        ''' <summary>
        ''' Handles "activate" messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This method is called when Visual Studio wants to activate this page.  
        ''' </devdoc>
        ''' <remarks>If this handler sets e.Cancel to true, the activation will not occur.</remarks>
        Protected Overrides Sub OnActivate(ByVal e As CancelEventArgs)
            Dim result As Integer = VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnActivateEntered, Nothing, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST)

            If result = VSConstants.MessageBoxResult.IDCANCEL Then
                e.Cancel = True
            End If

            MyBase.OnActivate(e)
        End Sub

        ''' <summary>
        ''' Handles "close" messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This event is raised when the page is closed.
        ''' </devdoc>
        Protected Overrides Sub OnClosed(ByVal e As EventArgs)
            VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnClosed, Nothing, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST)
        End Sub

        ''' <summary>
        ''' Handles "deactivate" messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This method is called when VS wants to deactivate this
        ''' page.  If the handler sets e.Cancel, the deactivation will not occur.
        ''' </devdoc>
        ''' <remarks>
        ''' A "deactivate" message is sent when focus changes to a different page in
        ''' the dialog.
        ''' </remarks>
        Protected Overrides Sub OnDeactivate(ByVal e As CancelEventArgs)
            Dim result As Integer = VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnDeactivateEntered, Nothing, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST)

            If result = VSConstants.MessageBoxResult.IDCANCEL Then
                e.Cancel = True
            End If
        End Sub

        ''' <summary>
        ''' Handles 'apply' messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This method is called when VS wants to save the user's 
        ''' changes (for example, when the user clicks OK in the dialog).
        ''' </devdoc>
        Protected Overrides Sub OnApply(ByVal e As PageApplyEventArgs)
            Dim result As Integer = VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnApplyEntered, Nothing, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST)

            If result = VSConstants.MessageBoxResult.IDCANCEL Then
                e.ApplyBehavior = ApplyKind.Cancel
            Else
                MyBase.OnApply(e)
            End If

            VsShellUtilities.ShowMessageBox(Site, Resources.MessageOnApply, Nothing, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST)
        End Sub

#End Region ' Event Handlers
    End Class
End Namespace
