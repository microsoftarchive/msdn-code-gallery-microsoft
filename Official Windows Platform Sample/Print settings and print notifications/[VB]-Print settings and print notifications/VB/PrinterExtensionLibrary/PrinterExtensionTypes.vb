' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved
'
'
' Abstract:
'
'     This file defines all types and interfaces that may be used to build a printer
'     extension application.
'

Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace Types
    '
    ' Enums and Constants
    '
    ''' <summary>
    ''' Maps to COM PrintSchemaConstrainedSetting
    ''' </summary>
    Public Enum PrintSchemaConstrainedSetting
        None = 0
        PrintTicket = 1
        Admin = 2
        Device = 3
    End Enum

    ''' <summary>
    ''' Maps to COM PrintSchemaSelectionType
    ''' </summary>
    Public Enum PrintSchemaSelectionType
        PickOne = 0
        PickMany = 1
    End Enum

#If WINDOWS_81_APIS Then
    ''' <summary>
    ''' Maps to COM PrintSchemaParameterDataType
    ''' </summary>
    Public Enum PrintSchemaParameterDataType
        [Integer] = 0
        NumericString = 1
        String = 2
    End Enum

    ''' <summary>
    ''' Maps to COM PrintJobStatus
    ''' </summary>
    <Flags> _
    Public Enum PrintJobStatus
        Paused = &H1
        [Error] = &H2
        Deleting = &H4
        Spooling = &H8
        Printing = &H10
        Offline = &H20
        PaperOut = &H40
        Printed = &H80
        Deleted = &H100
        BlockedDeviceQueue = &H200
        UserIntervention = &H400
        Restarted = &H800
        Complete = &H1000
        Retained = &H2000
    End Enum
#End If

    Public NotInheritable Class PrinterExtensionReason

        Private Sub New()
        End Sub

        ' An Enum was the first choice but the list of Guid is designed to be extendable.
        ' A read-only property was the second choice however this would have made new copies of the Guid.
        ' Using a class with static Guids balances both considerations.

        ''' <summary>
        ''' In this mode preferences for a print job or default print preferences is expected to be displayed.
        ''' Maps to C++ PRINTER_EXTENSION_REASON_PRINT_PREFERENCES
        ''' </summary>
        Public Shared PrintPreferences As New Guid("{EC8F261F-267C-469F-B5D6-3933023C29CC}")


        ''' <summary>
        ''' In this mode a status monitor for the print queue is expected to be displayed.
        ''' Maps to C++ PRINTER_EXTENSION_REASON_DRIVER_EVENT
        ''' </summary>
        Public Shared DriverEvent As New Guid("{23BB1328-63DE-4293-915B-A6A23D929ACB}")
    End Class

    Public NotInheritable Class PrintSchemaConstants

        Private Sub New()
        End Sub

        ''' <summary>
        ''' The namespace URI for the Print Schema keywords
        ''' </summary>
        Public Const KeywordsNamespaceUri As String = "http://schemas.microsoft.com/windows/2003/08/printing/printschemakeywords"
        ''' <summary>
        ''' The namespace URI for the Print Schema keywords V1.1
        ''' </summary>
        Public Const KeywordsV11NamespaceUri As String = "http://schemas.microsoft.com/windows/2013/05/printing/printschemakeywordsv11"
        ''' <summary>
        ''' The namespace URI for the Print Schema Framework
        ''' </summary>
        Public Const FrameworkNamespaceUri As String = "http://schemas.microsoft.com/windows/2003/08/printing/printschemaframework"
    End Class

    '
    ' Interfaces
    '

    ' The following interfaces are shared between the "Reference" and "Implementation"
    ' project. These interfaces are the public surface for the adapters that will remain
    ' internal to the "Implementation" project. It is done this way because the public
    ' surface and strong name must be the same for "Reference" and "Implementation".

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext
    ''' </summary>
    Public Interface IPrinterExtensionContext
        ''' <summary>
        ''' Maps to COM IPrinterExtensionContext::PrinterQueue
        ''' </summary>
        ReadOnly Property Queue() As IPrinterQueue

        ''' <summary>
        ''' Maps to COM IPrinterExtensionContext::PrintSchemaTicket
        ''' </summary>
        ReadOnly Property Ticket() As IPrintSchemaTicket

        ''' <summary>
        ''' Maps to COM IPrinterExtensionContext::DriverProperties
        ''' </summary>
        ReadOnly Property DriverProperties() As IPrinterPropertyBag

        ''' <summary>
        ''' Maps to COM IPrinterExtensionContext::UserProperties
        ''' </summary>
        ReadOnly Property UserProperties() As IPrinterPropertyBag
    End Interface

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs
    ''' </summary>
    Public Interface IPrinterExtensionEventArgs
        Inherits IPrinterExtensionContext

        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::BidiNotification
        ''' </summary>
        ReadOnly Property BidiNotification() As String

        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::ReasonId
        ''' </summary>
        ReadOnly Property ReasonId() As Guid


        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::Request
        ''' </summary>
        ReadOnly Property Request() As IPrinterExtensionRequest

        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::SourceApplication
        ''' </summary>
        ReadOnly Property SourceApplication() As String

        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::DetailedReasonId
        ''' </summary>
        ReadOnly Property DetailedReasonId() As Guid

        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::WindowModal
        ''' </summary>
        ReadOnly Property WindowModal() As Boolean

        ''' <summary>
        ''' Maps to COM IPrinterExtensionEventArgs::WindowParent
        ''' </summary>
        ReadOnly Property WindowParent() As IntPtr
    End Interface

