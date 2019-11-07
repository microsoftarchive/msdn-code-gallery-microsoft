'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Files
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private roamingFolder As StorageFolder = Nothing
    Private counter As Integer = 0

    Const filename As String = "sampleFile.txt"

    Public Sub New()
        Me.InitializeComponent()

        roamingFolder = ApplicationData.Current.RoamingFolder

        DisplayOutput()
    End Sub

    ' Guidance for Local, Roaming, and Temporary files.
    '
    ' Files are ideal for storing large data-sets, databases, or data that is
    ' in a common file-format.
    '
    ' Files can exist in either the Local, Roaming, or Temporary folders.
    '
    ' Roaming files will be synchronized across machines on which the user has
    ' singed in with a connected account.  Roaming of files is not instant; the
    ' system weighs several factors when determining when to send the data.  Usage
    ' of roaming data should be kept below the quota (available via the 
    ' RoamingStorageQuota property), or else roaming of data will be suspended.
    ' Files cannot be roamed while an application is writing to them, so be sure
    ' to close your application's file objects when they are no longer needed.
    '
    ' Local files are not synchronized and remain on the machine on which they
    ' were originally written.
    '
    ' Temporary files are subject to deletion when not in use.  The system 
    ' considers factors such as available disk capacity and the age of a file when
    ' determining when or whether to delete a temporary file.

    ' This sample illustrates reading and writing a file in the Roaming folder, though a
    ' Local or Temporary file could be used just as easily.

    Private Async Sub Increment_Click(sender As Object, e As RoutedEventArgs)
        counter += 1

        Dim file As StorageFile = Await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting)
        Await FileIO.WriteTextAsync(file, counter.ToString())

        DisplayOutput()
    End Sub

    Private Async Sub ReadCounter()
        Try
            Dim file As StorageFile = Await roamingFolder.GetFileAsync(filename)
            Dim text As String = Await FileIO.ReadTextAsync(file)
            OutputTextBlock.Text = "Counter: " & text
            counter = Integer.Parse(text)
        Catch generatedExceptionName As Exception
            OutputTextBlock.Text = "Counter: <not found>"
        End Try
    End Sub

    Private Sub DisplayOutput()
        ReadCounter()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
