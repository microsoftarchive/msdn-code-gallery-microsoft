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
Imports SDKTemplate
Imports System
Imports Windows.ApplicationModel.Store
Imports System.Threading.Tasks
Imports Windows.Storage
Imports Windows.ApplicationModel

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ExpiringProduct
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
        Await LoadExpiringProductProxyFileAsync()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to unload
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        RemoveHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf ExpiringProductRefreshScenario
        MyBase.OnNavigatingFrom(e)
    End Sub

    Private Async Function LoadExpiringProductProxyFileAsync() As Task
        Dim proxyDataFolder As StorageFolder = Await Package.Current.InstalledLocation.GetFolderAsync("data")
        Dim proxyFile As StorageFile = Await proxyDataFolder.GetFileAsync("expiring-product.xml")
        AddHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf ExpiringProductRefreshScenario
        Await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile)
    End Function

    Private Sub ExpiringProductRefreshScenario()
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        If licenseInformation.IsActive Then
            If licenseInformation.IsTrial Then
                rootPage.NotifyUser("You don't have full license", NotifyType.ErrorMessage)
            Else
                Dim productLicense1 = licenseInformation.ProductLicenses("product1")
                If productLicense1.IsActive Then
                    Dim longdateTemplate = New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate")
                    Product1Message.Text = "Product 1 expires on: " & longdateTemplate.Format(productLicense1.ExpirationDate)
                    Dim remainingDays = (productLicense1.ExpirationDate - DateTime.Now).Days
                    rootPage.NotifyUser("Product 1 expires in: " & remainingDays & " days.", NotifyType.StatusMessage)
                Else
                    rootPage.NotifyUser("Product 1 is not available.", NotifyType.ErrorMessage)
                End If
            End If
        Else
            rootPage.NotifyUser("You don't have active license.", NotifyType.ErrorMessage)
        End If
    End Sub
End Class
