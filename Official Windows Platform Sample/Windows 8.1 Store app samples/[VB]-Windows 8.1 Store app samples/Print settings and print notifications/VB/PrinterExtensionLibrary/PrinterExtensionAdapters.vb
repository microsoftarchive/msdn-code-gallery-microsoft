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
'     This file contains Adapters that wrap the PrinterExtension COM Interop types.
'
Imports System
Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports Microsoft.Samples.Printing.PrinterExtension.Types

' The following three classes are constructable adapters for the root of the
' object model.  The balance of the types will typically be interfaces. This
' choice was made so we can share the interface file between projects and enforce
' the same public surface from both the "Reference" and "Implementation" projects.

#Region "PrinterExtension adapter classes"

''' <summary>
''' Wraps an COM pointer to IPrinterExtensionContext
''' </summary>
Public Class PrinterExtensionContext
    Implements IPrinterExtensionContext

    ''' <summary>
    ''' Wraps an opaque COM pointer to IPrinterExtensionContext and provides usable methods
    ''' </summary>
    ''' <param name="comContext">Opaque COM pointer to IPrinterExtensionContext</param>
    Public Sub New(ByVal comContext As Object)
        _context = CType(comContext, PrinterExtensionLib.IPrinterExtensionContext)
    End Sub

    #Region "IPrinterExtensionContext methods"

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::PrinterQueue
    ''' </summary>
    Public ReadOnly Property Queue() As IPrinterQueue Implements IPrinterExtensionContext.Queue
        Get
            Return New PrinterQueue(_context.PrinterQueue)
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::PrintSchemaTicket
    ''' </summary>
    Public ReadOnly Property Ticket() As IPrintSchemaTicket Implements IPrinterExtensionContext.Ticket
        Get
            Return New PrintSchemaTicket(_context.PrintSchemaTicket)
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::DriverProperties
    ''' </summary>
    Public ReadOnly Property DriverProperties() As IPrinterPropertyBag Implements IPrinterExtensionContext.DriverProperties
        Get
            Try
                Return New PrinterPropertyBag(_context.DriverProperties, PrintPropertyBagType.DriverProperties)
            Catch e1 As Exception
                ' If the property bag is not found, instead of 
                ' throwing an exception, return null, which is more appropriate for a property 'get' operation.
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::UserProperties
    ''' </summary>
    Public ReadOnly Property UserProperties() As IPrinterPropertyBag Implements IPrinterExtensionContext.UserProperties
        Get
            Return New PrinterPropertyBag(_context.UserProperties, PrintPropertyBagType.UserProperties)
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Private _context As PrinterExtensionLib.IPrinterExtensionContext

    ' Prevent default construction
    Private Sub New()
    End Sub

    #End Region
End Class

''' <summary>
''' Wraps an COM pointer to IPrinterExtensionEventArgs
''' </summary>
Public Class PrinterExtensionEventArgs
    Inherits EventArgs
    Implements IPrinterExtensionEventArgs

    ''' <summary>
    ''' Wraps an opaque COM pointer to IPrinterExtensionEventArgs and provides usable methods
    ''' </summary>
    ''' <param name="comContext">Opaque COM pointer to IPrinterExtensionEventArgs</param>
    Public Sub New(ByVal eventArgs As Object)
        _eventArgs = CType(eventArgs, PrinterExtensionLib.IPrinterExtensionEventArgs)
        _context = New PrinterExtensionContext(eventArgs)
    End Sub

    #Region "IPrinterExtensionEventArgs methods"

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::BidiNotification
    ''' </summary>
    Public ReadOnly Property BidiNotification() As String Implements IPrinterExtensionEventArgs.BidiNotification
        Get
            Return _eventArgs.BidiNotification
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::ReasonId
    ''' </summary>
    Public ReadOnly Property ReasonId() As Guid Implements IPrinterExtensionEventArgs.ReasonId
        Get
            Return _eventArgs.ReasonId
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::Request
    ''' </summary>
    Public ReadOnly Property Request() As IPrinterExtensionRequest Implements IPrinterExtensionEventArgs.Request
        Get
            Return New PrinterExtensionRequest(_eventArgs.Request)
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::SourceApplication
    ''' </summary>
    Public ReadOnly Property SourceApplication() As String Implements IPrinterExtensionEventArgs.SourceApplication
        Get
            Return _eventArgs.SourceApplication
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::DetailedReasonId
    ''' </summary>
    Public ReadOnly Property DetailedReasonId() As Guid Implements IPrinterExtensionEventArgs.DetailedReasonId
        Get
            Return _eventArgs.DetailedReasonId
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::WindowModal
    ''' </summary>
    Public ReadOnly Property WindowModal() As Boolean Implements IPrinterExtensionEventArgs.WindowModal
        Get
            If _eventArgs.WindowModal <> 0 Then
                Return True
            End If
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionEventArgs::WindowParent
    ''' </summary>
    Public ReadOnly Property WindowParent() As IntPtr Implements IPrinterExtensionEventArgs.WindowParent
        Get
            Return _eventArgs.WindowParent
        End Get
    End Property

    #End Region

    #Region "IPrinterExtensionContext methods"

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::PrinterQueue
    ''' </summary>
    Public ReadOnly Property Queue() As IPrinterQueue Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrinterExtensionContext.Queue
        Get
            Return _context.Queue
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::PrintSchemaTicket
    ''' </summary>
    Public ReadOnly Property Ticket() As IPrintSchemaTicket Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrinterExtensionContext.Ticket
        Get
            Return _context.Ticket
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::DriverProperties
    ''' </summary>
    Public ReadOnly Property DriverProperties() As IPrinterPropertyBag Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrinterExtensionContext.DriverProperties
        Get
            Return _context.DriverProperties
        End Get
    End Property

    ''' <summary>
    ''' Maps to COM IPrinterExtensionContext::UserProperties
    ''' </summary>
    Public ReadOnly Property UserProperties() As IPrinterPropertyBag Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrinterExtensionContext.UserProperties
        Get
            Return _context.UserProperties
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Private _eventArgs As PrinterExtensionLib.IPrinterExtensionEventArgs

    ''' <summary>
    ''' Containment - since multiple inheritance is not possible in VB.
    ''' </summary>
    Private _context As PrinterExtensionContext

    #End Region
End Class

