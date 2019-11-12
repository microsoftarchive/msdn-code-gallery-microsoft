'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class ItemEventArgs(Of T)
        Inherits EventArgs
        Private ReadOnly _item As T

        Public Sub New(ByVal item As T)
            _item = item
        End Sub

        Private ReadOnly Property Item() As T
            Get
                Return _item
            End Get
        End Property
    End Class

    Friend Class CollectionWithEvents(Of T)
        Implements ICollection(Of T)
        Private ReadOnly _list As New List(Of T)()

        ''' <summary>
        ''' Fired once for each item that is added to the collection, after the item is added.
        ''' </summary>
        Public Event ItemAdded As EventHandler(Of ItemEventArgs(Of T))

        ''' <summary>
        ''' Fired once for each item that is removed from the collection, before the item is removed.
        ''' </summary>
        Public Event ItemRemoving As EventHandler(Of ItemEventArgs(Of Integer))

        ''' <summary>
        ''' Fired once for each action that removes one or more items from the collection, after the items have been removed.
        ''' </summary>
        Public Event ItemsRemoved As EventHandler

        Public Sub AddRange(ByVal content As IEnumerable(Of T))
            For Each t As T In content
                Add(t)
            Next t
        End Sub

#Region "ICollection<T> Members"

        Public Sub Add(ByVal item As T) Implements ICollection(Of T).Add
            _list.Add(item)
            RaiseEvent ItemAdded(Me, New ItemEventArgs(Of T)(item))
        End Sub

        Public Sub Clear() Implements ICollection(Of T).Clear
            FireItemRemoving(0, Count)
            _list.Clear()
            RaiseEvent ItemsRemoved(Me, EventArgs.Empty)
        End Sub

        Public Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array As T(), ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public ReadOnly Property Count() As Integer Implements ICollection(Of T).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of T).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
            Dim index As Integer = _list.IndexOf(item)
            If index < 0 Then
                Return False
            End If

            FireItemRemoving(index)
            _list.RemoveAt(index)
            RaiseEvent ItemsRemoved(Me, EventArgs.Empty)
            Return True
        End Function

#End Region ' ICollection<T> Members

#Region "IEnumerable<T> Members"

        Public Function GetEnumerator() As IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
            Return _list.GetEnumerator()
        End Function

#End Region ' IEnumerable<T> Members

#Region "IEnumerable Members"

        Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

#End Region ' IEnumerable Members

#Region "Private Members"

        Sub FireItemRemoving(firstIndex As Integer, Optional count As Integer = 1)
            If ItemRemovingEvent IsNot Nothing Then
                For i As Integer = firstIndex To firstIndex + count - 1
                    RaiseEvent ItemRemoving(Me, New ItemEventArgs(Of Integer)(i))
                Next i
            End If
        End Sub

#End Region ' Private Members
    End Class
End Namespace
