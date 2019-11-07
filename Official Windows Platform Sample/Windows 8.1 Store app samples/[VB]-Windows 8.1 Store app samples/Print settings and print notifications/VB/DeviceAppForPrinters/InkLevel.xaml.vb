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
Imports Windows.Devices.Enumeration
Imports Windows.Devices.Enumeration.Pnp

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class InkLevel
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private printHelper As PrintHelperClass

    Private Const keyPrinterName As String = "BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39"
    Private Const keyAsyncUIXML As String = "55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685"
    Private settings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

    Public Sub New()
        Me.InitializeComponent()

        ' Disables scenario navigation by hiding the navigation UI elements.
        CType(rootPage.FindName("Scenarios"), UIElement).Visibility = Windows.UI.Xaml.Visibility.Collapsed
        CType(rootPage.FindName("ScenarioListLabel"), UIElement).Visibility = Windows.UI.Xaml.Visibility.Collapsed
        CType(rootPage.FindName("DescriptionText"), UIElement).Visibility = Windows.UI.Xaml.Visibility.Collapsed
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        rootPage.NotifyUser("Notification updated", NotifyType.StatusMessage)
        DisplayBackgroundTaskTriggerDetails()

        ' Clearing the live tile status
        Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        Windows.UI.Notifications.BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear()
    End Sub

    ''' <summary>
    ''' Invoked when this page is navigated away from.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Unsubscribe from the OnInkLevelReceived event.
        If printHelper IsNot Nothing Then
            RemoveHandler printHelper.OnInkLevelReceived, AddressOf OnInkLevelReceived
            printHelper = Nothing
        End If
    End Sub

    Private Sub DisplayBackgroundTaskTriggerDetails()
        Dim outputText As String = vbCrLf

        Try
            Dim printerName As String = settings.Values(keyPrinterName).ToString()
            outputText &= ("Printer name from background task triggerDetails: " & printerName)
        Catch e1 As Exception
            outputText &= ("No printer name retrieved from background task triggerDetails ")
        End Try

        outputText &= vbCrLf
        Try
            Dim asyncUIXML As String = settings.Values(keyAsyncUIXML).ToString()
            outputText &= ("AsyncUI xml from background task triggerDetails: " & asyncUIXML)
        Catch e2 As Exception
            outputText &= ("No asyncUI xml retrieved from background task triggerDetails ")
        End Try

        ToastOutput.Text += outputText
    End Sub

    ''' <summary>
    ''' Enumerates the printers by the following steps.
    '''     1. Searching through all devices interfaces for printers.
    '''     2. Getting the container for each printer device.
    '''     3. Checking for association by comparing each container's PackageFamilyName property
    ''' </summary>
    ''' <param name="sender" type = "Windows.UI.Xaml.Controls.Button">A pointer to the button that the user hit to enumerate printers</param>
    ''' <param name="e">Arguments passed in by the event.</param>
    Private Async Sub EnumerateAssociatedPrinters(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Reset output text and associated printer array.
        AssociatedPrinters.Items.Clear()
        BidiOutput.Text = ""

        ' GUID string for printers.
        Dim printerInterfaceClass As String = "{0ecef634-6ef0-472a-8085-5ad023ecbccd}"
        Dim selector As String = "System.Devices.InterfaceClassGuid:=""" & printerInterfaceClass & """"

        ' By default, FindAllAsync does not return the containerId for the device it queries.
        ' We have to add it as an additonal property to retrieve. 
        Dim containerIdField As String = "System.Devices.ContainerId"
        Dim propertiesToRetrieve() As String = {containerIdField}

        ' Asynchronously find all printer devices.
        Dim deviceInfoCollection As DeviceInformationCollection = Await DeviceInformation.FindAllAsync(selector, propertiesToRetrieve)

        ' For each printer device returned, check if it is associated with the current app.
        For i As Integer = 0 To deviceInfoCollection.Count - 1
            Dim deviceInfo As DeviceInformation = deviceInfoCollection(i)
            FindAssociation(deviceInfo, deviceInfo.Properties(containerIdField).ToString())
        Next i
    End Sub

    ''' <summary>
    ''' Check if a printer is associated with the current application, if it is, add its interfaceId to a list of associated interfaceIds.
    ''' 
    '''     For each different app, it will have a different correctPackageFamilyName.
    '''     Look in the Visual Studio packagemanifest editor to see what it is for your app.
    ''' </summary>
    ''' <param name="deviceInfo">The deviceInformation of the printer.</param>
    Private Async Sub FindAssociation(ByVal deviceInfo As DeviceInformation, ByVal containerId As String)

        ' Specifically telling CreateFromIdAsync to retrieve the AppPackageFamilyName. 
        Dim packageFamilyName As String = "System.Devices.AppPackageFamilyName"
        Dim containerPropertiesToGet() As String = {packageFamilyName}

        ' CreateFromIdAsync needs braces on the containerId string.
        Dim containerIdwithBraces As String = "{" & containerId & "}"

        ' Asynchoronously getting the container information of the printer.
        Dim containerInfo As PnpObject = Await PnpObject.CreateFromIdAsync(PnpObjectType.DeviceContainer, containerIdwithBraces, containerPropertiesToGet)

        ' Printers could be associated with other device apps, only the ones with package family name
        ' matching this app's is associated with this app. The packageFamilyName for this app will be found in this app's packagemanifest
        Dim appPackageFamilyName As String = "Microsoft.SDKSamples.DeviceAppForPrinters.VB_8wekyb3d8bbwe"
        Dim prop = containerInfo.Properties

        ' If the packageFamilyName of the printer container matches the one for this app, the printer is associated with this app.
        Dim packageFamilyNameList() As String = CType(prop(packageFamilyName), String())
        If packageFamilyNameList IsNot Nothing Then
            For j As Integer = 0 To packageFamilyNameList.Length - 1
                If packageFamilyNameList(j).Equals(appPackageFamilyName) Then
                    AddToList(deviceInfo)
                End If
            Next j
        End If
    End Sub

    ''' <summary>
    ''' Adds the printer to the selection box to allow the user to select it.
    ''' </summary>
    ''' <param name="deviceInfo">Contains the device information of the printer to be added to the combo box.</param>
    Private Sub AddToList(ByVal deviceInfo As DeviceInformation)
        ' Creating a new display item so the user sees the friendly name instead of the interfaceId.
        Dim item As New ComboBoxItem()
        item.Content = TryCast(deviceInfo.Properties("System.ItemNameDisplay"), String)
        item.DataContext = deviceInfo.Id
        AssociatedPrinters.Items.Add(item)

        ' If this is the first printer to be added to the combo box, select it.
        If AssociatedPrinters.Items.Count = 1 Then
            AssociatedPrinters.SelectedIndex = 0
        End If
    End Sub

    ''' <summary>
    ''' Sends a ink status query to the selected printer.
    ''' </summary>
    ''' <param name="sender" type = "Windows.UI.Xaml.Controls.Button">A pointer to the button that the user hit to enumerate printers</param>
    ''' <param name="e">Arguments passed in by the event.</param>
    Private Sub GetInkStatus(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If AssociatedPrinters.Items.Count > 0 Then
            ' Get the printer that the user has selected to query.
            Dim selectedItem As ComboBoxItem = TryCast(AssociatedPrinters.SelectedItem, ComboBoxItem)

            ' The interfaceId is retrieved from the detail field.
            Dim interfaceId As String = TryCast(selectedItem.DataContext, String)

            Try
                ' Unsubscribe existing ink level event handler, if any.
                If printHelper IsNot Nothing Then
                    RemoveHandler printHelper.OnInkLevelReceived, AddressOf OnInkLevelReceived
                    printHelper = Nothing
                End If

                Dim context As Object = Windows.Devices.Printers.Extensions.PrintExtensionContext.FromDeviceId(interfaceId)

                ' Use the PrinterHelperClass to retrieve the bidi data and display it.
                printHelper = New PrintHelperClass(context)
                Try
                    AddHandler printHelper.OnInkLevelReceived, AddressOf OnInkLevelReceived
                    printHelper.SendInkLevelQuery()

                    rootPage.NotifyUser("Ink level query successful", NotifyType.StatusMessage)
                Catch e1 As Exception
                    rootPage.NotifyUser("Ink level query unsuccessful", NotifyType.ErrorMessage)
                End Try
            Catch e2 As Exception
                rootPage.NotifyUser("Error retrieving PrinterExtensionContext from InterfaceId", NotifyType.ErrorMessage)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' This event handler method is invoked when ink level data is available.
    ''' </summary>
    Private Sub OnInkLevelReceived(ByVal sender As Object, ByVal response As String)
        BidiOutput.Text = response
    End Sub
End Class
