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
Imports System.Collections.Generic
Imports Windows.Networking.NetworkOperators
Imports Windows.Devices.Sms

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class BroadbandDevice
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private deviceAccountId As IReadOnlyList(Of String) = Nothing
    Private deviceSelected As Integer = 0

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        PrepScenario1Sample()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Get Device Information' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UpdateData_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            GetCurrentDeviceInfo(deviceAccountId(deviceSelected))
            ' increment device count until reach number max number of devices and then start over
            deviceSelected = (deviceSelected + 1) Mod deviceAccountId.Count

            ' update Button with next device
            UpdateData.Content = "Get Information for Device #" & (deviceSelected + 1)
        End If
    End Sub

    Private Sub PrepScenario1Sample()
        rootPage.NotifyUser("", NotifyType.StatusMessage)

        Try
            deviceSelected = 0
            deviceAccountId = MobileBroadbandAccount.AvailableNetworkAccountIds

            If deviceAccountId.Count <> 0 Then
                rootPage.NotifyUser("Mobile Broadband Device(s) have been installed that grant access to this application", NotifyType.StatusMessage)

                NumDevices.Text = "There are " & deviceAccountId.Count & " account(s) installed."
                UpdateData.Content = "Get Information for Device #1"
                UpdateData.IsEnabled = True
            Else
                UpdateData.Content = "No available accounts detected"
                UpdateData.IsEnabled = False
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Error:" & ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub GetCurrentDeviceInfo(accountId As String)
        Try
            Dim mobileBroadbandAccount__1 = MobileBroadbandAccount.CreateFromNetworkAccountId(accountId)

            ProviderName.Text = mobileBroadbandAccount__1.ServiceProviderName
            ProviderGuid.Text = mobileBroadbandAccount__1.ServiceProviderGuid.ToString
            NetworkAccountId.Text = mobileBroadbandAccount__1.NetworkAccountId

            Dim currentNetwork = mobileBroadbandAccount__1.CurrentNetwork

            If currentNetwork IsNot Nothing Then
                Dim accessPointName__2 As String = currentNetwork.AccessPointName
                If String.IsNullOrEmpty(accessPointName__2) Then
                    accessPointName__2 = "(not connected)"
                End If

                NetRegister.Text = currentNetwork.NetworkRegistrationState.ToString
                NetRegError.Text = NetErrorToString(currentNetwork.RegistrationNetworkError)
                PacketAttachError.Text = NetErrorToString(currentNetwork.PacketAttachNetworkError)
                ActivateError.Text = NetErrorToString(currentNetwork.ActivationNetworkError)
                AccessPointName.Text = accessPointName__2
                NetworkAdapterId.Text = currentNetwork.NetworkAdapter.NetworkAdapterId.ToString
                NetworkType.Text = currentNetwork.NetworkAdapter.NetworkItem.GetNetworkTypes().ToString
                RegisteredProviderId.Text = currentNetwork.RegisteredProviderId
                RegisteredProviderName.Text = currentNetwork.RegisteredProviderName

                RegisteredDataClass.Text = currentNetwork.RegisteredDataClass.ToString
            Else
                NetRegister.Text = ""
                NetRegError.Text = ""
                PacketAttachError.Text = ""
                ActivateError.Text = ""
                AccessPointName.Text = ""
                NetworkAdapterId.Text = ""
                NetworkType.Text = ""
                RegisteredProviderId.Text = ""
                RegisteredProviderName.Text = ""
                RegisteredDataClass.Text = ""
            End If


            Dim deviceInformation = mobileBroadbandAccount__1.CurrentDeviceInformation

            If deviceInformation IsNot Nothing Then
                Dim mobileNumber__3 As String = ""
                If deviceInformation.TelephoneNumbers.Count > 0 Then
                    mobileNumber__3 = deviceInformation.TelephoneNumbers(0)
                End If

                DeviceManufacturer.Text = deviceInformation.Manufacturer
                DeviceModel.Text = deviceInformation.Model
                Firmware.Text = deviceInformation.FirmwareInformation
                CellularClasses.Text = deviceInformation.CellularClass.ToString

                DataClasses.Text = deviceInformation.DataClasses.ToString
                'If deviceInformation.DataClasses.HasFlag(Windows.Networking.NetworkOperators.DataClasses.Custom) Then
                '    DataClasses.Text &= " (custom is " & deviceInformation.CustomDataClass & ")"
                'End If

                MobileNumber.Text = mobileNumber__3
                SimId.Text = deviceInformation.SimIccId
                DeviceType.Text = deviceInformation.DeviceType.ToString
                DeviceId.Text = deviceInformation.DeviceId.ToString
                NetworkDeviceStatus.Text = deviceInformation.NetworkDeviceStatus.ToString

                If deviceInformation.CellularClass = CellularClass.Gsm Then
                    MobEquipIdLabel.Text = "IMEI:"
                    MobEquipIdValue.Text = deviceInformation.MobileEquipmentId

                    SubIdLabel.Text = "IMSI:"
                    SubIdValue.Text = deviceInformation.SubscriberId
                ElseIf deviceInformation.CellularClass = CellularClass.Cdma Then
                    MobEquipIdLabel.Text = "ESN/MEID:"
                    MobEquipIdValue.Text = deviceInformation.MobileEquipmentId

                    SubIdLabel.Text = "MIN/IRM:"
                    SubIdValue.Text = deviceInformation.SubscriberId
                Else
                    MobEquipIdLabel.Text = ""
                    MobEquipIdValue.Text = ""
                    SubIdLabel.Text = ""
                    SubIdValue.Text = ""
                End If
            Else
                DeviceManufacturer.Text = ""
                DeviceModel.Text = ""
                Firmware.Text = ""
                CellularClasses.Text = ""
                DataClasses.Text = ""
                MobileNumber.Text = ""
                SimId.Text = ""
                DeviceType.Text = ""
                DeviceId.Text = ""
                NetworkDeviceStatus.Text = ""
                MobEquipIdLabel.Text = ""
                MobEquipIdValue.Text = ""
                SubIdLabel.Text = ""
                SubIdValue.Text = ""
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Error:" & ex.Message, NotifyType.ErrorMessage)

            ProviderName.Text = ""
            ProviderGuid.Text = ""
            NetworkAccountId.Text = ""
            NetRegister.Text = ""
            NetRegError.Text = ""
            PacketAttachError.Text = ""
            ActivateError.Text = ""
            AccessPointName.Text = ""
            NetworkAdapterId.Text = ""
            NetworkType.Text = ""
            DeviceManufacturer.Text = ""
            DeviceModel.Text = ""
            Firmware.Text = ""
            CellularClasses.Text = ""
            DataClasses.Text = ""
            MobileNumber.Text = ""
            SimId.Text = ""
            DeviceType.Text = ""
            DeviceId.Text = ""
            NetworkDeviceStatus.Text = ""
            MobEquipIdLabel.Text = ""
            MobEquipIdValue.Text = ""
            SubIdLabel.Text = ""
            SubIdValue.Text = ""
            RegisteredProviderId.Text = ""
            RegisteredProviderName.Text = ""
            RegisteredDataClass.Text = ""
        End Try
    End Sub

    Private Function NetErrorToString(netError As UInteger) As String
        Return If(netError = 0, "none", netError.ToString)
    End Function
End Class
