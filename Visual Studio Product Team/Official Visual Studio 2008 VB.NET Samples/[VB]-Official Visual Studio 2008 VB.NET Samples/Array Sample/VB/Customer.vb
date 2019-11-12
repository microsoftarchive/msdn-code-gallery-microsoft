' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Customer
    Implements IComparable

    ' Used to control which field is used for comparisons.
    Private Shared s_CompareField As CompareField

    Private idValue As Integer
    Private nameValue As String

    Public Property Id() As Integer
        Get
            Return idValue
        End Get
        Set(ByVal Value As Integer)
            idValue = Value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return nameValue
        End Get
        Set(ByVal Value As String)
            nameValue = Value
        End Set
    End Property

    Public Enum CompareField
        Name
        Id
    End Enum

    Shared Sub New()
        ' Set the default compare field
        s_CompareField = CompareField.Name
    End Sub

    Public Shared Sub SetCompareKey(ByVal CompareKey As CompareField)
        ' Change the comparison field
        s_CompareField = CompareKey
    End Sub

    Public Sub New()
        ' Set default values by delegating to the next most complex constructor
        Me.New(-1, "No Name")
    End Sub

    Public Sub New(ByVal newId As Integer, ByVal newName As String)
        Me.Id = newId
        Me.Name = newName
    End Sub

    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
        ' First check to make sure we're only being compared to another customer.
        If TypeOf obj Is Customer Then
            ' Create a strongly typed reference
            Dim c As Customer = CType(obj, Customer)

            If s_CompareField = CompareField.Name Then
                ' Compare based on names
                Return Me.Name.CompareTo(c.Name)
            Else
                ' Compare based on ID
                Return Me.Id - c.Id
            End If
        Else
            Throw New ArgumentException("Customers can only be compared against other customers. The object being passed in was a " & obj.GetType.ToString())
        End If
    End Function

    Public Overrides Function ToString() As String
        ' Normally ToString returns the fully qualified typename.
        ' In this example it would be VBNET.HowTo.Arrays.Customer
        ' We are overriding it so that we can return a simple
        ' display string when we're added to a list box.
        Return String.Format("Id={0}, Name={1}", Me.Id, Me.Name)
    End Function
End Class
