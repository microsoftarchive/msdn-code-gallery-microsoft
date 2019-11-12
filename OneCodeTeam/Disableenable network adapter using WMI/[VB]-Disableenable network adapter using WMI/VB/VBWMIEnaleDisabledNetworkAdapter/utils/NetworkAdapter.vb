'****************************** Module Header ******************************\
' Module Name:  NetworkAdapter.vb
' Project:	    VBWMIEnableDisableNetworkAdapter
' Copyright (c) Microsoft Corporation.
' 
' The class NetworkAdapter supplies following features:
' 1. Get all the NetworkAdapters of the machine
' 2. Get NetEnabled Status of the NetworkAdapter  
' 3. Enable\Disable a NetworkAdapter
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System
Imports System.Collections.Generic
Imports System.Management
Imports System.Threading
Imports VBWMIEnaleDisabledNetworkAdapter.My.Resources


Public Class NetworkAdapter

#Region "Private Properties"

    ''' <summary>
    ''' Network Adapter DeviceId
    ''' </summary>
    ''' <remarks></remarks>
    Private _intDeviceId As Integer
    Property DeviceId As Integer
        Get
            Return _intDeviceId
        End Get
        Set(ByVal value As Integer)
            _intDeviceId = value
        End Set

    End Property

    ''' <summary>
    ''' Network Adapter ProductName
    ''' </summary>
    ''' <remarks></remarks>
    Private _strName As String
    Property Name() As String
        Get
            Return _strName
        End Get
        Set(ByVal value As String)
            _strName = value
        End Set
    End Property

    ''' <summary>
    ''' Network Adapter Connection Status
    ''' </summary>
    ''' <remarks></remarks>
    Private _intNetConnectionStatus As Integer
    Property NetConnectionStatus As Integer
        Get
            Return _intNetConnectionStatus
        End Get
        Set(ByVal value As Integer)
            _intNetConnectionStatus = value
        End Set
    End Property

    ''' <summary>
    ''' Network Adapter NetEnabled
    ''' </summary>
    ''' <remarks></remarks>
    Private _intNetEnabled As Integer
    Property NetEnabled As Integer
        Get
            Return _intNetEnabled
        End Get
        Set(ByVal value As Integer)
            _intNetEnabled = value
        End Set
    End Property

    ''' <summary>
    ''' Network Adapter Connection Status Descriptions
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared SaNetConnectionStatus As String() = New String() _
    {
        NetConnectionStatus0,
        NetConnectionStatus1,
        NetConnectionStatus2,
        NetConnectionStatus3,
        NetConnectionStatus4,
        NetConnectionStatus5,
        NetConnectionStatus6,
        NetConnectionStatus7,
        NetConnectionStatus8,
        NetConnectionStatus9,
        NetConnectionStatus10,
        NetConnectionStatus11,
        NetConnectionStatus12
    }

    ''' <summary>
    ''' Enum The Result Of Enable Or Disable Network Adapter
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum EnumEnableDisableResult
        Fail = -1
        Success = 1
        Unknow = 0
    End Enum

    ''' <summary>
    ''' Enum The Network Adapter NetEnabled Status Values
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum EnumNetEnabledStatus
        Disabled = -1
        Enabled = 1
        Unknow = 0
    End Enum

#End Region

#Region "Construct NetworkAdapter"

    Public Sub New(ByVal deviceId As Integer,
                   ByVal name As String,
                   ByVal netEnabled As Integer,
                   ByVal netConnectionStatus As Integer)
        Me.DeviceId = deviceId
        Me.Name = name
        Me.NetEnabled = netEnabled
        Me.NetConnectionStatus = netConnectionStatus
    End Sub

    Public Sub New(ByVal deviceId As Integer)
        Dim crtNetworkAdapter As New ManagementObject
        Dim strWQuery As String =
            String.Format("SELECT DeviceID, ProductName, NetEnabled, " _
                          & "NetConnectionStatus " _
                          & "FROM Win32_NetworkAdapter " _
                          & "WHERE DeviceID = {0}", DeviceId)

        Try
            Dim networkAdapter As ManagementObject
            For Each networkAdapter In WMIOperation.WMIQuery(strWQuery)
                'Expected to be only one ManagementObject instance.
                crtNetworkAdapter = networkAdapter
                Exit For
            Next

            Me.DeviceId = deviceId
            Name = crtNetworkAdapter.Item("ProductName").ToString
            NetEnabled = IIf(
                Convert.ToBoolean(crtNetworkAdapter.Item("NetEnabled").ToString), 1, -1)
            NetConnectionStatus =
                Convert.ToInt32(crtNetworkAdapter.Item("NetConnectionStatus").ToString)
        Catch nrExcepton As NullReferenceException
            ' If there is no a network adapter which deviceid equates to the argument 
            '"deviceId" just to construct a none exists network adapter
            Me.DeviceId = -1
            Name = String.Empty
            NetEnabled = 0
            NetConnectionStatus = -1
        End Try
    End Sub

