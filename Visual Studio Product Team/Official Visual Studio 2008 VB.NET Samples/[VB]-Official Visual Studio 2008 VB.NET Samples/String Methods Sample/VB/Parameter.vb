' Copyright (c) Microsoft Corporation. All rights reserved.
Public Enum ParameterTypes
    StringParameter
    IntegerParameter
End Enum

Public Class Parameter

    Private nameValue As String
    Private defaultValue As Object
    Private typeValue As ParameterTypes
    Private valueValue As Object

    Public Sub New(ByVal newName As String, ByVal newDefault As Object, ByVal newType As ParameterTypes)
        nameValue = newName
        Me.SetDefaultAndType(newDefault, newType)
        Me.Value = newDefault
    End Sub

    Public Property Name() As String
        Get
            Return nameValue
        End Get
        Set(ByVal Value As String)
            nameValue = Value
        End Set
    End Property

    Public ReadOnly Property ParameterType() As ParameterTypes
        Get
            Return typeValue
        End Get
    End Property

    Public Property Value() As Object
        Get
            Return valueValue
        End Get
        Set(ByVal Value As Object)
            ' If an integer, convert object to integer.
            If Me.ParameterType = ParameterTypes.IntegerParameter Then
                Try
                    valueValue = CInt(Value)
                Catch ex As Exception
                    Throw New Exception("Value does not match type of the parameter.")
                End Try
            Else
                valueValue = CStr(Value)
            End If
        End Set
    End Property


    Public Property ParameterDefault() As Object
        Get
            Return defaultValue
        End Get
        Set(ByVal Value As Object)
            If (Me.ParameterType = ParameterTypes.IntegerParameter) And (Value.GetType Is GetType(Integer)) Then
                defaultValue = Value
            ElseIf (Me.ParameterType = ParameterTypes.StringParameter) And (Value.GetType Is GetType(String)) Then
                defaultValue = Value
            Else
                Throw New Exception("Default value does not match default type.")
            End If
        End Set
    End Property

    Public Sub SetDefaultAndType(ByVal newDefault As Object, ByVal newType As ParameterTypes)
        If (newType = ParameterTypes.IntegerParameter) And (newDefault.GetType Is GetType(Integer)) Then
            defaultValue = newDefault
            typeValue = newType
        ElseIf (newType = ParameterTypes.StringParameter) And (newDefault.GetType Is GetType(String)) Then
            defaultValue = newDefault
            typeValue = newType
        Else
            Throw New Exception("Default value does not match default type.")
        End If
    End Sub

    Public Overrides Function ToString() As String
        If ParameterType = ParameterTypes.StringParameter Then
            Return ("""" & Value.ToString & """")
        Else
            Return (Value.ToString)
        End If

    End Function

End Class