''' <summary>
''' This class provides wraps IPrinterExtensionContextCollection in a IEnumerable interface
''' </summary>
Public NotInheritable Class PrinterQueuesEnumeratedEventArgs
    Inherits EventArgs
    Implements IEnumerable(Of IPrinterExtensionContext)

    #Region "IEnumerable<IPrinterExtensionContext> methods"

    Public Iterator Function GetEnumerator() As IEnumerator(Of IPrinterExtensionContext) Implements IEnumerable(Of IPrinterExtensionContext).GetEnumerator
        For i As UInteger = 0 To CUInt(_contextCollection.Count - 1)
            Yield New PrinterExtensionContext(_contextCollection.GetAt(i))
        Next i
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield CType(GetEnumerator(), IEnumerator)
    End Function

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal contextCollection As PrinterExtensionLib.IPrinterExtensionContextCollection)
        _contextCollection = contextCollection
    End Sub

    Private _contextCollection As PrinterExtensionLib.IPrinterExtensionContextCollection

    #End Region
End Class

#If WINDOWS_81_APIS Then
Friend NotInheritable Class PrinterExtensionAsyncOperation
    Implements IPrinterExtensionAsyncOperation

    #Region "IPrinterExtensionAsyncOperation methods"

    Public Sub Cancel() Implements IPrinterExtensionAsyncOperation.Cancel
        _asyncOperation.Cancel()
    End Sub

    #End Region

    #Region "Implementation methods"

    Friend Sub New(ByVal asyncOperation As PrinterExtensionLib.IPrinterExtensionAsyncOperation)
        _asyncOperation = asyncOperation
    End Sub

    Private _asyncOperation As PrinterExtensionLib.IPrinterExtensionAsyncOperation

    #End Region
End Class
#End If
#End Region

#Region "COM Adapter Classes"

'
' The following class provide an adapter that exposes a 'Stream' and wraps a
' COM pointer to PrinterExtensionLib.IStream
'
Friend Class PrinterExtensionLibIStreamAdapter
    Inherits Stream
    Implements IDisposable

    Public Sub New(ByVal stream As PrinterExtensionLib.IStream, Optional ByVal canWrite As Boolean = False, Optional ByVal canSeek As Boolean = False, Optional ByVal canRead As Boolean = True)
        If stream IsNot Nothing Then
            _printerExtensionIStream = stream
        Else
            Throw New ArgumentNullException("stream")
        End If
        _streamValidation = New StreamValidation(canWrite, canSeek, canRead)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub

    #Region "Overridden Stream methods"

    Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        _streamValidation.ValidateRead(buffer, offset, count)

        Dim bytesRead As UInteger = 0

        ' Pin the byte array so that it will not be moved by the garbage collector
        Dim tempBuffer(count - 1) As Byte
        Dim gcHandle As GCHandle = GCHandle.Alloc(tempBuffer, GCHandleType.Pinned)
        Try
            _printerExtensionIStream.RemoteRead(tempBuffer(0), Convert.ToUInt32(count), bytesRead)

            Array.Copy(tempBuffer, 0, buffer, offset, CInt(bytesRead)) ' Safe to cast. Cannot be bigger than 'int count'
        Finally
            gcHandle.Free()
        End Try

        Return CInt(bytesRead) ' Safe to cast; bytesRead can never be larger than 'int count'
    End Function

    Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        _streamValidation.ValidateWrite(buffer, offset, count)

        Dim written As UInteger

        ' Pin the byte array so that it will not be moved by the garbage collector
        Dim tempBuffer(count - 1) As Byte
        Dim gcHandle As GCHandle = GCHandle.Alloc(tempBuffer, GCHandleType.Pinned)
        Try
            Array.Copy(buffer, offset, tempBuffer, 0, count)
            _printerExtensionIStream.RemoteWrite(tempBuffer(0), Convert.ToUInt32(count), written)
        Finally
            gcHandle.Free()
        End Try

        If CInt(written) < count Then
            Throw New IOException()
        End If
    End Sub

    Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
        _streamValidation.ValidateSeek(offset, origin)

        Dim istreamSeekOrigin As UInteger = 0

        Select Case origin
            Case SeekOrigin.Begin
                istreamSeekOrigin = CUInt(PrinterExtensionLib.tagSTREAM_SEEK.STREAM_SEEK_SET)

            Case SeekOrigin.Current
                istreamSeekOrigin = CUInt(PrinterExtensionLib.tagSTREAM_SEEK.STREAM_SEEK_CUR)

            Case SeekOrigin.End
                istreamSeekOrigin = CUInt(PrinterExtensionLib.tagSTREAM_SEEK.STREAM_SEEK_END)
        End Select

        Dim dlibMove As PrinterExtensionLib._LARGE_INTEGER
        Dim plibNewPosition As PrinterExtensionLib._ULARGE_INTEGER

        dlibMove.QuadPart = offset

        _printerExtensionIStream.RemoteSeek(dlibMove, istreamSeekOrigin, plibNewPosition)
        Return Convert.ToInt64(plibNewPosition.QuadPart)
    End Function

    Public Overrides ReadOnly Property Length() As Long
        Get
            _streamValidation.ValidateSeek()

            Dim statstg As PrinterExtensionLib.tagSTATSTG = New PrinterExtensionLib.tagSTATSTG()
            _printerExtensionIStream.Stat(statstg, 1) ' STATSFLAG_NONAME
            Return Convert.ToInt64(statstg.cbSize.QuadPart)
        End Get
    End Property
    Public Overrides Property Position() As Long
        Get
            Return Seek(0, SeekOrigin.Current)
        End Get
        Set(ByVal value As Long)
            Seek(value, SeekOrigin.Begin)
        End Set
    End Property

    Public Overrides Sub SetLength(ByVal value As Long)
        _streamValidation.ValidateSeek()

        Dim libNewSize As PrinterExtensionLib._ULARGE_INTEGER
        libNewSize.QuadPart = Convert.ToUInt64(value)
        _printerExtensionIStream.SetSize(libNewSize)
    End Sub

    Public Overrides Sub Flush()
        _printerExtensionIStream.Commit(0)
    End Sub

    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return _streamValidation.CanRead
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return _streamValidation.CanWrite
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return _streamValidation.CanSeek
        End Get
    End Property

    #End Region

    #Region "IDisposable methods"

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If _disposed Then
            Return
        End If

        Try
            If disposing Then
                _streamValidation.Dispose()
            End If

            If _printerExtensionIStream IsNot Nothing Then
                Marshal.ReleaseComObject(_printerExtensionIStream)
                _printerExtensionIStream = Nothing
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
        _disposed = True
    End Sub

    #End Region

    #Region "Implementation details"

    ' Prevent default construction
    Private Sub New()
    End Sub

    Private _disposed As Boolean = False
    Private _printerExtensionIStream As PrinterExtensionLib.IStream = Nothing
    Private _streamValidation As StreamValidation = Nothing

    #End Region
End Class


'
' The following class provide an adapter that exposes a 'Stream' and wraps a
' COM pointer to the standard COM 'IStream' interface
'
Friend Class ComIStreamAdapter
    Inherits Stream
    Implements IDisposable

    Public Sub New(ByVal stream As System.Runtime.InteropServices.ComTypes.IStream, Optional ByVal canWrite As Boolean = False, Optional ByVal canSeek As Boolean = False, Optional ByVal canRead As Boolean = True)
        If stream IsNot Nothing Then
            _comIstream = stream
        Else
            Throw New ArgumentNullException("stream")
        End If
        _streamValidation = New StreamValidation(canWrite, canSeek, canRead)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub

    #Region "Overridden Stream methods"

    Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        _streamValidation.ValidateRead(buffer, offset, count)

        Dim bytesRead As UInteger = 0

        ' Pin the byte array so that it will not be moved by the garbage collector
        Dim tempBuffer(count - 1) As Byte
        Dim gcHandle As GCHandle = GCHandle.Alloc(tempBuffer, GCHandleType.Pinned)
        Dim bytesReadPtr As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(New Integer))
        Try
            _comIstream.Read(tempBuffer, count, bytesReadPtr)
            bytesRead = CUInt(Marshal.ReadInt32(bytesReadPtr))

            Array.Copy(tempBuffer, 0, buffer, offset, CInt(bytesRead)) ' Safe to cast. Cannot be bigger than 'int count'
        Finally
            Marshal.FreeHGlobal(bytesReadPtr)
            gcHandle.Free()
        End Try

        Return CInt(bytesRead) ' Safe to cast; bytesRead can never be larger than 'int count'
    End Function

    Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        _streamValidation.ValidateWrite(buffer, offset, count)

        Dim written As UInteger

        ' Pin the byte array so that it will not be moved by the garbage collector
        Dim tempBuffer(count - 1) As Byte
        Dim gcHandle As GCHandle = GCHandle.Alloc(tempBuffer, GCHandleType.Pinned)
        Dim writeCountPointer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(New Integer))
        Try
            Array.Copy(buffer, offset, tempBuffer, 0, count)

            _comIstream.Write(tempBuffer, count, writeCountPointer)
            written = CUInt(Marshal.ReadInt32(writeCountPointer)) ' safe to cast. 'written' is always non-negative
        Finally
            gcHandle.Free()
            Marshal.FreeHGlobal(writeCountPointer)
        End Try

        If CInt(written) < count Then
            Throw New IOException()
        End If
    End Sub

    Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
        _streamValidation.ValidateSeek(offset, origin)

        Dim istreamSeekOrigin As UInteger = 0

        Select Case origin
            Case SeekOrigin.Begin
                istreamSeekOrigin = CUInt(PrinterExtensionLib.tagSTREAM_SEEK.STREAM_SEEK_SET)

            Case SeekOrigin.Current
                istreamSeekOrigin = CUInt(PrinterExtensionLib.tagSTREAM_SEEK.STREAM_SEEK_CUR)

            Case SeekOrigin.End
                istreamSeekOrigin = CUInt(PrinterExtensionLib.tagSTREAM_SEEK.STREAM_SEEK_END)
        End Select

        Dim seekPositionPointer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(New Long))
        Dim seekPosition As Long = 0
        Try
            _comIstream.Seek(offset, CInt(istreamSeekOrigin), seekPositionPointer)
            seekPosition = Marshal.ReadInt64(seekPositionPointer)
        Finally
            Marshal.FreeHGlobal(seekPositionPointer)
        End Try

        Return seekPosition
    End Function

    Public Overrides ReadOnly Property Length() As Long
        Get
            _streamValidation.ValidateSeek()

            Dim statstg As System.Runtime.InteropServices.ComTypes.STATSTG = New ComTypes.STATSTG()
            _comIstream.Stat(statstg, 1) ' STATSFLAG_NONAME
            Return statstg.cbSize
        End Get
    End Property
    Public Overrides Property Position() As Long
        Get
            Return Seek(0, SeekOrigin.Current)
        End Get
        Set(ByVal value As Long)
            Seek(value, SeekOrigin.Begin)
        End Set
    End Property

    Public Overrides Sub SetLength(ByVal value As Long)
        _streamValidation.ValidateSeek()
        _comIstream.SetSize(value)
    End Sub

    Public Overrides Sub Flush()
        _comIstream.Commit(0)
    End Sub

    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return _streamValidation.CanRead
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return _streamValidation.CanWrite
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return _streamValidation.CanSeek
        End Get
    End Property

    #End Region

    #Region "IDisposable methods"

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If _disposed Then
            Return
        End If

        Try
            If disposing Then
                _streamValidation.Dispose()
            End If

            If _comIstream IsNot Nothing Then
                Marshal.ReleaseComObject(_comIstream)
                _comIstream = Nothing
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
        _disposed = True
    End Sub

    #End Region

    #Region "Implementation details"

    ' Prevent default construction
    Private Sub New()
    End Sub

    Private _disposed As Boolean = False
    Private _comIstream As System.Runtime.InteropServices.ComTypes.IStream = Nothing
    Private _streamValidation As StreamValidation = Nothing

    #End Region
End Class

Friend Class StreamValidation
    Implements IDisposable

    Friend Sub New(Optional ByVal canWrite As Boolean = False, Optional ByVal canSeek As Boolean = False, Optional ByVal canRead As Boolean = True)
        _canWrite = canWrite
        _canSeek = canSeek
        _canRead = canRead
    End Sub

    Friend Sub ValidateRead(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        If Not _canRead Then
            Throw New NotSupportedException()
        End If
        If _disposed = True Then
            Throw New ObjectDisposedException("COM IStream")
        End If
        If buffer Is Nothing Then
            Throw New ArgumentNullException("buffer")
        End If
        If offset < 0 Then
            Throw New ArgumentOutOfRangeException("offset")
        End If
        If count < 0 Then
            Throw New ArgumentOutOfRangeException("count")
        End If
        If (buffer.Length - offset) < count Then
            Throw New ArgumentException()
        End If
    End Sub

    Friend Sub ValidateWrite(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        If Not _canWrite Then
            Throw New NotSupportedException()
        End If
        If _disposed = True Then
            Throw New ObjectDisposedException("COM IStream")
        End If
        If buffer Is Nothing Then
            Throw New ArgumentNullException("buffer")
        End If
        If offset < 0 Then
            Throw New ArgumentOutOfRangeException("offset")
        End If
        If count < 0 Then
            Throw New ArgumentOutOfRangeException("count")
        End If
        If (buffer.Length - offset) < count Then
            Throw New ArgumentException("Insufficient buffer size")
        End If
    End Sub

    Friend Sub ValidateSeek(ByVal offset As Long, ByVal origin As SeekOrigin)
        ValidateSeek()
        If (origin < SeekOrigin.Begin) OrElse (origin > SeekOrigin.End) Then
            Throw New ArgumentException("Invalid value", "origin")
        End If
    End Sub

    Friend Sub ValidateSeek()
        If Not _canSeek Then
            Throw New NotSupportedException()
        End If
        If _disposed = True Then
            Throw New ObjectDisposedException("COM IStream")
        End If
    End Sub

    Public ReadOnly Property CanRead() As Boolean
        Get
            Return _canRead
        End Get
    End Property

    Public ReadOnly Property CanWrite() As Boolean
        Get
            Return _canWrite
        End Get
    End Property

    Public ReadOnly Property CanSeek() As Boolean
        Get
            Return _canSeek
        End Get
    End Property

    #Region "IDisposable methods"

    Public Sub Dispose() Implements IDisposable.Dispose
        _disposed = True
    End Sub

    #End Region

    #Region "Implementation details"

    Private _disposed As Boolean = False
    Private _canWrite As Boolean = False
    Private _canSeek As Boolean = False
    Private _canRead As Boolean = True

    #End Region
End Class

#End Region

#Region "PrintSchema Adapter Classes"

'
' Following are concrete implementation of the PrinterExtension interfaces
' These classes wrap the underlying COM interfaces.
'

Friend Class PrintSchemaOption
    Implements IPrintSchemaOption

    #Region "IPrintSchemaOption methods"

    Public ReadOnly Property Selected() As Boolean Implements IPrintSchemaOption.Selected
        Get
            Return If(0 = _option.Selected, False, True)
        End Get
    End Property

    Public ReadOnly Property Constrained() As PrintSchemaConstrainedSetting Implements IPrintSchemaOption.Constrained
        Get
            Return CType(_option.Constrained, PrintSchemaConstrainedSetting)
        End Get
    End Property

    #End Region

    #Region "IPrintSchemaDisplayableElement methods"

    Public ReadOnly Property DisplayName() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaDisplayableElement.DisplayName
        Get
            Return _option.DisplayName
        End Get
    End Property
    Public ReadOnly Property Name() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.Name
        Get
            Return _option.Name
        End Get
    End Property
    Public ReadOnly Property XmlNamespace() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.XmlNamespace
        Get
            Return _option.NamespaceUri
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal [option] As PrinterExtensionLib.IPrintSchemaOption)
        _option = [option]
    End Sub

    Friend Property InteropOption() As PrinterExtensionLib.IPrintSchemaOption
        Get
            Return _option
        End Get
        Set(ByVal value As PrinterExtensionLib.IPrintSchemaOption)
            _option = value
        End Set
    End Property

    '
    ' Create the correct 'PrintSchemaOption' subclass, possibly exposing one of the these interfaces
    ' 1. IPrintSchemaPageMediaSizeOption
    ' 2. IPrintSchemaNUpOption
    '
    Friend Shared Function CreateOptionSubclass(ByVal [option] As PrinterExtensionLib.IPrintSchemaOption) As IPrintSchemaOption
        ' IPrintSchemaNUpOption option
        If TypeOf [option] Is PrinterExtensionLib.IPrintSchemaNUpOption Then
            Return New PrintSchemaNUpOption([option])
        End If

        ' IPrintSchemaPageMediaSizeOption option
        If TypeOf [option] Is PrinterExtensionLib.IPrintSchemaPageMediaSizeOption Then
            Return New PrintSchemaPageMediaSizeOption([option])
        End If

        Return New PrintSchemaOption([option])
    End Function

    Friend _option As PrinterExtensionLib.IPrintSchemaOption

    ' Prevent default constuction
    Private Sub New()
    End Sub

    #End Region
End Class

Friend NotInheritable Class PrintSchemaPageMediaSizeOption
    Inherits PrintSchemaOption
    Implements IPrintSchemaPageMediaSizeOption

    Private ReadOnly Property IPrintSchemaOption_Selected() As Boolean
        Get
            Return MyBase.Selected
        End Get
    End Property

    Private ReadOnly Property IPrintSchemaOption_Constrained() As PrintSchemaConstrainedSetting
        Get
            Return MyBase.Constrained
        End Get
    End Property

    #Region "IPrintSchemaPageMediaSizeOption methods"

    Public ReadOnly Property HeightInMicrons() As UInteger Implements IPrintSchemaPageMediaSizeOption.HeightInMicrons
        Get
            Return _pageMediaSizeOption.HeightInMicrons
        End Get
    End Property

    Public ReadOnly Property WidthInMicrons() As UInteger Implements IPrintSchemaPageMediaSizeOption.WidthInMicrons
        Get
            Return _pageMediaSizeOption.WidthInMicrons
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal [option] As PrinterExtensionLib.IPrintSchemaOption)
        MyBase.New([option])
        _pageMediaSizeOption = TryCast(_option, PrinterExtensionLib.IPrintSchemaPageMediaSizeOption)
        If Nothing Is _pageMediaSizeOption Then
            Throw New NotImplementedException("Could not retrieve IPrintSchemaPageMediaSizeOption interface.")
        End If
    End Sub

    Private _pageMediaSizeOption As PrinterExtensionLib.IPrintSchemaPageMediaSizeOption

    #End Region
End Class

Friend NotInheritable Class PrintSchemaNUpOption
    Inherits PrintSchemaOption
    Implements IPrintSchemaNUpOption

    Private ReadOnly Property IPrintSchemaOption_Selected() As Boolean
        Get
            Return MyBase.Selected
        End Get
    End Property

    Private ReadOnly Property IPrintSchemaOption_Constrained() As PrintSchemaConstrainedSetting
        Get
            Return MyBase.Constrained
        End Get
    End Property

    #Region "IPrintSchemaNUpOption methods"

    Public ReadOnly Property PagesPerSheet() As UInteger Implements IPrintSchemaNUpOption.PagesPerSheet
        Get
            Return _nupOption.PagesPerSheet
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal [option] As PrinterExtensionLib.IPrintSchemaOption)
        MyBase.New([option])
        _nupOption = TryCast(_option, PrinterExtensionLib.IPrintSchemaNUpOption)
        If Nothing Is _nupOption Then
            Throw New NotImplementedException("Could not retrieve IPrintSchemaNUpOption interface.")
        End If
    End Sub

    Private _nupOption As PrinterExtensionLib.IPrintSchemaNUpOption

    #End Region
End Class

''' <summary>
''' This class provides wraps IPrintSchemaOptionCollection in a IEnumerable interface
''' </summary>
Friend NotInheritable Class PrintSchemaOptionsCollection
    Implements IEnumerable(Of IPrintSchemaOption)

    #Region "IEnumerable<IPrintSchemaOption> methods"

    Public Iterator Function GetEnumerator() As IEnumerator(Of IPrintSchemaOption) Implements IEnumerable(Of IPrintSchemaOption).GetEnumerator
        For i As UInteger = 0 To CUInt(_optionCollection.Count - 1)
            Yield New PrintSchemaOption(_optionCollection.GetAt(i))
        Next i
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield CType(GetEnumerator(), IEnumerator)
    End Function

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal optionCollection As PrinterExtensionLib.IPrintSchemaOptionCollection)
        _optionCollection = optionCollection
    End Sub

    Private _optionCollection As PrinterExtensionLib.IPrintSchemaOptionCollection

    #End Region
