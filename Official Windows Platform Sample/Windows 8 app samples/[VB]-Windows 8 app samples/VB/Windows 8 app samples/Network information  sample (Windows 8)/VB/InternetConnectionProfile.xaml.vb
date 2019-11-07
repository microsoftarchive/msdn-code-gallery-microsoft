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
Imports System.Collections.Generic
Imports Windows.Networking.Connectivity

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class InternetConnectionProfile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'InternetConnectionProfileButton' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub InternetConnectionProfile_Click(sender As Object, e As RoutedEventArgs)
        '
        'Get Internet Connected Profile Information
        '
        Dim connectionProfileInfo As String = String.Empty
        Try
            Dim InternetConnectionProfile As ConnectionProfile = NetworkInformation.GetInternetConnectionProfile()

            If InternetConnectionProfile Is Nothing Then
                rootPage.NotifyUser("Not connected to Internet" & vbLf, NotifyType.StatusMessage)
            Else
                connectionProfileInfo = GetConnectionProfile(InternetConnectionProfile)
                rootPage.NotifyUser(connectionProfileInfo, NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occured: " & ex.ToString, NotifyType.ErrorMessage)
        End Try

    End Sub

    '
    'Get Connection Profile name and cost information
    '
    Private Function GetConnectionProfile(connectionProfile As ConnectionProfile) As String
        Dim connectionProfileInfo As String = String.Empty
        If connectionProfile IsNot Nothing Then
            connectionProfileInfo = "Profile Name : " & connectionProfile.ProfileName & vbLf

            Select Case connectionProfile.GetNetworkConnectivityLevel()
                Case NetworkConnectivityLevel.None
                    connectionProfileInfo &= "Connectivity Level : None" & vbLf
                    Exit Select
                Case NetworkConnectivityLevel.LocalAccess
                    connectionProfileInfo &= "Connectivity Level : Local Access" & vbLf
                    Exit Select
                Case NetworkConnectivityLevel.ConstrainedInternetAccess
                    connectionProfileInfo &= "Connectivity Level : Constrained Internet Access" & vbLf
                    Exit Select
                Case NetworkConnectivityLevel.InternetAccess
                    connectionProfileInfo &= "Connectivity Level : Internet Access" & vbLf
                    Exit Select
            End Select


            Dim connectionCost As ConnectionCost = connectionProfile.GetConnectionCost()
            connectionProfileInfo &= GetConnectionCostInfo(connectionCost)
            Dim dataPlanStatus As DataPlanStatus = connectionProfile.GetDataPlanStatus()
            connectionProfileInfo &= GetDataPlanStatusInfo(dataPlanStatus)
            Dim netSecuritySettings as NetworkSecuritySettings = connectionProfile.NetworkSecuritySettings
            connectionProfileInfo &= GetNetworkSecuritySettingsInfo(netSecuritySettings)

        End If
        Return connectionProfileInfo
    End Function

    '
    'Get Profile Connection cost
    '
    Private Function GetConnectionCostInfo(connectionCost As ConnectionCost) As String
        Dim cost As String = String.Empty
        cost &= "Connection Cost Information: " & vbLf
        cost &= "====================" & vbLf

        Select Case connectionCost.NetworkCostType
            Case NetworkCostType.Unrestricted
                cost &= "Cost: Unrestricted"
                Exit Select
            Case NetworkCostType.Fixed
                cost &= "Cost: Fixed"
                Exit Select
            Case NetworkCostType.Variable
                cost &= "Cost: Variable"
                Exit Select
            Case NetworkCostType.Unknown
                cost &= "Cost: Unknown"
                Exit Select
            Case Else
                cost &= "Cost: Error"
                Exit Select
        End Select
        cost &= vbLf
        cost &= "Roaming: " & connectionCost.Roaming & vbLf
        cost &= "Over Data Limit: " & connectionCost.OverDataLimit & vbLf
        cost &= "Approaching Data Limit : " & connectionCost.ApproachingDataLimit & vbLf

        'Display cost based suggestions to the user
        cost &= CostBasedSuggestions(connectionCost)
        Return cost
    End Function

    '
    'Display Cost based suggestions to the user
    '
    Private Function CostBasedSuggestions(connectionCost As ConnectionCost) As String
        Dim costBasedSuggestionsString As String = String.Empty
        costBasedSuggestionsString &= "Cost Based Suggestions: " & vbLf
        costBasedSuggestionsString &= "====================" & vbLf

        If connectionCost.Roaming Then
            costBasedSuggestionsString &= "Connection is out of MNO's network, using the connection may result in additional charge. Application can implement High Cost behavior in this scenario" & vbLf
        ElseIf connectionCost.NetworkCostType = NetworkCostType.Variable Then
            costBasedSuggestionsString &= "Connection cost is variable, and the connection is charged based on usage, so application can implement the Conservative behavior" & vbLf
        ElseIf connectionCost.NetworkCostType = NetworkCostType.Fixed Then
            If connectionCost.OverDataLimit OrElse connectionCost.ApproachingDataLimit Then
                costBasedSuggestionsString &= "Connection has exceeded the usage cap limit or is approaching the datalimit, and the application can implement High Cost behavior in this scenario" & vbLf
            Else
                costBasedSuggestionsString &= "Application can implemement the Conservative behavior" & vbLf
            End If
        Else
            costBasedSuggestionsString &= "Application can implement the Standard behavior" & vbLf
        End If
        Return costBasedSuggestionsString
    End Function

    '
    'Display Profile Dataplan Status information
    '
    Private Function GetDataPlanStatusInfo(dataPlan As DataPlanStatus) As String
        Dim dataplanStatusInfo As String = String.Empty
        dataplanStatusInfo = "Dataplan Status Information:" & vbLf
        dataplanStatusInfo &= "====================" & vbLf

        If dataPlan.DataPlanUsage IsNot Nothing Then
            dataplanStatusInfo &= "Usage In Megabytes : " & dataPlan.DataPlanUsage.MegabytesUsed.ToString & vbLf
            dataplanStatusInfo &= "Last Sync Time : " & dataPlan.DataPlanUsage.LastSyncTime.ToString & vbLf
        Else
            dataplanStatusInfo &= "Usage In Megabytes : Not Defined" & vbLf
        End If

        Dim inboundBandwidth As System.Nullable(Of ULong) = dataPlan.InboundBitsPerSecond
        If inboundBandwidth.HasValue Then
            dataplanStatusInfo &= "InboundBitsPerSecond : " & inboundBandwidth & vbLf
        Else
            dataplanStatusInfo &= "InboundBitsPerSecond : Not Defined" & vbLf
        End If

        Dim outboundBandwidth As System.Nullable(Of ULong) = dataPlan.OutboundBitsPerSecond
        If outboundBandwidth.HasValue Then
            dataplanStatusInfo &= "OutboundBitsPerSecond : " & outboundBandwidth & vbLf
        Else
            dataplanStatusInfo &= "OutboundBitsPerSecond : Not Defined" & vbLf
        End If

        Dim dataLimit As System.Nullable(Of UInteger) = dataPlan.DataLimitInMegabytes
        If dataLimit.HasValue Then
            dataplanStatusInfo &= "DataLimitInMegabytes : " & dataLimit & vbLf
        Else
            dataplanStatusInfo &= "DataLimitInMegabytes : Not Defined" & vbLf
        End If

        Dim nextBillingCycle As System.Nullable(Of System.DateTimeOffset) = dataPlan.NextBillingCycle
        If nextBillingCycle.HasValue Then
            dataplanStatusInfo &= "NextBillingCycle : " & nextBillingCycle.ToString & vbLf
        Else
            dataplanStatusInfo &= "NextBillingCycle : Not Defined" & vbLf
        End If

        Dim maxTransferSize As System.Nullable(Of UInteger) = dataPlan.MaxTransferSizeInMegabytes
        If maxTransferSize.HasValue Then
            dataplanStatusInfo &= "MaxTransferSizeInMegabytes : " & maxTransferSize & vbLf
        Else
            dataplanStatusInfo &= "MaxTransferSizeInMegabytes : Not Defined" & vbLf
        End If
        Return dataplanStatusInfo
    End Function


    '
    'Get Network Security Settings information
    '
    Private Function GetNetworkSecuritySettingsInfo(netSecuritySettings As NetworkSecuritySettings) As String
        Dim networkSecurity As String = String.Empty
        networkSecurity &= "Network Security Settings: " & vbLf
        networkSecurity &= "====================" & vbLf

        If netSecuritySettings Is Nothing Then
            networkSecurity &= "Network Security Settings not available" & vbLf
            Return networkSecurity
        End If

        'NetworkAuthenticationType
        Select Case netSecuritySettings.NetworkAuthenticationType
            Case NetworkAuthenticationType.None
                networkSecurity &= "NetworkAuthenticationType: None"
                Exit Select
            Case NetworkAuthenticationType.Unknown
                networkSecurity &= "NetworkAuthenticationType: Unknown"
                Exit Select
            Case NetworkAuthenticationType.Open80211
                networkSecurity &= "NetworkAuthenticationType: Open80211"
                Exit Select
            Case NetworkAuthenticationType.SharedKey80211
                networkSecurity &= "NetworkAuthenticationType: SharedKey80211"
                Exit Select
            Case NetworkAuthenticationType.Wpa
                networkSecurity &= "NetworkAuthenticationType: Wpa"
                Exit Select
            Case NetworkAuthenticationType.WpaPsk
                networkSecurity &= "NetworkAuthenticationType: WpaPsk"
                Exit Select
            Case NetworkAuthenticationType.WpaNone
                networkSecurity &= "NetworkAuthenticationType: WpaNone"
                Exit Select
            Case NetworkAuthenticationType.Rsna
                networkSecurity &= "NetworkAuthenticationType: Rsna"
                Exit Select
            Case NetworkAuthenticationType.RsnaPsk
                networkSecurity &= "NetworkAuthenticationType: RsnaPsk"
                Exit Select
            Case Else
                networkSecurity &= "NetworkAuthenticationType: Error"
                Exit Select
        End Select
        networkSecurity &= vbLf

        'NetworkEncryptionType
        Select Case netSecuritySettings.NetworkEncryptionType
            Case NetworkEncryptionType.None
                networkSecurity &= "NetworkEncryptionType: None"
                Exit Select
            Case NetworkEncryptionType.Unknown
                networkSecurity &= "NetworkEncryptionType: Unknown"
                Exit Select
            Case NetworkEncryptionType.Wep
                networkSecurity &= "NetworkEncryptionType: Wep"
                Exit Select
            Case NetworkEncryptionType.Wep40
                networkSecurity &= "NetworkEncryptionType: Wep40"
                Exit Select
            Case NetworkEncryptionType.Wep104
                networkSecurity &= "NetworkEncryptionType: Wep104"
                Exit Select
            Case NetworkEncryptionType.Tkip
                networkSecurity &= "NetworkEncryptionType: Tkip"
                Exit Select
            Case NetworkEncryptionType.Ccmp
                networkSecurity &= "NetworkEncryptionType: Ccmp"
                Exit Select
            Case NetworkEncryptionType.WpaUseGroup
                networkSecurity &= "NetworkEncryptionType: WpaUseGroup"
                Exit Select
            Case NetworkEncryptionType.RsnUseGroup
                networkSecurity &= "NetworkEncryptionType: RsnUseGroup"
                Exit Select
            Case Else
                networkSecurity &= "NetworkEncryptionType: Error"
                Exit Select
        End Select
        networkSecurity &= vbLf
        Return networkSecurity
    End Function


End Class
