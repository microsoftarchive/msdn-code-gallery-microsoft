'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Partial Public Class settings
    Inherits basepage
    Public Sub New()
        InitializeComponent()

    End Sub

    ''' <summary>
    ''' Explicit saving of settings
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>The only time the settings are update is when the user
    ''' click Save. An alternative to this approach would be to save the settings
    ''' when the user navigates from this page.</remarks>
    Private Sub appbarSave_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Setting the properties will trigger the OnApplicationDataObjectChanged
        ' event, thus updating the properties on the basepage and therefore making
        ' the changes available to all pages that inherit from baspage.
        TryCast(Application.Current, App).HostName = txtServerName.Text
        TryCast(Application.Current, App).PortNumber = Convert.ToInt32(txtPortNumber.Text)
        TryCast(Application.Current, App).PlayAsX = CBool(rbPlayAsX.IsChecked)

        ' Once the settings have been saved navigate back to the main page.
        If NavigationService.CanGoBack Then
            NavigationService.GoBack()
        End If
    End Sub


End Class
