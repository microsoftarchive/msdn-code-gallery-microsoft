
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
Imports AccountPictureName

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Account Picture Name Sample "

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Get user's display name", .ClassType = GetType(Global.AccountPictureName.GetUserDisplayName)},
                                                                   New Scenario With {.Title = "Get user's first and last name", .ClassType = GetType(Global.AccountPictureName.GetUserFirstLastName)},
                                                                    New Scenario With {.Title = "Get user's Account Picture", .ClassType = GetType(Global.AccountPictureName.GetAccountPicture)},
                                                                    New Scenario With {.Title = "Set user's Account Picture and Listen for changes", .ClassType = GetType(Global.AccountPictureName.SetAccountPictureAndListen)}
                                                                  }


        ' Navigates to the Scenario "Set Account Picture and listen"
        Public Sub NavigateToSetAccountPictureAndListen()
            Dim index As Integer = -1
            ' Populate the ListBox with the list of scenarios as defined in Constants.cs.
            For Each s As Scenario In _ScenarioList
                index += 1
                If s.ClassType Is GetType(SetAccountPictureAndListen) Then
                    Exit For
                End If
            Next
            SuspensionManager.SessionState("SelectedScenario") = index
            Scenarios.SelectedIndex = index
            LoadScenario(_ScenarioList(index).ClassType)
            InvalidateSize()
        End Sub
    End Class


    Partial Public Class App
        Protected Overrides Sub OnActivated(args As IActivatedEventArgs)
            ' Check to see if the app was activated via a protocol
            If args.Kind = ActivationKind.Protocol Then
                Dim protocolArgs = DirectCast(args, ProtocolActivatedEventArgs)

                ' This app was activated via the Account picture apps section in PC Settings / Personalize / Account picture. 
                ' Here you would do app-specific logic so that the user receives account picture selection UX.
                If protocolArgs.Uri.Scheme = "ms-accountpictureprovider" Then

                    ' The Content might be null if App has not yet been activated, if so first activate the main page.
                    If Window.Current.Content Is Nothing Then
                        ConstructMainPage()
                    End If
                    ' The scenario is set to 4 (Set Account Picture) explicitly if Content has already been loaded
                    MainPage.Current.NavigateToSetAccountPictureAndListen()
                End If
            End If
        End Sub

        Private Sub ConstructMainPage()
            Dim rootFrame As New Frame()

            If Window.Current.Content Is Nothing Then
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            End If
            Window.Current.Activate()
        End Sub
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
