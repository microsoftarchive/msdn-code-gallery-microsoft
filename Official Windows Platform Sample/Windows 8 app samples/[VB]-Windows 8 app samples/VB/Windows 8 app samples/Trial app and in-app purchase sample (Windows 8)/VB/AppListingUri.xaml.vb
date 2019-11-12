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
Imports System.Threading.Tasks
Imports SDKTemplate
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Store
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class AppListingUri
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private licenseChangeHandler As LicenseChangedEventHandler = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        Await LoadAppListingUriProxyFileAsync()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to unload
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        'If licenseChangeHandler IsNot Nothing Then
        'TODO: Verifiy
        RemoveHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf AppListingUriRefreshScenario
        'End If
        MyBase.OnNavigatingFrom(e)
    End Sub

    Private Async Function LoadAppListingUriProxyFileAsync() As Task
        Dim proxyDataFolder As StorageFolder = Await Package.Current.InstalledLocation.GetFolderAsync("data")
        Dim proxyFile As StorageFile = Await proxyDataFolder.GetFileAsync("app-listing-uri.xml")
        AddHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf AppListingUriRefreshScenario
        Await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile)
    End Function

    Private Sub AppListingUriRefreshScenario()
    End Sub

    Private Sub DisplayLink_Click(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser(CurrentAppSimulator.LinkUri.AbsoluteUri, NotifyType.StatusMessage)
    End Sub
End Class
