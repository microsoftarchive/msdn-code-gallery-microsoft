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
        Inherits Global.SDKTemplate.Common.LayoutAwarePage
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Provisioning Agent Sample"

        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Provision for mobile network operators", .ClassType = GetType(ProvisioningAgent.ProvisionMno)},
                                                           New Scenario With {.Title = "Update profile cost", .ClassType = GetType(ProvisioningAgent.UpdateCost)},
                                                           New Scenario With {.Title = "Update profile data usage", .ClassType = GetType(ProvisioningAgent.UpdateUsage)},
                                                           New Scenario With {.Title = "Provision for other operators", .ClassType = GetType(ProvisioningAgent.ProvisionOtherOperator)}
                                                          }
    End Class

    Public Class Scenario
        Private m_Title As String
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property


        Private m_ClassType As Type
        Public Property ClassType As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

End Namespace

