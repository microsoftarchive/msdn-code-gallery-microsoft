' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data.Objects
Imports System.Linq
Imports System.Linq.Expressions

Namespace EmployeeTracker.Fakes

    ''' <summary>
    ''' Implementation of IObjectSet based on in-memory data
    ''' </summary>
    ''' <typeparam name="TEntity">Type of data to be stored in set</typeparam>
    Public NotInheritable Class FakeObjectSet(Of TEntity As Class)
        Implements IObjectSet(Of TEntity)
        ''' <summary>
        ''' The underlying data of this set
        ''' </summary>
        Private data As HashSet(Of TEntity)

        ''' <summary>
        ''' IQueryable version of underlying data
        ''' </summary>
        Private query As IQueryable

        ''' <summary>
        ''' Initializes a new instance of the FakeObjectSet class.
        ''' The instance contains no data.
        ''' </summary>
        Public Sub New()
            Me.data = New HashSet(Of TEntity)()
            Me.query = Me.data.AsQueryable()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the FakeObjectSet class.
        ''' The instance contains the supplied data data.
        ''' </summary>
        ''' <param name="testData">Data to be included in set</param>
        Public Sub New(ByVal testData As IEnumerable(Of TEntity))
            If testData Is Nothing Then
                Throw New ArgumentNullException("testData")
            End If

            Me.data = New HashSet(Of TEntity)(testData)
            Me.query = Me.data.AsQueryable()
        End Sub

        ''' <summary>
        ''' Gets the type of elements in this set
        ''' </summary>
        Private ReadOnly Property ElementType() As Type Implements IQueryable.ElementType
            Get
                Return Me.query.ElementType
            End Get
        End Property

        ''' <summary>
        ''' Gets the expression tree for this set
        ''' </summary>
        Private ReadOnly Property IQueryable_Expression() As Expression Implements IQueryable.Expression
            Get
                Return Me.query.Expression
            End Get
        End Property

        ''' <summary>
        ''' Gets the query provider for this set
        ''' </summary>
        Private ReadOnly Property Provider() As IQueryProvider Implements IQueryable.Provider
            Get
                Return Me.query.Provider
            End Get
        End Property

        ''' <summary>
        ''' Adds a new item to this set
        ''' </summary>
        ''' <param name="entity">The item to add</param>
        Public Sub AddObject(ByVal entity As TEntity) Implements IObjectSet(Of TEntity).AddObject
            If entity Is Nothing Then
                Throw New ArgumentNullException("entity")
            End If

            Me.data.Add(entity)
        End Sub

        ''' <summary>
        ''' Deletes a new item from this set
        ''' </summary>
        ''' <param name="entity">The item to delete</param>
        Public Sub DeleteObject(ByVal entity As TEntity) Implements IObjectSet(Of TEntity).DeleteObject
            If entity Is Nothing Then
                Throw New ArgumentNullException("entity")
            End If

            Me.data.Remove(entity)
        End Sub

        ''' <summary>
        ''' Attaches a new item to this set
        ''' </summary>
        ''' <param name="entity">The item to attach</param>
        Public Sub Attach(ByVal entity As TEntity) Implements IObjectSet(Of TEntity).Attach
            If entity Is Nothing Then
                Throw New ArgumentNullException("entity")
            End If

            Me.data.Add(entity)
        End Sub

        ''' <summary>
        ''' Detaches a new item from this set
        ''' </summary>
        ''' <param name="entity">The item to detach</param>
        Public Sub Detach(ByVal entity As TEntity) Implements IObjectSet(Of TEntity).Detach
            If entity Is Nothing Then
                Throw New ArgumentNullException("entity")
            End If

            Me.data.Remove(entity)
        End Sub

        ''' <summary>
        ''' Gets an enumerator for this set
        ''' </summary>
        ''' <returns>Returns an enumerator for all items in this set</returns>
        Private Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.data.GetEnumerator()
        End Function

        ''' <summary>
        ''' Gets a typed enumerator for this set
        ''' </summary>
        ''' <returns>Returns an enumerator for all items in this set</returns>
        Private Function GetEnumerator2() As IEnumerator(Of TEntity) Implements IEnumerable(Of TEntity).GetEnumerator
            Return Me.data.GetEnumerator()
        End Function
    End Class
End Namespace