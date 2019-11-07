' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Windows.Foundation
Imports Windows.UI
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media
Imports ViewManagement = Windows.UI.ViewManagement

Partial Class TargetButton
    Inherits Button
    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()

        ' Apply correct coloring when in High Contrast

        Dim accessibilitySettings As New ViewManagement.AccessibilitySettings()
        If Not (accessibilitySettings.HighContrast) Then
            'Off
            ' Use default colors

            Me.Background = New SolidColorBrush(Colors.Red)
            Me.BorderBrush = New SolidColorBrush(Colors.Black)

            Me.Circle4.Fill = New SolidColorBrush(Colors.Blue)
            Me.Circle3.Fill = New SolidColorBrush(Colors.Green)
            Me.Circle2.Fill = New SolidColorBrush(Colors.Yellow)
            Me.Circle1.Fill = New SolidColorBrush(Colors.White)
            Me.Circle4.Stroke = InlineAssignHelper(Me.Circle3.Stroke, InlineAssignHelper(Me.Circle2.Stroke, InlineAssignHelper(Me.Circle1.Stroke, New SolidColorBrush(Colors.Black))))
        Else
            ' Use High Contrast Colors

            Dim uiSettings As New ViewManagement.UISettings()

            Select Case accessibilitySettings.HighContrastScheme
                Case "High Contrast Black"
                    Me.Background = InlineAssignHelper(Me.Circle4.Fill, InlineAssignHelper(Me.Circle3.Fill, InlineAssignHelper(Me.Circle2.Fill, InlineAssignHelper(Me.Circle1.Fill, New SolidColorBrush(Colors.Black)))))
                    Me.BorderBrush = InlineAssignHelper(Me.Circle4.Stroke, InlineAssignHelper(Me.Circle3.Stroke, InlineAssignHelper(Me.Circle2.Stroke, InlineAssignHelper(Me.Circle1.Stroke, New SolidColorBrush(Colors.White)))))
                    Exit Select
                Case "High Contrast White"
                    Me.Background = InlineAssignHelper(Me.Circle4.Fill, InlineAssignHelper(Me.Circle3.Fill, InlineAssignHelper(Me.Circle2.Fill, InlineAssignHelper(Me.Circle1.Fill, New SolidColorBrush(Colors.White)))))
                    Me.BorderBrush = InlineAssignHelper(Me.Circle4.Stroke, InlineAssignHelper(Me.Circle3.Stroke, InlineAssignHelper(Me.Circle2.Stroke, InlineAssignHelper(Me.Circle1.Stroke, New SolidColorBrush(Colors.Black)))))
                    Exit Select
                Case Else
                    ' For all other High Contrast schemes
                    Me.Background = New SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.ButtonFace))
                    Me.BorderBrush = New SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.ButtonText))
                    Me.Circle4.Fill = InlineAssignHelper(Me.Circle3.Fill, InlineAssignHelper(Me.Circle2.Fill, InlineAssignHelper(Me.Circle1.Fill, New SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.Hotlight)))))
                    Me.Circle4.Stroke = InlineAssignHelper(Me.Circle3.Stroke, InlineAssignHelper(Me.Circle2.Stroke, InlineAssignHelper(Me.Circle1.Stroke, New SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.HighlightText)))))
                    Exit Select
            End Select
        End If
    End Sub

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class
