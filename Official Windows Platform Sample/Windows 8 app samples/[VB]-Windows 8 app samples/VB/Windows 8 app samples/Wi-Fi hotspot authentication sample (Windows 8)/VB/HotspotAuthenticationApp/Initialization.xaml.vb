'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.Background
Imports Windows.Data.Xml.Dom
Imports Windows.Networking.NetworkOperators
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports HotspotAuthenticationTask

Partial Public NotInheritable Class Initialization
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed to call methods in MainPage such as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Configure background task handler to perform authentication as default
        ConfigStore.AuthenticateThroughBackgroundTask = True

        ' Setup completion handler
        Dim isTaskRegistered = ScenarioCommon.Instance.RegisteredCompletionHandlerForBackgroundTask()

        ' Initialize button state
        UpdateButtonState(isTaskRegistered)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Provision' button to provision the embedded XML file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub ProvisionButton_Click(sender As Object, args As RoutedEventArgs)
        ProvisionButton.IsEnabled = False

        Try
            ' Open the installation folder
            Dim installLocation = Windows.ApplicationModel.Package.Current.InstalledLocation

            ' Access the provisioning file
            Dim provisioningFile = Await installLocation.GetFileAsync("ProvisioningData.xml")

            ' Load with XML parser
            Dim xmlDocument__1 = Await XmlDocument.LoadFromFileAsync(provisioningFile)

            ' Get raw XML
            Dim provisioningXml = xmlDocument__1.GetXml()

            ' Create ProvisiongAgent Object
            Dim provisioningAgent As New ProvisioningAgent

            ' Create ProvisionFromXmlDocumentResults Object
            Dim result = Await provisioningAgent.ProvisionFromXmlDocumentAsync(provisioningXml)

            If result.AllElementsProvisioned Then
                rootPage.NotifyUser("Provisioning was successful", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Provisioning result: " & result.ProvisionResultsXml, NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try

        ProvisionButton.IsEnabled = True
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Register' button to registers a background task for
    ''' the NetworkOperatorHotspotAuthentication event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RegisterButton_Click(sender As Object, args As RoutedEventArgs)
        Try
            ' Create a new background task builder.
            Dim taskBuilder As New BackgroundTaskBuilder

            ' Create a new NetworkOperatorHotspotAuthentication trigger.
            Dim trigger As New NetworkOperatorHotspotAuthenticationTrigger

            ' Associate the NetworkOperatorHotspotAuthentication trigger with the background task builder.
            taskBuilder.SetTrigger(trigger)

            ' Specify the background task to run when the trigger fires.
            taskBuilder.TaskEntryPoint = ScenarioCommon.BackgroundTaskEntryPoint

            ' Name the background task.
            taskBuilder.Name = ScenarioCommon.BackgroundTaskName

            ' Register the background task.
            Dim task = taskBuilder.Register()

            ' Associate progress and completed event handlers with the new background task.
            AddHandler task.Completed, AddressOf ScenarioCommon.Instance.OnBackgroundTaskCompleted

            UpdateButtonState(True)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Unregister' button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UnregisterButton_Click(sender As Object, args As RoutedEventArgs)
        UnregisterBackgroundTask()
        UpdateButtonState(False)
    End Sub

    ''' <summary>
    ''' Unregister background task
    ''' </summary>
    ''' <param name="name"></param>
    Private Sub UnregisterBackgroundTask()
        ' Loop through all background tasks and unregister any.
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = ScenarioCommon.BackgroundTaskName Then
                cur.Value.Unregister(True)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Update button state
    ''' </summary>
    ''' <param name="registered">True if background task is registered</param>
    Private Sub UpdateButtonState(registered As Boolean)
        If registered Then
            RegisterButton.IsEnabled = False
            UnregisterButton.IsEnabled = True
        Else
            RegisterButton.IsEnabled = True
            UnregisterButton.IsEnabled = False
        End If
    End Sub
End Class
