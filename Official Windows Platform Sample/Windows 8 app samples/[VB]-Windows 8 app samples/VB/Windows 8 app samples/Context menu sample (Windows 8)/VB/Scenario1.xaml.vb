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
Imports Windows.UI.Popups
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler AttachmentImage.RightTapped, AddressOf AttachmentImage_RightTapped
    End Sub

    Public Shared Function GetElementRect(element As FrameworkElement) As Rect
        Dim buttonTransform As GeneralTransform = element.TransformToVisual(Nothing)
        Dim point As Point = buttonTransform.TransformPoint(New Point)
        Return New Rect(point, New Size(element.ActualWidth, element.ActualHeight))
    End Function

    Private Async Sub AttachmentImage_RightTapped(sender As Object, e As RightTappedRoutedEventArgs)
        ' Create a menu and add commands specifying a callback delegate for each.
        ' Since command delegates are unique, no need to specify command Ids.
        Dim menu = New PopupMenu
        menu.Commands.Add(New UICommand("Open with", Sub(command)
                                                         OutputTextBlock.Text = "'" & command.Label.ToString & "' selected"
                                                     End Sub))
        menu.Commands.Add(New UICommand("Save attachment", Sub(command)
                                                               OutputTextBlock.Text = "'" & command.Label.ToString & "' selected"
                                                           End Sub))

        ' We don't want to obscure content, so pass in a rectangle representing the sender of the context menu event.
        ' We registered command callbacks; no need to handle the menu completion event
        OutputTextBlock.Text = "Context menu shown"
        Dim chosenCommand = Await menu.ShowForSelectionAsync(GetElementRect(DirectCast(sender, FrameworkElement)))
        If chosenCommand Is Nothing Then
            ' The command is null if no command was invoked.
            OutputTextBlock.Text = "Context menu dismissed"
        End If
    End Sub
End Class
