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
Imports System.Windows.Forms
Imports System.ComponentModel
Imports Microsoft.VisualStudio.Shell
Imports System.Runtime.InteropServices

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
    ''' <summary>
    ''' Extends the standard dialog functionality for implementing ToolsOptions pages, 
    ''' with support for the Visual Studio automation model, Windows Forms, and state 
    ''' persistence through the Visual Studio settings mechanism.
    ''' </summary>
    <Guid(GuidStrings.GuidPageCustom)>
    Public Class OptionsPageCustom
        Inherits DialogPage

#Region "Fields"

        Private optionsControl As OptionsCompositeControl

#End Region ' Fields

#Region "Properties"
        ''' <summary>
        ''' Gets the window an instance of DialogPage that it uses as its user interface.
        ''' </summary>
        ''' <devdoc>
        ''' The window this dialog page will use for its UI.
        ''' This window handle must be constant, so if you are
        ''' returning a Windows Forms control you must make sure
        ''' it does not recreate its handle.  If the window object
        ''' implements IComponent it will be sited by the 
        ''' dialog page so it can get access to global services.
        ''' </devdoc>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Protected Overrides ReadOnly Property Window() As IWin32Window
            Get
                If optionsControl Is Nothing Then
                    optionsControl = New OptionsCompositeControl()
                    optionsControl.Location = New Point(0, 0)
                    optionsControl.OptionsPage = Me
                End If
                Return optionsControl
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the path to the image file.
        ''' </summary>
        ''' <remarks>The property that needs to be persisted.</remarks>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        Public Property CustomBitmap As String

#End Region ' Properties

#Region "Methods"

        Protected Overrides Sub Dispose(disposing As Boolean)
            If (disposing) Then
                If optionsControl IsNot Nothing Then
                    optionsControl.Dispose()
                    optionsControl = Nothing
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Methods

    End Class
End Namespace