End Class

Friend NotInheritable Class PrintSchemaFeature
    Implements IPrintSchemaFeature

    #Region "IPrintSchemaFeature methods"

    Public ReadOnly Property SelectionType() As PrintSchemaSelectionType Implements IPrintSchemaFeature.SelectionType
        Get
            Return CType(_feature.SelectionType, PrintSchemaSelectionType)
        End Get
    End Property

    Public Function GetOption(ByVal name As String) As IPrintSchemaOption Implements IPrintSchemaFeature.GetOption
        Return GetOption(name, PrintSchemaConstants.KeywordsNamespaceUri)
    End Function

    Public Function GetOption(ByVal name As String, ByVal xmlNamespace As String) As IPrintSchemaOption Implements IPrintSchemaFeature.GetOption
        Dim [option] As PrinterExtensionLib.IPrintSchemaOption = _feature.GetOption(name, xmlNamespace)
        If [option] IsNot Nothing Then
            Return PrintSchemaOption.CreateOptionSubclass([option])
        End If

        Return Nothing
    End Function

    Public Property SelectedOption() As IPrintSchemaOption Implements IPrintSchemaFeature.SelectedOption
        Get
            Return PrintSchemaOption.CreateOptionSubclass(_feature.SelectedOption)
        End Get
        Set(ByVal value As IPrintSchemaOption)
            _feature.SelectedOption = (TryCast(value, PrintSchemaOption)).InteropOption
        End Set
    End Property

    Public ReadOnly Property DisplayUI() As Boolean Implements IPrintSchemaFeature.DisplayUI
        Get
            Return If(0 = _feature.DisplayUI, False, True)
        End Get
    End Property

    #End Region

    #Region "IPrintSchemaDisplayableElement methods"

    Public ReadOnly Property DisplayName() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaDisplayableElement.DisplayName
        Get
            Return _feature.DisplayName
        End Get
    End Property
    Public ReadOnly Property Name() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.Name
        Get
            Return _feature.Name
        End Get
    End Property
    Public ReadOnly Property XmlNamespace() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.XmlNamespace
        Get
            Return _feature.NamespaceUri
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal feature As PrinterExtensionLib.IPrintSchemaFeature)
        _feature = feature
    End Sub

    Friend ReadOnly Property InteropFeature() As PrinterExtensionLib.IPrintSchemaFeature
        Get
            Return _feature
        End Get
    End Property

    Private _feature As PrinterExtensionLib.IPrintSchemaFeature

    #End Region
