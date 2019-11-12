' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' This class is used to store a counter, as well as define a
''' ToString() method that can be used by a ComboBox to display the
''' instance and name of the counter.
''' </summary>
''' <remarks></remarks>
Public Class CounterDisplayItem
    ' Store an instance of the counter inside the class.
    Private counterValue As PerformanceCounter

    ' Define a constructor, that requires that the PerformanceCounter
    ' be passed.  Store the passed counter.
    Public Sub New(ByVal inCounter As PerformanceCounter)
        ' Only store the passed value if it is, indeed, a counter.
        If TypeName(inCounter) = "PerformanceCounter" Then
            counterValue = inCounter
        Else
            counterValue = Nothing
        End If
    End Sub

    ' This property gets or sets the PerformanceCounter stored by 
    ' a CounterDisplayItem object.
    Public Property Counter() As PerformanceCounter
        Get
            Return counterValue
        End Get
        Set(ByVal Value As PerformanceCounter)
            counterValue = Value
        End Set
    End Property

    ' This function overrides the ToString() method to display the
    ' information about the Counter that will be necessary for the 
    ' user to select the proper counter.
    Public Overrides Function ToString() As String
        If counterValue IsNot Nothing Then
            Return counterValue.InstanceName + " - " + counterValue.CounterName
        Else
            Return ""
        End If
    End Function

    ' This property returns a True if the counter is a custom counter, and 
    ' a false otherwise. Since there is no IsCustom property of a 
    ' PerformanceCounter object, a special bit of code is used. This code
    ' attempts to set the ReadOnly property to False, then read a 
    ' value from the counter. This will raise an exception if the counter
    ' is NOT a custom counter, otherwise it will not.
    Public ReadOnly Property IsCustom() As Boolean
        Get
            ' Store the current value of the ReadOnly property
            Dim isReadOnly As Boolean = counterValue.ReadOnly
            Try
                ' The only way NextValue works when ReadOnly
                ' is False, is if the Counter is a Custom counter.
                ' Unfortunately, there is no property in the 
                ' PerformanceCounter object that already returns whether
                ' the counter is Custom.
                counterValue.ReadOnly = False
                counterValue.NextValue()
                ' If it makes it here, it is a custom counter.
                Return True
            Catch exc As Exception
                ' This is not a custom counter.
                Return False
            Finally
                ' Reset the value to the previous value.
                counterValue.ReadOnly = isReadOnly
            End Try
        End Get
    End Property
End Class
