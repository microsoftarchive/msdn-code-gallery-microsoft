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

Imports Windows.Devices.Enumeration
Imports Windows.Devices.Enumeration.Pnp

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Containers
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Async Sub EnumerateDeviceContainers(sender As Object, eventArgs As RoutedEventArgs)
        Dim focusState As FocusState = EnumerateContainersButton.FocusState

        EnumerateContainersButton.IsEnabled = False

        Dim properties As String() = {"System.ItemNameDisplay", "System.Devices.ModelName", "System.Devices.Connected"}
        Dim containers = Await PnpObject.FindAllAsync(PnpObjectType.DeviceContainer, properties)

        DeviceContainersOutputList.Items.Clear()
        rootPage.NotifyUser(containers.Count.ToString & " device container(s) found", NotifyType.StatusMessage)
        For Each container As PnpObject In containers
            DeviceContainersOutputList.Items.Add(New DisplayItem(container))
        Next

        EnumerateContainersButton.IsEnabled = True
        EnumerateContainersButton.Focus(focusState)
    End Sub

    Private Class DisplayItem
        Public Sub New(container As PnpObject)
            m_name = DirectCast(container.Properties("System.ItemNameDisplay"), String)
            If m_name Is Nothing OrElse m_name.Length = 0 Then
                m_name = "*Unnamed*"
            End If

            m_id = "Id: " & container.Id
            m_properties &= "Property store count is: " & +container.Properties.Count & vbLf
            For Each prop In container.Properties
                m_properties &= prop.Key & " := " & prop.Value & vbLf
            Next
        End Sub

        Public ReadOnly Property Id() As String
            Get
                Return m_id
            End Get
        End Property
        Public ReadOnly Property Name() As String
            Get
                Return m_name
            End Get
        End Property
        Public ReadOnly Property Properties() As String
            Get
                Return m_properties
            End Get
        End Property

        ReadOnly m_id As String
        ReadOnly m_name As String
        ReadOnly m_properties As String
    End Class

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