End Class

Friend NotInheritable Class PrintSchemaPageImageableSize
    Implements IPrintSchemaPageImageableSize

    #Region "IPrintSchemaPageImageableSize methods"

    Public ReadOnly Property ExtentHeightInMicrons() As UInteger Implements IPrintSchemaPageImageableSize.ExtentHeightInMicrons
        Get
            Return _pageImageableSize.ExtentHeightInMicrons
        End Get
    End Property

    Public ReadOnly Property ExtentWidthInMicrons() As UInteger Implements IPrintSchemaPageImageableSize.ExtentWidthInMicrons
        Get
            Return _pageImageableSize.ExtentWidthInMicrons
        End Get
    End Property

    Public ReadOnly Property ImageableSizeHeightInMicrons() As UInteger Implements IPrintSchemaPageImageableSize.ImageableSizeHeightInMicrons
        Get
            Return _pageImageableSize.ImageableSizeHeightInMicrons
        End Get
    End Property

    Public ReadOnly Property ImageableSizeWidthInMicrons() As UInteger Implements IPrintSchemaPageImageableSize.ImageableSizeWidthInMicrons
        Get
            Return _pageImageableSize.ImageableSizeWidthInMicrons
        End Get
    End Property

    Public ReadOnly Property OriginHeightInMicrons() As UInteger Implements IPrintSchemaPageImageableSize.OriginHeightInMicrons
        Get
            Return _pageImageableSize.OriginHeightInMicrons
        End Get
    End Property

    Public ReadOnly Property OriginWidthInMicrons() As UInteger Implements IPrintSchemaPageImageableSize.OriginWidthInMicrons
        Get
            Return _pageImageableSize.OriginWidthInMicrons
        End Get
    End Property

    #End Region

    #Region "IPrintSchemaElement methods"

    Public ReadOnly Property Name() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.Name
        Get
            Return _pageImageableSize.Name
        End Get
    End Property
    Public ReadOnly Property XmlNamespace() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.XmlNamespace
        Get
            Return _pageImageableSize.NamespaceUri
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal pageImageableSize As PrinterExtensionLib.IPrintSchemaPageImageableSize)
        _pageImageableSize = pageImageableSize
    End Sub

    Private _pageImageableSize As PrinterExtensionLib.IPrintSchemaPageImageableSize

    #End Region
