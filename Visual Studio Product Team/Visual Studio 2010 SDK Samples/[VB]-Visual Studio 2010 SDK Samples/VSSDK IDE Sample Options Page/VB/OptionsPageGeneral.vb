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
Imports System.Drawing
Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports MSVSIP = Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
	''' <summary>
	' Extends a standard dialog functionality for implementing ToolsOptions pages, 
	' with support for the Visual Studio automation model, Windows Forms, and state 
	' persistence through the Visual Studio settings mechanism.
    ''' </summary>
    <ComVisible(True)> _
    <Guid(GuidStrings.GuidPageGeneral)> _
    Public Class OptionsPageGeneral
        Inherits MSVSIP.DialogPage
#Region "Fields"
        Private optionCustomInteger As Integer = 567
        Private optionCustomSize As New Size(50, 50)
        Private optionCustomString As String = "Hello World"
#End Region ' Fields

#Region "Properties"
        ''' <summary>
        ''' Gets or sets the String type custom option value.
        ''' </summary>
        ''' <remarks>The property that you want to be show in the options page.</remarks>
        <Category("String Options"), Description("My string option")> _
        Public Property OptionString() As String
            Get
                Return optionCustomString
            End Get
            Set(ByVal value As String)
                optionCustomString = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the integer type custom option value.
        ''' </summary>
        ''' <remarks>The property that you want to be show in the options page.</remarks>
        <Category("Integer Options"), Description("My integer option")> _
        Public Property OptionInteger() As Integer
            Get
                Return optionCustomInteger
            End Get
            Set(ByVal value As Integer)
                optionCustomInteger = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the Size type custom option value.
        ''' </summary>
        ''' <remarks>The property that you want to be show in the options page.</remarks>
        <Category("Expandable Options"), Description("My Expandable option")> _
        Public Property CustomSize() As Size
            Get
                Return optionCustomSize
            End Get
            Set(ByVal value As Size)
                optionCustomSize = value
            End Set
        End Property
#End Region ' Properties

#Region "Event Handlers"
        ''' <summary>
        ''' Handles "Activate" messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This method is called when Visual Studio wants to activate this page.  
        ''' </devdoc>
        ''' <remarks>If the Cancel property of the event is set to true, the page is not activated.</remarks>
        Protected Overrides Sub OnActivate(ByVal e As CancelEventArgs)
            Dim result As DialogResult = WinFormsHelper.ShowMessageBox(Resources.MessageOnActivateEntered, Resources.MessageOnActivateEntered, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If result = DialogResult.Cancel Then
                Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Cancelled the OnActivate event"))
                e.Cancel = True
            End If

            MyBase.OnActivate(e)
        End Sub

        ''' <summary>
        ''' Handles "Close" messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This event is raised when the page is closed.
        ''' </devdoc>
        Protected Overrides Sub OnClosed(ByVal e As EventArgs)
            WinFormsHelper.ShowMessageBox(Resources.MessageOnClosed)
        End Sub

        ''' <summary>
        ''' Handles "Deactive" messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This method is called when VS wants to deactivate this
        ''' page.  If true is set for the Cancel property of the event, 
        ''' the page is not deactivated.
        ''' </devdoc>
        ''' <remarks>
        ''' A "Deactive" message is sent when a dialog page's user interface 
        ''' window loses focus or is minimized but is not closed.
        ''' </remarks>
        Protected Overrides Sub OnDeactivate(ByVal e As CancelEventArgs)
            Dim result As DialogResult = WinFormsHelper.ShowMessageBox(Resources.MessageOnDeactivateEntered, Resources.MessageOnDeactivateEntered, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If result = DialogResult.Cancel Then
                Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Cancelled the OnDeactivate event"))
                e.Cancel = True
            End If
        End Sub

        ''' <summary>
        ''' Handles Apply messages from the Visual Studio environment.
        ''' </summary>
        ''' <devdoc>
        ''' This method is called when VS wants to save the user's 
        ''' changes then the dialog is dismissed.
        ''' </devdoc>
        Protected Overrides Sub OnApply(ByVal e As PageApplyEventArgs)
            Dim result As DialogResult = WinFormsHelper.ShowMessageBox(Resources.MessageOnApplyEntered)

            If result = DialogResult.Cancel Then
                Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Cancelled the OnApply event"))
                e.ApplyBehavior = ApplyKind.Cancel
            Else
                MyBase.OnApply(e)
            End If

            WinFormsHelper.ShowMessageBox(Resources.MessageOnApply)
        End Sub

#End Region ' Event Handlers
    End Class
End Namespace
