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
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario13
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        DXFLOptionCombo.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub


    Private Async Sub DXFLOptionCombo_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim combo As ComboBox = TryCast(sender, ComboBox)

        ' Setting the DXFeatureLevel qualifier value on all default contexts. This is needed
        ' since the async operation to retrive appdata.dat as a StorageFile object will happen 
        ' on a background thread. This is the only way to set the qualifier on a default context 
        ' used in a background  thread.

        ' In a different scenario, rather than obtaining a StorageFile object, another approach
        ' that could be used would be to use the ResourceMap.GetValue method to get the resolved
        ' resource candidate, and from that get the absolute file path. In that approach, the
        ' GetValue method takes a ResourceContext parameter, and so it would be possible to set
        ' the DXFeatureLevel qualifier value on a specific context rather than setting it in all
        ' contexts within the app.

        ResourceContext.SetGlobalQualifierValue("dxfeaturelevel", combo.SelectedValue.ToString())

        ' setting up the resource URI -- an ms-appx URI will be used to access the resource using
        ' a StorageFile method

        ' In a different scenario, rather than obtaining a StorageFile object, the 
        ' ResourceMap.GetValue method could be used to get the resolved candidate, from which an 
        ' absolute file path could be obtained. However, resource references used by the APIs in 
        ' Windows.ApplicationModel.Resources and Windows.ApplicationModel.Resources.Core use 
        ' ms-resource URIs, not ms-appx URIs. Within a resource map, files are always organized 
        ' under a "Files" top-level subtree. The ms-resource URI string for the file being accessed 
        ' here would be "ms-resource:///Files/appdata/appdata.dat".

        ' The URI string "ms-appx:///..." is rooted at the app package root.
        Dim uri As New Uri("ms-appx:///appdata/appdata.dat")

        ' The resource candidate will be resolved during the call to GetFileFromApplicationUriAsync 
        ' using the default context from a background thread.

        ' File IO uses asynchronous techniques. For more info, see
        ' http://msdn.microsoft.com/en-us/library/windows/apps/hh464924.aspx

        Dim file As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(uri)
        Dim content As String = Await FileIO.ReadTextAsync(file)
        ResultText.Text = content
    End Sub


End Class ' end of class