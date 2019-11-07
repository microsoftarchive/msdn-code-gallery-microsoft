' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' Since the base type SystemValueType.Long does not inherit from System.Object we cannot cast 
''' from System.Object[] to Int64[] when calling ToArray() on the generic classes.  So therefore we'll create
''' our own long class so we can cast from an array of System.Object to an array of LongClass.
''' </summary>
''' <remarks></remarks>
Public Class LongClass
    Implements IComparable

    Public Sub New(ByVal l As Long)
        v = l
    End Sub

    Private v As Long

    Public Property Value() As Long
        Get
            Return v
        End Get
        Set(ByVal Value As Long)
            v = Value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return v.ToString()
    End Function

    ''' <summary>
    ''' The compare to function is the implementation of the IComarable interface.  
    ''' For the customer class we will only implement the comparison of the ID field.
    ''' This means that any sorting done on an array or collection of these objects 
    ''' will sort by the customer ID.
    ''' </summary>
    Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo

        ' First check to make sure that we are comparing this instance to another MyLong.
        If TypeOf obj Is LongClass Then
            ' Create a strongly typed instance of obj.
            Dim l As LongClass
            l = CType(obj, LongClass)

            If l.Value = Me.Value Then
                Return 0
            ElseIf l.Value < Me.Value Then
                Return 1
            Else
                Return -1
            End If
        Else
            Throw New ArgumentException("LongClass instances can only be compared to other LongClass instances.")
        End If
    End Function

End Class
