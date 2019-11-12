' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Windows.Foundation
Imports Windows.Graphics.Display
Imports Windows.System
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Documents

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Page

#Region "Properties"
        Private _scenariosFrame As Frame
        Public Property ScenariosFrame As Frame
            Get
                Return _scenariosFrame
            End Get
            Set(value As Frame)
                _scenariosFrame = value
            End Set
        End Property

        Private _inputFrame As Frame
        Public Property InputFrame As Frame
            Get
                Return _inputFrame
            End Get
            Set(value As Frame)
                _inputFrame = value
            End Set
        End Property

        Private _outputFrame As Frame
        Public Property OutputFrame As Frame
            Get
                Return _outputFrame
            End Get
            Set(value As Frame)
                _outputFrame = value
            End Set
        End Property

        Private _rootNamespace As String
        Public Property RootNamespace As String
            Get
                Return _rootNamespace
            End Get
            Set(value As String)
                _rootNamespace = value
            End Set
        End Property
#End Region

#Region "Events"
        Public Event InputFrameLoaded As System.EventHandler
        Public Event OutputFrameLoaded As System.EventHandler
#End Region

        Public Sub New()
            InitializeComponent()

            _scenariosFrame = ScenarioList
            _inputFrame = ScenarioInput
            _outputFrame = ScenarioOutput

            SetFeatureName(FEATURE_NAME)

            AddHandler Loaded, AddressOf MainPage_Loaded
            AddHandler Window.Current.SizeChanged, AddressOf Window_SizeChanged
            AddHandler DisplayProperties.LogicalDpiChanged, AddressOf DisplayProperties_LogicalDpiChanged

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required
        End Sub

        Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs)
            ' Figure out what resolution and orientation we are in and respond appropriately
            CheckResolutionAndViewState()

            ' Load the ScenarioList page into the proper frame
            ScenarioList.Navigate(Type.GetType(_rootNamespace & ".ScenarioList"), Me)
        End Sub

#Region "Resolution and orientation code"
        Private Sub DisplayProperties_LogicalDpiChanged(sender As Object)
            CheckResolutionAndViewState()
        End Sub

        Private Sub CheckResolutionAndViewState()
            VisualStateManager.GoToState(Me, ApplicationView.Value.ToString & DisplayProperties.ResolutionScale.ToString, False)
        End Sub

        Private Sub Window_SizeChanged(sender As Object, e As Windows.UI.Core.WindowSizeChangedEventArgs)
            CheckResolutionAndViewState()
        End Sub
#End Region

        Private Sub SetFeatureName(str As String)
            FeatureName.Text = str
        End Sub

        Private Async Sub Footer_Click(sender As Object, e As RoutedEventArgs)
            Await Windows.System.Launcher.LaunchUriAsync(New Uri(DirectCast(sender, HyperlinkButton).Tag.ToString))
        End Sub

        Public Sub NotifyUser(strMessage As String, type As NotifyType)
            Select Case type
                ' Use the status message style.
                Case NotifyType.StatusMessage
                    StatusBlock.Style = TryCast(Resources("StatusStyle"), Style)
                    Exit Select
                    ' Use the error message style.
                Case NotifyType.ErrorMessage
                    StatusBlock.Style = TryCast(Resources("ErrorStyle"), Style)
                    Exit Select
            End Select
            StatusBlock.Text = strMessage

            If StatusBlock.Text = String.Empty Then
                StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Else
                StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Visible
            End If
        End Sub

        Public Sub DoNavigation(inPageType As Type, inFrame As Frame, outPageType As Type, outFrame As Frame)
            inFrame.Navigate(inPageType, Me)
            outFrame.Navigate(outPageType, Me)

            ' Raise InputFrameLoaded so downstream pages know that the input frame content has been loaded.
            RaiseEvent InputFrameLoaded(Me, New EventArgs)
            ' Raise OutputFrameLoaded so downstream pages know that the output frame content has been loaded.
            RaiseEvent OutputFrameLoaded(Me, New EventArgs)
        End Sub
    End Class

    Public Enum NotifyType
        StatusMessage
        ErrorMessage
    End Enum
End Namespace