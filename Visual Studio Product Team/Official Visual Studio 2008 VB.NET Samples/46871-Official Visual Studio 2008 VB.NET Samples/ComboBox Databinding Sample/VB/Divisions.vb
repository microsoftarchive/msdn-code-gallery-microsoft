' Define a structure for a sales division which has
' a division name and division id.
Public Structure Divisions
    Private divName As String
    Private divID As Integer

    Public Sub New(ByVal name As String, ByVal id As Integer)
        divName = name
        divID = id
    End Sub

    Public ReadOnly Property Name() As String
        Get
            Return divName
        End Get
    End Property

    Public ReadOnly Property Id() As Integer
        Get
            Return divID
        End Get
    End Property
End Structure
