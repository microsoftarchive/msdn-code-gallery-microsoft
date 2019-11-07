'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "DataBinding Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Binding modes", .ClassType = GetType(Global.DataBinding.Scenario1)},
                                                                   New Scenario With {.Title = "Value converters", .ClassType = GetType(Global.DataBinding.Scenario2)},
                                                                   New Scenario With {.Title = "Binding to a model", .ClassType = GetType(Global.DataBinding.Scenario3)},
                                                                   New Scenario With {.Title = "Indexers", .ClassType = GetType(Global.DataBinding.Scenario4)},
                                                                   New Scenario With {.Title = "Data templates", .ClassType = GetType(Global.DataBinding.Scenario5)},
                                                                   New Scenario With {.Title = "CollectionViewSource", .ClassType = GetType(Global.DataBinding.Scenario6)},
                                                                   New Scenario With {.Title = "Collection changes", .ClassType = GetType(Global.DataBinding.Scenario7)}
                                                                  }

        


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

