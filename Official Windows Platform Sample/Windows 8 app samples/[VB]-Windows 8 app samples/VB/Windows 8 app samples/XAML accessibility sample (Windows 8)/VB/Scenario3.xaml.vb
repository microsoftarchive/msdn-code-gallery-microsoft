'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.UI.Xaml.Automation
Imports Windows.UI.Xaml.Automation.Peers
Imports Windows.UI.Xaml.Automation.Provider
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Dim [me] As New MediaElementContainer(Scenario3Output)
        AutomationProperties.SetName([me], "CustomMediaElement")
    End Sub


    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

#Region "MediaElementContainer example code"
    Public Class MediaElementContainer
        Inherits ContentControl
        Const mediaUri As String = "Media/video.wmv"
        Private mediaElement As MediaElement
        Public Sub New(parent As Panel)
            parent.Children.Add(Me)
            mediaElement = New MediaElement() With {.Width = 200}

            Me.Content = mediaElement
            mediaElement.Source = New Uri(parent.BaseUri, mediaUri)
            Me.IsTabStop = True
            AutomationProperties.SetAutomationId(mediaElement, "MediaElement1")
            AutomationProperties.SetName(mediaElement, "MediaElement")
        End Sub


        Protected Overrides Function OnCreateAutomationPeer() As AutomationPeer
            Return New MediaContainerAP(Me, mediaElement)
        End Function
    End Class

    Public Class MediaContainerAP
        Inherits FrameworkElementAutomationPeer
        Implements IRangeValueProvider
        Implements IToggleProvider

        Private _mediaElement As MediaElement
        Private _labeledBy As FrameworkElement

        Public Sub New(owner As MediaElementContainer, mediaElement As MediaElement)
            MyBase.New(owner)
            _mediaElement = MediaElement
        End Sub

        Public Sub New(owner As MediaElementContainer, mediaElement As MediaElement, labeledBy As FrameworkElement)
            Me.New(owner, mediaElement)
            _labeledBy = labeledBy
        End Sub

        Protected Overrides Function GetPatternCore(patternInterface__1 As PatternInterface) As Object
            If patternInterface__1 = PatternInterface.RangeValue Then
                Return Me
            ElseIf patternInterface__1 = PatternInterface.Toggle Then
                Return Me
            End If
            Return Nothing
        End Function


        Protected Overrides Function GetAutomationControlTypeCore() As AutomationControlType
            Return AutomationControlType.Group
        End Function
        
        Protected Overrides Function GetLocalizedControlTypeCore() as String
            Return "Video"
        End Function
        
        Protected Overrides Function GetClassNameCore() As String
            Return "MediaElementContainer"
        End Function

        Public ReadOnly Property IsReadOnly() As Boolean Implements IRangeValueProvider.IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property LargeChange() As Double Implements IRangeValueProvider.LargeChange
            Get
                Return DirectCast(_mediaElement, MediaElement).NaturalDuration.TimeSpan.TotalSeconds / 10
            End Get
        End Property

        Public ReadOnly Property Maximum() As Double Implements IRangeValueProvider.Maximum
            Get
                Return DirectCast(_mediaElement, MediaElement).NaturalDuration.TimeSpan.TotalSeconds
            End Get
        End Property

        Public ReadOnly Property Minimum() As Double Implements IRangeValueProvider.Minimum
            Get
                Return 0.0
            End Get
        End Property

        Public Shadows Sub SetValue(value As Double) Implements IRangeValueProvider.SetValue
            If value > Maximum OrElse value < 0 Then
                Throw New ArgumentException("Seeking to a point that is out of range", "value")
            End If
            DirectCast(_mediaElement, MediaElement).Position = TimeSpan.FromSeconds(value)
        End Sub

        Public ReadOnly Property SmallChange() As Double Implements IRangeValueProvider.SmallChange
            Get
                Return DirectCast(_mediaElement, MediaElement).NaturalDuration.TimeSpan.TotalSeconds / 50
            End Get
        End Property

        Public ReadOnly Property Value() As Double Implements IRangeValueProvider.Value
            Get
                Return DirectCast(_mediaElement, MediaElement).Position.TotalSeconds
            End Get
        End Property

        Public Sub Toggle() Implements IToggleProvider.Toggle
            If DirectCast(_mediaElement, MediaElement).CurrentState = MediaElementState.Playing Then
                If DirectCast(_mediaElement, MediaElement).CanPause Then
                    DirectCast(_mediaElement, MediaElement).Pause()
                Else
                    DirectCast(_mediaElement, MediaElement).Stop()
                End If
            ElseIf DirectCast(_mediaElement, MediaElement).CurrentState = MediaElementState.Paused OrElse DirectCast(_mediaElement, MediaElement).CurrentState = MediaElementState.Stopped Then
                DirectCast(_mediaElement, MediaElement).Play()
            End If
        End Sub

        Public ReadOnly Property ToggleState() As Windows.UI.Xaml.Automation.ToggleState Implements IToggleProvider.ToggleState
            Get
                If DirectCast(_mediaElement, MediaElement).CurrentState = MediaElementState.Playing Then
                    Return Windows.UI.Xaml.Automation.ToggleState.On
                ElseIf DirectCast(_mediaElement, MediaElement).CurrentState = MediaElementState.Stopped OrElse DirectCast(_mediaElement, MediaElement).CurrentState = MediaElementState.Paused Then
                    Return Windows.UI.Xaml.Automation.ToggleState.Off
                Else
                    Return Windows.UI.Xaml.Automation.ToggleState.Indeterminate
                End If
            End Get
        End Property

    End Class
#End Region

End Class
