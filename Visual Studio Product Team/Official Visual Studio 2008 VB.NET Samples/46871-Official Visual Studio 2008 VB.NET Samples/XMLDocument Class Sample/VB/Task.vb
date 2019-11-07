' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Task

    ' Delegate that points to the implementation of the specified task
    Public Delegate Function ExecuteTask() As String

    Private methodValue As ExecuteTask
    Private nameValue As String

    Public Sub New(ByVal newName As String, ByVal newMethod As ExecuteTask)
        Name = newName
        Method = newMethod
    End Sub

    Public Overrides Function ToString() As String
        Return Name
    End Function

    Public Property Method() As ExecuteTask
        Get
            Return methodValue
        End Get
        Set(ByVal Value As ExecuteTask)
            methodValue = Value
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

End Class
