'************************** Module Header ******************************'
' Module Name:  MainForm.vb
' Project:      VBWinFormExAeroToClient
' Copyright (c) Microsoft Corporation.
' 
' This is the Main UI of this application. The user could set the Aero effect
' of the demo Form.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************'

Partial Public Class MainForm
    Inherits Form
    Private demoForm As GlassForm = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Show the Demo Form or change the style of the Demo Form.
    ''' </summary>
    Private Sub btnApply_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnApply.Click

        If demoForm Is Nothing OrElse demoForm.IsDisposed Then
            demoForm = New GlassForm()
            demoForm.Text = "Demo Form"
            AddHandler demoForm.DWMCompositionChanged, _
                AddressOf demoForm_DWMCompositionChanged
            demoForm.Show()
        End If

        Try
            demoForm.ExtendFrameEnabled = chkExtendFrame.Checked
            If demoForm.ExtendFrameEnabled Then
                If chkEntendToEntireClientArea.Checked Then
                    demoForm.GlassMargins = New NativeMethods.MARGINS(-1)
                Else
                    demoForm.GlassMargins = New NativeMethods.MARGINS(
                        Integer.Parse(tbLeft.Text),
                        Integer.Parse(tbRight.Text),
                        Integer.Parse(tbTop.Text),
                        Integer.Parse(tbBottom.Text))
                End If
            End If

            demoForm.BlurBehindWindowEnabled = chkBlurBehindWindow.Checked
            If demoForm.BlurBehindWindowEnabled Then
                If chkEnableEntireFormBlurEffect.Checked Then
                    demoForm.BlurRegion = Nothing
                Else
                    demoForm.BlurRegion = New Region(
                        New Rectangle(
                            Integer.Parse(tbX.Text),
                            Integer.Parse(tbY.Text),
                            Integer.Parse(tbWidth.Text),
                            Integer.Parse(tbHeight.Text)))
                End If
            End If

            demoForm.Invalidate()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Display whether the Aero Glass Style is enabled.
    ''' </summary>
    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load
        UpdateUI(GlassForm.IsAeroGlassStyleSupported())
    End Sub

    ''' <summary>
    ''' Update the UI if the DWMComposition Changed.
    ''' </summary>
    Private Sub demoForm_DWMCompositionChanged(ByVal sender As Object,
                                               ByVal e As EventArgs)
        UpdateUI(GlassForm.IsAeroGlassStyleSupported())
    End Sub

    ''' <summary>
    ''' Enable or disable some controls if the DWMComposition Changed.
    ''' </summary>
    ''' <param name="isAeroGlassStyleSupported"></param>
    Private Sub UpdateUI(ByVal isAeroGlassStyleSupported As Boolean)
        Me.lbAeroGlassStyleSupported.Text = String.Format(
            "Aero GlassStyle Supported : {0} ", isAeroGlassStyleSupported)

        If isAeroGlassStyleSupported Then
            chkExtendFrame.Enabled = True
            chkBlurBehindWindow.Enabled = True
            chkEntendToEntireClientArea.Enabled = True
            chkEnableEntireFormBlurEffect.Enabled = True
            btnApply.Enabled = True
        Else
            chkExtendFrame.Enabled = False
            chkBlurBehindWindow.Enabled = False
            chkEntendToEntireClientArea.Enabled = False
            chkEnableEntireFormBlurEffect.Enabled = False
            btnApply.Enabled = False

            chkExtendFrame.Checked = False
            chkBlurBehindWindow.Checked = False
            chkEntendToEntireClientArea.Checked = False
            chkEnableEntireFormBlurEffect.Checked = False
        End If
    End Sub
End Class

