' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices.WindowsRuntime
Imports System.Threading.Tasks
Imports Windows.Foundation

' Printer Extension types library
Imports Microsoft.Samples.Printing.PrinterExtension
Imports Microsoft.Samples.Printing.PrinterExtension.Types

''' <summary>
''' This WinMD library works with the print ticket from the printer extension context
''' passed in from JavaScript.  It looks up available and
''' currently selected options for print features, and surfaces this information
''' to JavaScript as strings or arrays of strings.  It's also used for setting options.
''' </summary>
Public NotInheritable Class PrintHelperClass
    Private Enum OptionInfoType
        DisplayName
        Index
    End Enum

    ''' <summary>
    ''' List of capabilities that this printer queue supports.
    ''' It is very important that the print capabilities is retrieved only as many times as absolutely required because:
    '''     1. It is an expensive operation in terms of time.
    '''     2. The returning capabilities could be in a different order as before.
    ''' </summary>
    Private _capabilities As IPrintSchemaCapabilities

    ''' <summary>
    ''' The printer extension context of this printer queue.
    ''' </summary>
    Private context As PrinterExtensionContext

    ''' <summary>
    ''' A dictionary of the IPrintSchema options.
    ''' </summary>
    Private featureOptions As New Dictionary(Of String, List(Of IPrintSchemaOption))()

    ''' <summary>
    ''' The IPrinterQueue object of the printer.
    ''' </summary>
    Private printerQueue As IPrinterQueue

    ''' <summary>
    ''' Represents the event that is raised when ink level data is available.
    ''' </summary>
    Public Event OnInkLevelReceived As EventHandler(Of String)

    ''' <summary>
    ''' Represents the dispatcher object for the main UI thread. This is required because in JavaScript,
    ''' events handlers need to be invoked on the UI thread. This isn't a requirement for VB code though.
    ''' </summary>
    Private dispatcher As Windows.UI.Core.CoreDispatcher

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="context">The PrinterExtensionContext from the app.</param>
    Public Sub New(ByVal context As Object)
        Me.context = New PrinterExtensionContext(context)
        printerQueue = Me.context.Queue
        dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher
    End Sub

    ''' <summary>
    ''' Destructor
    ''' </summary>
    Protected Overrides Sub Finalize()
        ' Remove the event handler for the bidi response.
        Try
            RemoveHandler printerQueue.OnBidiResponseReceived, AddressOf OnBidiResponseReceived
        Catch e1 As Exception
            ' Destructors must not throw any exceptions.
        End Try
    End Sub

    ''' <summary>
    ''' Get a list of options for a specified feature.
    ''' </summary>
    ''' <param name="feature">The feature whose options will be retrieved</param>
    ''' <returns></returns>
    Private Function GetCachedFeatureOptions(ByVal feature As String) As List(Of IPrintSchemaOption)
        If False = featureOptions.ContainsKey(feature) Then
            ' The first time this feature's options are retrieved, cache a copy of the list
            featureOptions(feature) = Capabilities.GetOptions(Capabilities.GetFeatureByKeyName(feature)).ToList()
        End If
        Return featureOptions(feature)
    End Function

    ''' <summary>
    ''' Returns the capabilities for the context's print ticket.  This method only looks up the
    ''' capabilities the first time it's called, and it returns the cached capabilities on subsequent calls
    ''' to avoid making a repeated expensive call to retrieve the capabilities from the print ticket.
    ''' </summary>
    Private ReadOnly Property Capabilities() As IPrintSchemaCapabilities
        Get
            If Nothing IsNot _capabilities Then
                Return _capabilities
            Else
                If Nothing Is context OrElse Nothing Is context.Ticket Then
                    Return Nothing
                End If

                _capabilities = context.Ticket.GetCapabilities()
                Return _capabilities
            End If
        End Get
    End Property

    ''' <summary>
    ''' Retrieve the ink level, and raise the onInkLevelReceived event when data is available.
    ''' </summary>
    Public Sub SendInkLevelQuery()
        AddHandler printerQueue.OnBidiResponseReceived, AddressOf OnBidiResponseReceived

        ' Send the query.
        Dim queryString As String = "\Printer.Consumables"
        printerQueue.SendBidiQuery(queryString)
    End Sub


    ''' <summary>
    ''' This event handler is invoked when a Bidi response is raised.
    ''' </summary>
    Private Async Sub OnBidiResponseReceived(ByVal sender As Object, ByVal responseArguments As PrinterQueueEventArgs)
        ' Invoke the ink level event with appropriate data.
        ' Dispatching this event invocation to the UI thread is required because in JavaScript,
        ' events handlers need to be invoked on the UI thread.
        Await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() RaiseEvent OnInkLevelReceived(sender, ParseResponse(responseArguments)))
    End Sub

    ''' <summary>
    ''' Parse the bidi response argument.
    ''' </summary>
    ''' <param name="responseArguments">The bidi response</param>
    ''' <returns>A string that contains either the bidi response string or explains the invalid result.</returns>
    Private Function ParseResponse(ByVal responseArguments As PrinterQueueEventArgs) As String
        If responseArguments.StatusHResult = CInt(HRESULT.S_OK) Then
            Return responseArguments.Response
        Else
            Return InvalidHResult(responseArguments.StatusHResult)
        End If
    End Function

    ''' <summary>
    ''' Displays the invalid result returned by the bidi query as a string output statement.
    ''' </summary>
    ''' <param name="result">The HRESULT returned by the bidi query, assumed to be not HRESULT.S_OK.</param>
    ''' <returns>A string that can be displayed to the user explaining the HRESULT.</returns>
    Private Function InvalidHResult(ByVal result As Integer) As String
        Select Case result
            Case CInt(HRESULT.E_INVALIDARG)
                Return "Invalid Arguments"
            Case CInt(HRESULT.E_OUTOFMEMORY)
                Return "Out of Memory"
            Case CInt(HRESULT.ERROR_NOT_FOUND)
                Return "Not found"
            Case CInt(HRESULT.S_FALSE)
                Return "False"
            Case CInt(HRESULT.S_PT_NO_CONFLICT)
                Return "PT No Conflict"
            Case Else
                Return "Undefined status: 0x" & result.ToString("X")
        End Select
    End Function

    ''' <summary>
    ''' Determines whether the print capabilities contain the specified feature.
    ''' </summary>
    ''' <param name="feature">The feature to search the capabilities for</param>
    ''' <returns>True if the capabilities and the ticket contain the specified feature, False if the feature was not found</returns>
    Public Function FeatureExists(ByVal feature As String) As Boolean
        If String.IsNullOrWhiteSpace(feature) Then
            Return False
        End If

        If Nothing IsNot Capabilities Then
            Dim capsFeature As IPrintSchemaFeature = Capabilities.GetFeatureByKeyName(feature)
            Dim ticketFeature As IPrintSchemaFeature = context.Ticket.GetFeatureByKeyName(feature)

            If capsFeature IsNot Nothing AndAlso ticketFeature IsNot Nothing Then
                Return True
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Set a specified feature's selected option to the specified option in the print ticket
    ''' </summary>
    ''' <param name="feature">The feature whose option will be set</param>
    ''' <param name="optionIndex">The index of the option that will be selected in the list of options retrieved for the specified feature</param>
    Public Sub SetFeatureOption(ByVal feature As String, ByVal optionIndex As String)
        If String.IsNullOrWhiteSpace(feature) OrElse String.IsNullOrWhiteSpace(optionIndex) Then
            Return
        End If

        ' convert the index from string to int
        Dim index = Integer.Parse(optionIndex)
        ' Get the feature in the context's print ticket
        Dim ticketFeature = context.Ticket.GetFeatureByKeyName(feature)

        If Nothing IsNot ticketFeature Then
            ' Set the option only if the user has selected an option different from the original option.
            ' Note that for options with user defined parameters, such as CustomMediaSize, extra information is needed
            ' than what is present in the Print Capabilities.  For simplicity, we have chosen to use the default
            ' parameters in this sample app. But developers should use specialized UI to prompt the user for the required parameters.
            If index <> Integer.Parse(GetSelectedOptionIndex(feature)) Then
                ' Look up the specified option in the print capabilities, and set it as the feature's selected option
                Dim [option] = GetCachedFeatureOptions(feature)(index)
                ticketFeature.SelectedOption = [option]
            End If
        End If
    End Sub

    ''' <summary>
    ''' Get an array of a specified type of option information items for a specified feature in print capabilities.
    ''' This function is called by Javascript to retrieve option display names and indeces
    ''' </summary>
    ''' <param name="feature">The feature whose options' information will be returned</param>
    ''' <param name="infoTypeString">The type of information about the option to be looked up.  Valid strings include "DisplayName", and "Index"</param>
    ''' <returns>An array of strings corresponding to each option's value for information of the specified type</returns>
    Public Function GetOptionInfo(ByVal feature As String, ByVal infoTypeString As String) As String()
        Dim options = New List(Of String)()

        If String.IsNullOrWhiteSpace(infoTypeString) Then
            Return Nothing
        End If

        ' Parse infoTypeString to match it to an OptionInfoType
        Dim infoType As OptionInfoType
        If (Not System.Enum.TryParse(infoTypeString, infoType)) OrElse (Not System.Enum.IsDefined(GetType(OptionInfoType), infoType)) Then
            Return Nothing
        End If

        Dim schemaOptions = GetCachedFeatureOptions(feature)

        If infoType = OptionInfoType.DisplayName Then
            ' Select just the DisplayName from each option
            options.AddRange(schemaOptions.Select(Function([option]) [option].DisplayName))
        ElseIf infoType = OptionInfoType.Index Then
            ' Generate a range starting from 0 and equal in length to the number of options, and select each number in the range as a string
            options.AddRange(Enumerable.Range(0, schemaOptions.Count).Select(Function(index) index.ToString()))
        End If

        Return options.ToArray()
    End Function

    ''' <summary>
    ''' Gets the index of the currently selected option in the list of options for a specified feature in the current print ticket
    ''' </summary>
    ''' <param name="feature">The feature whose currently selected option will be looked up</param>
    ''' <returns>String-based representation of the index in the list of options of the specified feature's currently selected option</returns>
    Public Function GetSelectedOptionIndex(ByVal feature As String) As String
        Dim options As List(Of IPrintSchemaOption) = GetCachedFeatureOptions(feature)

        ' Iterate through all the options, return the index of the selected option
        For i As Integer = 0 To options.Count - 1
            If options(i).Selected Then
                Return i.ToString()
            End If
        Next i

        ' It is possible for the PrintTicket object to not contain a current selection for this feature causing none 
        ' of the options in the print capabilities to be marked as selected.  In this case, the developers should 
        ' be familiar enough with the printer hardware to display and set the feature to the correct printer default option.
        ' Because this is a generic sample app, the first option will be displayed and set when the user hits the back button.
        Return "0"
    End Function

    ''' <summary>
    ''' Gets the display name for a specified feature in the print capabilities
    ''' </summary>
    ''' <param name="feature">The feature whose display name will be looked up</param>
    ''' <returns>The display name of the specified feature</returns>
    Public Function GetFeatureDisplayName(ByVal feature As String) As String
        If Nothing Is Capabilities Then
            Return Nothing
        End If

        Dim capsFeature As IPrintSchemaFeature = Capabilities.GetFeatureByKeyName(feature)
        If Nothing IsNot capsFeature Then
            Return capsFeature.DisplayName
        Else
            Return "Feature """ & feature & """ was not found in the print capabilities"
        End If
    End Function

    ''' <summary>
    ''' Checks each possible option for a particular feature for constraints.
    ''' </summary>
    ''' <param name="feature">The feature whose options will be looked up</param>
    ''' <returns>A boolean array that is true for any option that is constrained under the last saved print ticket.</returns>
    Public Function GetOptionConstraints(ByVal feature As String) As Boolean()
        Dim options As List(Of IPrintSchemaOption) = featureOptions(feature)
        Dim constrainedList(options.Count - 1) As Boolean

        ' Check the constrained value for each possible option of the feature
        For Each [option] As IPrintSchemaOption In options
            Dim optionIndex As Integer = options.IndexOf([option])
            constrainedList(optionIndex) = Not options(optionIndex).Constrained.Equals(PrintSchemaConstrainedSetting.None)
        Next [option]

        Return constrainedList

    End Function

    ''' <summary>
    ''' This is a wrapper to make saving the ticket asynchronous.
    ''' </summary>
    ''' <returns>IAsyncOperation so that this method is:  
    '''     1) awaitable in VB.
    '''     2) consumed as a WinJS.promise on which we can then use .then() or .done().</returns>
    Public Function SaveTicketAsync(ByVal request As Windows.Devices.Printers.Extensions.PrintTaskConfigurationSaveRequest, ByVal printerExtensionContext As Object) As IAsyncAction
        Return AsyncInfo.Run(Async Function(o)
                                 Await Task.Run(Sub()
                                                    request.Save(printerExtensionContext)
                                                End Sub)
                             End Function)
    End Function

    ''' <summary>
    ''' Provides a friendlier way to use HRESULT error codes.
    ''' </summary>
    Private Enum HRESULT As Integer
        S_OK = &H0
        S_FALSE = &H1
        S_PT_NO_CONFLICT = &H40001
        E_INVALID_DATA = CInt(&H8007000D)
        E_INVALIDARG = CInt(&H80070057)
        E_OUTOFMEMORY = CInt(&H8007000E)
        ERROR_NOT_FOUND = CInt(&H80070490)
    End Enum

End Class