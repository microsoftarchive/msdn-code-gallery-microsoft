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
Partial Public NotInheritable Class InAppPurchase
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
        Await LoadInAppPurchaseProxyFileAsync()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to unload
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        RemoveHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf InAppPurchaseRefreshScenario
        MyBase.OnNavigatingFrom(e)
    End Sub

    Private Async Function LoadInAppPurchaseProxyFileAsync() As Task
        Dim proxyDataFolder As StorageFolder = Await Package.Current.InstalledLocation.GetFolderAsync("data")
        Dim proxyFile As StorageFile = Await proxyDataFolder.GetFileAsync("in-app-purchase.xml")
        AddHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf InAppPurchaseRefreshScenario
        Await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile)

        ' setup application upsell message
        Dim listing As ListingInformation = Await CurrentAppSimulator.LoadListingInformationAsync()
        Dim product1 = listing.ProductListings("product1")
        Dim product2 = listing.ProductListings("product2")
        Product1SellMessage.Text = "You can buy " & product1.Name & " for: " & product1.FormattedPrice & "."
        Product2SellMessage.Text = "You can buy " & product2.Name & " for: " & product2.FormattedPrice & "."
    End Function

    Private Sub InAppPurchaseRefreshScenario()
    End Sub

    Private Sub TryProduct1_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        Dim productLicense = licenseInformation.ProductLicenses("product1")
        If productLicense.IsActive Then
            rootPage.NotifyUser("You can use Product 1.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Async Sub BuyProduct1Button_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        Dim productLicense = licenseInformation.ProductLicenses("product1")
        If Not productLicense.IsActive Then
            rootPage.NotifyUser("Buying Product 1...", NotifyType.StatusMessage)
            Try
                Await CurrentAppSimulator.RequestProductPurchaseAsync("product1", False)
                rootPage.NotifyUser("You bought Product 1.", NotifyType.StatusMessage)
            Catch generatedExceptionName As Exception
                rootPage.NotifyUser("Unable to buy Product 1.", NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("You already own Product 1.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub TryProduct2_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        Dim productLicense = licenseInformation.ProductLicenses("product2")
        If productLicense.IsActive Then
            rootPage.NotifyUser("You can use Product 2.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("You don't own Product 2. You must buy Product 2 before you can use it.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Async Sub BuyProduct2_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        Dim productLicense = licenseInformation.ProductLicenses("product2")
        If Not productLicense.IsActive Then
            rootPage.NotifyUser("Buying Product 2...", NotifyType.StatusMessage)
            Try
                Await CurrentAppSimulator.RequestProductPurchaseAsync("product2", False)
                rootPage.NotifyUser("You bought Product 2.", NotifyType.StatusMessage)
            Catch generatedExceptionName As Exception
                rootPage.NotifyUser("Unable to buy Product 2.", NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("You already own Product 2.", NotifyType.ErrorMessage)
        End If
    End Sub
End Class
