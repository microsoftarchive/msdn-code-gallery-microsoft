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

    Partial Public Class MainPage
        Inherits PhoneApplicationPage
        ' Constructor
        Public Sub New()
            InitializeComponent()

            ' Set the page DataContext property to the ViewModel.
            Me.DataContext = App.ViewModel
        End Sub

        Private Sub newTaskAppBarButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            NavigationService.Navigate(New Uri("/NewTaskPage.xaml", UriKind.Relative))
        End Sub


        Private Sub deleteTaskButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' Cast the parameter as a button.
            Dim button = TryCast(sender, Button)

            If button IsNot Nothing Then
                ' Get a handle for the to-do item bound to the button.
                Dim toDoForDelete As ToDoItem = TryCast(button.DataContext, ToDoItem)

                App.ViewModel.DeleteToDoItem(toDoForDelete)
            End If

            ' Put the focus back to the main page.
            Me.Focus()
        End Sub

        Protected Overrides Sub OnNavigatedFrom(ByVal e As System.Windows.Navigation.NavigationEventArgs)
            ' Save changes to the database.
            App.ViewModel.SaveChangesToDB()
        End Sub
    End Class
