' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Page

        Private _scenariosFrame As Frame

        Public Property ScenariosFrame() As Frame
            Get
                Return _scenariosFrame
            End Get
            Set(ByVal value As Frame)
                _scenariosFrame = value
            End Set
        End Property

        Private _inputFrame As Frame

        Public Property InputFrame() As Frame
            Get
                Return _inputFrame
            End Get
            Set(ByVal value As Frame)
                _inputFrame = value
            End Set
        End Property

        Private _outputFrame As Frame

        Public Property OutputFrame() As Frame
            Get
                Return _outputFrame
            End Get
            Set(ByVal value As Frame)
                _outputFrame = value
            End Set
        End Property

        Private _rootNamespace As String

        Public Property RootNamespace() As String
            Get
                Return _rootNamespace
            End Get
            Set(ByVal value As String)
                _rootNamespace = value
            End Set
        End Property




        Public Event InputFrameLoaded As System.EventHandler
        Public Event OutputFrameLoaded As System.EventHandler


        Public Sub New()
            InitializeComponent()

            _scenariosFrame = ScenarioList
            _inputFrame = ScenarioInput
            _outputFrame = ScenarioOutput

            SetFeatureName(FEATURE_NAME)

            AddHandler Loaded, AddressOf MainPage_Loaded
            AddHandler Window.Current.SizeChanged, AddressOf MainPage_SizeChanged

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required
        End Sub

        Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' Figure out what resolution and orientation we are in and respond appropriately
            CheckLayout()

            ' Load the ScenarioList page into the proper frame
            ScenarioList.Navigate(Type.GetType(_rootNamespace & ".ScenarioList"), Me)
        End Sub




        Private Sub CheckLayout()
            Dim visualState As String = If(Window.Current.Bounds.Width < 768, "Below768Layout", "DefaultLayout")
            VisualStateManager.GoToState(Me, visualState, False)
        End Sub

        Private Sub MainPage_SizeChanged(ByVal sender As Object, ByVal args As Windows.UI.Core.WindowSizeChangedEventArgs)
            CheckLayout()
        End Sub


        Private Sub SetFeatureName(ByVal str As String)
            FeatureName.Text = str
        End Sub

        Private Async Sub Footer_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Await Windows.System.Launcher.LaunchUriAsync(New Uri(CType(sender, HyperlinkButton).Tag.ToString()))
        End Sub

        Public Sub NotifyUser(ByVal strMessage As String, ByVal type As NotifyType)
            Select Case type
                Case NotifyType.StatusMessage
                    StatusBlock.Style = TryCast(Resources("StatusStyle"), Style)
                Case NotifyType.ErrorMessage
                    StatusBlock.Style = TryCast(Resources("ErrorStyle"), Style)
            End Select
            StatusBlock.Text = strMessage
        End Sub

        Public Sub DoNavigation(ByVal inPageType As Type, ByVal inFrame As Frame, ByVal outPageType As Type, ByVal outFrame As Frame)
            inFrame.Navigate(inPageType, Me)
            outFrame.Navigate(outPageType, Me)

            ' Raise InputFrameLoaded so downstream pages know that the input frame content has been loaded.
            RaiseEvent InputFrameLoaded(Me, New EventArgs())
            ' Raise OutputFrameLoaded so downstream pages know that the output frame content has been loaded.
            RaiseEvent OutputFrameLoaded(Me, New EventArgs())
        End Sub
    End Class

    Public Enum NotifyType
        StatusMessage
        ErrorMessage
    End Enum
End Namespace

