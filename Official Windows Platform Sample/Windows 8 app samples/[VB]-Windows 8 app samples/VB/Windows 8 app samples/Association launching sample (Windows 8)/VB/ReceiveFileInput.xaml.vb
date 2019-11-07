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
Imports AssociationLaunching
Imports SDKTemplate

Partial Public NotInheritable Class ReceiveFileInput
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As RootPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        Dim rootPage = TryCast(e.Parameter, RootPage)

        '' Display the result of the file activation if we got here as a result of being activated for a file.
        If rootPage.FileEvent IsNot Nothing Then
            rootPage.NotifyUser("File activation received. The number of files received is " & rootPage.FileEvent.Files.Count & ". The first received file is " & rootPage.FileEvent.Files(0).Name & ".", NotifyType.StatusMessage)
        End If
    End Sub
End Class
