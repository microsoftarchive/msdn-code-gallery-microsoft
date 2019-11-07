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
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Automation
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks

''' <summary>
''' A page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ImagingProperties
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private m_futureAccess As StorageItemAccessList = StorageApplicationPermissions.FutureAccessList
    Private m_localSettings As IPropertySet = Windows.Storage.ApplicationData.Current.LocalSettings.Values
    Private m_imageProperties As ImageProperties
    Private m_fileToken As String

    ' A pointer back to the main page.
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        AddHandler App.Current.Suspending, AddressOf SaveDataToPersistedState

        ' Reset scenario state before starting.
        ResetSessionState()

        ' Attempt to load the previously saved scenario state.
        If (m_localSettings.ContainsKey("scenario1FileToken")) Then
            RestoreDataFromPersistedState()
        End If
    End Sub

    ''' <summary>
    ''' When the application is about to enter a suspended state, save the user edited properties text.
    ''' This method does not use the SuspensionManager helper class (defined in SuspensionManager.cs).
    ''' </summary>
    Private Sub SaveDataToPersistedState(sender As Object, args As SuspendingEventArgs)
        ' Only save state if we have valid data.
        If (m_fileToken <> Nothing) Then
            ' Requesting a deferral prevents the application from being immediately suspended.
            Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral()

            ' LocalSettings does not support overwriting existing items, so first clear the collection.
            ResetPersistedState()

            m_localSettings.Add("scenario1Title", TitleTextbox.Text)
            m_localSettings.Add("scenario1Keywords", KeywordsTextbox.Text)
            m_localSettings.Add("scenario1DateTaken", DateTakenTextblock.Text)
            m_localSettings.Add("scenario1Make", MakeTextblock.Text)
            m_localSettings.Add("scenario1Model", ModelTextblock.Text)
            m_localSettings.Add("scenario1Orientation", OrientationTextblock.Text)
            m_localSettings.Add("scenario1LatDeg", LatDegTextbox.Text)
            m_localSettings.Add("scenario1LatMin", LatMinTextbox.Text)
            m_localSettings.Add("scenario1LatSec", LatSecTextbox.Text)
            m_localSettings.Add("scenario1LatRef", LatRefTextbox.Text)
            m_localSettings.Add("scenario1LongDeg", LongDegTextbox.Text)
            m_localSettings.Add("scenario1LongMin", LongMinTextbox.Text)
            m_localSettings.Add("scenario1LongSec", LongSecTextbox.Text)
            m_localSettings.Add("scenario1LongRef", LongRefTextbox.Text)
            m_localSettings.Add("scenario1Exposure", ExposureTextblock.Text)
            m_localSettings.Add("scenario1FNumber", FNumberTextblock.Text)
            m_localSettings.Add("scenario1FileToken", m_fileToken)

            deferral.Complete()
        End If
    End Sub

    ''' <summary>
    ''' Reads the file token and property text from the persisted state, and calls
    ''' DisplayImageUIAsync().
    ''' </summary>
    Private Async Sub RestoreDataFromPersistedState()
        Try
            rootPage.NotifyUser("Loading image file from persisted state...", NotifyType.StatusMessage)

            m_fileToken = m_localSettings("scenario1FileToken").ToString
            Dim file As StorageFile = Await m_futureAccess.GetFileAsync(m_fileToken)
            m_imageProperties = Await file.Properties.GetImagePropertiesAsync()

            Dim propertyText As New Dictionary(Of String, String)()
            propertyText.Add("Title", m_localSettings("scenario1Title").ToString)
            propertyText.Add("Keywords", m_localSettings("scenario1Keywords").ToString)
            propertyText.Add("DateTaken", m_localSettings("scenario1DateTaken").ToString)
            propertyText.Add("Make", m_localSettings("scenario1Make").ToString)
            propertyText.Add("Model", m_localSettings("scenario1Model").ToString)
            propertyText.Add("Orientation", m_localSettings("scenario1Orientation").ToString)
            propertyText.Add("LatDeg", m_localSettings("scenario1LatDeg").ToString)
            propertyText.Add("LatMin", m_localSettings("scenario1LatMin").ToString)
            propertyText.Add("LatSec", m_localSettings("scenario1LatSec").ToString)
            propertyText.Add("LatRef", m_localSettings("scenario1LatRef").ToString)
            propertyText.Add("LongDeg", m_localSettings("scenario1LongDeg").ToString)
            propertyText.Add("LongMin", m_localSettings("scenario1LongMin").ToString)
            propertyText.Add("LongSec", m_localSettings("scenario1LongSec").ToString)
            propertyText.Add("LongRef", m_localSettings("scenario1LongRef").ToString)
            propertyText.Add("Exposure", m_localSettings("scenario1Exposure").ToString)
            propertyText.Add("FNumber", m_localSettings("scenario1FNumber").ToString)

            Await DisplayImageUIAsync(file, propertyText)

            rootPage.NotifyUser("Loaded file from persisted state: " & file.Name, NotifyType.StatusMessage)

            CloseButton.IsEnabled = True
            ApplyButton.IsEnabled = True
        Catch err As Exception
            rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            ResetSessionState()
            ResetPersistedState()
        End Try
    End Sub

    ''' <summary>
    ''' Load an image from disk and display some basic imaging properties. Invoked
    ''' when the user clicks on the Open button.
    ''' </summary>
    Private Async Sub Open_Click(sender As Object, e As RoutedEventArgs)
        ResetSessionState()
        ResetPersistedState()

        Try
            rootPage.NotifyUser("Opening image file...", NotifyType.StatusMessage)

            Dim file As StorageFile = Await Helpers.GetFileFromOpenPickerAsync()

            ' Request persisted access permissions to the file the user selected.
            ' This allows the app to directly load the file in the future without relying on a
            ' broker such as the file picker.
            m_fileToken = m_futureAccess.Add(file)

            ' Windows.Storage.FileProperties.ImageProperties provides convenience access to
            ' commonly-used properties such as geolocation and keywords. It also accepts
            ' queries for Windows property system keys such as "System.Photo.Aperture".
            m_imageProperties = Await file.Properties.GetImagePropertiesAsync()

            ' In seconds
            ' F-stop values defined by EXIF spec
            Dim requests As String() = {"System.Photo.ExposureTime", "System.Photo.FNumber"}

            Dim retrievedProps As IDictionary(Of String, Object) = Await m_imageProperties.RetrievePropertiesAsync(requests)
            Await DisplayImageUIAsync(file, GetImagePropertiesForDisplay(retrievedProps))

            rootPage.NotifyUser("Loaded file from picker: " & file.Name, NotifyType.StatusMessage)
            CloseButton.IsEnabled = True
            ApplyButton.IsEnabled = True
        Catch err As Exception
            rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            ResetPersistedState()
            ResetSessionState()
        End Try
    End Sub

    ''' <summary>
    ''' Gathers the imaging properties read from a file and formats them into a single object
    ''' that can be consumed by DisplayImageUIAsync(). This method also reads from m_imageProperties.
    ''' </summary>
    ''' <param name="retrievedProps">Contains System.Photo.ExposureTime and System.Photo.FNumber</param>
    ''' <returns>Dictionary of strings to be used in DisplayImageUI().</returns>
    Private Function GetImagePropertiesForDisplay(retrievedProps As IDictionary(Of String, Object)) As IDictionary(Of String, String)
        Dim propertyText = New Dictionary(Of String, String)()

        ' Some of the properties need to be converted/formatted.
        Dim keywordsText As String = String.Join(Environment.NewLine, m_imageProperties.Keywords)

        Dim exposureText As String
        If retrievedProps.ContainsKey("System.Photo.ExposureTime") Then
            exposureText = (CDbl(retrievedProps("System.Photo.ExposureTime")) * 1000).ToString & " ms"
        Else
            exposureText = ""
        End If

        Dim fNumberText As String
        If retrievedProps.ContainsKey("System.Photo.FNumber") Then
            fNumberText = CDbl(retrievedProps("System.Photo.FNumber")).ToString("F1")
        Else
            fNumberText = ""
        End If

        Dim orientationText As String = Helpers.GetOrientationString(m_imageProperties.Orientation)

        Dim latRefText As String = ""
        Dim longRefText As String = ""
        Dim latDegText As String = ""
        Dim latMinText As String = ""
        Dim latSecText As String = ""
        Dim longDegText As String = ""
        Dim longMinText As String = ""
        Dim longSecText As String = ""

        ' Do a simple check if GPS data exists.
        If (m_imageProperties.Latitude.HasValue) AndAlso (m_imageProperties.Longitude.HasValue) Then
            Dim latitude As Double = m_imageProperties.Latitude.Value
            Dim longitude As Double = m_imageProperties.Longitude.Value

            ' Latitude and longitude are returned as double precision numbers,
            ' but we want to convert to degrees/minutes/seconds format.
            latRefText = If((latitude >= 0), "N", "S")
            longRefText = If((longitude >= 0), "E", "W")
            Dim latDeg As Double = Math.Floor(Math.Abs(latitude))
            latDegText = latDeg.ToString
            Dim latMin As Double = Math.Floor((Math.Abs(latitude) - latDeg) * 60)
            latMinText = latMin.ToString
            latSecText = ((Math.Abs(latitude) - latDeg - latMin / 60) * 3600).ToString
            Dim longDeg As Double = Math.Floor(Math.Abs(longitude))
            longDegText = longDeg.ToString
            Dim longMin As Double = Math.Floor((Math.Abs(longitude) - longDeg) * 60)
            longMinText = longMin.ToString
            longSecText = ((Math.Abs(longitude) - longDeg - longMin / 60) * 3600).ToString
        Else
            latRefText = InlineAssignHelper(longRefText, InlineAssignHelper(latDegText, InlineAssignHelper(latMinText, InlineAssignHelper(latSecText, InlineAssignHelper(longDegText, InlineAssignHelper(longMinText, InlineAssignHelper(longSecText, "")))))))
        End If

        propertyText.Add("Title", m_imageProperties.Title)
        propertyText.Add("Keywords", keywordsText)
        propertyText.Add("DateTaken", m_imageProperties.DateTaken.ToString)
        propertyText.Add("Make", m_imageProperties.CameraManufacturer)
        propertyText.Add("Model", m_imageProperties.CameraModel)
        propertyText.Add("Orientation", orientationText)
        propertyText.Add("LatRef", latRefText)
        propertyText.Add("LongRef", longRefText)
        propertyText.Add("LatDeg", latDegText)
        propertyText.Add("LatMin", latMinText)
        propertyText.Add("LatSec", latSecText)
        propertyText.Add("LongDeg", longDegText)
        propertyText.Add("LongMin", longMinText)
        propertyText.Add("LongSec", longSecText)
        propertyText.Add("Exposure", exposureText)
        propertyText.Add("FNumber", fNumberText)

        Return propertyText
    End Function

    ''' <summary>
    ''' Asynchronously displays the image file and properties text in the UI. This method is called
    ''' when the user loads a new file, or when the app resumes from a suspended state.
    ''' </summary>
    ''' <param name="file">The image to be displayed.</param>
    ''' <param name="propertyText">Collection of property text to be displayed. Must contain the following keys:
    ''' Title, Keywords, DateTaken, Make, Model, LatRef, LongRef, LatDeg, LatMin, LatSec, LongDeg, LongMin,
    ''' LongSec, Orientation, Exposure, FNumber.</param>
    Private Async Function DisplayImageUIAsync(file As StorageFile, propertyText As IDictionary(Of String, String)) As task
        Dim src As New BitmapImage()
        src.SetSource(Await file.OpenAsync(FileAccessMode.Read))
        Image1.Source = src
        AutomationProperties.SetName(Image1, file.Name)

        TitleTextbox.Text = propertyText("Title")
        KeywordsTextbox.Text = propertyText("Keywords")
        DateTakenTextblock.Text = propertyText("DateTaken")
        MakeTextblock.Text = propertyText("Make")
        ModelTextblock.Text = propertyText("Model")
        OrientationTextblock.Text = propertyText("Orientation")
        LatDegTextbox.Text = propertyText("LatDeg")
        LatMinTextbox.Text = propertyText("LatMin")
        LatSecTextbox.Text = propertyText("LatSec")
        LatRefTextbox.Text = propertyText("LatRef")
        LongDegTextbox.Text = propertyText("LongDeg")
        LongMinTextbox.Text = propertyText("LongMin")
        LongSecTextbox.Text = propertyText("LongSec")
        LongRefTextbox.Text = propertyText("LongRef")
        ExposureTextblock.Text = propertyText("Exposure")
        FNumberTextblock.Text = propertyText("FNumber")
    End Function

    ''' <summary>
    ''' Updates the file with the user-edited properties. The m_imageProperties object
    ''' is still usable after the method completes.
    ''' </summary>
    Private Async Sub Apply_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.NotifyUser("Saving file...", NotifyType.StatusMessage)

            m_imageProperties.Title = TitleTextbox.Text

            ' Keywords are stored as an IList of strings.
            If KeywordsTextbox.Text.Length > 0 Then
                Dim keywordsArray As String() = KeywordsTextbox.Text.Split(vbLf)
                m_imageProperties.Keywords.Clear()

                For i As UInteger = 0 To keywordsArray.Length - 1
                    m_imageProperties.Keywords.Add(keywordsArray(i))
                Next
            End If

            Dim propertiesToSave As New PropertySet()

            ' Perform some simple validation of the GPS data and package it in a format
            ' better suited for writing to the file.
            Dim gpsWriteFailed As Boolean = False

            Dim latitude As Double() = {Math.Floor(Double.Parse(LatDegTextbox.Text)), Math.Floor(Double.Parse(LatMinTextbox.Text)), Double.Parse(LatSecTextbox.Text)}

            Dim longitude As Double() = {Math.Floor(Double.Parse(LongDegTextbox.Text)), Math.Floor(Double.Parse(LongMinTextbox.Text)), Double.Parse(LongSecTextbox.Text)}

            Dim latitudeRef As String = LatRefTextbox.Text.ToUpper()
            Dim longitudeRef As String = LongRefTextbox.Text.ToUpper()

            If (latitude(0) >= 0 AndAlso latitude(0) <= 90) AndAlso (latitude(1) >= 0 AndAlso latitude(1) <= 60) AndAlso (latitude(2) >= 0 AndAlso latitude(2) <= 60) AndAlso (latitudeRef = "N" OrElse latitudeRef = "S") AndAlso (longitude(0) >= 0 AndAlso longitude(0) <= 180) AndAlso (latitude(1) >= 0 AndAlso longitude(1) <= 60) AndAlso (longitude(2) >= 0 AndAlso longitude(2) <= 60) AndAlso (longitudeRef = "E" OrElse longitudeRef = "W") Then
                propertiesToSave.Add("System.GPS.LatitudeRef", latitudeRef)
                propertiesToSave.Add("System.GPS.LongitudeRef", longitudeRef)

                ' The Latitude and Longitude properties are read-only. Instead,
                ' write to System.GPS.LatitudeNumerator, LatitudeDenominator, etc.
                ' These are length 3 arrays of integers. For simplicity, the
                ' seconds data is rounded to the nearest 10000th.
                Dim latitudeNumerator As UInteger() = {CUInt(latitude(0)), CUInt(latitude(1)), CUInt(latitude(2) * 10000)}

                Dim longitudeNumerator As UInteger() = {CUInt(longitude(0)), CUInt(longitude(1)), CUInt(longitude(2) * 10000)}

                ' LatitudeDenominator and LongitudeDenominator share the same values.
                Dim denominator As UInteger() = {1, 1, 10000}

                propertiesToSave.Add("System.GPS.LatitudeNumerator", latitudeNumerator)
                propertiesToSave.Add("System.GPS.LatitudeDenominator", denominator)
                propertiesToSave.Add("System.GPS.LongitudeNumerator", longitudeNumerator)
                propertiesToSave.Add("System.GPS.LongitudeDenominator", denominator)
            Else
                gpsWriteFailed = True
            End If

            ' SavePropertiesAsync commits edits to the top level properties (e.g. Title) as
            ' well as any Windows properties contained within the propertiesToSave parameter.
            Await m_imageProperties.SavePropertiesAsync(propertiesToSave)

            rootPage.NotifyUser(If(gpsWriteFailed, "GPS data invalid; other properties successfully updated", "All properties successfully updated"), NotifyType.StatusMessage)
        Catch err As Exception
            rootPage.NotifyUser("Error: " & err.Message, NotifyType.ErrorMessage)
            ResetSessionState()
            ResetPersistedState()
        End Try
    End Sub

    ''' <summary>
    ''' Closing the file brings the scenario back to the default initialized state.
    ''' </summary>
    Private Sub Close_Click(sender As Object, e As RoutedEventArgs)
        ResetSessionState()
        ResetPersistedState()
    End Sub

    ''' <summary>
    ''' Clear all of the state for this scenario that is stored in app data.
    ''' This method does not use the SuspensionManager helper class (defined in SuspensionManager.cs).
    ''' </summary>
    Private Sub ResetPersistedState()
        m_localSettings.Remove("scenario1FileToken")
        m_localSettings.Remove("scenario1Title")
        m_localSettings.Remove("scenario1Keywords")
        m_localSettings.Remove("scenario1DateTaken")
        m_localSettings.Remove("scenario1Make")
        m_localSettings.Remove("scenario1Model")
        m_localSettings.Remove("scenario1Orientation")
        m_localSettings.Remove("scenario1LatDeg")
        m_localSettings.Remove("scenario1LatMin")
        m_localSettings.Remove("scenario1LatSec")
        m_localSettings.Remove("scenario1LatRef")
        m_localSettings.Remove("scenario1LongDeg")
        m_localSettings.Remove("scenario1LongMin")
        m_localSettings.Remove("scenario1LongSec")
        m_localSettings.Remove("scenario1LongRef")
        m_localSettings.Remove("scenario1Exposure")
        m_localSettings.Remove("scenario1FNumber")
    End Sub

    ''' <summary>
    ''' Clear all of the state that is stored in memory and in the UI.
    ''' </summary>
    Private Async Sub ResetSessionState()
        m_imageProperties = Nothing
        m_fileToken = Nothing

        CloseButton.IsEnabled = False
        ApplyButton.IsEnabled = False

        Dim placeholderImage As StorageFile = Await Package.Current.InstalledLocation.GetFileAsync("Assets\placeholder-sdk.png")
        Dim bitmapImage As New BitmapImage()
        bitmapImage.SetSource(Await placeholderImage.OpenAsync(FileAccessMode.Read))
        Image1.Source = bitmapImage
        AutomationProperties.SetName(Image1, "A placeholder image")

        TitleTextbox.Text = ""
        KeywordsTextbox.Text = ""
        DateTakenTextblock.Text = ""
        MakeTextblock.Text = ""
        ModelTextblock.Text = ""
        OrientationTextblock.Text = ""
        LatDegTextbox.Text = ""
        LatMinTextbox.Text = ""
        LatSecTextbox.Text = ""
        LatRefTextbox.Text = ""
        LongDegTextbox.Text = ""
        LongMinTextbox.Text = ""
        LongSecTextbox.Text = ""
        LongRefTextbox.Text = ""
        ExposureTextblock.Text = ""
        FNumberTextblock.Text = ""
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class
