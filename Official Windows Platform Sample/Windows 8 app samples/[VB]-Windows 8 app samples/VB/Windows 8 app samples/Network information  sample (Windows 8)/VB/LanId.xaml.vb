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
Imports Windows.Networking.Connectivity

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class LanId
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
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LanId_Click(sender As Object, e As RoutedEventArgs)
        '
        'Display Lan Identifiers - Infrastructure ID, Port ID, Network Adapter ID
        '
        Dim lanIdentifierData As String = String.Empty
        Try
            Dim lanIdentifiers = NetworkInformation.GetLanIdentifiers()
            If lanIdentifiers.Count <> 0 Then
                lanIdentifierData = "Number of Lan Identifiers retrieved: " & lanIdentifiers.Count & vbLf
                lanIdentifierData &= "=============================================" & vbLf
                For i = 0 To lanIdentifiers.Count - 1
                    'Display Lan Identifier data for each identifier
                    lanIdentifierData &= GetLanIdentifierData(lanIdentifiers(i)).ToString
                    lanIdentifierData &= "------------------------------------------------" & vbLf
                Next
                rootPage.NotifyUser(lanIdentifierData.ToString, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("No Lan Identifier Data found", NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occured: " & ex.ToString, NotifyType.ErrorMessage)
        End Try
    End Sub

    '
    'Get Lan Identifier Data
    '
    Private Function GetLanIdentifierData(lanIdentifier As LanIdentifier) As String
        Dim lanIdentifierData As String = String.Empty
        If lanIdentifier Is Nothing Then
            Return lanIdentifierData
        End If

        If lanIdentifier.InfrastructureId IsNot Nothing Then
            lanIdentifierData &= "Infrastructure Type: " & lanIdentifier.InfrastructureId.Type.ToString & vbLf
            lanIdentifierData &= "Infrastructure Value: "
            Dim infrastructureIdValue = lanIdentifier.InfrastructureId.Value
            For Each value In infrastructureIdValue
                lanIdentifierData &= value.ToString & " "
            Next
        End If

        If lanIdentifier.PortId IsNot Nothing Then
            lanIdentifierData &= vbLf & "Port Type : " & lanIdentifier.PortId.Type.ToString & vbLf
            lanIdentifierData &= "Port Value: "
            Dim portIdValue = lanIdentifier.PortId.Value
            For Each value In portIdValue
                lanIdentifierData &= value.ToString & " "
            Next
        End If

        If lanIdentifier.NetworkAdapterId.ToString IsNot Nothing Then
            lanIdentifierData &= vbLf & "Network Adapter Id : " & lanIdentifier.NetworkAdapterId.ToString & vbLf
        End If
        Return lanIdentifierData
    End Function
End Class
