' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.IO

Public Class MainForm


    Const StringBufferLength As Integer = 255

#Region "API Calls"

    ''' <summary>
    ''' This demonstrates the different calling variations using the function Beep.  Check
    ''' The declarations in class CallingVariations.
    ''' </summary>
    Private Sub btnBeep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBeep.Click
        If rbDeclare.Checked Then
            CallingVariations.DeclareBeep(1000, 1000)
        ElseIf rbDLLImport.Checked Then
            CallingVariations.DLLImportBeep(1000, 1000)
        ElseIf rbANSI.Checked Then
            CallingVariations.ANSIBeep(1000, 1000)
        ElseIf rbUnicode.Checked Then
            CallingVariations.UnicodeBeep(1000, 1000)
        ElseIf rbAuto.Checked Then
            CallingVariations.AutoBeep(1000, 1000)
        End If
    End Sub

    ''' <summary>
    ''' This creates a directory if possible, and updates the status txtFunctionOutput.
    ''' </summary>
    Private Sub btnCreateDirectory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateDirectory.Click
        Dim security As New Win32API.SECURITY_ATTRIBUTES()

        ' The function needs to know the structure size when it is marshaled to unmanaged code.
        security.nLength = Marshal.SizeOf(security)
        security.lpSecurityDescriptor = 0
        security.bInheritHandle = 0

        If Win32API.CreateDirectory(txtDirectory.Text, security) Then
            txtFunctionOutput.Text = "Directory created."
        Else
            txtFunctionOutput.Text = "Directory not created."
        End If
    End Sub

    ''' <summary>
    ''' This gets the number of free clusters on a disk, using the Win32 API call GetDiskFreeSpace,
    ''' and updates txtFunctionOutput.
    ''' </summary>
    Private Sub btnGetFreeSpace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetFreeSpace.Click
        Dim rootPathName As String
        Dim sectorsPerCluster As Integer
        Dim bytesPerSector As Integer
        Dim numberOfFreeClusters As Integer
        Dim totalNumberOfClusters As Integer


        rootPathName = txtDriveLetter.Text & ":" & Path.DirectorySeparatorChar

        Win32API.GetDiskFreeSpace(rootPathName, sectorsPerCluster, bytesPerSector, _
            numberOfFreeClusters, totalNumberOfClusters)

        txtFunctionOutput.Text = "Number of Free Clusters: " & _
            numberOfFreeClusters.ToString()
    End Sub

    ''' <summary>
    ''' This gets the number of free bytes on a disk, using the Win32 API call GetDiskFreeSpaceEx,
    ''' and updates txtFunctionOutput.
    ''' </summary>
    Private Sub btnGetDiskFreeSpaceEx_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetDiskFreeSpaceEx.Click
        Dim rootPathName As String
        Dim freeBytesToCaller As Integer
        Dim totalNumberOfBytes As Integer
        Dim totalNumberOfFreeBytes As UInt32

        rootPathName = txtDriveLetter.Text & ":" & Path.DirectorySeparatorChar

        Win32API.GetDiskFreeSpaceEx(rootPathName, freeBytesToCaller, totalNumberOfBytes, _
            totalNumberOfFreeBytes)

        txtFunctionOutput.Text = "Number of Free Bytes: " & _
            totalNumberOfFreeBytes.ToString()
    End Sub

    ''' <summary>
    ''' This shows the drive type and updates the txtFunctionOutput 
    ''' textbox using the Win32 API call GetDriveType.
    ''' </summary>
    Private Sub btnGetDriveType_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetDriveType.Click
        Dim rootPathName As String
        rootPathName = txtDriveLetter.Text & ":" & Path.DirectorySeparatorChar

        Select Case Win32API.GetDriveType(rootPathName)
            Case 2
                txtFunctionOutput.Text = "Drive type: Removable"
            Case 3
                txtFunctionOutput.Text = "Drive type: Fixed"
            Case Is = 4
                txtFunctionOutput.Text = "Drive type: Remote"
            Case Is = 5
                txtFunctionOutput.Text = "Drive type: Cd-Rom"
            Case Is = 6
                txtFunctionOutput.Text = "Drive type: Ram disk"
            Case Else
                txtFunctionOutput.Text = "Drive type: Unrecognized"
        End Select
    End Sub

    ''' <summary>
    ''' This gets the Operating system version and updates txtFunctionOutput. 
    ''' </summary>
    Private Sub btnGetOSVersion_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetOSVersion.Click
        Dim versionInfo As New Win32API.OSVersionInfo()

        ' The function needs to know the structure size when it is marshaled to unmanaged code.
        versionInfo.OSVersionInfoSize = Marshal.SizeOf(versionInfo)
        Win32API.GetVersionEx(versionInfo)

        txtFunctionOutput.Text = "Build Number is: " & versionInfo.buildNumber.ToString() & Chr(13) & Chr(10)
        txtFunctionOutput.Text += "Major Version Number is: " & versionInfo.majorVersion.ToString()
    End Sub

    ''' <summary>
    ''' This suspends the computer if it is possible.
    ''' </summary>
    Private Sub btnHibernate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHibernate.Click
        If Win32API.IsPwrHibernateAllowed() <> 0 Then
            If MsgBox("Do you want to hibernate this computer?", MsgBoxStyle.YesNo, "Confirm Hibernation") = MsgBoxResult.Yes Then
                Win32API.SetSuspendState(1, 0, 0)
            End If
        Else
            txtFunctionOutput.Text = "Your computer does not support hibernation.  " & _
                "This may be due to system settings or simply a computer bios that does not support hibernation."
        End If
    End Sub

    ''' <summary>
    ''' Sets the mouse buttons to the default settings.
    ''' </summary>
    Private Sub btnResetMouseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetMouseButton.Click
        Win32API.SwapMouseButton(0)
    End Sub

    ''' <summary>
    ''' Swaps the mouse buttons.
    ''' </summary>
    Private Sub btnSwapMouseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSwapMouseButton.Click
        Win32API.SwapMouseButton(1)
    End Sub
#End Region

    ''' <summary>
    ''' This subroutine clears the process list view and fills it with all active processes.
    ''' </summary>
    Private Sub btnRefreshActiveProcesses_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefreshActiveProcesses.Click
        lvwProcessList.Items.Clear()

        ' Call WinAPI function EnumWindows, specifying FillActiveProcessList function as
        ' the function to be called once per active process.  Since EnumWindows is 
        ' unmanaged code, it is necessary to create a delegate to allow it to call 
        ' FillActiveProcessList which is managed code.
        Win32API.EnumWindows(New Win32API.EnumWindowsCallback(AddressOf _
            FillActiveProcessList), 0)
    End Sub

    ''' <summary>
    ''' This subroutine clears the active windows list and fills it with all active windows.
    ''' </summary>
    Private Sub btnRefreshActiveWindows_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefreshActiveWindows.Click
        lbActiveWindows.Items.Clear()

        ' EnumWindowDllImport works the same as in btnRefreshActiveProcesses_Click, 
        ' however, it is defined using DllImport instead of Declare.
        Win32API.EnumWindowsDllImport(New Win32API.EnumWindowsCallback(AddressOf _
            FillActiveWindowsList), 0)
    End Sub

    ''' <summary>
    ''' This Sub finds an active window based on the values in the window caption and class 
    ''' name text boxes, and brings it to the foreground.
    ''' </summary>
    Private Sub btnShow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShow.Click
        Dim hWnd As Integer

        ' There are four overloads for the Win32API function FindWindow in the Win32API 
        ' class, allowing either a String or an Integer to be passed to the class name 
        ' and window name.  If either of the fields are blank, passing a 0 to the 
        ' parameter marshalls NULL to the function call.
        If txtWindowCaption.Text = "" And txtClassName.Text = "" Then
            ' FindWindowAny takes to Integer parameters and finds any available window.
            hWnd = Win32API.FindWindowAny(0, 0)
        ElseIf txtWindowCaption.Text = "" And txtClassName.Text <> "" Then
            ' FindWindowNullWindowCaption attempts to locate a window by class name alone.
            hWnd = Win32API.FindWindowNullWindowCaption(txtClassName.Text, 0)
        ElseIf txtWindowCaption.Text <> "" And txtClassName.Text = "" Then
            ' FindWindowNullClassName attempts to locate a window by window name alone. 
            hWnd = Win32API.FindWindowNullClassName(0, txtWindowCaption.Text)
        Else
            ' FindWindow searches for a window by class name and window name.
            hWnd = Win32API.FindWindow(txtClassName.Text, txtWindowCaption.Text)
        End If

        ' If the window isn't found FindWindow sets the windows handle to 0.  If the 
        ' handle is 0 display an error message, otherwise bring window to foreground.
        If hWnd = 0 Then
            MsgBox("Specified window is not running.", MsgBoxStyle.Exclamation, Me.Text)
        Else
            ' Set the window as foreground.
            Win32API.SetForegroundWindow(hWnd)

            ' If window is minimized, simply restore, otherwise show it.  Notice the 
            ' declaration of Win32API.IsIconic defines the return value as Boolean
            ' allowing .NET to marshall the integer value to a Boolean.
            If Win32API.IsIconic(hWnd) Then
                Win32API.ShowWindow(hWnd, Win32API.SW_RESTORE)
            Else
                Win32API.ShowWindow(hWnd, Win32API.SW_SHOW)
            End If
        End If
    End Sub

    ''' <summary>
    ''' This Sub checks the window caption and class name text boxes and updates 
    ''' lblFunctionCalled label accordingly.  
    ''' </summary>
    Private Sub ChangeFunctionCalledLabel()
        If txtWindowCaption.Text = "" And txtClassName.Text = "" Then
            lblFunctionCalled.Text = _
                "FindWindow (ClassName As Integer, WindowName As Integer) will be called."
        ElseIf txtWindowCaption.Text = "" And txtClassName.Text <> "" Then
            lblFunctionCalled.Text = _
                "FindWindow (ClassName As String, WindowName As Integer) will be called."
        ElseIf txtWindowCaption.Text <> "" And txtClassName.Text = "" Then
            lblFunctionCalled.Text = _
                "FindWindow (ClassName As Integer, WindowName As String) will be called."
        Else
            lblFunctionCalled.Text = _
                "FindWindow (ClassName As String, WindowName As String) will be called."
        End If
    End Sub

    ''' <summary>
    ''' This function is called once for each active process by EnumWindows.  It gets the
    ''' Window caption and class name and updates the process item listview.
    ''' </summary>
    Function FillActiveProcessList(ByVal hWnd As Integer, ByVal lParam As Integer) As Boolean
        Dim windowText As New StringBuilder(StringBufferLength)
        Dim className As New StringBuilder(StringBufferLength)

        ' Get the Window Caption and Class Name.  Notice that the definition of Win32API
        ' functions in the Win32API class are declared differently than in VB6.  All Longs
        ' have been replaced with Integers, and Strings by StringBuilders.
        Win32API.GetWindowText(hWnd, windowText, StringBufferLength)
        Win32API.GetClassName(hWnd, className, StringBufferLength)

        ' Add a new process item to the Processes list view.
        Dim processItem As New ListViewItem(windowText.ToString, 0)
        processItem.SubItems.Add(className.ToString)
        processItem.SubItems.Add(hWnd.ToString)
        lvwProcessList.Items.Add(processItem)
        Return True
    End Function

    ''' <summary>
    ''' This function is called once for each active process by EnumWindows.  
    ''' It calls ProcessIsActiveWindow to verify that it is a valid window, and
    ''' updates the active window listbox.
    ''' </summary>
    Function FillActiveWindowsList(ByVal hWnd As Integer, ByVal lParam As Integer) As Boolean
        Dim windowText As New StringBuilder(StringBufferLength)

        ' Get the Window Caption.
        Win32API.GetWindowText(hWnd, windowText, StringBufferLength)

        ' Only add valid windows to the active windows listbox.
        If ProcessIsActiveWindow(hWnd) Then
            lbActiveWindows.Items.Add(windowText)
        End If

        Return True
    End Function

    ''' <summary>
    ''' This function calls various Win32API functions to determine if a windows process
    ''' is a valid active window..
    ''' </summary>
    Function ProcessIsActiveWindow(ByVal hWnd As Integer) As Boolean
        Dim windowText As New StringBuilder(StringBufferLength)
        Dim windowIsOwned As Boolean
        Dim windowStyle As Integer

        ' Get the windows caption, owner information, and window style.
        Win32API.GetWindowText(hWnd, windowText, StringBufferLength)
        windowIsOwned = Win32API.GetWindow(hWnd, Win32API.GW_OWNER) <> 0
        windowStyle = Win32API.GetWindowLong(hWnd, Win32API.GWL_EXSTYLE)

        ' Do not allow invisible processes.  
        If Not Win32API.IsWindowVisible(hWnd) Then
            Return False
        End If

        ' Window must have a caption
        If windowText.ToString.Equals("") Then
            Return False
        End If

        ' Should not have a parent
        If Win32API.GetParent(hWnd) <> 0 Then
            Return False
        End If

        ' Don't allow unowned tool tips
        If (windowStyle And Win32API.WS_EX_TOOLWINDOW) <> 0 And Not windowIsOwned Then
            Return False
        End If

        ' Don't allow applications with owners
        If (windowStyle And Win32API.WS_EX_APPWINDOW) = 0 And windowIsOwned Then
            Return False
        End If

        ' All criteria were met, window is a valid active window.
        Return True
    End Function

    ''' <summary>
    ''' When txtClassName changes, update the lblFunctionCalled label to show which 
    ''' function will be called.
    ''' </summary>
    Private Sub txtClassName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtClassName.TextChanged
        ChangeFunctionCalledLabel()
    End Sub


    ''' <summary>
    ''' When txtWindowCaption changes, update the lblFunctionCalled label to show which 
    ''' function will be called.
    ''' </summary>
    Private Sub txtWindowCaption_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtWindowCaption.TextChanged
        ChangeFunctionCalledLabel()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
