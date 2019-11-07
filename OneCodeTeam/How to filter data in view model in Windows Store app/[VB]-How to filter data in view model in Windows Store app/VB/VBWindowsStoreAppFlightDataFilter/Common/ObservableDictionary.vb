Namespace Common

    ''' <summary>
    ''' Implementation of IObservableMap that supports reentrancy for use as a default
    ''' view model.
    ''' </summary>
    Public Class ObservableDictionary
        Implements IObservableMap(Of String, Object)

        Private Class ObservableDictionaryChangedEventArgs
            Implements IMapChangedEventArgs(Of String)

            Private _change As CollectionChange
            Private _key As String

            Public Sub New(change As CollectionChange, key As String)
                Me._change = change
                Me._key = key
            End Sub

            Public ReadOnly Property CollectionChange As CollectionChange Implements IMapChangedEventArgs(Of String).CollectionChange
                Get
                    Return _change
                End Get
            End Property

            Public ReadOnly Property Key As String Implements IMapChangedEventArgs(Of String).Key
                Get
                    Return _key
                End Get
            End Property

        End Class

        Public Event MapChanged(sender As IObservableMap(Of String, Object), [event] As IMapChangedEventArgs(Of String)) Implements IObservableMap(Of String, Object).MapChanged
        Private _dictionary As New Dictionary(Of String, Object)()

        Private Sub InvokeMapChanged(change As CollectionChange, key As String)
            RaiseEvent MapChanged(Me, New ObservableDictionaryChangedEventArgs(change, key))
        End Sub

        Public Sub Add(key As String, value As Object) Implements IDictionary(Of String, Object).Add
            Me._dictionary.Add(key, value)
            Me.InvokeMapChanged(CollectionChange.ItemInserted, key)
        End Sub

        Public Sub AddKeyValuePair(item As KeyValuePair(Of String, Object)) Implements ICollection(Of KeyValuePair(Of String, Object)).Add
            Me.Add(item.Key, item.Value)
        End Sub

        Public Function Remove(key As String) As Boolean Implements IDictionary(Of String, Object).Remove
            If Me._dictionary.Remove(key) Then
                Me.InvokeMapChanged(CollectionChange.ItemRemoved, key)
                Return True
            End If
            Return False
        End Function

        Public Function RemoveKeyValuePair(item As KeyValuePair(Of String, Object)) As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).Remove
            Dim currentValue As Object = Nothing
            If Me._dictionary.TryGetValue(item.Key, currentValue) AndAlso
                Object.Equals(item.Value, currentValue) AndAlso Me._dictionary.Remove(item.Key) Then

                Me.InvokeMapChanged(CollectionChange.ItemRemoved, item.Key)
                Return True
            End If
            Return False
        End Function

        Default Public Property Item(key As String) As Object Implements IDictionary(Of String, Object).Item
            Get
                Return Me._dictionary(key)
            End Get
            Set(value As Object)
                Me._dictionary(key) = value
                Me.InvokeMapChanged(CollectionChange.ItemChanged, key)
            End Set
        End Property

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, Object)).Clear
            Dim priorKeys As String() = Me._dictionary.Keys.ToArray()
            Me._dictionary.Clear()
            For Each key As String In priorKeys
                Me.InvokeMapChanged(CollectionChange.ItemRemoved, key)
            Next
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, Object)) As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).Contains
            Return Me._dictionary.Contains(item)
        End Function

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, Object)).Count
            Get
                Return Me._dictionary.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IDictionary(Of String, Object).ContainsKey
            Return Me._dictionary.ContainsKey(key)
        End Function

        Public ReadOnly Property Keys As ICollection(Of String) Implements IDictionary(Of String, Object).Keys
            Get
                Return Me._dictionary.Keys
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As Object) As Boolean Implements IDictionary(Of String, Object).TryGetValue
            Return Me._dictionary.TryGetValue(key, value)
        End Function

        Public ReadOnly Property Values As ICollection(Of Object) Implements IDictionary(Of String, Object).Values
            Get
                Return Me._dictionary.Values
            End Get
        End Property

        Public Function GetGenericEnumerator() As IEnumerator(Of KeyValuePair(Of String, Object)) Implements IEnumerable(Of KeyValuePair(Of String, Object)).GetEnumerator
            Return Me._dictionary.GetEnumerator()
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me._dictionary.GetEnumerator()
        End Function

        Public Sub CopyTo(array() As KeyValuePair(Of String, Object), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, Object)).CopyTo
            Dim arraySize As Integer = array.Length
            For Each pair As KeyValuePair(Of String, Object) In Me._dictionary
                If arrayIndex >= arraySize Then Exit For
                array(arrayIndex) = pair
                arrayIndex += 1
            Next
        End Sub

    End Class

End Namespace