End Class

#If WINDOWS_81_APIS Then
Friend NotInheritable Class PrintSchemaParameterDefinition
    Implements IPrintSchemaParameterDefinition

    #Region "IPrintSchemaParameterDefinition methods"

    Public ReadOnly Property UserInputRequired() As Boolean Implements IPrintSchemaParameterDefinition.UserInputRequired
        Get
            If _parameter.UserInputRequired <> 0 Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property UnitType() As String Implements IPrintSchemaParameterDefinition.UnitType
        Get
            Return _parameter.UnitType
        End Get
    End Property

    Public ReadOnly Property DataType() As PrintSchemaParameterDataType Implements IPrintSchemaParameterDefinition.DataType
        Get
            Return CType(_parameter.DataType, PrintSchemaParameterDataType)
        End Get
    End Property

    Public ReadOnly Property RangeMin() As Integer Implements IPrintSchemaParameterDefinition.RangeMin
        Get
            Return _parameter.RangeMin
        End Get
    End Property

    Public ReadOnly Property RangeMax() As Integer Implements IPrintSchemaParameterDefinition.RangeMax
        Get
            Return _parameter.RangeMax
        End Get
    End Property
    #End Region

    #Region "IPrintSchemaDisplayableItem methods"

    Public ReadOnly Property DisplayName() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaDisplayableElement.DisplayName
        Get
            Return _parameter.DisplayName
        End Get
    End Property
    Public ReadOnly Property Name() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.Name
        Get
            Return _parameter.Name
        End Get
    End Property
    Public ReadOnly Property XmlNamespace() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.XmlNamespace
        Get
            Return _parameter.NamespaceUri
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal parameter As PrinterExtensionLib.IPrintSchemaParameterDefinition)
        _parameter = parameter
    End Sub

    Private _parameter As PrinterExtensionLib.IPrintSchemaParameterDefinition

    #End Region
End Class

Friend NotInheritable Class PrintSchemaParameterInitializer
    Implements IPrintSchemaParameterInitializer

    #Region "IPrintSchemaParameterInitializer methods"

    Public Property StringValue() As String Implements IPrintSchemaParameterInitializer.StringValue
        Get
            Dim value As Object = _parameter.get_Value()
            Return CStr(value)
        End Get
        Set(ByVal value As String)
            _parameter.set_Value(value)
        End Set
    End Property

    Public Property IntegerValue() As Integer Implements IPrintSchemaParameterInitializer.IntegerValue
        Get
            Dim value As Object = _parameter.get_Value()
            Return CInt(Fix(value))
        End Get
        Set(ByVal value As Integer)
            _parameter.set_Value(value)
        End Set
    End Property
    #End Region

    #Region "IPrintSchemaElement methods"

    Public ReadOnly Property Name() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.Name
        Get
            Return _parameter.Name
        End Get
    End Property
    Public ReadOnly Property XmlNamespace() As String Implements Microsoft.Samples.Printing.PrinterExtension.Types.IPrintSchemaElement.XmlNamespace
        Get
            Return _parameter.NamespaceUri
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal parameter As PrinterExtensionLib.IPrintSchemaParameterInitializer)
        _parameter = parameter
    End Sub

    Private _parameter As PrinterExtensionLib.IPrintSchemaParameterInitializer

    #End Region
End Class
#End If

Friend NotInheritable Class PrintSchemaCapabilities
    Implements IPrintSchemaCapabilities

    #Region "IPrintSchemaCapabilities methods"

    Public Function GetFeatureByKeyName(ByVal keyName As String) As IPrintSchemaFeature Implements IPrintSchemaCapabilities.GetFeatureByKeyName
        Dim feature As PrinterExtensionLib.IPrintSchemaFeature = _capabilities.GetFeatureByKeyName(keyName)
        If feature IsNot Nothing Then
            Return New PrintSchemaFeature(feature)
        End If

        Return Nothing
    End Function

    Public Function GetFeature(ByVal featureName As String) As IPrintSchemaFeature Implements IPrintSchemaCapabilities.GetFeature
        Return GetFeature(featureName, PrintSchemaConstants.KeywordsNamespaceUri)
    End Function

    Public Function GetFeature(ByVal featureName As String, ByVal xmlNamespace As String) As IPrintSchemaFeature Implements IPrintSchemaCapabilities.GetFeature
        Dim feature As PrinterExtensionLib.IPrintSchemaFeature = _capabilities.GetFeature(featureName, xmlNamespace)
        If feature IsNot Nothing Then
            Return New PrintSchemaFeature(feature)
        End If

        Return Nothing
    End Function

    Public ReadOnly Property PageImageableSize() As IPrintSchemaPageImageableSize Implements IPrintSchemaCapabilities.PageImageableSize
        Get
            Return New PrintSchemaPageImageableSize(_capabilities.PageImageableSize)
        End Get
    End Property

    Public ReadOnly Property JobCopiesAllDocumentsMaxValue() As UInteger Implements IPrintSchemaCapabilities.JobCopiesAllDocumentsMaxValue
        Get
            Dim value As UInteger = _capabilities.JobCopiesAllDocumentsMaxValue
            If value = 0 Then
                Throw New NotSupportedException("Property ""JobCopiesAllDocumentsMaxValue"" not found in print capabilities.")
            End If

            Return value
        End Get
    End Property

    Public ReadOnly Property JobCopiesAllDocumentsMinValue() As UInteger Implements IPrintSchemaCapabilities.JobCopiesAllDocumentsMinValue
        Get
            Dim value As UInteger = _capabilities.JobCopiesAllDocumentsMinValue
            If value = 0 Then
                Throw New NotSupportedException("Property ""JobCopiesAllDocumentsMinValue"" not found in print capabilities.")
            End If

            Return value
        End Get
    End Property

    Public Function GetSelectedOptionInPrintTicket(ByVal feature As IPrintSchemaFeature) As IPrintSchemaOption Implements IPrintSchemaCapabilities.GetSelectedOptionInPrintTicket
        Dim f As PrintSchemaFeature = TryCast(feature, PrintSchemaFeature)
        Dim [option] As PrinterExtensionLib.IPrintSchemaOption = _capabilities.GetSelectedOptionInPrintTicket(f.InteropFeature)

        If [option] IsNot Nothing Then
            Return PrintSchemaOption.CreateOptionSubclass([option])
        End If

        Return Nothing
    End Function

    Public Function GetOptions(ByVal pFeature As IPrintSchemaFeature) As IEnumerable(Of IPrintSchemaOption) Implements IPrintSchemaCapabilities.GetOptions
        Return New PrintSchemaOptionsCollection(_capabilities.GetOptions((TryCast(pFeature, PrintSchemaFeature)).InteropFeature))
    End Function

    Public Function GetReadStream() As Stream Implements IPrintSchemaCapabilities.GetReadStream
        Return New ComIStreamAdapter(XmlStream, False, True, True) ' canRead -  canSeek -  canWrite
    End Function

    Public Function GetWriteStream() As Stream Implements IPrintSchemaCapabilities.GetWriteStream
        Return New ComIStreamAdapter(XmlStream, True, True, False) ' canRead -  canSeek -  canWrite
    End Function

