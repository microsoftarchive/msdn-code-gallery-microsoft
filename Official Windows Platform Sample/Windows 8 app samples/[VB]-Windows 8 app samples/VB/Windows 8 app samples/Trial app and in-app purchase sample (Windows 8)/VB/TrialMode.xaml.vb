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
Partial Public NotInheritable Class TrialMode
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
        Await LoadTrialModeProxyFileAsync()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to unload
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If licenseChangeHandler IsNot Nothing Then
            RemoveHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf TrialModeRefreshScenario
        End If
        MyBase.OnNavigatingFrom(e)
    End Sub

    Private Async Function LoadTrialModeProxyFileAsync() As Task
        Dim proxyDataFolder As StorageFolder = Await Package.Current.InstalledLocation.GetFolderAsync("data")
        Dim proxyFile As StorageFile = Await proxyDataFolder.GetFileAsync("trial-mode.xml")
        AddHandler CurrentAppSimulator.LicenseInformation.LicenseChanged, AddressOf TrialModeRefreshScenario
        Await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile)
        ' setup application upsell message
        Dim listing As ListingInformation = Await CurrentAppSimulator.LoadListingInformationAsync()
        PurchasePrice.Text = "You can buy the full app for: " & listing.FormattedPrice & "."
    End Function

    Private Sub TrialModeRefreshScenario()
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        If licenseInformation.IsActive Then
            If licenseInformation.IsTrial Then
                LicenseMode.Text = "Current license mode: Trial license"
            Else
                LicenseMode.Text = "Current license mode: Full license"
            End If
        Else
            LicenseMode.Text = "Current license mode: Inactive license"
        End If
    End Sub

    ''' <summary>
    ''' Invoked when remaining trial time needs to be calculated
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TrialTime_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        If licenseInformation.IsActive Then
            If licenseInformation.IsTrial Then
                Dim remainingTrialTime = (licenseInformation.ExpirationDate - DateTime.Now).Days
                rootPage.NotifyUser("You can use this app for " & remainingTrialTime & " more days before the trial period ends.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("You have a full license. The trial time is not meaningful.", NotifyType.ErrorMessage)
            End If
        Else
            rootPage.NotifyUser("You don't have a license. The trial time can't be determined.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ''' Invoked when Trial product is clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TrialProduct_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        If licenseInformation.IsActive Then
            If licenseInformation.IsTrial Then
                rootPage.NotifyUser("You are using a trial version of this app.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("You no longer have a trial version of this app.", NotifyType.ErrorMessage)
            End If
        Else
            rootPage.NotifyUser("You don't have a license for this app.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ''' Invoked when a product available in full version is clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub FullProduct_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        If licenseInformation.IsActive Then
            If licenseInformation.IsTrial Then
                rootPage.NotifyUser("You are using a trial version of this app.", NotifyType.ErrorMessage)
            Else
                rootPage.NotifyUser("You are using a fully-licensed version of this app.", NotifyType.StatusMessage)
            End If
        Else
            rootPage.NotifyUser("You don't have a license for this app.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ''' Invoked when attempting to convert trial to full
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub ConvertTrial_Click(sender As Object, e As RoutedEventArgs)
        Dim licenseInformation As LicenseInformation = CurrentAppSimulator.LicenseInformation
        rootPage.NotifyUser("Buying the full license...", NotifyType.StatusMessage)
        If licenseInformation.IsTrial Then
            Try
                Await CurrentAppSimulator.RequestAppPurchaseAsync(False)
                rootPage.NotifyUser("You successfully upgraded your app to the fully-licensed version.", NotifyType.StatusMessage)
            Catch generatedExceptionName As Exception
                rootPage.NotifyUser("The upgrade transaction failed. You still have a trial license for this app.", NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("You already bought this app and have a fully-licensed version.", NotifyType.ErrorMessage)
        End If
    End Sub
End Class
