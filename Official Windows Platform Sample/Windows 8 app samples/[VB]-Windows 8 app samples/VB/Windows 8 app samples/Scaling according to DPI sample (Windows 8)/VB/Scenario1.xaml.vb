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
    Const DEFAULT_LOGICALPPI As Integer = 96
    Const PERCENT As Integer = 100

    Private Shared ReadOnly MinDPI As New Dictionary(Of ResolutionScale, String)() From
    {
        {ResolutionScale.Invalid,         "Unknown"},
        {ResolutionScale.Scale100Percent, "No minimum DPI for this scale"},
        {ResolutionScale.Scale140Percent, "174 DPI"},
        {ResolutionScale.Scale180Percent, "240 DPI"}
    }

    Private Shared ReadOnly MinResolution As New Dictionary(Of ResolutionScale, String) From
    {
        {ResolutionScale.Invalid,         "Unknown"},
        {ResolutionScale.Scale100Percent, "1024x768 (min resolution needed to run apps)"},
        {ResolutionScale.Scale140Percent, "1920x1080"},
        {ResolutionScale.Scale180Percent, "2560x1440"}
    }

    Private ReadOnly Images As Dictionary(Of ResolutionScale, BitmapImage)

    Public Sub New()
        Me.InitializeComponent()
        AddHandler DisplayProperties.LogicalDpiChanged, AddressOf DisplayProperties_LogicalDpiChanged

        Images = New Dictionary(Of ResolutionScale, BitmapImage)() From
                 {
                    {ResolutionScale.Invalid,         New BitmapImage(New Uri(BaseUri, "/Assets/projector.scale-100.png"))},
                    {ResolutionScale.Scale100Percent, New BitmapImage(New Uri(BaseUri, "/Assets/projector.scale-100.png"))},
                    {ResolutionScale.Scale140Percent, New BitmapImage(New Uri(BaseUri, "/Assets/projector.scale-140.png"))},
                    {ResolutionScale.Scale180Percent, New BitmapImage(New Uri(BaseUri, "/Assets/projector.scale-180.png"))}
                 }
    End Sub

    Private Sub ResetOutput()
        ScalingText.Text = (DisplayProperties.LogicalDpi * PERCENT / DEFAULT_LOGICALPPI).ToString & "%"
        MinDPIText.Text = MinDPI(DisplayProperties.ResolutionScale)
        MinScreenResolutionText.Text = MinResolution(DisplayProperties.ResolutionScale)
        LogicalDPIText.Text = DisplayProperties.LogicalDpi.ToString() & " DPI"
        ManualLoadImage.Source = Images(DisplayProperties.ResolutionScale)
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ResetOutput()
    End Sub

    Private Sub DisplayProperties_LogicalDpiChanged(sender As Object)
        ResetOutput()
    End Sub
End Class
