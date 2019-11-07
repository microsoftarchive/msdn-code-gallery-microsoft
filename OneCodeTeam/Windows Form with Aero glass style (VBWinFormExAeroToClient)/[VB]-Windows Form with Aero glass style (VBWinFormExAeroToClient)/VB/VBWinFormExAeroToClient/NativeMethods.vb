'******************************** Module Header ***********************************'
' Module Name:       NativeMethods.vb
' Project:           VBWinFormExAeroToClient
' Copyright (c)      Microsoft Corporation.
' 
' This class wraps the DwmIsCompositionEnabled, DwmExtendFrameIntoClientArea and 
' DwmEnableBlurBehindWindow Functions in dwmapi.dll.
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************************

Imports System.Runtime.InteropServices

Public NotInheritable Class NativeMethods
    ''' <summary>
    ''' Obtain a value that indicates whether Desktop Window Manager
    ''' (DWM) composition is enabled. 
    ''' </summary>
    <DllImport("dwmapi.dll", CharSet:=CharSet.Auto, PreserveSig:=False,
        SetLastError:=True)>
    Friend Shared Sub DwmIsCompositionEnabled(<Out()> ByRef pfEnable As Boolean)
    End Sub

    ''' <summary>
    ''' Extend the window frame into the client area.
    ''' </summary>
    <DllImport("dwmapi.dll", CharSet:=CharSet.Auto, PreserveSig:=False,
        SetLastError:=True)>
    Friend Shared Sub DwmExtendFrameIntoClientArea(ByVal hWnd As IntPtr,
                                                   <[In]()> ByRef margins As MARGINS)
    End Sub

    ''' <summary>
    ''' Enable the blur effect on a specified window.
    ''' </summary>
    <DllImport("dwmapi.dll", CharSet:=CharSet.Auto, PreserveSig:=False,
        SetLastError:=True)>
    Friend Shared Sub DwmEnableBlurBehindWindow(ByVal hWnd As IntPtr,
                                                ByVal pBlurBehind As DWM_BLURBEHIND)
    End Sub

    ''' <summary>
    ''' The point of MARGINS structure that describes the margins to use when
    ''' extending the frame into the client area.
    ''' </summary>
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Structure MARGINS
        ' Width of the left border that retains its size.
        Public cxLeftWidth As Integer

        ' Width of the right border that retains its size.
        Public cxRightWidth As Integer

        ' Height of the top border that retains its size.
        Public cyTopHeight As Integer

        ' Height of the bottom border that retains its size.
        Public cyBottomHeight As Integer

        Public Sub New(ByVal margin As Integer)
            cxLeftWidth = margin
            cxRightWidth = margin
            cyTopHeight = margin
            cyBottomHeight = margin
        End Sub

        Public Sub New(ByVal leftWidth As Integer, ByVal rightWidth As Integer,
                       ByVal topHeight As Integer, ByVal bottomHeight As Integer)
            cxLeftWidth = leftWidth
            cxRightWidth = rightWidth
            cyTopHeight = topHeight
            cyBottomHeight = bottomHeight
        End Sub

        ''' <summary>
        ''' Determine whether there is a negative value, or the value is valid
        ''' for a Form.
        ''' </summary>
        Public Function IsNegativeOrOverride(ByVal formClientSize As Size) As Boolean
            Return cxLeftWidth < 0 OrElse cxRightWidth < 0 _
                OrElse cyBottomHeight < 0 OrElse cyTopHeight < 0 _
                OrElse (cxLeftWidth + cxRightWidth) > formClientSize.Width _
                OrElse (cyTopHeight + cyBottomHeight) > formClientSize.Height
        End Function
    End Structure

    ''' <summary>
    ''' Specify Desktop Window Manager (DWM) blur-behind properties. 
    ''' </summary>
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Friend Class DWM_BLURBEHIND
        ' Indicate the members of this structure have been set.
        Public dwFlags As UInteger

        ' The flag specify  whether the subsequent compositions of the window
        ' blurring the content behind it or not.
        <MarshalAs(UnmanagedType.Bool)>
        Public fEnable As Boolean

        ' The region where the glass style will be applied.
        Public hRegionBlur As IntPtr

        ' Whether the windows color should be transited to match the maximized 
        ' windows or not.
        <MarshalAs(UnmanagedType.Bool)>
        Public fTransitionOnMaximized As Boolean

        ' Flags used to indicate the  members contain valid information.
        Public Const DWM_BB_ENABLE As UInteger = &H1
        Public Const DWM_BB_BLURREGION As UInteger = &H2
        Public Const DWM_BB_TRANSITIONONMAXIMIZED As UInteger = &H4
    End Class

End Class

