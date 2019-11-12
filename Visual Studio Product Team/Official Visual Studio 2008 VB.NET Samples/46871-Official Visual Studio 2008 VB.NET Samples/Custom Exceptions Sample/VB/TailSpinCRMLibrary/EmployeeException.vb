''' <summary>
''' An example type that derives from Exception
''' </summary>
Public Class EmployeeException
    Inherits CRMSystemException
    Public Sub New(ByVal Message As String)
        MyBase.New(Message)
    End Sub
End Class