#If WINDOWS_81_APIS Then
    Public Function GetParameterDefinition(ByVal parameterName As String) As IPrintSchemaParameterDefinition Implements IPrintSchemaCapabilities.GetParameterDefinition
        Return GetParameterDefinition(parameterName, PrintSchemaConstants.KeywordsNamespaceUri)
    End Function

    Public Function GetParameterDefinition(ByVal parameterName As String, ByVal xmlNamespace As String) As IPrintSchemaParameterDefinition Implements IPrintSchemaCapabilities.GetParameterDefinition
        Dim parameter As PrinterExtensionLib.IPrintSchemaParameterDefinition = _capabilities2.GetParameterDefinition(parameterName, xmlNamespace)
        If parameter IsNot Nothing Then
            Return New PrintSchemaParameterDefinition(parameter)
        End If

        Return Nothing
    End Function
#End If

    Private ReadOnly Property XmlStream() As System.Runtime.InteropServices.ComTypes.IStream
        Get
            Dim istream As System.Runtime.InteropServices.ComTypes.IStream = TryCast(_capabilities.XmlNode, System.Runtime.InteropServices.ComTypes.IStream)

            Return istream
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal caps As PrinterExtensionLib.IPrintSchemaCapabilities)
        _capabilities = caps
#If WINDOWS_81_APIS Then
        _capabilities2 = CType(caps, PrinterExtensionLib.IPrintSchemaCapabilities2)
#End If
    End Sub

    Private _capabilities As PrinterExtensionLib.IPrintSchemaCapabilities
#If WINDOWS_81_APIS Then
    Private _capabilities2 As PrinterExtensionLib.IPrintSchemaCapabilities2
#End If
    #End Region
End Class

Friend Class PrintSchemaAsyncOperation
    Implements IPrintSchemaAsyncOperation

    #Region "IPrintSchemaAsyncOperation methods"

    Public Sub Cancel() Implements IPrintSchemaAsyncOperation.Cancel
        _asyncOperation.Cancel()
    End Sub

    Public Sub Start() Implements IPrintSchemaAsyncOperation.Start
        _asyncOperation.Start()
    End Sub

    Public Event Completed As EventHandler(Of PrintSchemaAsyncOperationEventArgs) Implements IPrintSchemaAsyncOperation.Completed

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal asyncOperation As PrinterExtensionLib.PrintSchemaAsyncOperation)
        _asyncOperation = asyncOperation
        AddHandler _asyncOperation.Completed, AddressOf _asyncOperation_Completed
    End Sub

    Private Sub _asyncOperation_Completed(ByVal printTicket As PrinterExtensionLib.IPrintSchemaTicket, ByVal hrOperation As Integer)
        If CompletedEvent IsNot Nothing Then
            Dim ticket As IPrintSchemaTicket = New PrintSchemaTicket(printTicket)
            RaiseEvent Completed(Me, New PrintSchemaAsyncOperationEventArgs(ticket, hrOperation))
        End If

        ' This subscriber object (current object) holds a reference to the publishing object (i.e the underlying COM object)
        ' because it is a class member, and the publishing object holds a reference to the subscriber via the registered delegate.
        ' This implies neither object will be garbage collected until the application terminates.
        ' It's expected the event is fired once per instance so unsubscribing the delegate has no side effects.
        RemoveHandler _asyncOperation.Completed, AddressOf _asyncOperation_Completed
        Marshal.ReleaseComObject(_asyncOperation)
        _asyncOperation = Nothing
    End Sub

    Private _asyncOperation As PrinterExtensionLib.PrintSchemaAsyncOperation

    #End Region
End Class

Friend Class PrintSchemaTicket
    Implements IPrintSchemaTicket

    #Region "IPrintSchemaTicket methods"

    Public Function GetCapabilities() As IPrintSchemaCapabilities Implements IPrintSchemaTicket.GetCapabilities
        Return New PrintSchemaCapabilities(_printTicket.GetCapabilities())
    End Function

    Public Function GetFeature(ByVal featureName As String) As IPrintSchemaFeature Implements IPrintSchemaTicket.GetFeature
        Return GetFeature(featureName, PrintSchemaConstants.KeywordsNamespaceUri)
    End Function

    Public Function GetFeature(ByVal featureName As String, ByVal xmlNamespace As String) As IPrintSchemaFeature Implements IPrintSchemaTicket.GetFeature
        Dim feature As PrinterExtensionLib.IPrintSchemaFeature = _printTicket.GetFeature(featureName, xmlNamespace)
        If feature IsNot Nothing Then
            Return New PrintSchemaFeature(feature)
        End If

        Return Nothing
    End Function

    Public Function GetFeatureByKeyName(ByVal keyName As String) As IPrintSchemaFeature Implements IPrintSchemaTicket.GetFeatureByKeyName
        Dim feature As PrinterExtensionLib.IPrintSchemaFeature = _printTicket.GetFeatureByKeyName(keyName)
        If feature IsNot Nothing Then
            Return New PrintSchemaFeature(feature)
        End If

        Return Nothing
    End Function

    Public Property JobCopiesAllDocuments() As UInteger Implements IPrintSchemaTicket.JobCopiesAllDocuments
        Get
            Dim value As UInteger = _printTicket.JobCopiesAllDocuments
            If value = 0 Then
                Throw New NotSupportedException("Property ""JobCopiesAllDocuments"" not found in print ticket.")
            End If

            Return value
        End Get
        Set(ByVal value As UInteger)
            _printTicket.JobCopiesAllDocuments = value
        End Set
    End Property

    Public Function GetReadStream() As Stream Implements IPrintSchemaTicket.GetReadStream
        Return New ComIStreamAdapter(XmlStream, False, True, True) ' canRead -  canSeek -  canWrite
    End Function

    Public Function GetWriteStream() As Stream Implements IPrintSchemaTicket.GetWriteStream
        Return New ComIStreamAdapter(XmlStream, True, True, False) ' canRead -  canSeek -  canWrite
    End Function

    Private ReadOnly Property XmlStream() As System.Runtime.InteropServices.ComTypes.IStream
        Get
            Dim istream As System.Runtime.InteropServices.ComTypes.IStream = TryCast(_printTicket.XmlNode, System.Runtime.InteropServices.ComTypes.IStream)

            Return istream
        End Get
    End Property

    Public Function ValidateAsync() As IPrintSchemaAsyncOperation Implements IPrintSchemaTicket.ValidateAsync
        Dim interopAsyncOperation As PrinterExtensionLib.PrintSchemaAsyncOperation = Nothing
        _printTicket.ValidateAsync(interopAsyncOperation)
        Return New PrintSchemaAsyncOperation(interopAsyncOperation)
    End Function

    Public Function CommitAsync(ByVal printTicketCommit As IPrintSchemaTicket) As IPrintSchemaAsyncOperation Implements IPrintSchemaTicket.CommitAsync
        Dim interopTicket As PrinterExtensionLib.IPrintSchemaTicket = (TryCast(printTicketCommit, PrintSchemaTicket))._printTicket
        Dim interopAsyncOperation As PrinterExtensionLib.PrintSchemaAsyncOperation = Nothing
        _printTicket.CommitAsync(interopTicket, interopAsyncOperation)
        Return New PrintSchemaAsyncOperation(interopAsyncOperation)
    End Function

    Public Sub NotifyXmlChanged() Implements IPrintSchemaTicket.NotifyXmlChanged
        _printTicket.NotifyXmlChanged()
    End Sub
