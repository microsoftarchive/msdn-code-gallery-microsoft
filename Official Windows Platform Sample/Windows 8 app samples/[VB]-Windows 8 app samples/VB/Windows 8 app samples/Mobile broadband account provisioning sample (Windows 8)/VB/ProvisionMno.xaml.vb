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
Imports Windows.Networking.NetworkOperators
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ProvisionMno
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current
    Private util As Util

    Public Sub New
        Me.InitializeComponent
        util = New Util
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'ProvisionMnoButton' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub ProvisionMno_Click(sender As Object, e As RoutedEventArgs)
        If provXmlText.Text = "" Then
            rootPage.NotifyUser("Provisioning XML cannot be empty", NotifyType.ErrorMessage)
            Return
        End If

        Dim provisioningXML As String = provXmlText.Text

        Try
            ' Get the network account ID.
            Dim networkAccIds As IReadOnlyList(Of String) = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds

            If networkAccIds.Count = 0 Then
                rootPage.NotifyUser("No network account ID found", NotifyType.ErrorMessage)
                Return
            End If

            ProvisionMnoButton.IsEnabled = False

            ' For the sake of simplicity, assume we want to use the first account.
            ' Refer to the MobileBroadbandAccount API's how to select a specific account ID.
            Dim networkAccountId As String = networkAccIds(0)

            ' Create provisioning agent for specified network account ID
            Dim provisioningAgent As Windows.Networking.NetworkOperators.ProvisioningAgent = Windows.Networking.NetworkOperators.ProvisioningAgent.CreateFromNetworkAccountId(networkAccountId)

            ' Provision using XML
            Dim result As ProvisionFromXmlDocumentResults = Await provisioningAgent.ProvisionFromXmlDocumentAsync(provisioningXML)

            If result.AllElementsProvisioned Then
                ' Provisioning is done successfully
                rootPage.NotifyUser("Device was successfully configured", NotifyType.StatusMessage)
            Else
                ' Error has occured during provisioning
                ' And hence displaying result XML containing
                ' errors
                rootPage.NotifyUser(util.ParseResultXML(result.ProvisionResultsXml), NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occured: " & ex.ToString, NotifyType.ErrorMessage)
        End Try

        ProvisionMnoButton.IsEnabled = True
    End Sub
End Class
