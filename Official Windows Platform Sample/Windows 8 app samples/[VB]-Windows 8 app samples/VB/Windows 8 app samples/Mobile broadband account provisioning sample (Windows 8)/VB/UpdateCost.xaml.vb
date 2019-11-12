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
Imports Windows.Networking
Imports Windows.Networking.Connectivity
Imports Windows.Networking.NetworkOperators
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class UpdateCost
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current
    Private util As Util
    Private profileMediaType As ProfileMediaType
    Private networkCostType As NetworkCostType

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
        AddHandler mediaType.SelectionChanged, AddressOf MediaType_SelectionChanged
        mediaType.SelectedItem = mediaType_Wlan

        AddHandler networkCostCategory.SelectionChanged, AddressOf NetworkCostCategory_SelectionChanged
        networkCostCategory.SelectedItem = cost_unknown
    End Sub

    Private Sub MediaType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If mediaType.SelectedItem Is mediaType_Wlan Then
            profileMediaType = profileMediaType.Wlan
        ElseIf mediaType.SelectedItem Is mediaType_Wwan Then
            profileMediaType = profileMediaType.Wwan
        End If
    End Sub

    Private Sub NetworkCostCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If networkCostCategory.SelectedItem Is cost_unknown Then
            networkCostType = networkCostType.Unknown
        ElseIf networkCostCategory.SelectedItem Is cost_unrestricted Then
            networkCostType = networkCostType.Unrestricted
        ElseIf networkCostCategory.SelectedItem Is cost_fixed Then
            networkCostType = networkCostType.Fixed
        ElseIf networkCostCategory.SelectedItem Is cost_variable Then
            networkCostType = networkCostType.Variable
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'UpdateCostButton' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UpdateCost_Click(sender As Object, e As RoutedEventArgs)
        If profileNameText.Text = "" Then
            rootPage.NotifyUser("Profile name cannot be empty", NotifyType.ErrorMessage)
            Exit Sub
        End If

        Dim profileName As String = profileNameText.Text

        Try
            ' Get the network account ID.
            Dim networkAccIds As IReadOnlyList(Of String) = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds

            If networkAccIds.Count = 0 Then
                rootPage.NotifyUser("No network account ID found", NotifyType.ErrorMessage)
                Exit Sub
            End If

            UpdateCostButton.IsEnabled = False

            ' For the sake of simplicity, assume we want to use the first account.
            ' Refer to the MobileBroadbandAccount API's how to select a specific account ID.
            Dim networkAccountId As String = networkAccIds(0)

            ' Create provisioning agent for specified network account ID
            Dim provisioningAgent As Windows.Networking.NetworkOperators.ProvisioningAgent = Windows.Networking.NetworkOperators.ProvisioningAgent.CreateFromNetworkAccountId(networkAccountId)

            ' Retrieve associated provisioned profile
            Dim provisionedProfile As ProvisionedProfile = provisioningAgent.GetProvisionedProfile(profileMediaType, profileName)

            ' Set the new cost
            provisionedProfile.UpdateCost(networkCostType)

            rootPage.NotifyUser("Profile " & profileName & " has been updated with the cost type as " & networkCostType.ToString, NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occured: " & ex.ToString, NotifyType.ErrorMessage)
        End Try

        UpdateCostButton.IsEnabled = True
    End Sub
End Class
