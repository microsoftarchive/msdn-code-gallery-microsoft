' Copyright (c) Microsoft Corporation. All rights reserved.
' This class is marked as Serializable and it implements
' ISerializable, which allows for custom serialization. ISerializable requires
' implementation of the GetObjectData method, as well as an additional constructor
' that will be called on deserialization.
Imports System.Runtime.Serialization

' This attribute makes the class serializable.
<Serializable()> Public Class CustomSerializableClass
    Implements ISerializable

    ' Because custom serialization is being used in this example, the NonSerialized
    ' attribute is not enforced. Instead, the writer of the class determines what is
    ' and isn't serialized, based on the GetObjectData method below. Note that in this
    ' class, even though NonSerializedVariable is marked as NonSerialized, 
    ' the field is serialized anyway.

    Public PublicVariable As Integer
    Private privateVariable As Integer
    <NonSerialized()> Public NonSerializedVariable As Integer


    ' Simple constructor to load in values for the fields.
    Public Sub New(ByVal argx As Integer, ByVal argy As Integer, ByVal argz As Integer)
        Me.PublicVariable = argx
        Me.privateVariable = argy
        Me.NonSerializedVariable = argz
    End Sub

    ''' <summary>
    ''' This is the special constructor called during 
    ''' deserialization. Note that both fields and custom
    ''' information (like a time stamp) can be serialized.
    ''' </summary>
    Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        Me.PublicVariable = info.GetInt32("PublicVariable")
        Me.privateVariable = info.GetInt32("privateVariable")
        Me.NonSerializedVariable = info.GetInt32("NonSerializedVariable")
        Dim d As Date = info.GetDateTime("TimeStamp")
    End Sub

    Public ReadOnly Property PublicProperty() As Integer
        Get
            Return privateVariable
        End Get
    End Property

    Public Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext) Implements ISerializable.GetObjectData
        With info
            .AddValue("PublicVariable", Me.PublicVariable)
            .AddValue("privateVariable", -1)
            .AddValue("NonSerializedVariable", Me.NonSerializedVariable)
            .AddValue("TimeStamp", Date.Now)
        End With
    End Sub
End Class