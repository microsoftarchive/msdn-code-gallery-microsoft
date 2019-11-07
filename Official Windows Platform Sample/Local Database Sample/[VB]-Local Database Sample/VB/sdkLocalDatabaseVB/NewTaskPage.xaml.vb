'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports sdkLocalDatabaseVB.LocalDatabaseSample.Model

    Partial Public Class NewTaskPage
        Inherits PhoneApplicationPage
        Public Sub New()
            InitializeComponent()

            ' Set the page DataContext property to the ViewModel.
            Me.DataContext = App.ViewModel
        End Sub

        Private Sub appBarOkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Confirm there is some text in the text box.
            If newTaskNameTextBox.Text.Length > 0 Then
                ' Create a new to-do item.
                Dim newToDoItem As ToDoItem = New ToDoItem With {.ItemName = newTaskNameTextBox.Text, .Category = CType(categoriesListPicker.SelectedItem, ToDoCategory)}

                ' Add the item to the ViewModel.
                App.ViewModel.AddToDoItem(newToDoItem)

                ' Return to the main page.
                If NavigationService.CanGoBack Then
                    NavigationService.GoBack()
                End If
            End If
        End Sub

        Private Sub appBarCancelButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Return to the main page.
            If NavigationService.CanGoBack Then
                NavigationService.GoBack()
            End If
        End Sub
    End Class
