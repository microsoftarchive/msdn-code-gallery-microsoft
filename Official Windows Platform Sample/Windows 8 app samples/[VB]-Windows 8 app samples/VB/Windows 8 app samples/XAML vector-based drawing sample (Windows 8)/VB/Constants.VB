'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "XAML vector-based drawing sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Shapes", .ClassType = GetType(Global.Drawing.Scenario1)},
                                                                  New Scenario With {.Title = "Strokes and fills", .ClassType = GetType(Global.Drawing.Scenario2)},
                                                                  New Scenario With {.Title = "Gradient", .ClassType = GetType(Global.Drawing.Scenario3)},
                                                                  New Scenario With {.Title = "Clipping", .ClassType = GetType(Global.Drawing.Scenario4)},
                                                                  New Scenario With {.Title = "Paths", .ClassType = GetType(Global.Drawing.Scenario5)}
                                                                 }



        ' Converts between ComboBox indices and colors
        Public Function ConvertIndexToColor(index As Integer) As Color
            Select Case index
                Case 0
                    Return Colors.Red
                Case 1
                    Return Colors.Green
                Case 2
                    Return Colors.Blue
                Case 3
                    Return Colors.Gray
                Case Else
                    Return Colors.White
            End Select
        End Function


        Public Event MainPageResized()
        Public AutoSizeInputSectionWhenSnapped = True



    End Class

    Public Class Scenario
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
