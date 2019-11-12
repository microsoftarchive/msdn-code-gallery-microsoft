'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate

Imports System
Imports System.Collections.Generic
Imports Windows.Graphics.Display
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media.Imaging

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private Shared ReadOnly MinDPI As New Dictionary(Of ResolutionScale, String)() From {{ResolutionScale.Invalid, "Unknown"}, {ResolutionScale.Scale100Percent, "No minimum DPI for this scale"}, {ResolutionScale.Scale140Percent, "174 DPI"}, {ResolutionScale.Scale180Percent, "240 DPI"}}

    Private Shared ReadOnly MinResolution As New Dictionary(Of ResolutionScale, String)() From {{ResolutionScale.Invalid, "Unknown"}, {ResolutionScale.Scale100Percent, "1024x768 (min resolution needed to run apps)"}, {ResolutionScale.Scale140Percent, "1440x1080"}, {ResolutionScale.Scale180Percent, "1920x1440"}}

    Private Const imageBase As String = "http://www.contoso.com/imageScale{0}.png"

    Public Sub New()
        Me.InitializeComponent()
        Dim displayInformation As DisplayInformation = displayInformation.GetForCurrentView()
        AddHandler displayInformation.DpiChanged, AddressOf DisplayProperties_DpiChanged
    End Sub

    Private Sub ResetOutput()
        Dim displayInformation As DisplayInformation = displayInformation.GetForCurrentView()
        Dim scale As ResolutionScale = displayInformation.ResolutionScale
        Dim scaleValue As String = CInt(scale).ToString()
        ScalingText.Text = scaleValue & "%"
        ManualLoadURL.Text = String.Format(imageBase, scaleValue)
        MinDPIText.Text = MinDPI(scale)
        MinScreenResolutionText.Text = MinResolution(scale)
        LogicalDPIText.Text = displayInformation.LogicalDpi.ToString() & " DPI"
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ResetOutput()
    End Sub

    Private Sub DisplayProperties_DpiChanged(ByVal sender As DisplayInformation, ByVal args As Object)
        ResetOutput()
    End Sub
End Class