#If WINDOWS_81_APIS Then
    Public Function GetParameterInitializer(ByVal parameterName As String) As IPrintSchemaParameterInitializer Implements IPrintSchemaTicket.GetParameterInitializer
        Return GetParameterInitializer(parameterName, PrintSchemaConstants.KeywordsNamespaceUri)
    End Function

    Public Function GetParameterInitializer(ByVal parameterName As String, ByVal xmlNamespace As String) As IPrintSchemaParameterInitializer Implements IPrintSchemaTicket.GetParameterInitializer
        Dim parameter As PrinterExtensionLib.IPrintSchemaParameterInitializer = _printTicket2.GetParameterInitializer(parameterName, xmlNamespace)
        If parameter IsNot Nothing Then
            Return New PrintSchemaParameterInitializer(parameter)
        End If

        Return Nothing
    End Function
#End If

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal printTicket As PrinterExtensionLib.IPrintSchemaTicket)
        _printTicket = printTicket
#If WINDOWS_81_APIS Then
        _printTicket2 = CType(printTicket, PrinterExtensionLib.IPrintSchemaTicket2)
#End If
    End Sub

    Private _printTicket As PrinterExtensionLib.IPrintSchemaTicket
#If WINDOWS_81_APIS Then
    Private _printTicket2 As PrinterExtensionLib.IPrintSchemaTicket2
#End If

    #End Region
End Class

Friend Enum PrintPropertyBagType
    QueueProperties
    DriverProperties
    UserProperties
End Enum

Friend Class PrinterPropertyBag
    Implements IPrinterPropertyBag

    #Region "IPrinterPropertyBag methods"

    Public Function GetBool(ByVal propertyName As String) As Boolean Implements IPrinterPropertyBag.GetBool
        Try
            Dim integerEquivalent As Integer = _bag.GetBool(propertyName)
            Dim boolEquivalent As Boolean = True
            If integerEquivalent = 0 Then
                boolEquivalent = False
            End If
            Return boolEquivalent
        Catch e As ArgumentException
            ' Fix the type of exception thrown when the property does not exist.
            If ShouldConvertExceptionType(e) Then
                Throw New FileNotFoundException("", e)
            End If
            Throw
        End Try
    End Function

    Public Function GetBytes(ByVal propertyName As String) As Byte() Implements IPrinterPropertyBag.GetBytes
        Try
            Dim count As UInteger = 0
            Dim intptrData As IntPtr = Marshal.AllocCoTaskMem(IntPtr.Size)
            _bag.GetBytes(propertyName, count, intptrData)

            Dim data(CInt(count - 1)) As Byte
            Marshal.Copy(Marshal.ReadIntPtr(intptrData), data, 0, CInt(count))
            Marshal.FreeCoTaskMem(Marshal.ReadIntPtr(intptrData))
            Marshal.FreeCoTaskMem(intptrData)
            Return data
        Catch e As ArgumentException
            ' Fix the type of exception thrown when the property does not exist.
            If ShouldConvertExceptionType(e) Then
                Throw New FileNotFoundException("", e)
            End If
            Throw
        End Try
    End Function

    Public Function GetInt(ByVal propertyName As String) As Integer Implements IPrinterPropertyBag.GetInt
        Try
            Return _bag.GetInt32(propertyName)
        Catch e As ArgumentException
            ' Fix the type of exception thrown when the property does not exist.
            If ShouldConvertExceptionType(e) Then
                Throw New FileNotFoundException("", e)
            End If
            Throw
        End Try
    End Function

    Public Function GetString(ByVal propertyName As String) As String Implements IPrinterPropertyBag.GetString
        Try
            Return _bag.GetString(propertyName)
        Catch e As ArgumentException
            ' Fix the type of exception thrown when the property does not exist.
            If ShouldConvertExceptionType(e) Then
                Throw New FileNotFoundException("", e)
            End If
            Throw
        End Try
    End Function

    Public Sub SetBool(ByVal propertyName As String, ByVal value As Boolean) Implements IPrinterPropertyBag.SetBool
        Dim integerEquivalent As Integer = 1
        If value = False Then
            integerEquivalent = 0
        End If

        _bag.SetBool(propertyName, integerEquivalent)
    End Sub

    Public Sub SetBytes(ByVal propertyName As String, ByVal data() As Byte) Implements IPrinterPropertyBag.SetBytes
        ' Pin the byte array so that it will not be moved by the garbage collector
        ' This would not be required if the COM Interop function took in a byte[] parameter
        ' as opposed to a byte parameter
        Dim gcHandle As GCHandle = GCHandle.Alloc(data, GCHandleType.Pinned)
        Try
            _bag.SetBytes(propertyName, Convert.ToUInt32(data.Length), data(0))
        Finally
            gcHandle.Free()
        End Try
    End Sub

    Public Sub SetInt(ByVal propertyName As String, ByVal value As Integer) Implements IPrinterPropertyBag.SetInt
        _bag.SetInt32(propertyName, value)
    End Sub

    Public Sub SetString(ByVal propertyName As String, ByVal value As String) Implements IPrinterPropertyBag.SetString
        _bag.SetString(propertyName, value)
    End Sub

    Public Function GetReadStream(ByVal propertyName As String) As Stream Implements IPrinterPropertyBag.GetReadStream
        Try
            Return New PrinterExtensionLibIStreamAdapter(_bag.GetReadStream(propertyName), False, True, True)
        Catch e As COMException
            ' Fix the type of exception thrown when the property does not exist.
            If ShouldConvertExceptionType(e) Then
                Throw New FileNotFoundException("", e)
            End If
            Throw
        End Try
    End Function

    Public Function GetWriteStream(ByVal propertyName As String) As Stream Implements IPrinterPropertyBag.GetWriteStream
        Return New PrinterExtensionLibIStreamAdapter(_bag.GetWriteStream(propertyName), True, True, False)
    End Function

    #End Region

    #Region "Indexer"

    Default Public ReadOnly Property Item(ByVal name As String) As PrinterProperty Implements IPrinterPropertyBag.Item
        Get
            Return New PrinterProperty(Me, name)
        End Get
    End Property

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal bag As PrinterExtensionLib.IPrinterPropertyBag, ByVal type As PrintPropertyBagType)
        _bag = bag
        _type = type
    End Sub

    ''' <summary>
    ''' Check if exception thrown when a property is not found in a property bag needs to be converted.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <returns>True if the exception type needs to be converted.</returns>
    Private Function ShouldConvertExceptionType(ByVal e As Exception) As Boolean
        ' Only the driver property bag throws exceptions other than 'FileNotFoundException'
        ' when a property is not found.
        If _type <> PrintPropertyBagType.DriverProperties Then
            Return False
        ElseIf TypeOf e Is ArgumentException Then
            Return True
        ElseIf TypeOf e Is COMException Then
            ' Since there is no portable way to check the HRESULT across classic .Net and 
            ' .Net for Windows Store apps, all COMExceptions encountered are converted.
            Return True
        End If
        Return False
    End Function

    Private _bag As PrinterExtensionLib.IPrinterPropertyBag
    Private _type As PrintPropertyBagType

    #End Region
End Class

#If WINDOWS_81_APIS Then
Public NotInheritable Class PrintJob
    Implements IPrintJob

    #Region "IPrintJob methods"

    Public ReadOnly Property Name() As String Implements IPrintJob.Name
        Get
            Return _job.Name
        End Get
    End Property

    Public ReadOnly Property Id() As ULong Implements IPrintJob.Id
        Get
            Return _job.Id
        End Get
    End Property

    Public ReadOnly Property PrintedPages() As ULong Implements IPrintJob.PrintedPages
        Get
            Return _job.PrintedPages
        End Get
    End Property

    Public ReadOnly Property TotalPages() As ULong Implements IPrintJob.TotalPages
        Get
            Return _job.TotalPages
        End Get
    End Property

    Public ReadOnly Property Status() As PrintJobStatus Implements IPrintJob.Status
        Get
            Return CType(_job.Status, PrintJobStatus)
        End Get
    End Property

    Public ReadOnly Property SubmissionTime() As Date Implements IPrintJob.SubmissionTime
        Get
            Return _job.SubmissionTime
        End Get
    End Property

    Public Sub RequestCancel() Implements IPrintJob.RequestCancel
        _job.RequestCancel()
    End Sub

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal job As PrinterExtensionLib.IPrintJob)
        _job = job
    End Sub

    Private _job As PrinterExtensionLib.IPrintJob

    #End Region
