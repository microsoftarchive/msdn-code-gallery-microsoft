'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports Microsoft.Win32
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.PlatformUI

''' <summary>
''' Summary description for SccProviderToolWindow.
''' </summary>
<Guid("ADC98052-bbbb-42f2-8085-723ca3712763")> _
Public Class SccProviderToolWindow
    Inherits ToolWindowPane
    Private control As SccProviderToolWindowControl

    Public Sub New()
        MyBase.New(Nothing)
        ' Set the window title.
        Me.Caption = Resources.ResourceManager.GetString("ToolWindowCaption")

        ' Set the CommandID for the window ToolBar.
        Me.ToolBar = New CommandID(GuidList.guidSccProviderCmdSet, CommandIds.imnuToolWindowToolbarMenu)

        ' Set the icon for the frame.
        ' Bitmap strip resource ID.
        Me.BitmapResourceID = CommandIds.ibmpToolWindowsImages
        ' Index in the bitmap strip.
        Me.BitmapIndex = CommandIds.iconSccProviderToolWindow

        control = New SccProviderToolWindowControl()

        ' Initialize the toolwindow colors to respect the current theme
        SetDefaultColors()

        ' Sign up to theme changes to keep the colors up to date
        AddHandler VSColorTheme.ThemeChanged, AddressOf Me.VSColorTheme_ThemeChanged

    End Sub

    Private Sub SetDefaultColors()
        Dim defaultBackground As Color = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey)
        Dim defaultForeground As Color = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey)

        UpdateWindowColors(defaultBackground, defaultForeground)
    End Sub

    Public Sub VSColorTheme_ThemeChanged(e As ThemeChangedEventArgs)
        SetDefaultColors()
    End Sub

    Public Overrides ReadOnly Property Window() As IWin32Window
        Get
            Return CType(control, IWin32Window)
        End Get
    End Property

    ''' <include file='doc\WindowPane.uex' path='docs/doc[@for="WindowPane.Dispose1"]' />
    ''' <devdoc>
    '''     Called when this tool window pane is being disposed.
    ''' </devdoc>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then

            ' Unsubscribe from theme changed events
            RemoveHandler VSColorTheme.ThemeChanged, AddressOf Me.VSColorTheme_ThemeChanged

            If control IsNot Nothing Then
                Try
                    If TypeOf control Is IDisposable Then
                        control.Dispose()
                    End If
                Catch e As Exception
                    System.Diagnostics.Debug.Fail(String.Format("Failed to dispose {0} controls." & VisualBasic.Constants.vbLf & "{1}", Me.GetType().FullName, e.Message))
                End Try
                control = Nothing
            End If

            Dim windowFrame As IVsWindowFrame = CType(Me.Frame, IVsWindowFrame)
            If windowFrame IsNot Nothing Then
                ' Note: don't check for the return code here.
                windowFrame.CloseFrame(CUInt(__FRAMECLOSE.FRAMECLOSE_SaveIfDirty))
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary>
    ''' This function is only used to "do something noticeable" when the toolbar button is clicked.
    ''' It is called from the package.
    ''' A typical tool window may not need this function.
    ''' 
    ''' The current behavior change the background color of the control and swaps with the text color
    ''' </summary>
    Public Sub ToolWindowToolbarCommand()

        Dim defaultBackground As Color = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey)
        Dim defaultForeground As Color = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey)

        If (Me.control.BackColor = defaultBackground) Then
            ' Swap the colors
            UpdateWindowColors(defaultForeground, defaultBackground)
        Else
            ' Put back the default colors
            UpdateWindowColors(defaultBackground, defaultForeground)
        End If
    End Sub

    Private Sub UpdateWindowColors(clrBackground As Color, clrForeground As Color)
        ' Update the window background
        Me.control.BackColor = clrBackground
        Me.control.ForeColor = clrForeground

        ' Also update the label
        For Each child As Control In Me.control.Controls
            child.BackColor = Me.control.BackColor
            child.ForeColor = Me.control.ForeColor
        Next
    End Sub

End Class
