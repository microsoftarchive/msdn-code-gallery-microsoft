'****************************** Module Header ******************************\
' Module Name:  ItemsPage.xaml.vb
' Project:      VBWindowsStoreAppFlightDataFilter
' Copyright (c) Microsoft Corporation.
'
' ItemsPage. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports VBWindowsStoreAppFlightDataFilter.ViewModel
' The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

''' <summary>
''' A page that displays a collection of item previews.  In the Split Application this page
''' is used to display and select one of the available groups.
''' </summary>
Public NotInheritable Class ItemsPage
    Inherits Page

    ''' <summary>
    ''' NavigationHelper is used on each page to aid in navigation and 
    ''' process lifetime management
    ''' </summary>
    Public ReadOnly Property NavigationHelper As Common.NavigationHelper
        Get
            Return Me._navigationHelper
        End Get
    End Property
    Private _navigationHelper As Common.NavigationHelper

    ''' <summary>
    ''' This can be changed to a strongly typed view model.
    ''' </summary>
    Public ReadOnly Property DefaultViewModel As MainViewModel
        Get
            Return Me._defaultViewModel
        End Get
    End Property
    Private _defaultViewModel As New MainViewModel()

    Public Sub New()
        InitializeComponent()
        Me._navigationHelper = New Common.NavigationHelper(Me)
        AddHandler Me._navigationHelper.LoadState, AddressOf NavigationHelper_LoadState
        AddHandler Me.SizeChanged, AddressOf ItemsPage_SizeChanged
    End Sub

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event; typically <see cref="NavigationHelper"/>
    ''' </param>
    ''' <param name="e">Event data that provides both the navigation parameter passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested and
    ''' a dictionary of state preserved by this page during an earlier
    ''' session.  The state will be null the first time a page is visited.</param>
    Private Sub NavigationHelper_LoadState(sender As Object, e As Common.LoadStateEventArgs)
        ' TODO: Assign a bindable collection of items to Me.DefaultViewModel("Items")
    End Sub

#Region "NavigationHelper registration"

    ''' The methods provided in this section are simply used to allow
    ''' NavigationHelper to respond to the page's navigation methods.
    ''' 
    ''' Page specific logic should be placed in event handlers for the  
    ''' <see cref="Common.NavigationHelper.LoadState"/>
    ''' and <see cref="Common.NavigationHelper.SaveState"/>.
    ''' The navigation parameter is available in the LoadState method 
    ''' in addition to page state preserved during an earlier session.

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedFrom(e)
    End Sub

#End Region

    Private Async Sub Footer_Click(sender As Object, e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(TryCast(sender, HyperlinkButton).Tag.ToString()))
    End Sub

    Private Sub ItemsPage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If e.NewSize.Width <= 500 Then
            VisualStateManager.GoToState(Me, "MinimalLayout", True)
        ElseIf e.NewSize.Width < e.NewSize.Height Then
            VisualStateManager.GoToState(Me, "PortraitLayout", True)
        Else
            VisualStateManager.GoToState(Me, "DefaultLayout", True)
        End If
    End Sub
End Class
