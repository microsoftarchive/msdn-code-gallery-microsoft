'********************************* Module Header ***********************************\
' Module Name:	  GlassForm.vb
' Project:		  VBWinFormExAeroToClient
' Copyright (c)   Microsoft Corporation.
' 
' This class is a base class that is used to create a glass style win form on Vista or 
' Windows7.
' 
' VBWinFormExAeroToClient example demonstrates how to use DwmExtendFrameIntoClientArea to
' extend the Windows Vista glass frame into the client area of a Windows Form application.
' 
' There are 2 approaches to achieve this effect.
' 
' A. Extend the frame to achieve the Aero effect.  
'    1. Extend the frame using the DwmExtendFrameIntoClientArea method. The margins parameter
'       of this method defines the margins of the form, in other words, the width and height
'       of the frame.       
'    2. Make the region in the margins transparent.     
'    Because the region belongs to the frame now, so it will have the Aero effect.
' 
' B. Enable the blur effect on the form to achieve the Aero effect.
'    1. Make the region transparent.
'    2. Enable the blur effect on the form using the DwmEnableBlurBehindWindow method.
'    Then the region will have a Aero effect.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************************

Imports System.Security.Permissions


<PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust"),
 PermissionSetAttribute(SecurityAction.InheritanceDemand, Name:="FullTrust")>
