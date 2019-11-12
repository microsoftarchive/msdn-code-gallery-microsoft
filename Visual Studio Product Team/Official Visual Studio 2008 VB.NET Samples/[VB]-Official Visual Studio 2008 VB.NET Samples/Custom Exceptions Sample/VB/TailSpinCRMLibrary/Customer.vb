' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Customer

    Public idValue As Integer
    Public firstNameValue As String
    Public lastNameValue As String


    Public Property ID() As Integer
        Get
            Return idValue
        End Get
        Set(ByVal value As Integer)
            idValue = value
        End Set
    End Property

    Public Property FirstName() As String
        Get
            Return firstNameValue
        End Get
        Set(ByVal value As String)
            firstNameValue = value
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return lastNameValue
        End Get
        Set(ByVal value As String)
            lastNameValue = value
        End Set
    End Property

    Public Shared Function EditCustomer(ByVal Id As Integer) As Customer
        ' Pretend we look for the customer in our database
        ' We can't find it, so we notify our caller in a way in 
        ' which they can't ignore us.

        Dim message As String
        message = String.Format("The customer you requested by Id {0} could not be found.", Id)

        ' Create the exception.
        Dim exp As New CustomerNotFoundException(message)
        ' Throw it to our caller.
        Throw exp

    End Function

    Public Shared Sub DeleteCustomer(ByVal Id As Integer)
        ' Pretend we find the customer in the database
        ' but realize we shouldn't delete them, maybe for
        ' security reasons.

        Dim c As New Customer()
        With c
            .Id = Id
            .FirstName = "Mary"
            .LastName = "Baker"
        End With

        ' This could fail if we don't have rights.
        Dim user As String
        Try
            user = System.Environment.UserDomainName & "\" & System.Environment.UserName
        Catch pexp As Exception
            user = "Unavailable"
        End Try

        Dim message As String
        message = String.Format("The customer you requested {0} {1} could not be deleted. Your account {2} does not have permission.", c.FirstName, c.LastName, user)

        Dim exp As New CustomerNotDeletedException(message, c, user)
        exp.LogError()

        Throw exp

    End Sub
End Class








