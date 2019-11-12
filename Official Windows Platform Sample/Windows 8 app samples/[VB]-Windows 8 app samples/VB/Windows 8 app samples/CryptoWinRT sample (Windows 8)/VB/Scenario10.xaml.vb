'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Security.Cryptography
Imports Windows.Security.Cryptography.Core
Imports Windows.Security.Cryptography.DataProtection
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario10
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

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
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Public Async Sub SampleDataProtectionStream(descriptor As String)
        Scenario10Text.Text &= "*** Sample Stream Data Protection for " & descriptor & " ***" & vbCrLf

        Dim data As IBuffer = CryptographicBuffer.GenerateRandom(10000)
        Dim reader1 As DataReader, reader2 As DataReader
        Dim buff1 As IBuffer, buff2 As IBuffer

        Dim Provider As New DataProtectionProvider(descriptor)
        Dim originalData As New InMemoryRandomAccessStream

        'Populate the new memory stream
        Dim outputStream As IOutputStream = originalData.GetOutputStreamAt(0)
        Dim writer As New DataWriter(outputStream)
        writer.WriteBuffer(data)
        Await writer.StoreAsync
        Await outputStream.FlushAsync

        'open new memory stream for read
        Dim source As IInputStream = originalData.GetInputStreamAt(0)

        'Open the output memory stream
        Dim protectedData As New InMemoryRandomAccessStream
        Dim dest As IOutputStream = protectedData.GetOutputStreamAt(0)

        ' Protect
        Await Provider.ProtectStreamAsync(source, dest)

        'Flush the output
        If Await dest.FlushAsync Then
            Scenario10Text.Text &= "    Protected output was successfully flushed" & vbCrLf
        End If

        'Verify the protected data does not match the original
        reader1 = New DataReader(originalData.GetInputStreamAt(0))
        reader2 = New DataReader(protectedData.GetInputStreamAt(0))

        Await reader1.LoadAsync(CUInt(originalData.Size))
        Await reader2.LoadAsync(CUInt(protectedData.Size))

        Scenario10Text.Text &= "    Size of original stream:  " & originalData.Size & vbCrLf
        Scenario10Text.Text &= "    Size of protected stream:  " & protectedData.Size & vbCrLf

        If originalData.Size = protectedData.Size Then
            buff1 = reader1.ReadBuffer(CUInt(originalData.Size))
            buff2 = reader2.ReadBuffer(CUInt(protectedData.Size))
            If CryptographicBuffer.Compare(buff1, buff2) Then
                Scenario10Text.Text &= "ProtectStreamAsync returned unprotected data"
                Exit Sub
            End If
        End If

        Scenario10Text.Text &= "    Stream Compare completed.  Streams did not match." & vbCrLf

        source = protectedData.GetInputStreamAt(0)

        Dim unprotectedData As New InMemoryRandomAccessStream
        dest = unprotectedData.GetOutputStreamAt(0)

        ' Unprotect
        Dim Provider2 As New DataProtectionProvider
        Await Provider2.UnprotectStreamAsync(source, dest)

        If Await dest.FlushAsync Then
            Scenario10Text.Text &= "    Unprotected output was successfully flushed" & vbCrLf
        End If

        'Verify the unprotected data does match the original
        reader1 = New DataReader(originalData.GetInputStreamAt(0))
        reader2 = New DataReader(unprotectedData.GetInputStreamAt(0))

        Await reader1.LoadAsync(CUInt(originalData.Size))
        Await reader2.LoadAsync(CUInt(unprotectedData.Size))

        Scenario10Text.Text &= "    Size of original stream:  " & originalData.Size & vbCrLf
        Scenario10Text.Text &= "    Size of unprotected stream:  " & unprotectedData.Size & vbCrLf

        buff1 = reader1.ReadBuffer(CUInt(originalData.Size))
        buff2 = reader2.ReadBuffer(CUInt(unprotectedData.Size))
        If Not CryptographicBuffer.Compare(buff1, buff2) Then
            Scenario10Text.Text &= "UnrotectStreamAsync did not return expected data"
            Exit Sub
        End If

        Scenario10Text.Text &= "*** Done!" & vbCrLf
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunSample_Click(sender As Object, e As RoutedEventArgs)
        Dim descriptor As String = tbDescriptor.Text
        SampleDataProtectionStream(descriptor)
    End Sub
End Class