#If WINDOWS_81_APIS Then
    ''' <summary>
    ''' Maps to COM IPrinterExtensionAsyncOperation
    ''' </summary>
    Public Interface IPrinterExtensionAsyncOperation
        ''' <summary>
        ''' Maps to COM IPrinterExtensionAsyncOperation::Cancel
        ''' </summary>
        Sub Cancel()
    End Interface
#End If

    ''' <summary>
    ''' Maps to COM IPrintSchemaElement
    ''' </summary>
    Public Interface IPrintSchemaElement
        ''' <summary>
        ''' Maps to COM IPrintSchemaElement::Name
        ''' </summary>
        ReadOnly Property Name() As String

        ''' <summary>
        ''' Maps to COM IPrintSchemaElement::NamespaceUri
        ''' </summary>
        ReadOnly Property XmlNamespace() As String
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaDisplayableElement
    ''' </summary>
    Public Interface IPrintSchemaDisplayableElement
        Inherits IPrintSchemaElement

        ''' <summary>
        ''' Maps to COM IPrintSchemaDisplayableElement::DisplayName
        ''' </summary>
        ReadOnly Property DisplayName() As String
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaOption
    ''' </summary>
    Public Interface IPrintSchemaOption
        Inherits IPrintSchemaDisplayableElement

        ''' <summary>
        ''' Maps to COM IPrintSchemaOption::Selected
        ''' </summary>
        ReadOnly Property Selected() As Boolean

        ''' <summary>
        ''' Maps to COM IPrintSchemaOption::Constrained
        ''' </summary>
        ReadOnly Property Constrained() As PrintSchemaConstrainedSetting
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaPageMediaSizeOption
    ''' </summary>
    Public Interface IPrintSchemaPageMediaSizeOption
        Inherits IPrintSchemaOption

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageMediaSizeOption::HeightInMicrons
        ''' </summary>
        ReadOnly Property HeightInMicrons() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageMediaSizeOption::WidthInMicrons
        ''' </summary>
        ReadOnly Property WidthInMicrons() As UInteger
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaNUpOption
    ''' </summary>
    Public Interface IPrintSchemaNUpOption
        Inherits IPrintSchemaOption

        ''' <summary>
        ''' Maps to COM IPrintSchemaNUpOption::PagesPerSheet
        ''' </summary>
        ReadOnly Property PagesPerSheet() As UInteger
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaFeature
    ''' </summary>
    Public Interface IPrintSchemaFeature
        Inherits IPrintSchemaDisplayableElement

        ''' <summary>
        ''' Maps to COM IPrintSchemaFeature::SelectedOption
        ''' </summary>
        Property SelectedOption() As IPrintSchemaOption

        ''' <summary>
        ''' Maps to COM IPrintSchemaFeature::SelectionType
        ''' </summary>
        ReadOnly Property SelectionType() As PrintSchemaSelectionType

        ''' <summary>
        ''' Maps to COM IPrintSchemaFeature::GetOption
        ''' </summary>
        Function GetOption(ByVal optionName As String) As IPrintSchemaOption

        ''' <summary>
        ''' Maps to COM IPrintSchemaFeature::GetOption
        ''' </summary>
        Function GetOption(ByVal optionName As String, ByVal xmlNamespace As String) As IPrintSchemaOption

        ''' <summary>
        ''' Maps to COM IPrintSchemaFeature::DisplayUI
        ''' </summary>
        ReadOnly Property DisplayUI() As Boolean
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaPageImageableSize
    ''' </summary>
    Public Interface IPrintSchemaPageImageableSize
        Inherits IPrintSchemaElement

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageImageableSize::ExtentHeightInMicrons
        ''' </summary>
        ReadOnly Property ExtentHeightInMicrons() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageImageableSize::ExtentWidthInMicrons
        ''' </summary>
        ReadOnly Property ExtentWidthInMicrons() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageImageableSize::ImageableSizeHeightInMicrons
        ''' </summary>
        ReadOnly Property ImageableSizeHeightInMicrons() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageImageableSize::ImageableSizeWidthInMicrons
        ''' </summary>
        ReadOnly Property ImageableSizeWidthInMicrons() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageImageableSize::OriginHeightInMicrons
        ''' </summary>
        ReadOnly Property OriginHeightInMicrons() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaPageImageableSize::OriginWidthInMicrons
        ''' </summary>
        ReadOnly Property OriginWidthInMicrons() As UInteger
    End Interface

