' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Linq
Imports System.Collections.Generic
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
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports System.IO
Imports Windows.Security.Cryptography.Certificates
Imports System.Threading.Tasks
Imports Windows.ApplicationModel.Resources
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = TryCast(outputFrame.FindName("FlipView1"), FlipView)
    End Sub
#End Region

#Region "Sample Click Handlers - modify if you need them, delete them otherwise"
    ''' <summary>
    ''' Handler for ImportPfx button, if you have a button or buttons on this page
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub ImportPfx_Click(sender As Object, e As RoutedEventArgs)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim outputTextBlock As TextBlock = TryCast(outputFrame.FindName("Scenario2Result"), TextBlock)

        Try
            outputTextBlock.Text = "Importing PFX certificate ..."
            Await ImportPfxFile("Certificate.pfx")


            outputTextBlock.Text &= vbCrLf & "Certificate installation succeeded. The certificate is in the appcontainer Personal certificate store"
        Catch ex As Exception
            outputTextBlock.Text &= vbCrLf & "Certificate installation failed with error: " + ex.ToString
        End Try
    End Sub

    Private Async Function ImportPfxFile(filePath As String) As Task
        ' Load the pfx certificate from resource string.
        Dim rl As New ResourceLoader
        Dim pfxCertificate As String = rl.GetString("Certificate")

        Dim password As String = "sampletest"
        'password to access the certificate in PFX format
        Dim friendlyName As String = "test pfx certificate"


        'call Certificate Enrollment funciton importPFXData to install the certificate
        Await CertificateEnrollmentManager.ImportPfxDataAsync(pfxCertificate, password, ExportOption.NotExportable, KeyProtectionLevel.NoConsent, InstallOptions.None, friendlyName)
    End Function
#End Region

End Class
