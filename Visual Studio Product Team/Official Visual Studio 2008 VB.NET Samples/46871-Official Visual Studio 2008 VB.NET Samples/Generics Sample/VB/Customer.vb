' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
'''The customer class represents a customer name and an ID number.  By implementing the IComparable
''' interface we can support sorting array's and collections of this class. 
''' </summary>
Public Class Customer
    Implements IComparable

    Private mName As String
    Private mId As Integer

    Public Sub New(ByVal new_name As String, ByVal new_id As Integer)
        mName = new_name
        mId = new_id
    End Sub


    Public Overrides Function ToString() As String
        Return mId.ToString() + ": " + mName
    End Function


    ''' <summary>
    '''The compare to function is the implementation of the IComarable interface.  For the customer
    ''' class we will only implement the comparison of the ID field.  This means that any sorting
    ''' done on an array or collection of these objects will sort by the customer ID.
    ''' </summary>
    Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo


        ' First check to make sure that we are comparing this instance to another customer.
        If TypeOf obj Is Customer Then
            ' Create a strongly typed instance of obj.
            Dim c As Customer
            c = CType(obj, Customer)

            If c.ID = Me.ID Then
                Return 0
            ElseIf c.ID < Me.ID Then
                Return 1
            Else
                Return -1
            End If
        Else
            Throw New ArgumentException("Customers can only be compared to other customers.")
        End If
    End Function

    ''' <summary>
    ''' The name of the customer
    ''' </summary>
    Public Property Name() As String
        Get
            Return mName
        End Get
        Set(ByVal Value As String)
            mName = Value
        End Set
    End Property

    ''' <summary>
    ''' The ID of the customer
    ''' </summary>
    Public Property ID() As Integer
        Get
            Return mId
        End Get
        Set(ByVal Value As Integer)
            mId = Value
        End Set
    End Property

End Class