#If WINDOWS_81_APIS Then
    ''' <summary>
    ''' Maps to COM IPrintSchemaParameterDefinition
    ''' </summary>
    Public Interface IPrintSchemaParameterDefinition
        Inherits IPrintSchemaDisplayableElement

        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterDefinition::UserInputRequired
        ''' </summary>
        ReadOnly Property UserInputRequired() As Boolean

        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterDefinition::UnitType
        ''' </summary>
        ReadOnly Property UnitType() As String

        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterDefinition::DataType
        ''' </summary>
        ReadOnly Property DataType() As PrintSchemaParameterDataType

        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterDefinition::RangeMin
        ''' </summary>
        ReadOnly Property RangeMin() As Integer

        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterDefinition::RangeMax
        ''' </summary>
        ReadOnly Property RangeMax() As Integer
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaParameterInitializer
    ''' </summary>
    Public Interface IPrintSchemaParameterInitializer
        Inherits IPrintSchemaElement

        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterInitializer::Value
        ''' </summary>
        Property StringValue() As String
        ''' <summary>
        ''' Maps to COM IPrintSchemaParameterInitializer::Value
        ''' </summary>
        Property IntegerValue() As Integer
    End Interface
#End If

    ''' <summary>
    ''' Maps to COM IPrintSchemaCapabilities
    ''' </summary>
    Public Interface IPrintSchemaCapabilities
        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::GetFeatureByKeyName
        ''' </summary>
        Function GetFeatureByKeyName(ByVal keyName As String) As IPrintSchemaFeature

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::GetFeature
        ''' </summary>
        Function GetFeature(ByVal featureName As String) As IPrintSchemaFeature

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::GetFeature
        ''' </summary>
        Function GetFeature(ByVal featureName As String, ByVal xmlNamespace As String) As IPrintSchemaFeature

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::PageImageableSize
        ''' </summary>
        ReadOnly Property PageImageableSize() As IPrintSchemaPageImageableSize

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::JobCopiesAllDocumentsMinValue
        ''' </summary>
        ReadOnly Property JobCopiesAllDocumentsMaxValue() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::JobCopiesAllDocumentsMaxValue
        ''' </summary>
        ReadOnly Property JobCopiesAllDocumentsMinValue() As UInteger

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::GetSelectedOptionInPrintTicket
        ''' </summary>
        Function GetSelectedOptionInPrintTicket(ByVal feature As IPrintSchemaFeature) As IPrintSchemaOption

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities::GetOptions
        ''' </summary>
        Function GetOptions(ByVal feature As IPrintSchemaFeature) As IEnumerable(Of IPrintSchemaOption)

        ''' <summary>
        ''' Replaces COM IPrintSchemaCapabilities::XmlNode
        ''' </summary>
        Function GetReadStream() As Stream

        ''' <summary>
        ''' Replaces COM IPrintSchemaCapabilities::XmlNode
        ''' </summary>
        Function GetWriteStream() As Stream

