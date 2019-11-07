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
Imports Windows.UI.Xaml.Media
Imports Windows.Devices.Printers.Extensions

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Preferences
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private configuration As PrintTaskConfiguration
    Private printerExtensionContext As Object
    Private printHelper As PrintHelperClass

    ' The features in this sample were chosen because they're available on a wide range
    ' of printer drivers.
    Private features() As String = {"PageOrientation", "PageOutputColor", "PageMediaSize", "PageMediaType"}
    Private selections() As String = {Nothing, Nothing, Nothing, Nothing}

    Public Sub New()
        Me.InitializeComponent()

        configuration = rootPage.Config
        printerExtensionContext = rootPage.Context
        printHelper = New PrintHelperClass(printerExtensionContext)

        ' Disable scenario navigation by hiding the scenario list UI elements
        CType(rootPage.FindName("Scenarios"), UIElement).Visibility = Windows.UI.Xaml.Visibility.Collapsed
        CType(rootPage.FindName("ScenarioListLabel"), UIElement).Visibility = Windows.UI.Xaml.Visibility.Collapsed
        CType(rootPage.FindName("DescriptionText"), UIElement).Visibility = Windows.UI.Xaml.Visibility.Collapsed

        DisplaySettings()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        If Nothing Is configuration Then
            rootPage.NotifyUser("Configuration arguments cannot be null", NotifyType.ErrorMessage)
            Return
        End If

        ' Add an event listener for saverequested (the back button of the flyout is pressed).
        AddHandler configuration.SaveRequested, AddressOf OnSaveRequested
    End Sub

    ''' <summary>
    ''' Displays the advanced print setting information.
    ''' </summary>
    ''' <param name="refreshed">Boolean representing the method should check for constraints,
    ''' defaults to false unless specified.</param>
    Private Sub DisplaySettings(Optional ByVal constraints As Boolean = False)
        PrintOptions.Visibility = Windows.UI.Xaml.Visibility.Visible
        WaitPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed

        ' Fill in the drop-down select controls for some common printing features.
        Dim featureLabels() As TextBlock = {PageOrientationLabel, PageOutputColorLabel, PageMediaSizeLabel, PageMediaTypeLabel}
        Dim featureBoxes() As ComboBox = {PageOrientationBox, PageOutputColorBox, PageMediaSizeBox, PageMediaTypeBox}

        For i As Integer = 0 To features.Length - 1
            ' Only display a feature if it exists
            featureLabels(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed
            featureBoxes(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Dim feature As String = features(i)

            ' Check whether the currently selected printer's capabilities include this feature.
            If Not printHelper.FeatureExists(feature) Then
                Continue For
            End If

            ' Fill in the labels so that they display the display name of each feature.
            featureLabels(i).Text = printHelper.GetFeatureDisplayName(feature)
            Dim index() As String = printHelper.GetOptionInfo(feature, "Index")
            Dim displayName() As String = printHelper.GetOptionInfo(feature, "DisplayName")
            Dim selectedOption As String = printHelper.GetSelectedOptionIndex(feature)

            ' Unless specified, do not get constraints
            Dim constrainedList() As Boolean = If(constraints, printHelper.GetOptionConstraints(feature), New Boolean(index.Length - 1) {})

            ' Populate the combo box with the options for the current feature.
            PopulateBox(featureBoxes(i), index, displayName, selectedOption, constrainedList)
            selections(i) = selectedOption

            ' Everytime the selection for a feature changes, we update our local cached set of selections.
            AddHandler featureBoxes(i).SelectionChanged, AddressOf OnFeatureOptionsChanged

            ' Show existing features
            featureLabels(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            featureBoxes(i).Visibility = Windows.UI.Xaml.Visibility.Visible
        Next i
    End Sub

    ''' <summary>
    ''' Populate the combo box with the options for the current feature.
    ''' </summary>
    ''' <param name="box">The combo box to be populated.</param>
    ''' <param name="index">The index of the option.</param>
    ''' <param name="displayName">The display name that the user sees instead of the index.</param>
    ''' <param name="selectedOption">The option that is selected right now.</param>
    Private Sub PopulateBox(ByVal box As ComboBox, ByVal index() As String, ByVal displayName() As String, ByVal selectedOption As String, ByVal constrainedList() As Boolean)
        ' Clear the combobox of any options from previous UI refresh before repopulating it.
        RemoveHandler box.SelectionChanged, AddressOf OnFeatureOptionsChanged
        box.Items.Clear()
        ' There should be only one displayName for each possible option.
        If index.Length = displayName.Length Then
            For i As Integer = 0 To index.Length - 1
                ' Create a new DisplayItem so the user will see the friendly displayName instead of the index.
                Dim newItem As New ComboBoxItem()
                newItem.Content = displayName(i)
                newItem.DataContext = index(i)
                newItem.Foreground = If(constrainedList(i), New SolidColorBrush(Colors.Red), New SolidColorBrush(Colors.Black))
                box.Items.Add(newItem)

                ' Display current selected option as selected in the combo box.
                If selectedOption = index(i) Then
                    box.SelectedIndex = i
                    box.Foreground = newItem.Foreground
                End If
            Next i
        End If
    End Sub



    ''' <summary>
    ''' When the user changed any selection, update local cached selection.
    ''' </summary>
    ''' <param name="sender" type="Windows.UI.Xaml.Combobox">The combobox whose selection has been changed.</param>
    ''' <param name="args"></param>
    Private Sub OnFeatureOptionsChanged(ByVal sender As Object, ByVal args As SelectionChangedEventArgs)
        Dim comboBox As ComboBox = TryCast(sender, ComboBox)

        For i As Integer = 0 To features.Length - 1
            If features(i) & "Box" = comboBox.Name Then
                selections(i) = TryCast((TryCast(comboBox.SelectedItem, ComboBoxItem)).DataContext, String)
            End If
        Next i
    End Sub

    ''' <summary>
    ''' The event handler that is fired when the user clicks on the back button.
    ''' Saves the print ticket.
    ''' This is fired on a different thread than the main thread so will need a dispatcher to get back on the main thread to access objects created on the main thread.
    ''' </summary>
    ''' <param name="sender">The back button.</param>
    ''' <param name="args">Arguments passed in by the event.</param>
    Private Async Sub OnSaveRequested(ByVal sender As Object, ByVal args As PrintTaskConfigurationSaveRequestedEventArgs)
        If Nothing Is printHelper OrElse Nothing Is printerExtensionContext OrElse Nothing Is args Then
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser("onSaveRequested: args, printHelper, and context cannot be null", NotifyType.ErrorMessage))
            Return
        End If

        ' Get the request object, which has the save method that allows saving updated print settings.
        Dim request As PrintTaskConfigurationSaveRequest = args.Request

        If Nothing Is request Then
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser("onSaveRequested: request cannot be null", NotifyType.ErrorMessage))
            Return
        End If

        Dim deferral As PrintTaskConfigurationSaveRequestedDeferral = request.GetDeferral()

        ' Two separate messages are dispatched to:
        ' 1) put up a popup panel,
        ' 2) set the each options to the print ticket and attempt to save it,
        ' 3) tear down the popup panel if the print ticket could not be saved.
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     PrintOptions.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                                                                                     WaitPanel.Visibility = Windows.UI.Xaml.Visibility.Visible
                                                                                 End Sub)

        ' Go through all the feature select elements, look up the selected
        ' option name, and update the context
        ' for each feature
        For i = 0 To features.Length - 1
            ' Set the feature's selected option in the context's print ticket.
            ' The printerExtensionContext object is updated with each iteration of this loop
            printHelper.SetFeatureOption(features(i), selections(i))
        Next i

        Dim ticketSaved As Boolean
        Try
            ' This save request will throw an exception if ticket validation fails.
            ' When the exception is thrown, the app flyout will remain.
            ' If you want the flyout to remain regardless of outcome, you can call
            ' request.Cancel(). This should be used sparingly, however, as it could
            ' disrupt the entire the print flow and will force the user to 
            ' light dismiss to restart the entire experience.
            request.Save(printerExtensionContext)

            If configuration IsNot Nothing Then
                RemoveHandler configuration.SaveRequested, AddressOf OnSaveRequested
            End If
            ticketSaved = True
        Catch exp As Exception
            ' Check if the HResult from the exception is from an invalid ticket, otherwise rethrow the exception
            If exp.HResult.Equals(CInt(&H8007000D)) Then ' E_INVALID_DATA
                ticketSaved = False
            Else
                Throw
            End If
        End Try

        ' If ticket isn't saved, refresh UI and notify user
        If Not ticketSaved Then
            Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         rootPage.NotifyUser("Failed to save the print ticket", NotifyType.ErrorMessage)
                                                                                         DisplaySettings(True)
                                                                                     End Sub)
        End If
        deferral.Complete()
    End Sub

End Class
