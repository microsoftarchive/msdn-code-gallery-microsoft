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
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq

' Directive for the data model.
Imports sdkLocalDatabaseVB.LocalDatabaseSample.Model



Namespace LocalDatabaseSample.ViewModel
    Public Class ToDoViewModel
        Implements INotifyPropertyChanged
        ' LINQ to SQL data context for the local database.
        Private toDoDB As ToDoDataContext

        ' Class constructor, create the data context object.
        Public Sub New(ByVal toDoDBConnectionString As String)
            toDoDB = New ToDoDataContext(toDoDBConnectionString)
        End Sub

        ' All to-do items.
        Private _allToDoItems As ObservableCollection(Of ToDoItem)
        Public Property AllToDoItems() As ObservableCollection(Of ToDoItem)
            Get
                Return _allToDoItems
            End Get
            Set(ByVal value As ObservableCollection(Of ToDoItem))
                _allToDoItems = value
                NotifyPropertyChanged("AllToDoItems")
            End Set
        End Property

        ' To-do items associated with the home category.
        Private _homeToDoItems As ObservableCollection(Of ToDoItem)
        Public Property HomeToDoItems() As ObservableCollection(Of ToDoItem)
            Get
                Return _homeToDoItems
            End Get
            Set(ByVal value As ObservableCollection(Of ToDoItem))
                _homeToDoItems = value
                NotifyPropertyChanged("HomeToDoItems")
            End Set
        End Property

        ' To-do items associated with the work category.
        Private _workToDoItems As ObservableCollection(Of ToDoItem)
        Public Property WorkToDoItems() As ObservableCollection(Of ToDoItem)
            Get
                Return _workToDoItems
            End Get
            Set(ByVal value As ObservableCollection(Of ToDoItem))
                _workToDoItems = value
                NotifyPropertyChanged("WorkToDoItems")
            End Set
        End Property

        ' To-do items associated with the hobbies category.
        Private _hobbiesToDoItems As ObservableCollection(Of ToDoItem)
        Public Property HobbiesToDoItems() As ObservableCollection(Of ToDoItem)
            Get
                Return _hobbiesToDoItems
            End Get
            Set(ByVal value As ObservableCollection(Of ToDoItem))
                _hobbiesToDoItems = value
                NotifyPropertyChanged("HobbiesToDoItems")
            End Set
        End Property

        ' A list of all categories, used by the add task page.
        Private _categoriesList As List(Of ToDoCategory)
        Public Property CategoriesList() As List(Of ToDoCategory)
            Get
                Return _categoriesList
            End Get
            Set(ByVal value As List(Of ToDoCategory))
                _categoriesList = value
                NotifyPropertyChanged("CategoriesList")
            End Set
        End Property

        ' Write changes in the data context to the database.
        Public Sub SaveChangesToDB()
            toDoDB.SubmitChanges()
        End Sub

        ' Query database and load the collections and list used by the pivot pages.
        Public Sub LoadCollectionsFromDatabase()

            ' Specify the query for all to-do items in the database.
            Dim toDoItemsInDB = From todo As ToDoItem In toDoDB.Items
                                Select todo

            ' Query the database and load all to-do items.
            AllToDoItems = New ObservableCollection(Of ToDoItem)(toDoItemsInDB)

            ' Specify the query for all categories in the database.
            Dim toDoCategoriesInDB = From category As ToDoCategory In toDoDB.Categories
                                     Select category


            ' Query the database and load all associated items to their respective collections.
            For Each category In toDoCategoriesInDB
                Select Case category.Name
                    Case "Home"
                        HomeToDoItems = New ObservableCollection(Of ToDoItem)(category.ToDos)
                    Case "Work"
                        WorkToDoItems = New ObservableCollection(Of ToDoItem)(category.ToDos)
                    Case "Hobbies"
                        HobbiesToDoItems = New ObservableCollection(Of ToDoItem)(category.ToDos)
                    Case Else
                End Select
            Next category

            ' Load a list of all categories.
            CategoriesList = toDoDB.Categories.ToList()

        End Sub

        ' Add a to-do item to the database and collections.
        Public Sub AddToDoItem(ByVal newToDoItem As ToDoItem)
            ' Add a to-do item to the data context.
            toDoDB.Items.InsertOnSubmit(newToDoItem)

            ' Save changes to the database.
            toDoDB.SubmitChanges()

            ' Add a to-do item to the "all" observable collection.
            AllToDoItems.Add(newToDoItem)

            ' Add a to-do item to the appropriate filtered collection.
            Select Case newToDoItem.Category.Name
                Case "Home"
                    HomeToDoItems.Add(newToDoItem)
                Case "Work"
                    WorkToDoItems.Add(newToDoItem)
                Case "Hobbies"
                    HobbiesToDoItems.Add(newToDoItem)
                Case Else
            End Select
        End Sub

        ' Remove a to-do task item from the database and collections.
        Public Sub DeleteToDoItem(ByVal toDoForDelete As ToDoItem)

            ' Remove the to-do item from the "all" observable collection.
            AllToDoItems.Remove(toDoForDelete)

            ' Remove the to-do item from the data context.
            toDoDB.Items.DeleteOnSubmit(toDoForDelete)

            ' Remove the to-do item from the appropriate category.   
            Select Case toDoForDelete.Category.Name
                Case "Home"
                    HomeToDoItems.Remove(toDoForDelete)
                Case "Work"
                    WorkToDoItems.Remove(toDoForDelete)
                Case "Hobbies"
                    HobbiesToDoItems.Remove(toDoForDelete)
                Case Else
            End Select

            ' Save changes to the database.
            toDoDB.SubmitChanges()
        End Sub


#Region "INotifyPropertyChanged Members"

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ' Used to notify Silverlight that a property has changed.
        Private Sub NotifyPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
#End Region
    End Class
End Namespace
