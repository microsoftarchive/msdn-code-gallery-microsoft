' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation

Namespace Global.AssociationLaunching
    Partial Public NotInheritable Class ReceiveUriInput
        Inherits Page
        ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
        Private rootPage As rootPage = Nothing

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            ' Get a pointer to our main page
            rootPage = TryCast(e.Parameter, rootPage)

            '' Display the result of the protocol activation if we got here as a result of being activated for a protocol.
            If rootPage.ProtocolEvent IsNot Nothing Then
                rootPage.NotifyUser("Protocol activation received. The received URI is " & rootPage.ProtocolEvent.Uri.AbsoluteUri & ".", NotifyType.StatusMessage)
            End If
        End Sub
    End Class
End Namespace