Public Class GlassForm
    Inherits Form

    ' Inform all top-level windows that Desktop Window Manager (DWM) 
    ' composition has been enabled or disabled.
    Private Const WM_DWMCOMPOSITIONCHANGED As Integer = &H31E

    ' Sent to a window in order to determine what part of the window 
    ' corresponds to a particular screen coordinate.
    Private Const WM_NCHITTEST As Integer = &H84

    ' In a client area.
    Private Const HTCLIENT As Integer = &H1

    ' A less frequently used Color that is used as the TransparencyKey.  
    Private Shared _transparentColor As Color = Color.DarkTurquoise

    ''' <summary>
    ''' Specify whether extending the frame is enabled.
    ''' </summary>
    Public Property ExtendFrameEnabled() As Boolean

    ''' <summary>
    ''' Specify whether the blur effect is enabled.
    ''' </summary>
    Public Property BlurBehindWindowEnabled() As Boolean

    Private _marginRegion As Region = Nothing

    ''' <summary>
    ''' Set the frame border. 
    ''' </summary>
    Public Property GlassMargins() As NativeMethods.MARGINS

    Private _blurRegion As Region = Nothing

    ''' <summary>
    ''' The region that the blur effect will be applied.
    ''' </summary>
    Public Property BlurRegion() As Region
        Get
            Return _blurRegion

        End Get
        Set(ByVal value As Region)
            If _blurRegion IsNot Nothing Then
                _blurRegion.Dispose()
            End If
            _blurRegion = value
        End Set
    End Property

    Public Event DWMCompositionChanged As EventHandler

    ''' <summary>
    ''' Set the TransparencyKey.
    ''' </summary>
    Public Sub New()
        Me.TransparencyKey = _transparentColor
    End Sub

    ''' <summary>
    ''' When the size of this form changes, redraw the form.
    ''' </summary>
    Protected Overrides Sub OnSizeChanged(ByVal e As EventArgs)
        MyBase.OnSizeChanged(e)
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' When the form is painted, set the region where the glass effect 
    ''' is applied.
    ''' </summary>
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        If ((Not ExtendFrameEnabled) AndAlso (Not BlurBehindWindowEnabled)) _
            OrElse (Not IsAeroGlassStyleSupported()) Then
            Return
        End If

        Using transparentBrush As Brush = New SolidBrush(_transparentColor)

            ' Extend the frame.
            If ExtendFrameEnabled Then
                Dim glassMargins = Me.GlassMargins

                ' Extend the frame.
                NativeMethods.DwmExtendFrameIntoClientArea(Me.Handle, glassMargins)

                ' Make the region in the margins transparent. 
                _marginRegion = New Region(Me.ClientRectangle)

                ' If the glassMargins contains a negative value, or the values are not valid,
                ' then make the whole form transparent.
                If Me.GlassMargins.IsNegativeOrOverride(Me.ClientSize) Then
                    e.Graphics.FillRegion(transparentBrush, _marginRegion)

                    ' By default, exclude the region of the client area.
                Else
                    _marginRegion.Exclude(New Rectangle(
                                          Me.GlassMargins.cxLeftWidth,
                                          Me.GlassMargins.cyTopHeight,
                                          Me.ClientSize.Width - Me.GlassMargins.cxLeftWidth - Me.GlassMargins.cxRightWidth,
                                          Me.ClientSize.Height - Me.GlassMargins.cyTopHeight - Me.GlassMargins.cyBottomHeight))
                    e.Graphics.FillRegion(transparentBrush, _marginRegion)
                End If

                ' Reset the frame to the default state.
            Else
                Dim glassMargins = New NativeMethods.MARGINS(-1)
                NativeMethods.DwmExtendFrameIntoClientArea(Me.Handle, glassMargins)
            End If

            ' Enable the blur effect on the form.
            If BlurBehindWindowEnabled Then
                ResetDwmBlurBehind(True, e.Graphics)

                If Me.BlurRegion IsNot Nothing Then
                    e.Graphics.FillRegion(transparentBrush, BlurRegion)
                Else
                    e.Graphics.FillRectangle(transparentBrush, Me.ClientRectangle)
                End If
            Else
                ResetDwmBlurBehind(False, Nothing)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' Enable or disable the blur effect on the form.
    ''' </summary>
    Private Sub ResetDwmBlurBehind(ByVal enable As Boolean, ByVal graphics As Graphics)
        Try
            Dim bbh As New NativeMethods.DWM_BLURBEHIND()

            If enable Then
                bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE
                bbh.fEnable = True

                If Me.BlurRegion IsNot Nothing Then
                    bbh.hRegionBlur = Me.BlurRegion.GetHrgn(graphics)
                Else
                    ' Apply the blur glass effect to the entire window.
                    bbh.hRegionBlur = IntPtr.Zero
                End If
            Else
                bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE _
                    Or NativeMethods.DWM_BLURBEHIND.DWM_BB_BLURREGION
                ' Turn off the glass effect.
                bbh.fEnable = False
                ' Apply the blur glass effect to the entire window.
                bbh.hRegionBlur = IntPtr.Zero
            End If
            NativeMethods.DwmEnableBlurBehindWindow(Me.Handle, bbh)
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Make sure the current computer is able to display the glass style windows.
    ''' </summary>
    ''' <returns>
    ''' The flag that specify whether DWM composition is enabled or not.
    ''' </returns>
    Public Shared Function IsAeroGlassStyleSupported() As Boolean
        Dim isDWMEnable As Boolean = False
        Try
            ' Check that the glass is enabled by using the DwmIsCompositionEnabled. 
            ' It is supported in version 6.0 or above of the operating system.
            If Environment.OSVersion.Version.Major >= 6 Then
                ' Make sure the Glass is enabled by the user.
                NativeMethods.DwmIsCompositionEnabled(isDWMEnable)
            End If
        Catch
        End Try

        Return isDWMEnable
    End Function

    ''' <summary>
    ''' This method makes that users can drag the form by click the extended frame.
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        ' Let the normal WndProc process it.
        MyBase.WndProc(m)

        Select Case m.Msg
            Case WM_NCHITTEST
                ' The mouse is inside the client area
                If HTCLIENT = m.Result.ToInt32() Then
                    ' Parse the WM_NCHITTEST message parameters
                    ' get the mouse pointer coordinates (in screen coordinates)
                    Dim p As New Point()
                    ' low order word
                    p.X = (m.LParam.ToInt32() And &HFFFF)
                    ' high order word
                    p.Y = (m.LParam.ToInt32() >> 16)

                    ' Convert screen coordinates to client area coordinates
                    p = PointToClient(p)

                    ' If it's on glass, then convert it from an HTCLIENT
                    ' message to an HTCAPTION message and let Windows handle it 
                    ' from then on.
                    If PointIsOnGlass(p) Then
                        m.Result = New IntPtr(2)
                    End If
                End If
            Case WM_DWMCOMPOSITIONCHANGED

                ' Release the resource when glass effect is not supported.
                RaiseEvent DWMCompositionChanged(Me, EventArgs.Empty)
            Case Else
        End Select
    End Sub

    ''' <summary>
    ''' Check that the point is inside the glass area.
    ''' </summary>
    Private Function PointIsOnGlass(ByVal p As Point) As Boolean
        If Me._marginRegion Is Nothing Then
            Return False
        Else
            Return Me._marginRegion.IsVisible(p)
        End If
    End Function

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)

        If Me._marginRegion IsNot Nothing Then
            _marginRegion.Dispose()
        End If

        If Me.BlurRegion IsNot Nothing Then
            Me.BlurRegion.Dispose()
        End If
    End Sub

End Class
