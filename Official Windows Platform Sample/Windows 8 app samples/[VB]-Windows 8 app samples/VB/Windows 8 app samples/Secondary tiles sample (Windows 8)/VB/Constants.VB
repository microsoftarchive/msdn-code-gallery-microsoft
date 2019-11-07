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

        Public Const FEATURE_NAME As String = "Secondary Tile Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Pin Tile", .ClassType = GetType(SecondaryTiles.PinTile)},
                                                                  New Scenario With {.Title = "Unpin Tile", .ClassType = GetType(SecondaryTiles.UnpinTile)},
                                                                  New Scenario With {.Title = "Enumerate Tiles", .ClassType = GetType(SecondaryTiles.EnumerateTiles)},
                                                                  New Scenario With {.Title = "Is Tile Pinned?", .ClassType = GetType(SecondaryTiles.TilePinned)},
                                                                  New Scenario With {.Title = "Show Activation Arguments", .ClassType = GetType(SecondaryTiles.LaunchedFromSecondaryTile)},
                                                                  New Scenario With {.Title = "Secondary Tile Notifications", .ClassType = GetType(SecondaryTiles.SecondaryTileNotification)},
                                                                  New Scenario With {.Title = "Pin/Unpin Through Appbar", .ClassType = GetType(SecondaryTiles.PinFromAppbar)},
                                                                  New Scenario With {.Title = "Update Secondary Tile Default Logo", .ClassType = GetType(SecondaryTiles.UpdateAsync)}
                                                                 }




#Region "Secondary Tile specific "

        Public Const logoSecondaryTileId As String = "SecondaryTile.Logo"
        Public Const dynamicTileId As String = "SecondaryTile.LiveTile"
        Public Const appbarTileId As String = "SecondaryTile.AppBar"


        ' Gets the rectangle of the element
        Public Shared Function GetElementRect(element As FrameworkElement) As Rect
            Dim buttonTransform As GeneralTransform = element.TransformToVisual(Nothing)
            Dim point As Point = buttonTransform.TransformPoint(New Point())
            Return New Rect(point, New Size(element.ActualWidth, element.ActualHeight))
        End Function

        ' Navigates to the Scenario "Show Activation Arguments"
        Public Sub NavigateToLaunchedFromSecondaryTile()
            Dim index As Integer = -1
            ' Populate the ListBox with the list of scenarios as defined in Constants.cs.
            For Each s As Scenario In _ScenarioList
                index += 1
                If s.ClassType Is GetType(SecondaryTiles.LaunchedFromSecondaryTile) Then
                    Exit For
                End If
            Next

            SuspensionManager.SessionState("SelectedScenario") = index
            Scenarios.SelectedIndex = index
            LoadScenario(_ScenarioList(index).ClassType)
            InvalidateSize()
        End Sub

#End Region

        Public LaunchArgs As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs
        
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

