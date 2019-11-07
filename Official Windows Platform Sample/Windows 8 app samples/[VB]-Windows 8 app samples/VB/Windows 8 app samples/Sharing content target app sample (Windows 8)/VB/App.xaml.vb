'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Activation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Namespace Global.SDKTemplate
    Partial NotInheritable Class App
        Inherits Application
        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLaunched(args As LaunchActivatedEventArgs)
            Dim rootFrame = New Frame()
            rootFrame.Navigate(GetType(DefaultPage))
            Window.Current.Content = rootFrame
            Window.Current.Activate()
        End Sub

        Protected Overrides Sub OnShareTargetActivated(args As ShareTargetActivatedEventArgs)
            Dim rootFrame = New Frame()
            rootFrame.Navigate(GetType(MainPage), args.ShareOperation)
            Window.Current.Content = rootFrame
            Window.Current.Activate()
        End Sub
    End Class
End Namespace
