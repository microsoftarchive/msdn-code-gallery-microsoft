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

        Public Const FEATURE_NAME As String = "Tiles  Sample "

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Send tile notification with text", .ClassType = GetType(Global.Tiles.SendTextTile)},
                                                                  New Scenario With {.Title = "Send tile notification with local images", .ClassType = GetType(Global.Tiles.SendLocalImageTile)},
                                                                  New Scenario With {.Title = "Send tile notification with web images", .ClassType = GetType(Global.Tiles.SendWebImageTile)},
                                                                  New Scenario With {.Title = "Send badge notification", .ClassType = GetType(Global.Tiles.SendBadge)},
                                                                  New Scenario With {.Title = "Preview all tile notification templates", .ClassType = GetType(Global.Tiles.PreviewAllTemplates)},
                                                                  New Scenario With {.Title = "Enable notification queue and tags", .ClassType = GetType(Global.Tiles.EnableNotificationQueue)},
                                                                  New Scenario With {.Title = "Use notification expiration", .ClassType = GetType(Global.Tiles.NotificationExpiration)},
                                                                  New Scenario With {.Title = "Image protocols and baseUri", .ClassType = GetType(Global.Tiles.ImageProtocols)},
                                                                  New Scenario With {.Title = "Globalization, localization, scale, and accessibility", .ClassType = GetType(Global.Tiles.Globalization)}
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