#If WINDOWS_81_APIS Then
        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities2::GetParameterDefinition
        ''' </summary>
        Function GetParameterDefinition(ByVal parameterName As String) As IPrintSchemaParameterDefinition

        ''' <summary>
        ''' Maps to COM IPrintSchemaCapabilities2::GetParameterDefinition
        ''' </summary>
        Function GetParameterDefinition(ByVal parameterName As String, ByVal xmlNamespace As String) As IPrintSchemaParameterDefinition
#End If
    End Interface

    ''' <summary>
    ''' The EventArgs for the PrintSchemaAsyncOperation
    ''' Maps to COM IPrintSchemaAsyncOperationEvent
    ''' </summary>
    Public Class PrintSchemaAsyncOperationEventArgs
        Inherits EventArgs

        '
        ' Event arguments
        '
        ''' <summary>
        ''' Maps to COM IPrintSchemaAsyncOperationEvent::Completed, parameter 'hrOperation'
        ''' </summary>
        Public ReadOnly Property StatusHResult() As Integer
            Get
                Return _statusHResult
            End Get
        End Property

        ''' <summary>
        ''' Maps to COM IPrintSchemaAsyncOperationEvent::Completed, parameter 'pTicket'
        ''' </summary>
        Public ReadOnly Property Ticket() As IPrintSchemaTicket
            Get
                Return _printTicket
            End Get
        End Property

        '
        ' Implementation details
        '
        Friend Sub New(ByVal printTicket As IPrintSchemaTicket, ByVal statusHResult As Integer)
            _statusHResult = statusHResult
            _printTicket = printTicket
        End Sub

        Private _statusHResult As Integer
        Private _printTicket As IPrintSchemaTicket
    End Class

    ''' <summary>
    ''' Maps to COM IPrintSchemaAsyncOperation
    ''' </summary>
    Public Interface IPrintSchemaAsyncOperation
        ''' <summary>
        ''' Maps to COM IPrintSchemaAsyncOperationEvent::Completed
        ''' </summary>
        Event Completed As EventHandler(Of PrintSchemaAsyncOperationEventArgs)

        ''' <summary>
        ''' Maps to COM IPrintSchemaAsyncOperation::Start
        ''' </summary>
        Sub Start()

        ''' <summary>
        ''' Maps to COM IPrintSchemaAsyncOperation::Cancel
        ''' </summary>
        Sub Cancel()
    End Interface

    ''' <summary>
    ''' Maps to COM IPrintSchemaTicket
    ''' </summary>
    Public Interface IPrintSchemaTicket
        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::GetFeatureByKeyName
        ''' </summary>
        Function GetFeatureByKeyName(ByVal featureName As String) As IPrintSchemaFeature

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::GetFeature
        ''' </summary>
        Function GetFeature(ByVal featureName As String) As IPrintSchemaFeature

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::GetFeature
        ''' </summary>
        Function GetFeature(ByVal featureName As String, ByVal xmlNamespace As String) As IPrintSchemaFeature

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::ValidateAsync
        ''' </summary>
        Function ValidateAsync() As IPrintSchemaAsyncOperation

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::CommitAsync
        ''' </summary>
        Function CommitAsync(ByVal printTicketCommit As IPrintSchemaTicket) As IPrintSchemaAsyncOperation

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::NotifyXmlChanged
        ''' </summary>
        Sub NotifyXmlChanged()

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::GetCapabilities
        ''' </summary>
        Function GetCapabilities() As IPrintSchemaCapabilities

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket::JobCopiesAllDocuments
        ''' </summary>
        Property JobCopiesAllDocuments() As UInteger

        ''' <summary>
        ''' Replaces COM IPrintSchemaTicket::XmlNode
        ''' </summary>
        Function GetReadStream() As Stream

        ''' <summary>
        ''' Replaces COM IPrintSchemaTicket::XmlNode
        ''' </summary>
        Function GetWriteStream() As Stream

#If WINDOWS_81_APIS Then
        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket2::GetParameterInitializer
        ''' </summary>
        Function GetParameterInitializer(ByVal parameterName As String) As IPrintSchemaParameterInitializer

        ''' <summary>
        ''' Maps to COM IPrintSchemaTicket2::GetParameterInitializer
        ''' </summary>
        Function GetParameterInitializer(ByVal parameterName As String, ByVal xmlNamespace As String) As IPrintSchemaParameterInitializer
#End If
    End Interface

    ''' <summary>
    ''' Maps to COM IPrinterPropertyBag
    ''' </summary>
    Public Interface IPrinterPropertyBag
        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::GetBool
        ''' </summary>
        Function GetBool(ByVal propertyName As String) As Boolean

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::SetBool
        ''' </summary>
        Sub SetBool(ByVal propertyName As String, ByVal value As Boolean)

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::GetInt32
        ''' </summary>
        Function GetInt(ByVal propertyName As String) As Integer

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::SetInt32
        ''' </summary>
        Sub SetInt(ByVal propertyName As String, ByVal value As Integer)

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::GetString
        ''' </summary>
        Function GetString(ByVal propertyName As String) As String

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::SetString
        ''' </summary>
        Sub SetString(ByVal propertyName As String, ByVal value As String)

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::GetBytes
        ''' </summary>
        Function GetBytes(ByVal propertyName As String) As Byte()

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::SetBytes
        ''' </summary>
        Sub SetBytes(ByVal propertyName As String, ByVal value() As Byte)

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::GetReadStream
        ''' </summary>
        Function GetReadStream(ByVal propertyName As String) As Stream

        ''' <summary>
        ''' Maps to COM IPrinterPropertyBag::GetWriteStream
        ''' </summary>
        Function GetWriteStream(ByVal propertyName As String) As Stream

        ''' <summary>
        ''' Indexer for the properties
        ''' </summary>
        ''' <param name="name">Property name</param>
        ''' <returns>An instance of 'PrinterProperty' used to get/set values</returns>
        Default ReadOnly Property Item(ByVal name As String) As PrinterProperty
    End Interface

    ''' <summary>
    ''' Represents one property returned by the indexer in IPrinterPropertyBag
    ''' </summary>
    Public Class PrinterProperty
        Friend Sub New(ByVal bag As IPrinterPropertyBag, ByVal name As String)
            _bag = bag
            _name = name
        End Sub

        ''' <summary>
        ''' Prevents default construction
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Get/Set a value of type 'bool'
        ''' </summary>
        Public Property Bool() As Boolean
            Get
                Return _bag.GetBool(_name)
            End Get
            Set(ByVal value As Boolean)
                _bag.SetBool(_name, value)
            End Set
        End Property

        ''' <summary>
        ''' Get/Set a value of type 'Int32'
        ''' </summary>
        Public Property Int() As Integer
            Get
                Return _bag.GetInt(_name)
            End Get
            Set(ByVal value As Integer)
                _bag.SetInt(_name, value)
            End Set
        End Property

        ''' <summary>
        ''' Get/Set a value of type 'byte[]'
        ''' </summary>
        Public Property Bytes() As Byte()
            Get
                Return _bag.GetBytes(_name)
            End Get
            Set(ByVal value As Byte())
                _bag.SetBytes(_name, value)
            End Set
        End Property

        ''' <summary>
        ''' Get/Set a value of type 'string'
        ''' </summary>
        Public Property Name() As String
            Get
                Return _bag.GetString(_name)
            End Get
            Set(ByVal value As String)
                _bag.SetString(_name, value)
            End Set
        End Property

        ''' <summary>
        ''' Get a read/write Stream corresponding to this property name
        ''' </summary>
        Public ReadOnly Property WriteStream() As Stream
            Get
                Return _bag.GetWriteStream(_name)
            End Get
        End Property

        ''' <summary>
        ''' Get a read-only Stream corresponding to this property name
        ''' </summary>
        Public ReadOnly Property ReadStream() As Stream
            Get
                Return _bag.GetReadStream(_name)
            End Get
        End Property

        '
        ' Implementation details
        '
        Private _bag As IPrinterPropertyBag
        Private _name As String
    End Class

