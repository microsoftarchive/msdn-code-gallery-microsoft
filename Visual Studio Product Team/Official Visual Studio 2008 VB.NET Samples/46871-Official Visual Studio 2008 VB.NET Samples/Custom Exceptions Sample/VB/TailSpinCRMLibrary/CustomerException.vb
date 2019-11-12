''' <summary>
''' Base class for all exceptions defined in this project.  All CustomerExceptions include
''' a Customer property, which may be used to obtain additional information about the customer
''' object that the exception pertains to.
''' </summary>
''' <remarks></remarks>
Public Class CustomerException
    Inherits CRMSystemException

    Private appSourceValue As String
    Private customerValue As Customer

    Public Sub New(ByVal Message As String, ByVal ReqCustomer As Customer)
        MyBase.New(Message)
        Me.customerValue = ReqCustomer
        Me.appSourceValue = "Tailspin Toys CRM Customer Module"
    End Sub

    ''' <summary>
    ''' Property that exposes additional information about the customer object that caused the error
    ''' </summary>
    Public ReadOnly Property CustomerInfo() As Customer
        Get
            Return customerValue
        End Get
    End Property

    ' We wan't exceptions at this level to use
    ' our AppSource, not our parents which becomes
    ' important when someone calls LogError.
    ''' <summary>
    ''' 
    ''' </summary>
    Public Overrides ReadOnly Property AppSource() As String
        Get
            Return appSourceValue
        End Get
    End Property
End Class