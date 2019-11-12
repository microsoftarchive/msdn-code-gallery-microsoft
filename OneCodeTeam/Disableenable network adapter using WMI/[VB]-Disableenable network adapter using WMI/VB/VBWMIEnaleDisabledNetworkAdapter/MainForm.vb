'****************************** Module Header ******************************\
' Module Name:  MainForm.vb
' Project:	    VBWMIEnableDisableNetworkAdapter
' Copyright (c) Microsoft Corporation.
' 
' This is the main form of this application. It is used to initialize the UI  
' and handle the events 
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
Imports System.Threading
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Security.Principal
Imports VBWMIEnaleDisabledNetworkAdapter.My.Resources


Public Class MainForm

#Region "Private Properties"

    ''' <summary>
    ''' All Network Adapters in the machine
    ''' </summary>
    Private _allNetworkAdapters As List(Of NetworkAdapter) = New List(Of NetworkAdapter)

    ''' <summary>
    ''' A ProgressInfo form
    ''' </summary>
    Private _progressInfoForm As ProgressInfoForm = New ProgressInfoForm

    ''' <summary>
    ''' The Current Operation Network Adapter
    ''' </summary>
    Private _currentNetworkAdapter As NetworkAdapter

#End Region

#Region "Construct EnableDisableNetworkAdapter"

    Public Sub New()
        If isAdministrator() Then
            InitializeComponent()
            ShowAllNetworkAdapters()
            tsslbResult.Text =
                String.Format("{0}[{1}]",
                              StatusTextInitial,
                              _allNetworkAdapters.Count.ToString)
        Else
            MessageBox.Show(MsgElevatedRequire, _
                OneCodeCaption, _
                MessageBoxButtons.OK, _
                MessageBoxIcon.Warning)
            Environment.Exit(1)
        End If
    End Sub

#End Region

#Region "Event Handler"

    ''' <summary>
    ''' Button on click event handler
    ''' Click enable or disable the network adapter
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub BtnEnableDisableNetworAdaptetClick(
                                                 ByVal sender As Object,
                                                 ByVal e As EventArgs)
        Dim btnEnableDisableNetworkAdapter As Button = DirectCast(sender, Button)

        ' The result of enable or disable Network Adapter
        ' result ={-1: Fail;0:Unkown;1:Success}
        Dim result As Integer = -1
        Dim deviceId As Integer =
            DirectCast(btnEnableDisableNetworkAdapter.Tag, Integer())(0)
        Dim showProgressInfoThreadProc As New Thread(
            New ThreadStart(AddressOf ShowProgressInfo))
        Try
            _currentNetworkAdapter = New NetworkAdapter(deviceId)

            ' To avoid the condition of the network adapter netenable change caused 
            ' by any other tool or operation (ex. Disconnected the Media) after you 
            ' start the sample and before you click the enable\disable button.
            ' In this case, the network adapter status shown in windows form is not
            ' meet the real status.

            ' If the network adapter status is shown corrected, just enable or disable
            ' if as usual.
            If (DirectCast(btnEnableDisableNetworkAdapter.Tag, Integer())(1) =
                _currentNetworkAdapter.NetEnabled) Then

                ' If Network Adapter NetConnectionStatus in ("Hardware not present",
                ' Hardware disabled","Hardware malfunction","Media disconnected"), 
                ' it will can not be enabled.
                If (((_currentNetworkAdapter.NetEnabled = -1) _
                     AndAlso (_currentNetworkAdapter.NetConnectionStatus >= 4)) _
                 AndAlso (_currentNetworkAdapter.NetConnectionStatus <= 7)) Then
                    MessageBox.Show(
                        String.Format(
                            "{0}({1}) [{2}] {3} [{4}]",
                            New Object() {
                                StatusTextBegin,
                                _currentNetworkAdapter.DeviceId,
                                Name,
                                CanNotEnableMsg,
                                NetworkAdapter.SaNetConnectionStatus(
                                    _currentNetworkAdapter.NetConnectionStatus)}),
                        OneCodeCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand)
                Else
                    showProgressInfoThreadProc.Start()
                    result = _currentNetworkAdapter.EnableOrDisableNetworkAdapter(
                        IIf((_currentNetworkAdapter.NetEnabled = 1), "Disable", "Enable"))
                    showProgressInfoThreadProc.Abort()
                End If
                ' If the network adapter status are not show corrected, just to refresh 
                ' the form to show the correct network adapter status.
            Else
                ShowAllNetworkAdapters()
                result = 1
            End If
        Catch nrException As NullReferenceException
            ' If failed to construct _currentNetworkAdapter the result will be fail.
        End Try

        ' If successfully enable or disable the Network Adapter
        If (result > 0) Then
            ShowAllNetworkAdapters()
            tsslbResult.Text =
                String.Format(
                    "{0}[{1}] ({2}) {3}",
                    New Object() {
                        StatusTextBegin,
                        _currentNetworkAdapter.DeviceId,
                        _currentNetworkAdapter.Name,
                        IIf((DirectCast(btnEnableDisableNetworkAdapter.Tag,
                             Integer())(1) = 1),
                            StatusTextSuccessDisableEnd,
                            StatusTextSuccessEnableEnd)})

            tsslbResult.ForeColor = Color.Green
        Else
            tsslbResult.Text =
                String.Format(
                    "{0}[{1}] ({2}) {3}",
                    New Object() {
                        StatusTextBegin,
                        _currentNetworkAdapter.DeviceId,
                        _currentNetworkAdapter.Name,
                        IIf((DirectCast(btnEnableDisableNetworkAdapter.Tag,
                             Integer())(1) = 1),
                            StatusTextFailDisableEnd,
                            StatusTextFailEnableEnd)})

            tsslbResult.ForeColor = Color.Red
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' You need to run this sample as Administrator
    ''' Check whether the application is run as administrator
    ''' </summary>
    ''' <returns>Whether the application is run as administrator</returns>
    Private Function isAdministrator() As Boolean
        Dim principal As New WindowsPrincipal(WindowsIdentity.GetCurrent)
        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function

    ''' <summary>
    ''' Show all Network Adapters in the Enable\DisableNetworkAdapter window
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowAllNetworkAdapters()
        grpNetworkAdapters.Controls.Clear()
        _allNetworkAdapters = NetworkAdapter.GetAllNetworkAdapter
        Dim i As Integer = 0

        Dim netAdapter As NetworkAdapter
        For Each netAdapter In _allNetworkAdapters
            i += 1
            Dim cuNetworkAdapter As UCNetworkAdapter =
                New UCNetworkAdapter(
                netAdapter,
                New EventHandler(AddressOf BtnEnableDisableNetworAdaptetClick),
                New Point(10, (30 * i)),
                grpNetworkAdapters)
        Next
    End Sub

    ''' <summary>
    ''' Show progress info while enabling or disabling a Network Adapter.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowProgressInfo()
        tsslbResult.Text = String.Empty

        Dim c As Control
        For Each c In _progressInfoForm.Controls
            Dim control = TryCast(c, Label)
            If (control IsNot Nothing) Then
                control.Text =
                    String.Concat(
                        New String() {
                                         StatusTextBegin,
                                         "[",
                                         _currentNetworkAdapter.DeviceId.ToString,
                                         "] (", _currentNetworkAdapter.Name,
                                         ") ",
                                         IIf((_currentNetworkAdapter.GetNetEnabled <> 1),
                                             ProgressTextEnableEnd,
                                             ProgressTextDisableEnd)})
            End If
        Next

        _progressInfoForm.LocationX =
            (Location.X + ((Width - _progressInfoForm.Width) / 2))
        _progressInfoForm.LocationY =
            (Location.Y + ((Height - _progressInfoForm.Height) / 2))
        _progressInfoForm.ShowDialog()
    End Sub

#End Region

End Class