#End Region

#Region "Enable Or Disable Network Adapter"

    ''' <summary>
    ''' Function To Enabled Or Disabled A Network Adapter
    ''' </summary>
    ''' <param name="strOperation">The Operation Method</param>
    ''' <returns>The Result Of Enabled Or Disabled A Network Adapter</returns>
    ''' <remarks></remarks>
    Public Function EnableOrDisableNetworkAdapter(ByVal strOperation As String) As Integer
        Dim resultEnableDisableNetworkAdapter As Integer = EnumEnableDisableResult.Unknow
        Dim crtNetworkAdapter As New ManagementObject
        Dim strWQuery As String = ("SELECT DeviceID, ProductName, NetEnabled, " _
                                   & "NetConnectionStatus " _
                                   & "FROM Win32_NetworkAdapter " _
                                   & "WHERE DeviceID = " & DeviceId.ToString())

        Try
            Dim networkAdapter As ManagementObject
            For Each networkAdapter In WMIOperation.WMIQuery(strWQuery)
                crtNetworkAdapter = networkAdapter
            Next

            crtNetworkAdapter.InvokeMethod(strOperation, Nothing)
            Thread.Sleep(500)

            Do While (GetNetEnabled() <> IIf((strOperation.Trim = "Enable"),
                                             EnumNetEnabledStatus.Enabled,
                                             EnumNetEnabledStatus.Disabled))
                Thread.Sleep(100)
            Loop
            resultEnableDisableNetworkAdapter = EnumEnableDisableResult.Success
        Catch nrException As NullReferenceException
            ' If there is a NullReferenceException the result will be fail
            resultEnableDisableNetworkAdapter = EnumEnableDisableResult.Fail
        End Try

        crtNetworkAdapter.Dispose()

        Return resultEnableDisableNetworkAdapter
    End Function

#End Region

#Region "Get All Network Adapters In The Machine"

    ''' <summary>
    ''' Get All Network Adapters In The Machine
    ''' </summary>
    ''' <returns>List Of Network Adapters In The Machine</returns>
    ''' <remarks></remarks>
    Public Shared Function GetAllNetworkAdapter() As List(Of NetworkAdapter)
        Dim allNetworkAdapter As New List(Of NetworkAdapter)

        ' Manufacturer <> 'Microsoft'to get all none virtual devices.
        ' Because AdapterType property will be null if the NetworkAdapter is disabled,   
        ' so we do not use NetworkAdapter = 'Ethernet 802.3' or NetworkAdapter = 'Wireles’
        Dim strWQuery As String = "SELECT DeviceID, ProductName, NetEnabled, " _
                                  & "NetConnectionStatus " _
                                  & "FROM Win32_NetworkAdapter " _
                                  & "WHERE Manufacturer <> 'Microsoft'"
        Dim moNetworkAdapter As ManagementObject

        For Each moNetworkAdapter In WMIOperation.WMIQuery(strWQuery)
            Try
                allNetworkAdapter.Add(
                    New NetworkAdapter(
                        Convert.ToInt32(moNetworkAdapter.Item("DeviceID").ToString), _
                        moNetworkAdapter.Item("ProductName").ToString,
                        IIf(
                            Convert.ToBoolean(
                                moNetworkAdapter.Item("NetEnabled").ToString),
                                EnumNetEnabledStatus.Enabled,
                                EnumNetEnabledStatus.Disabled),
                            Convert.ToInt32(
                            moNetworkAdapter.Item("NetConnectionStatus").ToString)))
                Continue For
            Catch nrException As NullReferenceException
                ' Ignore some other devices like the bluetooth, that need user 
                ' interaction to enable or disable.
                Continue For
            End Try
        Next
        Return allNetworkAdapter
    End Function

#End Region

#Region "Get Network Adapter NetEnabled Property"

    ''' <summary>
    ''' Get Network Adapter NetEnabled Property
    ''' </summary>
    ''' <returns>Network Adapter NetEnabled Property</returns>
    ''' <remarks></remarks>
    Public Function GetNetEnabled() As Integer
        Dim enabled As Integer = EnumNetEnabledStatus.Unknow
        Dim strWQuery As String =
            String.Format("SELECT NetEnabled FROM Win32_NetworkAdapter " _
                          & "WHERE DeviceID = {0}", Me.DeviceId)

        Try
            Dim networkAdapter As ManagementObject
            For Each networkAdapter In WMIOperation.WMIQuery(strWQuery)
                NetEnabled = IIf(
                    Convert.ToBoolean(networkAdapter.Item("NetEnabled").ToString), 1, -1)
            Next
        Catch nrException As NullReferenceException
            ' For NullReferenceException return (EnumNetEnabledStatus.Unknow)
        End Try

        Return NetEnabled
    End Function

#End Region

End Class
