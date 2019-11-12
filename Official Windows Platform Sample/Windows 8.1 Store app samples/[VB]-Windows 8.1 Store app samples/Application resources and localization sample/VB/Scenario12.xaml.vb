'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.Resources.Core

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario12
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Async Sub Scenario12Button_Show_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Two coding patterns will be used:
        '   1. Get a ResourceContext on the UI thread using GetForCurrentView and pass 
        '      to the non-UI thread
        '   2. Get a ResourceContext on the non-UI thread using GetForViewIndependentUse
        '
        ' Two analogous patterns could be used for ResourceLoader instead of ResourceContext.


        ' pattern 1: get a ResourceContext for the UI thread
        Dim defaultContextForUiThread As ResourceContext = ResourceContext.GetForCurrentView()

        ' pattern 2: we'll create a view-independent context in the non-UI worker thread


        ' We need some things in order to display results in the UI (doing that
        ' for purposes of this sample, to show that work was actually done in the
        ' worker thread):
        Dim uiDependentResourceList As New List(Of String)()
        Dim uiIndependentResourceList As New List(Of String)()


        ' use a worker thread for the heavy lifting so the UI isn't blocked
        Await Windows.System.Threading.ThreadPool.RunAsync(Sub(source)
                                                               ' pattern 1: the defaultContextForUiThread variable was created above and is visible here
                                                               ' pattern 2: get a view-independent ResourceContext
                                                               ' NOTE: The ResourceContext obtained using GetForViewIndependentUse() has no scale qualifier
                                                               ' value set. If this ResourceContext is used in its default state to retrieve a resource, that 
                                                               ' will work provided that the resource does not have any scale-qualified variants. But if the
                                                               ' resource has any scale-qualified variants, then that will fail at runtime.
                                                               '
                                                               ' A scale qualifier value on this ResourceContext can be set programmatically. If that is done,
                                                               ' then the ResourceContext can be used to retrieve a resource that has scale-qualified variants.
                                                               ' But if the scale qualifier is reset (e.g., using the ResourceContext.Reset() method), then
                                                               ' it will return to the default state with no scale qualifier value set, and cannot be used
                                                               ' to retrieve any resource that has scale-qualified variants.
                                                               ' simulate processing a number of items
                                                               ' just using a single string resource: that's sufficient to demonstrate 
                                                               ' pattern 1: use the ResourceContext from the UI thread
                                                               ' pattern 2: use the view-independent ResourceContext
                                                               Dim stringResourceMap As ResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources")
                                                               Dim defaultViewIndependentResourceContext As ResourceContext = ResourceContext.GetForViewIndependentUse()
                                                               For i = 0 To 3
                                                                   Dim listItem1 As String = stringResourceMap.GetValue("string1", defaultContextForUiThread).ValueAsString
                                                                   uiDependentResourceList.Add(listItem1)
                                                                   Dim listItem2 As String = stringResourceMap.GetValue("string1", defaultViewIndependentResourceContext).ValueAsString
                                                                   uiIndependentResourceList.Add(listItem2)
                                                               Next i
                                                           End Sub)

        ' Display the results in one go. (A more finessed design might add results
        ' in the UI asynchronously, but that goes beyond what this sample is 
        ' demonstrating.)
        ViewDependentResourcesList.ItemsSource = uiDependentResourceList
        ViewIndependentResourcesList.ItemsSource = uiIndependentResourceList
    End Sub

End Class