End Class

''' <summary>
''' This class provides wraps IPrintJobCollection in a IEnumerable interface
''' </summary>
Public NotInheritable Class PrintJobCollection
    Implements IEnumerable(Of IPrintJob)

    #Region "IEnumerable<IPrintJob> methods"

    Public Iterator Function GetEnumerator() As IEnumerator(Of IPrintJob) Implements IEnumerable(Of IPrintJob).GetEnumerator
        For i As UInteger = 0 To _jobCollection.Count - 1
            Yield New PrintJob(_jobCollection.GetAt(i))
        Next i
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return CType(GetEnumerator(), IEnumerator)
    End Function

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal jobCollection As PrinterExtensionLib.IPrintJobCollection)
        _jobCollection = jobCollection
    End Sub

    Private _jobCollection As PrinterExtensionLib.IPrintJobCollection

    #End Region
End Class

Friend NotInheritable Class PrinterQueueView
    Implements IPrinterQueueView

    #Region "IPrinterQueueView methods"

    Public Sub SetViewRange(ByVal viewOffset As UInteger, ByVal viewSize As UInteger) Implements IPrinterQueueView.SetViewRange
        _view.SetViewRange(viewOffset, viewSize)
    End Sub

    Public Custom Event OnChanged As EventHandler(Of PrinterQueueViewEventArgs) Implements IPrinterQueueView.OnChanged
        AddHandler(ByVal value As EventHandler(Of PrinterQueueViewEventArgs))
            If _onChangedEvent Is Nothing Then
                AddHandler _view.OnChanged, AddressOf _view_OnChanged
            End If
            AddHandler _onChanged, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of PrinterQueueViewEventArgs))
            RemoveHandler _onChanged, value
            If _onChangedEvent Is Nothing Then
                RemoveHandler _view.OnChanged, AddressOf _view_OnChanged
            End If
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As PrinterQueueViewEventArgs)
            RaiseEvent _onChanged(sender, e)
        End RaiseEvent
    End Event

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal view As PrinterExtensionLib.PrinterQueueView)
        _view = view
    End Sub

    Private Sub _view_OnChanged(ByVal pCollection As PrinterExtensionLib.IPrintJobCollection, ByVal ulViewOffset As UInteger, ByVal ulViewSize As UInteger, ByVal ulCountJobsInPrintQueue As UInteger)
        RaiseEvent _onChanged(Me, New PrinterQueueViewEventArgs(New PrintJobCollection(pCollection), ulViewOffset, ulViewSize, ulCountJobsInPrintQueue))
    End Sub

    Private _view As PrinterExtensionLib.PrinterQueueView
    Private Event _onChanged As EventHandler(Of PrinterQueueViewEventArgs)

    #End Region
End Class

Friend NotInheritable Class PrinterBidiSetRequestCallback
    Implements PrinterExtensionLib.IPrinterBidiSetRequestCallback

    #Region "IPrinterBidiSetRequestCallback methods"

    Public Sub Completed(ByVal response As String, ByVal statusHResult As Integer)
        _callback.Completed(response, statusHResult)
    End Sub

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal callback As IPrinterBidiSetRequestCallback)
        _callback = callback
    End Sub

    Private _callback As IPrinterBidiSetRequestCallback

    #End Region
End Class
#End If

Friend NotInheritable Class PrinterQueue
    Implements IPrinterQueue

    #Region "IPrinterQueue methods"

    Public ReadOnly Property Name() As String Implements IPrinterQueue.Name
        Get
            Return _queue.Name
        End Get
    End Property

    Public Sub SendBidiQuery(ByVal bidiQuery As String) Implements IPrinterQueue.SendBidiQuery
        _queue.SendBidiQuery(bidiQuery)
    End Sub

    Public ReadOnly Property Handle() As IntPtr Implements IPrinterQueue.Handle
        Get
            Return _queue.Handle
        End Get
    End Property

    Public Function GetProperties() As IPrinterPropertyBag Implements IPrinterQueue.GetProperties
        Return New PrinterPropertyBag(_queue.GetProperties(), PrintPropertyBagType.QueueProperties)
    End Function
    Public Custom Event OnBidiResponseReceived As EventHandler(Of PrinterQueueEventArgs) Implements IPrinterQueue.OnBidiResponseReceived
        AddHandler(ByVal value As EventHandler(Of PrinterQueueEventArgs))
            If _onBidiResponseReceivedEvent Is Nothing Then
                AddHandler _queue.OnBidiResponseReceived, AddressOf _queue_OnBidiResponseReceived
            End If
            AddHandler _onBidiResponseReceived, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of PrinterQueueEventArgs))
            RemoveHandler _onBidiResponseReceived, value
            If _onBidiResponseReceivedEvent Is Nothing Then
                RemoveHandler _queue.OnBidiResponseReceived, AddressOf _queue_OnBidiResponseReceived
            End If
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As PrinterQueueEventArgs)
            RaiseEvent _onBidiResponseReceived(sender, e)
        End RaiseEvent
    End Event

#If WINDOWS_81_APIS Then
    Public Function SendBidiSetRequestAsync(ByVal bidiRequest As String, ByVal callback As IPrinterBidiSetRequestCallback) As IPrinterExtensionAsyncOperation Implements IPrinterQueue.SendBidiSetRequestAsync
        Dim comCallback As New PrinterBidiSetRequestCallback(callback)
        Dim comCallbackInterface As PrinterExtensionLib.IPrinterBidiSetRequestCallback = comCallback
        Return New PrinterExtensionAsyncOperation(_queue2.SendBidiSetRequestAsync(bidiRequest, comCallbackInterface))
    End Function

    Public Function GetPrinterQueueView(ByVal viewOffset As UInteger, ByVal viewSize As UInteger) As IPrinterQueueView Implements IPrinterQueue.GetPrinterQueueView
        Return New PrinterQueueView(_queue2.GetPrinterQueueView(viewOffset, viewSize))
    End Function
#End If

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal queue As PrinterExtensionLib.PrinterQueue)
        _queue = queue
#If WINDOWS_81_APIS Then
        _queue2 = CType(queue, PrinterExtensionLib.IPrinterQueue2)
#End If
    End Sub

    Private Sub _queue_OnBidiResponseReceived(ByVal bstrResponse As String, ByVal hrStatus As Integer)
        RaiseEvent _onBidiResponseReceived(Me, New PrinterQueueEventArgs(bstrResponse, hrStatus))
    End Sub

    Private Event _onBidiResponseReceived As EventHandler(Of PrinterQueueEventArgs)
#If WINDOWS_81_APIS Then
    Private _queue2 As PrinterExtensionLib.IPrinterQueue2
#End If
    Private _queue As PrinterExtensionLib.PrinterQueue

    #End Region
End Class

Friend NotInheritable Class PrinterExtensionRequest
    Implements IPrinterExtensionRequest

    #Region "IPrinterExtensionRequest methods"

    Public Sub Complete() Implements IPrinterExtensionRequest.Complete
        _request.Complete()
    End Sub

    Public Sub Cancel(ByVal hr As Integer, ByVal logMessage As String) Implements IPrinterExtensionRequest.Cancel
        _request.Cancel(hr, logMessage)
    End Sub

    #End Region

    #Region "Implementation details"

    Friend Sub New(ByVal request As PrinterExtensionLib.IPrinterExtensionRequest)
        _request = request
    End Sub

    Private _request As PrinterExtensionLib.IPrinterExtensionRequest

    #End Region
End Class

#End Region

