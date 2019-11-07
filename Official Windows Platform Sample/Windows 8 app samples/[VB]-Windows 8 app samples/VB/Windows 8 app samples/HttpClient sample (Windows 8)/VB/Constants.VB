'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Generic
Imports System
Imports Microsoft.Samples.Networking.HttpClientSample

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage
        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "HttpClient Sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Public Shared _ScenarioList As New List(Of Scenario)() From {New Scenario() With {.Title = "GET Text", .ClassType = GetType(Scenario1)},
                                                           New Scenario() With {.Title = "GET Stream", .ClassType = GetType(Scenario2)},
                                                           New Scenario() With {.Title = "GET List", .ClassType = GetType(Scenario3)},
                                                           New Scenario() With {.Title = "POST Text", .ClassType = GetType(Scenario4)},
                                                           New Scenario() With {.Title = "POST Stream", .ClassType = GetType(Scenario5)},
                                                           New Scenario() With {.Title = "POST Multipart", .ClassType = GetType(Scenario6)}}
    End Class

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
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


