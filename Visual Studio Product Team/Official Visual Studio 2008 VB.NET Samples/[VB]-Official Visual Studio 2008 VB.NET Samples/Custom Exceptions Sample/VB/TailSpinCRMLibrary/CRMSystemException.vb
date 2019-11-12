''' <summary>
''' Base exception for all exceptions defined for the CRM System.
''' </summary>
''' <remarks></remarks>
Public Class CRMSystemException
    Inherits System.ApplicationException

    Private appSourceValue As String

    Public Sub New(ByVal Message As String)
        MyBase.New(Message)
        Me.appSourceValue = "Tailspin Toys CRM System"
    End Sub

    ''' <summary>
    ''' Log the error
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub LogError()
        My.Application.Log.WriteEntry(Me.Message)
    End Sub

    ''' <summary>
    ''' The source (company) for the exception
    ''' </summary>
    Public Overridable ReadOnly Property AppSource() As String
        Get
            Return appSourceValue
        End Get
    End Property
End Class