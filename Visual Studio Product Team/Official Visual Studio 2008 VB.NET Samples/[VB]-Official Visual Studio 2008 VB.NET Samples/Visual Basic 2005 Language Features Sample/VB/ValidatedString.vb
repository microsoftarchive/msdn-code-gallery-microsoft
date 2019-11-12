' Copyright (c) Microsoft Corporation. All rights reserved.

''' <summary>
''' Custom class to store string values and enable additional validation
''' </summary>
''' <remarks>Valid strings have length less than 256 characters</remarks>
Friend Class ValidatedString

    ' Internal variables
    Dim m_value As String = ""
    Dim m_isValid As Boolean = True

    ' Defines & operator that behaves like string & operator.
    Public Shared Operator &(ByVal s1 As ValidatedString, ByVal s2 As ValidatedString) As ValidatedString
        Dim concatString As String = s1.Value & s2.Value

        Return New ValidatedString(concatString)
    End Operator

    ' ToString will now return the Value property value.
    Public Overrides Function ToString() As String
        Return Me.Value.ToString
    End Function

    Public Sub New(ByVal value As String)
        Me.Value = value
    End Sub

    Public Property Value() As String
        Get
            Return Me.m_value
        End Get
        '** mark property setter as private since we want custom validation
        ' code to run, but we don't want to expose a public setter.
        Private Set(ByVal value As String)
            Me.m_value = value

            ' Perform validation and set flag after setting value.
            Me.m_isValid = Validate()
        End Set
    End Property

    ' Custom property that tests to see if value length is <= 255.
    Public ReadOnly Property IsValid() As Boolean
        Get
            Return Validate()
        End Get
    End Property

    Private Function Validate() As Boolean
        If Me.m_value.Length <= 255 Then
            Return True
        Else
            Return False
        End If
    End Function

End Class