#If WINDOWS_81_APIS Then
    ''' <summary>
    ''' Maps to COM IPrintJob
    ''' </summary>
    Public Interface IPrintJob
        ''' <summary>
        ''' Maps to COM IPrintJob::Name
        ''' </summary>
        ReadOnly Property Name() As String

        ''' <summary>
        ''' Maps to COM IPrintJob::Id
        ''' </summary>
        ReadOnly Property Id() As ULong

        ''' <summary>
        ''' Maps to COM IPrintJob::PrintedPages
        ''' </summary>
        ReadOnly Property PrintedPages() As ULong

        ''' <summary>
        ''' Maps to COM IPrintJob::TotalPages
        ''' </summary>
        ReadOnly Property TotalPages() As ULong

        ''' <summary>
        ''' Maps to COM IPrintJob::Status
        ''' </summary>
        ReadOnly Property Status() As PrintJobStatus

        ''' <summary>
        ''' Maps to COM IPrintJob::SubmissionTime
        ''' </summary>
        ReadOnly Property SubmissionTime() As Date

        ''' <summary>
        ''' Maps to COM IPrintJob::RequestCancel
        ''' </summary>
        Sub RequestCancel()
    End Interface

    ''' <summary>
    ''' Maps to COM IPrinterQueueViewEvent
    ''' </summary>
    Public Class PrinterQueueViewEventArgs
        Inherits EventArgs

        ''' <summary>
        ''' Maps to COM IPrinterQueueViewEvent::OnChanged, parameter 'pCollection'
        ''' </summary>
        Public ReadOnly Property Collection() As IEnumerable(Of IPrintJob)
            Get
                Return _collection
            End Get
        End Property

        ''' <summary>
        ''' Maps to COM IPrinterQueueViewEvent::OnChanged, parameter 'ulViewOffset'
        ''' </summary>
        Public ReadOnly Property ViewOffset() As UInteger
            Get
                Return _viewOffset
            End Get
        End Property

        ''' <summary>
        ''' Maps to COM IPrinterQueueViewEvent::OnChanged, parameter 'ulViewSize'
        ''' </summary>
        Public ReadOnly Property ViewSize() As UInteger
            Get
                Return _viewSize
            End Get
        End Property

        ''' <summary>
        ''' Maps to COM IPrinterQueueViewEvent::OnChanged, parameter 'ulCountJobsInPrintQueue'
        ''' </summary>
        Public ReadOnly Property CountJobsInPrintQueue() As UInteger
            Get
                Return _countJobsInPrintQueue
            End Get
        End Property

        #Region "Implementation details"

        Friend Sub New(ByVal collection As IEnumerable(Of IPrintJob), ByVal viewOffset As UInteger, ByVal viewSize As UInteger, ByVal countJobsInPrintQueue As UInteger)
            _collection = collection
            _viewOffset = viewOffset
            _viewSize = viewSize
            _countJobsInPrintQueue = countJobsInPrintQueue
        End Sub

        Private _collection As IEnumerable(Of IPrintJob)
        Private _viewOffset As UInteger
        Private _viewSize As UInteger
        Private _countJobsInPrintQueue As UInteger

        #End Region
    End Class

    ''' <summary>
    ''' Maps to COM IPrinterQueueView
    ''' </summary>
    Public Interface IPrinterQueueView
        ''' <summary>
        ''' Maps to COM IPrinterQueueView::SetViewRange
        ''' </summary>
        Sub SetViewRange(ByVal viewOffset As UInteger, ByVal viewSize As UInteger)

        ''' <summary>
        ''' Maps to COM IPrinterQueueViewEvent::OnChanged
        ''' </summary>
        Event OnChanged As EventHandler(Of PrinterQueueViewEventArgs)
    End Interface

    ''' <summary>
    ''' Maps to COM IPrinterBidiSetRequestCallback
    ''' </summary>
    Public Interface IPrinterBidiSetRequestCallback
        ''' <summary>
        ''' Maps to COM IPrinterBidiSetRequestCallback::Completed
        ''' </summary>
        Sub Completed(ByVal response As String, ByVal statusHResult As Integer)
    End Interface
