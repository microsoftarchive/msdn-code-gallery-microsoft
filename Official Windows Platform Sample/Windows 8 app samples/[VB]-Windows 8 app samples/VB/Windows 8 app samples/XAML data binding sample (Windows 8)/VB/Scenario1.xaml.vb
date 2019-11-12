'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub Scenario1Reset(sender As Object, e As RoutedEventArgs)
        sliderOneWayDataSource.Value = 10
        sliderTwoWayDataSource.Value = 50
        sliderOneTimeDataSource.Value = 100
    End Sub

End Class
