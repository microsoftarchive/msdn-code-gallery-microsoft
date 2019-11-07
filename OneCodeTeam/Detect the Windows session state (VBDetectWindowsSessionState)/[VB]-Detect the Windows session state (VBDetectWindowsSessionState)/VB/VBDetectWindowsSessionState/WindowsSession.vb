'*************************** Module Header ******************************'
' Module Name:  WindowsSession.vb
' Project:	    VBDetectWindowsSessionState
' Copyright (c) Microsoft Corporation.
' 
' The enum WindowsSession is used to subscribe SystemEvents.SessionSwitch event.
' 
' It imports the method OpenInputDesktop to check whether current session is locked.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Imports System.Security.Permissions

Public Class WindowsSession
    Implements IDisposable

    ' Required to read objects on the desktop.
    Const _desktop_ReadObjects As Integer = &H1

    ' Required to write objects on the desktop.
    Const _desktop_WriteObjects As Integer = &H80

    ''' <summary>
    ''' Open the desktop that receives user input.
    ''' This method is used to check whether the desktop is locked. If the function 
    ''' fails, the return value is IntPtr.Zero. 
    ''' Note:
    '''      If UAC popups a secure desktop, this method may also fail. There is 
    '''      no API for differentiate between Locked Desktop and UAC popup.
    ''' </summary>
    ''' <param name="dwFlags">
    ''' This parameter can be zero or the following value.
    ''' 0x0001: Allows processes running in other accounts on the desktop to set
    ''' hooks in this process.
    ''' </param>
    ''' <param name="fInherit">
    ''' If this value is TRUE, processes created by this process will inherit the 
    ''' handle. Otherwise, the processes do not inherit this handle.
    ''' </param>
    ''' <param name="dwDesiredAccess">
    ''' The access to the desktop. See 
    ''' http://msdn.microsoft.com/en-us/library/ms682575(VS.85).aspx
    ''' This sample will use DESKTOP_READOBJECTS (0x0001L) and 
    ''' DESKTOP_WRITEOBJECTS (0x0080L). 
    ''' </param>
    ''' <returns>If the function fails, the return value is IntPtr.Zero. </returns>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function OpenInputDesktop(ByVal dwFlags As Integer,
                                             ByVal fInherit As Boolean,
                                             ByVal dwDesiredAccess As Integer) As IntPtr
    End Function

    ''' <summary>
    ''' Close an open handle to a desktop object.
    ''' </summary>
    ''' <param name="hDesktop">
    ''' A handle to the desktop to be closed.
    ''' </param>
    ''' <returns>
    ''' If the function succeeds, then return true.
    ''' </returns>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function CloseDesktop(ByVal hDesktop As IntPtr) As Boolean
    End Function

    ' Specify whether this instance is disposed.
    Private _disposed As Boolean

    Public Event StateChanged As EventHandler(Of SessionSwitchEventArgs)

    ''' <summary>
    ''' Initialize the instance.
    ''' Register the SystemEvents.SessionSwitch event.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
        AddHandler SystemEvents.SessionSwitch, AddressOf SystemEvents_SessionSwitch
    End Sub



    ''' <summary>
    ''' Handle the SystemEvents.SessionSwitchc event.
    ''' </summary>
    Private Sub SystemEvents_SessionSwitch(ByVal sender As Object,
                                           ByVal e As SessionSwitchEventArgs)
        Me.OnStateChanged(e)
    End Sub

    ''' <summary>
    ''' Raise the StateChanged event.
    ''' </summary>
    Protected Overridable Sub OnStateChanged(ByVal e As SessionSwitchEventArgs)
        RaiseEvent StateChanged(Me, e)
    End Sub

    ''' <summary>
    ''' Check whether current session is locked.
    ''' Note: If UAC popups a secure desktop, this method may also fail. 
    '''       There is no API for differentiating between Locked Desktop
    '''       and UAC Secure Desktop. 
    ''' </summary>
    ''' <returns></returns>
    Public Function IsLocked() As Boolean
        Dim hDesktop As IntPtr = IntPtr.Zero

        Try

            ' Opens the desktop that receives user input.
            hDesktop = OpenInputDesktop(
                0, False, _desktop_ReadObjects Or _desktop_WriteObjects)

            ' If hDesktop is IntPtr.Zero, then the session is locked.
            Return hDesktop = IntPtr.Zero
        Finally

            ' Close an open handle to a desktop object.
            If hDesktop <> IntPtr.Zero Then
                CloseDesktop(hDesktop)
            End If
        End Try
    End Function

    ''' <summary>
    ''' Dispose the resources.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' Protect from being called multiple times.
        If _disposed Then
            Return
        End If

        If disposing Then
            ' Clean up all managed resources.
            RemoveHandler SystemEvents.SessionSwitch,
                AddressOf SystemEvents_SessionSwitch

        End If

        _disposed = True
    End Sub
End Class