#End If

    ''' <summary>
    ''' Maps to COM IPrinterQueueEvent
    ''' </summary>
    Public Class PrinterQueueEventArgs
        Inherits EventArgs

        ''' <summary>
        ''' Maps to COM IPrinterQueueEvent::OnBidiResponseReceived, parameter 'bstrResponse'
        ''' </summary>
        Public ReadOnly Property Response() As String
            Get
                Return _response
            End Get
        End Property

        ''' <summary>
        ''' Maps to COM IPrinterQueueEvent::OnBidiResponseReceived, parameter 'hrStatus'
        ''' </summary>
        Public ReadOnly Property StatusHResult() As Integer
            Get
                Return _statusHResult
            End Get
        End Property

        '
        ' Implementation details
        '
        Public Sub New(ByVal response As String, ByVal statusHResult As Integer)
            _response = response
            _statusHResult = statusHResult
        End Sub

        Private _statusHResult As Integer
        Private _response As String
    End Class

    ''' <summary>
    ''' Maps to COM IPrinterQueue
    ''' </summary>
    Public Interface IPrinterQueue
        ''' <summary>
        ''' Maps to COM IPrinterQueue::Handle
        ''' </summary>
        ReadOnly Property Handle() As IntPtr

        ''' <summary>
        ''' Maps to COM IPrinterQueue::Name
        ''' </summary>
        ReadOnly Property Name() As String

        ''' <summary>
        ''' Maps to COM IPrinterQueue::SendBidiQuery
        ''' </summary>
        Sub SendBidiQuery(ByVal bidiQuery As String)

        ''' <summary>
        ''' Maps to COM IPrinterQueue::GetProperties
        ''' </summary>
        Function GetProperties() As IPrinterPropertyBag

