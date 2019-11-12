'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Media.Imaging
Imports SDKTemplate
Imports Windows.Devices.Enumeration
Imports Windows.Devices.Enumeration.Pnp



''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Interfaces
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub InterfaceClasses_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If InterfaceClasses.SelectedItem Is PrinterInterfaceClass Then
            InterfaceClassGuid.Text = "{0ECEF634-6EF0-472A-8085-5AD023ECBCCD}"
        ElseIf InterfaceClasses.SelectedItem Is WebcamInterfaceClass Then
            InterfaceClassGuid.Text = "{E5323777-F976-4F5B-9B55-B94699C46E44}"
        ElseIf InterfaceClasses.SelectedItem Is WpdInterfaceClass Then
            InterfaceClassGuid.Text = "{6AC27878-A6FA-4155-BA85-F98F491D4F33}"
        End If
    End Sub

    Private Async Sub EnumerateDeviceInterfaces(sender As Object, eventArgs As RoutedEventArgs)
        Dim focusState As FocusState = EnumerateInterfacesButton.FocusState
        DeviceInterfacesOutputList.Items.Clear()
        EnumerateInterfacesButton.IsEnabled = False

        Try
            Dim selector = "System.Devices.InterfaceClassGuid:=""" & InterfaceClassGuid.Text & """"
            Dim interfaces = Await DeviceInformation.FindAllAsync(selector, Nothing)

            rootPage.NotifyUser(interfaces.Count & " device interface(s) found" & vbLf & vbLf, NotifyType.StatusMessage)
            For Each deviceInterface As DeviceInformation In interfaces
                Dim thumbnail As DeviceThumbnail = Await deviceInterface.GetThumbnailAsync()
                Dim glyph As DeviceThumbnail = Await deviceInterface.GetGlyphThumbnailAsync()
                DeviceInterfacesOutputList.Items.Add(New DisplayItem(deviceInterface, thumbnail, glyph))
            Next
        Catch generatedExceptionName As ArgumentException
            'The ArgumentException gets thrown by FindAllAsync when the GUID isn't formatted properly
            'The only reason we're catching it here is because the user is allowed to enter GUIDs without validation
            'In normal usage of the API, this exception handling probably wouldn't be necessary when using known-good GUIDs
            rootPage.NotifyUser("Caught ArgumentException. Verify that you've entered a valid interface class GUID.", NotifyType.ErrorMessage)
        End Try

        EnumerateInterfacesButton.IsEnabled = True
        EnumerateInterfacesButton.Focus(focusState)
    End Sub

    Private Class DisplayItem
        Public Sub New(deviceInterface As DeviceInformation, thumbnail As DeviceThumbnail, glyph As DeviceThumbnail)
            m_name = DirectCast(deviceInterface.Properties("System.ItemNameDisplay"), String)

            m_id = "ID: " & deviceInterface.Id
            m_isEnabled = "IsEnabled: " & deviceInterface.IsEnabled
            thumb = thumbnail
            glyphThumb = glyph
        End Sub

        Public ReadOnly Property Name() As String
            Get
                Return m_name
            End Get
        End Property
        Public ReadOnly Property Id() As String
            Get
                Return m_id
            End Get
        End Property
        Public ReadOnly Property IsEnabled() As String
            Get
                Return m_isEnabled
            End Get
        End Property
        Public ReadOnly Property Thumbnail() As BitmapImage
            Get
                Dim bmp = New BitmapImage()
                bmp.SetSource(thumb)
                Return bmp
            End Get
        End Property

        Public ReadOnly Property GlyphThumbnail() As BitmapImage
            Get
                Dim bmp = New BitmapImage()
                bmp.SetSource(glyphThumb)
                Return bmp
            End Get
        End Property

        ReadOnly m_name As String
        ReadOnly m_id As String
        ReadOnly m_isEnabled As String
        ReadOnly thumb As DeviceThumbnail
        ReadOnly glyphThumb As DeviceThumbnail
    End Class

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
