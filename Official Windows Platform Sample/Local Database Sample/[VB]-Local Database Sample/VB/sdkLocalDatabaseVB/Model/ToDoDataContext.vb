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
Imports System.ComponentModel
Imports System.Data.Linq
Imports System.Data.Linq.Mapping

Namespace LocalDatabaseSample.Model

    Public Class ToDoDataContext
        Inherits DataContext
        ' Pass the connection string to the base class.
        Public Sub New(ByVal connectionString As String)
            MyBase.New(connectionString)
        End Sub

        ' Specify a table for the to-do items.
        Public Items As Table(Of ToDoItem)

        ' Specify a table for the categories.
        Public Categories As Table(Of ToDoCategory)
    End Class

    <Table()>
    Public Class ToDoItem
        Implements INotifyPropertyChanged, INotifyPropertyChanging

        ' Define ID: private field, public property, and database column.
        Private _toDoItemId As Integer

        <Column(IsPrimaryKey:=True, IsDbGenerated:=True, DbType:="INT NOT NULL Identity", CanBeNull:=False, AutoSync:=AutoSync.OnInsert)>
        Public Property ToDoItemId() As Integer
            Get
                Return _toDoItemId
            End Get
            Set(ByVal value As Integer)
                If _toDoItemId <> value Then
                    NotifyPropertyChanging("ToDoItemId")
                    _toDoItemId = value
                    NotifyPropertyChanged("ToDoItemId")
                End If
            End Set
        End Property

        ' Define item name: private field, public property, and database column.
        Private _itemName As String

        <Column()>
        Public Property ItemName() As String
            Get
                Return _itemName
            End Get
            Set(ByVal value As String)
                If _itemName <> value Then
                    NotifyPropertyChanging("ItemName")
                    _itemName = value
                    NotifyPropertyChanged("ItemName")
                End If
            End Set
        End Property

        ' Define completion value: private field, public property, and database column.
        Private _isComplete As Boolean

        <Column()>
        Public Property IsComplete() As Boolean
            Get
                Return _isComplete
            End Get
            Set(ByVal value As Boolean)
                If _isComplete <> value Then
                    NotifyPropertyChanging("IsComplete")
                    _isComplete = value
                    NotifyPropertyChanged("IsComplete")
                End If
            End Set
        End Property

        ' Version column aids update performance.
        <Column(IsVersion:=True)>
        Private _version As Binary


        ' Internal column for the associated ToDoCategory ID value
        <Column()>
        Friend _categoryId As Integer

        ' Entity reference, to identify the ToDoCategory "storage" table
        Private _category As EntityRef(Of ToDoCategory)

        ' Association, to describe the relationship between this key and that "storage" table
        <Association(Storage:="_category", ThisKey:="_categoryId", OtherKey:="Id", IsForeignKey:=True)>
        Public Property Category() As ToDoCategory
            Get
                Return _category.Entity
            End Get
            Set(ByVal value As ToDoCategory)
                NotifyPropertyChanging("Category")
                _category.Entity = value

                If value IsNot Nothing Then
                    _categoryId = value.Id
                End If

                NotifyPropertyChanging("Category")
            End Set
        End Property


#Region "INotifyPropertyChanged Members"

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ' Used to notify that a property changed
        Private Sub NotifyPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

#End Region

#Region "INotifyPropertyChanging Members"

        Public Event PropertyChanging As PropertyChangingEventHandler Implements INotifyPropertyChanging.PropertyChanging

        ' Used to notify that a property is about to change
        Private Sub NotifyPropertyChanging(ByVal propertyName As String)
            RaiseEvent PropertyChanging(Me, New PropertyChangingEventArgs(propertyName))
        End Sub

#End Region
    End Class


    <Table()>
    Public Class ToDoCategory
        Implements INotifyPropertyChanged, INotifyPropertyChanging

        ' Define ID: private field, public property, and database column.
        Private _id As Integer

        <Column(DbType:="INT NOT NULL IDENTITY", IsDbGenerated:=True, IsPrimaryKey:=True)>
        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                NotifyPropertyChanging("Id")
                _id = value
                NotifyPropertyChanged("Id")
            End Set
        End Property

        ' Define category name: private field, public property, and database column.
        Private _name As String

        <Column()>
        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                NotifyPropertyChanging("Name")
                _name = value
                NotifyPropertyChanged("Name")
            End Set
        End Property

        ' Version column aids update performance.
        <Column(IsVersion:=True)>
        Private _version As Binary

        ' Define the entity set for the collection side of the relationship.
        Private _todos As EntitySet(Of ToDoItem)

        <Association(Storage:="_todos", OtherKey:="_categoryId", ThisKey:="Id")>
        Public Property ToDos() As EntitySet(Of ToDoItem)
            Get
                Return Me._todos
            End Get
            Set(ByVal value As EntitySet(Of ToDoItem))
                Me._todos.Assign(value)
            End Set
        End Property


        ' Assign handlers for the add and remove operations, respectively.
        Public Sub New()
            _todos = New EntitySet(Of ToDoItem)(New Action(Of ToDoItem)(AddressOf Me.attach_ToDo), New Action(Of ToDoItem)(AddressOf Me.detach_ToDo))
        End Sub

        ' Called during an add operation
        Private Sub attach_ToDo(ByVal toDo As ToDoItem)
            NotifyPropertyChanging("ToDoItem")
            toDo.Category = Me
        End Sub

        ' Called during a remove operation
        Private Sub detach_ToDo(ByVal toDo As ToDoItem)
            NotifyPropertyChanging("ToDoItem")
            toDo.Category = Nothing
        End Sub

#Region "INotifyPropertyChanged Members"

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ' Used to notify that a property changed
        Private Sub NotifyPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

#End Region

#Region "INotifyPropertyChanging Members"

        Public Event PropertyChanging As PropertyChangingEventHandler Implements INotifyPropertyChanging.PropertyChanging

        ' Used to notify that a property is about to change
        Private Sub NotifyPropertyChanging(ByVal propertyName As String)
            RaiseEvent PropertyChanging(Me, New PropertyChangingEventArgs(propertyName))
        End Sub

#End Region
    End Class


End Namespace
