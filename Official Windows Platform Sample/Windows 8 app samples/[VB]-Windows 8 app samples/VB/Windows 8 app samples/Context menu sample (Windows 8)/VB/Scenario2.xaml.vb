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
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.System
Imports Windows.UI.Popups
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler ReadOnlyTextBox.ContextMenuOpening, AddressOf ReadOnlyTextBox_ContextMenuOpening
    End Sub

    ' returns a rect for selected text
    ' if no text is selected, returns caret location
    ' textbox should not be empty
    Private Function GetTextboxSelectionRect(textbox As TextBox) As Rect
        Dim rectFirst As Rect, rectLast As Rect
        If textbox.SelectionStart = textbox.Text.Length Then
            rectFirst = textbox.GetRectFromCharacterIndex(textbox.SelectionStart - 1, True)
        Else
            rectFirst = textbox.GetRectFromCharacterIndex(textbox.SelectionStart, False)
        End If

        Dim lastIndex As Integer = textbox.SelectionStart + textbox.SelectionLength
        If lastIndex = textbox.Text.Length Then
            rectLast = textbox.GetRectFromCharacterIndex(lastIndex - 1, True)
        Else
            rectLast = textbox.GetRectFromCharacterIndex(lastIndex, False)
        End If

        Dim buttonTransform As GeneralTransform = textbox.TransformToVisual(Nothing)
        Dim point As Point = buttonTransform.TransformPoint(New Point)

        ' Make sure that we return a valid rect if selection is on multiple lines
        ' and end of the selection is to the left of the start of the selection.
        Dim x As Double, y As Double, dx As Double, dy As Double
        y = point.Y + rectFirst.Top
        dy = rectLast.Bottom - rectFirst.Top
        If rectLast.Right > rectFirst.Left Then
            x = point.X + rectFirst.Left
            dx = rectLast.Right - rectFirst.Left
        Else
            x = point.X + rectLast.Right
            dx = rectFirst.Left - rectLast.Right
        End If

        Return New Rect(x, y, dx, dy)
    End Function

    Private Async Sub ReadOnlyTextBox_ContextMenuOpening(sender As Object, e As ContextMenuEventArgs)
        e.Handled = True
        Dim textbox As TextBox = DirectCast(sender, TextBox)
        If textbox.SelectionLength > 0 Then
            ' Create a menu and add commands specifying an id value for each instead of a delegate.
            Dim menu = New PopupMenu
            menu.Commands.Add(New UICommand("Copy", Nothing, 1))
            menu.Commands.Add(New UICommandSeparator)
            menu.Commands.Add(New UICommand("Highlight", Nothing, 2))
            menu.Commands.Add(New UICommand("Look up", Nothing, 3))

            ' We don't want to obscure content, so pass in a rectangle representing the selection area.
            ' NOTE: this code only handles textboxes with a single line. If a textbox has multiple lines,
            '       then the context menu should be placed at cursor/pointer location by convention.
            OutputTextBlock.Text = "Context menu shown"
            Dim rect As Rect = GetTextboxSelectionRect(textbox)
            Dim chosenCommand = Await menu.ShowForSelectionAsync(rect)
            If chosenCommand IsNot Nothing Then
                Select Case CInt(chosenCommand.Id)
                    Case 1
                        Dim selectedText As String = DirectCast(sender, TextBox).SelectedText
                        Dim dataPackage = New DataPackage
                        dataPackage.SetText(selectedText)
                        Clipboard.SetContent(dataPackage)
                        OutputTextBlock.Text = "'" & chosenCommand.Label & "'(" & chosenCommand.Id.ToString & ") selected; '" & selectedText & "' copied to clipboard"
                        Exit Select

                    Case 2
                        OutputTextBlock.Text = "'" & chosenCommand.Label & "'(" & chosenCommand.Id.ToString & ") selected"
                        Exit Select

                    Case 3
                        OutputTextBlock.Text = "'" & chosenCommand.Label & "'(" & chosenCommand.Id.ToString & ") selected"
                        Exit Select
                End Select
            Else
                OutputTextBlock.Text = "Context menu dismissed"
            End If
        Else
            OutputTextBlock.Text = "Context menu not shown because there is no text selected"
        End If
    End Sub
End Class