#If WINDOWS_81_APIS Then
        ''' <summary>
        ''' Maps to COM IPrinterQueue2::SendBidiSetRequestAsync
        ''' </summary>
        ''' <param name="callback">Maps to COM callback type IPrinterBidiSetRequestCallback</param>
        ''' <returns>IPrinterExtensionAsyncOperation - async operation context</returns>
        Function SendBidiSetRequestAsync(ByVal bidiRequest As String, ByVal callback As IPrinterBidiSetRequestCallback) As IPrinterExtensionAsyncOperation

        ''' <summary>
        ''' Maps to COM IPrinterQueue2::GetPrinterQueueView
        ''' </summary>
        Function GetPrinterQueueView(ByVal viewOffset As UInteger, ByVal viewSize As UInteger) As IPrinterQueueView
#End If

        ''' <summary>
        ''' Maps to COM IPrinterQueueEvent::OnBidiResponseReceived
        ''' </summary>
        Event OnBidiResponseReceived As EventHandler(Of PrinterQueueEventArgs)
    End Interface

    ''' <summary>
    ''' Maps to COM IPrinterExtensionRequest
    ''' </summary>
    Public Interface IPrinterExtensionRequest
        ''' <summary>
        ''' Maps to COM IPrinterExtensionRequest::Complete
        ''' </summary>
        Sub Complete()

        ''' <summary>
        ''' Maps to COM IPrinterExtensionRequest::Cancel
        ''' </summary>
        Sub Cancel(ByVal statusHResult As Integer, ByVal logMessage As String)
    End Interface
End Namespace

