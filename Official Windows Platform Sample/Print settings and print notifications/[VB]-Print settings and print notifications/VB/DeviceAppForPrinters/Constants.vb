'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Activation
Imports Windows.Devices.Printers.Extensions

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Device App For Printers Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Ink Level", .ClassType = GetType(DeviceAppForPrinters.InkLevel)}, _
            New Scenario() With {.Title = "Advanced Settings", .ClassType = GetType(DeviceAppForPrinters.Preferences)} _
        }

        Public Config As PrintTaskConfiguration
        Public Context As Object

        Public Sub LoadAdvancedPrintSettingsContext(ByVal args As PrintTaskSettingsActivatedEventArgs)
            Config = args.Configuration
            Context = Config.PrinterExtensionContext
            LoadScenario(GetType(DeviceAppForPrinters.Preferences))
        End Sub
    End Class

    Partial Class App
        Inherits Application

        Protected Overrides Sub OnActivated(ByVal args As IActivatedEventArgs)
            If args.Kind = ActivationKind.PrintTaskSettings Then
                Dim rootFrame As New Frame()
                If Nothing Is Window.Current.Content Then
                    rootFrame.Navigate(GetType(MainPage))
                    Window.Current.Content = rootFrame
                End If
                Window.Current.Activate()

                Dim _mainPage As MainPage = CType(rootFrame.Content, MainPage)

                ' Load advanced printer preferences scenario
                _mainPage.LoadAdvancedPrintSettingsContext(CType(args, PrintTaskSettingsActivatedEventArgs))
            End If
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
