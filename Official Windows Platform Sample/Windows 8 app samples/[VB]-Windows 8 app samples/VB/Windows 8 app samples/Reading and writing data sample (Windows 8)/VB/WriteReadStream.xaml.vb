'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports System
Imports System.Diagnostics
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class WriteReadStream
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
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Copy Strings' button.  Here we will parse the
    ''' strings contained in the ElementsToWrite text block, write them to a stream using
    ''' DataWriter, retrieve them using DataReader, and output the results in the
    ''' ElementsRead text block.
    ''' </summary>
    ''' <param name="sender">Contains information about the button that fired the event.</param>
    ''' <param name="e">Contains state information and event data associated with a routed event.</param>
    Private Async Sub TransferData(sender As Object, e As RoutedEventArgs)
        ' Initialize the in-memory stream where data will be stored.
        Using stream = New Windows.Storage.Streams.InMemoryRandomAccessStream()
            ' Create the data writer object backed by the in-memory stream.
            Using dataWriter = New Windows.Storage.Streams.DataWriter(stream)
                dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8
                dataWriter.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian

                ' Parse the input stream and write each element separately.
                Dim inputElements As String() = ElementsToWrite.Text.Split(";"c)
                For Each inputElement As String In inputElements
                    Dim inputElementSize As UInteger = dataWriter.MeasureString(inputElement)
                    dataWriter.WriteUInt32(inputElementSize)
                    dataWriter.WriteString(inputElement)
                Next

                ' Send the contents of the writer to the backing stream.
                Await dataWriter.StoreAsync()

                ' For the in-memory stream implementation we are using, the flushAsync call is superfluous,
                ' but other types of streams may require it.
                Await dataWriter.FlushAsync()

                ' In order to prolong the lifetime of the stream, detach it from the DataWriter so that it 
                ' will not be closed when Dispose() is called on dataWriter. Were we to fail to detach the 
                ' stream, the call to dataWriter.Dispose() would close the underlying stream, preventing 
                ' its subsequent use by the DataReader below.
                dataWriter.DetachStream()
            End Using

            ' Create the input stream at position 0 so that the stream can be read from the beginning.
            Using inputStream = stream.GetInputStreamAt(0)
                Using dataReader = New Windows.Storage.Streams.DataReader(inputStream)
                    ' The encoding and byte order need to match the settings of the writer we previously used.
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8
                    dataReader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian

                    ' Once we have written the contents successfully we load the stream.
                    Await dataReader.LoadAsync(CUInt(stream.Size))

                    Dim receivedStrings = ""

                    ' Keep reading until we consume the complete stream.
                    While dataReader.UnconsumedBufferLength > 0
                        ' Note that the call to readString requires a length of "code units" to read. This
                        ' is the reason each string is preceded by its length when "on the wire".
                        Dim bytesToRead As UInteger = dataReader.ReadUInt32()
                        receivedStrings += dataReader.ReadString(bytesToRead) + vbLf
                    End While

                    ' Populate the ElementsRead text block with the items we read from the stream.
                    ElementsRead.Text = receivedStrings
                End Using
            End Using
        End Using
    End Sub
End Class
