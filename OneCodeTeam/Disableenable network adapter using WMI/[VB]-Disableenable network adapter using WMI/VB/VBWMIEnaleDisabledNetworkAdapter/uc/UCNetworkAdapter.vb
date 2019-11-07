'****************************** Module Header ******************************\
' Module Name:  UCNetworkAdapter.vb
' Project:	    VBWMIEnableDisableNetworkAdapter
' Copyright (c) Microsoft Corporation.
' 
' This is the user control which shows information of a Network Adapter in the main form.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Windows.Forms
Imports System.Drawing
Imports VBWMIEnaleDisabledNetworkAdapter.My.Resources

Public Class UCNetworkAdapter

#Region "Construct UCNetworkAdapter"

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(
                  ByVal networkAdapter As NetworkAdapter,
                  ByVal eventHandler As EventHandler,
                  ByVal point As Point,
                  ByVal parent As Control)
        InitializeComponent()
        pctNetworkAdapter.Image = IIf((networkAdapter.NetEnabled > 0),
                                      Resources.ImgEnabledNetworkAdapter,
                                      Resources.ImgDisabledNetworkAdapter)
        lbProductName.Text = networkAdapter.Name
        lbConnectionStatus.Text =
            networkAdapter.SaNetConnectionStatus(networkAdapter.NetConnectionStatus)
        btnEnableDisable.Text = IIf((networkAdapter.NetEnabled > 0),
                                    Resources.BtnText_Disable,
                                    Resources.BtnText_Enable)
        btnEnableDisable.Tag =
            New Integer() {networkAdapter.DeviceId, networkAdapter.NetEnabled}

        AddHandler btnEnableDisable.Click, eventHandler
        Location = point
        MyBase.Parent = parent
    End Sub

#End Region

End Class
