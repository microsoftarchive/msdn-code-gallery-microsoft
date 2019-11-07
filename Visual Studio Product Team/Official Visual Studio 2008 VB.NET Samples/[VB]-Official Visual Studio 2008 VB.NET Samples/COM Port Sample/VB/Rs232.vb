' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Public Class RS232

    ' Declare the necessary class variables, and their initial values.		
    Private mhRS As Integer = -1   ' Handle to Com Port									
    Private miPort As Integer = 1   ' Default is COM1	
    Private miTimeout As Integer = 70   ' Timeout in ms
    Private miBaudRate As Integer = 9600
    Private meParity As DataParity = 0
    Private meStopBit As DataStopBit = 0
    Private miDataBit As Integer = 8
    Private miBufferSize As Integer = 512   ' Buffers size default to 512 bytes
    Private mabtRxBuf As Byte()   ' Receive buffer	
    Private meMode As Mode  ' Class working mode	
    Private mbWaitOnRead As Boolean
    Private mbWaitOnWrite As Boolean
    Private mbWriteErr As Boolean
    Private muOverlapped As OVERLAPPED
    Private muOverlappedW As OVERLAPPED
    Private muOverlappedE As OVERLAPPED
    Private mabtTmpTxBuf As Byte()  ' Temporary buffer used by Async Tx
    Private moThreadTx As Thread
    Private moThreadRx As Thread
    Private miTmpBytes2Read As Integer
    Private meMask As EventMasks

#Region "Enums"

    ' This enumeration provides Data Parity values.
    Public Enum DataParity
        Parity_None = 0
        Pariti_Odd
        Parity_Even
        Parity_Mark
    End Enum

    ' This enumeration provides Data Stop Bit values.
    '   It is set to begin with a one, so that the enumeration values
    '   match the actual values.
    Public Enum DataStopBit
        StopBit_1 = 1
        StopBit_2
    End Enum

    ' This enumeration contains values used to purge the various buffers.
    Private Enum PurgeBuffers
        RXAbort = &H2
        RXClear = &H8
        TxAbort = &H1
        TxClear = &H4
    End Enum

    ' This enumeration provides values for the lines sent to the Comm Port
    Private Enum Lines
        SetRts = 3
        ClearRts = 4
        SetDtr = 5
        ClearDtr = 6
        ResetDev = 7   '	 Reset device if possible
        SetBreak = 8   '	 Set the device break line.
        ClearBreak = 9   '	 Clear the device break line.
    End Enum
    ' This enumeration provides values for the Modem Status, since
    '   we'll be communicating primarily with a modem.
    ' Note that the Flags() attribute is set to allow for a bitwise
    '   combination of values.
    <Flags()> Public Enum ModemStatusBits
        ClearToSendOn = &H10
        DataSetReadyOn = &H20
        RingIndicatorOn = &H40
        CarrierDetect = &H80
    End Enum

    ' This enumeration provides values for the Working mode
    Public Enum Mode
        NonOverlapped
        Overlapped
    End Enum

    ' This enumeration provides values for the Comm Masks used.
    ' Note that the Flags() attribute is set to allow for a bitwise
    '   combination of values.
    <Flags()> Public Enum EventMasks
        RxChar = &H1
        RXFlag = &H2
        TxBufferEmpty = &H4
        ClearToSend = &H8
        DataSetReady = &H10
        ReceiveLine = &H20
        Break = &H40
        StatusError = &H80
        Ring = &H100
    End Enum
#End Region

#Region "Structures"
    ' This is the DCB structure used by the calls to the Windows API.
    <StructLayout(LayoutKind.Sequential, Pack:=1)> Private Structure DCB
        Public DCBlength As Integer
        Public BaudRate As Integer
        Public Bits1 As Integer
        Public wReserved As Int16
        Public XonLim As Int16
        Public XoffLim As Int16
        Public ByteSize As Byte
        Public Parity As Byte
        Public StopBits As Byte
        Public XonChar As Byte
        Public XoffChar As Byte
        Public ErrorChar As Byte
        Public EofChar As Byte
        Public EvtChar As Byte
        Public wReserved2 As Int16
    End Structure

    ' This is the CommTimeOuts structure used by the calls to the Windows API.
    <StructLayout(LayoutKind.Sequential, Pack:=1)> Private Structure COMMTIMEOUTS
        Public ReadIntervalTimeout As Integer
        Public ReadTotalTimeoutMultiplier As Integer
        Public ReadTotalTimeoutConstant As Integer
        Public WriteTotalTimeoutMultiplier As Integer
        Public WriteTotalTimeoutConstant As Integer
    End Structure

    ' This is the CommConfig structure used by the calls to the Windows API.
    <StructLayout(LayoutKind.Sequential, Pack:=1)> Private Structure COMMCONFIG
        Public dwSize As Integer
        Public wVersion As Int16
        Public wReserved As Int16
        Public dcbx As DCB
        Public dwProviderSubType As Integer
        Public dwProviderOffset As Integer
        Public dwProviderSize As Integer
        Public wcProviderData As Byte
    End Structure

    ' This is the OverLapped structure used by the calls to the Windows API.
    <StructLayout(LayoutKind.Sequential, Pack:=1)> Public Structure OVERLAPPED
        Public Internal As Integer
        Public InternalHigh As Integer
        Public Offset As Integer
        Public OffsetHigh As Integer
        Public hEvent As Integer
    End Structure
#End Region

#Region "Exceptions"

    ' This class defines a customized channel exception. This exception is
    '   raised when a NACK is raised.
    Public Class CIOChannelException : Inherits ApplicationException
        Sub New(ByVal Message As String)
            MyBase.New(Message)
        End Sub
        Sub New(ByVal Message As String, ByVal InnerException As Exception)
            MyBase.New(Message, InnerException)
        End Sub
    End Class

    ' This class defines a customized timeout exception.
    Public Class IOTimeoutException : Inherits CIOChannelException
        Sub New(ByVal Message As String)
            MyBase.New(Message)
        End Sub
        Sub New(ByVal Message As String, ByVal InnerException As Exception)
            MyBase.New(Message, InnerException)
        End Sub
    End Class

#End Region

#Region "Events"
    ' These events allow the program using this class to react to Comm Port
    '   events.
    Public Event DataReceived(ByVal Source As RS232, ByVal DataBuffer() As Byte)
    Public Event TxCompleted(ByVal Source As RS232)
    Public Event CommEvent(ByVal Source As RS232, ByVal Mask As EventMasks)
#End Region

#Region "Constants"
    ' These constants are used to make the code clearer.
    Private Const PURGE_RXABORT As Integer = &H2
    Private Const PURGE_RXCLEAR As Integer = &H8
    Private Const PURGE_TXABORT As Integer = &H1
    Private Const PURGE_TXCLEAR As Integer = &H4
    Private Const GENERIC_READ As Integer = &H80000000
    Private Const GENERIC_WRITE As Integer = &H40000000
    Private Const OPEN_EXISTING As Integer = 3
    Private Const INVALID_HANDLE_VALUE As Integer = -1
    Private Const IO_BUFFER_SIZE As Integer = 1024
    Private Const FILE_FLAG_OVERLAPPED As Integer = &H40000000
    Private Const ERROR_IO_PENDING As Integer = 997
    Private Const WAIT_OBJECT_0 As Integer = 0
    Private Const ERROR_IO_INCOMPLETE As Integer = 996
    Private Const WAIT_TIMEOUT As Integer = &H102&
    Private Const INFINITE As Integer = &HFFFFFFFF


#End Region

#Region "Properties"

    ' This property gets or sets the BaudRate
    Public Property BaudRate() As Integer
        Get
            Return miBaudRate
        End Get
        Set(ByVal Value As Integer)
            miBaudRate = Value
        End Set
    End Property

    ' This property gets or sets the BufferSize
    Public Property BufferSize() As Integer
        Get
            Return miBufferSize
        End Get
        Set(ByVal Value As Integer)
            miBufferSize = Value
        End Set
    End Property

    ' This property gets or sets the DataBit.
    Public Property DataBit() As Integer
        Get
            Return miDataBit
        End Get
        Set(ByVal Value As Integer)
            miDataBit = Value
        End Set
    End Property

    ' This write-only property sets or resets the DTR line.
    Public WriteOnly Property Dtr() As Boolean
        Set(ByVal Value As Boolean)
            If Not mhRS = -1 Then
                If Value Then
                    EscapeCommFunction(mhRS, Lines.SetDtr)
                Else
                    EscapeCommFunction(mhRS, Lines.ClearDtr)
                End If
            End If
        End Set
    End Property

    ' This read-only property returns an array of bytes that represents
    '   the input coming into the Comm Port.
    Overridable ReadOnly Property InputStream() As Byte()
        Get
            Return mabtRxBuf
        End Get
    End Property

    ' This read-only property returns a string that represents
    '   the data coming into to the Comm Port.
    Overridable ReadOnly Property InputStreamString() As String
        Get
            Dim oEncoder As New System.Text.ASCIIEncoding()
            Return oEncoder.GetString(Me.InputStream)
        End Get
    End Property

    ' This property returns the open status of the Comm Port.
    ReadOnly Property IsOpen() As Boolean
        Get
            Return CBool(mhRS <> -1)
        End Get
    End Property

    ' This read-only property returns the status of the modem.
    Public ReadOnly Property ModemStatus() As ModemStatusBits
        Get
            If mhRS = -1 Then
                Throw New ApplicationException("Please initialize and open " + _
                    "port before using this method")
            Else
                ' Retrieve modem status
                Dim lpModemStatus As Integer
                If Not GetCommModemStatus(mhRS, lpModemStatus) Then
                    Throw New ApplicationException("Unable to get modem status")
                Else
                    Return CType(lpModemStatus, ModemStatusBits)
                End If
            End If
        End Get
    End Property

    ' This property gets or sets the Parity
    Public Property Parity() As DataParity
        Get
            Return meParity
        End Get
        Set(ByVal Value As DataParity)
            meParity = Value
        End Set
    End Property

    ' This property gets or sets the Port
    Public Property Port() As Integer
        Get
            Return miPort
        End Get
        Set(ByVal Value As Integer)
            miPort = Value
        End Set
    End Property

    ' This write-only property sets or resets the RTS line.
    Public WriteOnly Property Rts() As Boolean
        Set(ByVal Value As Boolean)
            If Not mhRS = -1 Then
                If Value Then
                    EscapeCommFunction(mhRS, Lines.SetRts)
                Else
                    EscapeCommFunction(mhRS, Lines.ClearRts)
                End If
            End If
        End Set
    End Property

    ' This property gets or sets the StopBit
    Public Property StopBit() As DataStopBit
        Get
            Return meStopBit
        End Get
        Set(ByVal Value As DataStopBit)
            meStopBit = Value
        End Set
    End Property

    ' This property gets or sets the Timeout
    Public Overridable Property Timeout() As Integer
        Get
            Return miTimeout
        End Get
        Set(ByVal Value As Integer)
            miTimeout = CInt(IIf(Value = 0, 500, Value))
            ' If Port is open updates it on the fly
            pSetTimeout()
        End Set
    End Property

    ' This property gets or sets the working mode to overlapped
    '   or non-overlapped.
    Public Property WorkingMode() As Mode
        Get
            Return meMode
        End Get
        Set(ByVal Value As Mode)
            meMode = Value
        End Set
    End Property

#End Region

#Region "Win32API"
    ' The following functions are the required Win32 functions needed to 
    '   make communication with the Comm Port possible.

    <DllImport("kernel32.dll")> Private Shared Function BuildCommDCB( _
        ByVal lpDef As String, ByRef lpDCB As DCB) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function ClearCommError( _
        ByVal hFile As Integer, ByVal lpErrors As Integer, _
        ByVal l As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function CloseHandle( _
        ByVal hObject As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function CreateEvent( _
        ByVal lpEventAttributes As Integer, ByVal bManualReset As Integer, _
        ByVal bInitialState As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpName As String) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function CreateFile( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String, _
        ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, _
        ByVal lpSecurityAttributes As Integer, _
        ByVal dwCreationDisposition As Integer, _
        ByVal dwFlagsAndAttributes As Integer, _
        ByVal hTemplateFile As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function EscapeCommFunction( _
        ByVal hFile As Integer, ByVal ifunc As Long) As Boolean
    End Function

    <DllImport("kernel32.dll")> Private Shared Function FormatMessage( _
        ByVal dwFlags As Integer, ByVal lpSource As Integer, _
        ByVal dwMessageId As Integer, ByVal dwLanguageId As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpBuffer As String, _
        ByVal nSize As Integer, ByVal Arguments As Integer) As Integer
    End Function

    Private Declare Function FormatMessage Lib "kernel32" Alias _
     "FormatMessageA" (ByVal dwFlags As Integer, ByVal lpSource As Integer, _
     ByVal dwMessageId As Integer, ByVal dwLanguageId As Integer, _
     ByVal lpBuffer As StringBuilder, ByVal nSize As Integer, _
     ByVal Arguments As Integer) As Integer

    <DllImport("kernel32.dll")> Public Shared Function GetCommModemStatus( _
        ByVal hFile As Integer, ByRef lpModemStatus As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll")> Private Shared Function GetCommState( _
        ByVal hCommDev As Integer, ByRef lpDCB As DCB) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function GetCommTimeouts( _
        ByVal hFile As Integer, ByRef lpCommTimeouts As COMMTIMEOUTS) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function GetLastError() As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function GetOverlappedResult( _
        ByVal hFile As Integer, ByRef lpOverlapped As OVERLAPPED, _
        ByRef lpNumberOfBytesTransferred As Integer, _
        ByVal bWait As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function PurgeComm( _
        ByVal hFile As Integer, ByVal dwFlags As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function ReadFile( _
        ByVal hFile As Integer, ByVal Buffer As Byte(), _
        ByVal nNumberOfBytesToRead As Integer, _
        ByRef lpNumberOfBytesRead As Integer, _
        ByRef lpOverlapped As OVERLAPPED) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function SetCommTimeouts( _
        ByVal hFile As Integer, ByRef lpCommTimeouts As COMMTIMEOUTS) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function SetCommState( _
        ByVal hCommDev As Integer, ByRef lpDCB As DCB) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function SetupComm( _
        ByVal hFile As Integer, ByVal dwInQueue As Integer, _
        ByVal dwOutQueue As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function SetCommMask( _
        ByVal hFile As Integer, ByVal lpEvtMask As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function WaitCommEvent( _
        ByVal hFile As Integer, ByRef Mask As EventMasks, _
        ByRef lpOverlap As OVERLAPPED) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function WaitForSingleObject( _
        ByVal hHandle As Integer, ByVal dwMilliseconds As Integer) As Integer
    End Function

    <DllImport("kernel32.dll")> Private Shared Function WriteFile( _
        ByVal hFile As Integer, ByVal Buffer As Byte(), _
        ByVal nNumberOfBytesToWrite As Integer, _
        ByRef lpNumberOfBytesWritten As Integer, _
        ByRef lpOverlapped As OVERLAPPED) As Integer
    End Function

#End Region

#Region "Methods"

    ' This subroutine invokes a thread to perform an asynchronous read.
    '   This routine should not be called directly, but is used
    '   by the class.
    Public Sub R()
        Dim iRet As Integer = Read(miTmpBytes2Read)
    End Sub

    ' This subroutine invokes a thread to perform an asynchronous write.
    '   This routine should not be called directly, but is used
    '   by the class.
    Public Sub W()
        Write(mabtTmpTxBuf)
    End Sub

    ' This subroutine uses another thread to read from the Comm Port. It 
    '   raises RxCompleted when done. It reads an integer.
    Public Overloads Sub AsyncRead(ByVal Bytes2Read As Integer)
        If meMode <> Mode.Overlapped Then Throw New ApplicationException( _
            "Async Methods allowed only when WorkingMode=Overlapped")
        miTmpBytes2Read = Bytes2Read
        moThreadTx = New Thread(AddressOf R)
        moThreadTx.Start()
    End Sub

    ' This subroutine uses another thread to write to the Comm Port. It 
    '   raises TxCompleted when done. It writes an array of bytes.
    Public Overloads Sub AsyncWrite(ByVal Buffer() As Byte)
        If meMode <> Mode.Overlapped Then Throw New ApplicationException( _
            "Async Methods allowed only when WorkingMode=Overlapped")
        If mbWaitOnWrite = True Then Throw New ApplicationException( _
            "Unable to send message because of pending transmission.")
        mabtTmpTxBuf = Buffer
        moThreadTx = New Thread(AddressOf W)
        moThreadTx.Start()
    End Sub

    ' This subroutine uses another thread to write to the Comm Port. It 
    '   raises TxCompleted when done. It writes a string.
    Public Overloads Sub AsyncWrite(ByVal Buffer As String)
        Dim oEncoder As New System.Text.ASCIIEncoding()
        Dim aByte() As Byte = oEncoder.GetBytes(Buffer)
        Me.AsyncWrite(aByte)
    End Sub

    ' This function takes the ModemStatusBits and returns a boolean value
    '   signifying whether the Modem is active.
    Public Function CheckLineStatus(ByVal Line As ModemStatusBits) As Boolean
        Return Convert.ToBoolean(ModemStatus And Line)
    End Function

    ' This subroutine clears the input buffer.
    Public Sub ClearInputBuffer()
        If Not mhRS = -1 Then
            PurgeComm(mhRS, PURGE_RXCLEAR)
        End If
    End Sub

    ' This subroutine closes the Comm Port.
    Public Sub Close()
        If mhRS <> -1 Then
            CloseHandle(mhRS)
            mhRS = -1
        End If
    End Sub

    ' This subroutine opens and initializes the Comm Port
    Public Overloads Sub Open()
        ' Get Dcb block,Update with current data
        Dim uDcb As DCB, iRc As Integer
        ' Set working mode
        Dim iMode As Integer = Convert.ToInt32(IIf(meMode = Mode.Overlapped, _
            FILE_FLAG_OVERLAPPED, 0))
        ' Initializes Com Port
        If miPort > 0 Then
            Try
                ' Creates a COM Port stream handle 
                mhRS = CreateFile("COM" & miPort.ToString, _
                GENERIC_READ Or GENERIC_WRITE, 0, 0, _
                OPEN_EXISTING, iMode, 0)
                If mhRS <> -1 Then
                    ' Clear all comunication errors
                    Dim lpErrCode As Integer
                    iRc = ClearCommError(mhRS, lpErrCode, 0&)
                    ' Clears I/O buffers
                    iRc = PurgeComm(mhRS, PurgeBuffers.RXClear Or _
                        PurgeBuffers.TxClear)
                    ' Gets COM Settings
                    iRc = GetCommState(mhRS, uDcb)
                    ' Updates COM Settings
                    Dim sParity As String = "NOEM"
                    sParity = sParity.Substring(meParity, 1)
                    ' Set DCB State
                    Dim sDCBState As String = String.Format( _
                        "baud={0} parity={1} data={2} stop={3}", _
                        miBaudRate, sParity, miDataBit, CInt(meStopBit))
                    iRc = BuildCommDCB(sDCBState, uDcb)
                    iRc = SetCommState(mhRS, uDcb)
                    If iRc = 0 Then
                        Dim sErrTxt As String = pErr2Text(GetLastError())
                        Throw New CIOChannelException( _
                            "Unable to set COM state0" & sErrTxt)
                    End If
                    ' Setup Buffers (Rx,Tx)
                    iRc = SetupComm(mhRS, miBufferSize, miBufferSize)
                    ' Set Timeouts
                    pSetTimeout()
                Else
                    ' Raise Initialization problems
                    Throw New CIOChannelException( _
                        "Unable to open COM" & miPort.ToString)
                End If
            Catch Ex As Exception
                ' Generica error
                Throw New CIOChannelException(Ex.Message, Ex)
            End Try
        Else
            ' Port not defined, cannot open
            Throw New ApplicationException("COM Port not defined, " + _
                "use Port property to set it before invoking InitPort")
        End If
    End Sub

    ' This subroutine opens and initializes the Comm Port (overloaded
    '   to support parameters).
    Public Overloads Sub Open(ByVal Port As Integer, _
        ByVal BaudRate As Integer, ByVal DataBit As Integer, _
        ByVal Parity As DataParity, ByVal StopBit As DataStopBit, _
        ByVal BufferSize As Integer)

        Me.Port = Port
        Me.BaudRate = BaudRate
        Me.DataBit = DataBit
        Me.Parity = Parity
        Me.StopBit = StopBit
        Me.BufferSize = BufferSize
        Open()
    End Sub

    ' This function translates an API error code to text.
    Private Function pErr2Text(ByVal lCode As Integer) As String
        Dim sRtrnCode As New StringBuilder(256)
        Dim lRet As Integer

        lRet = FormatMessage(&H1000, 0, lCode, 0, sRtrnCode, 256, 0)
        If lRet > 0 Then
            Return sRtrnCode.ToString
        Else
            Return "Error not found."
        End If

    End Function

    ' This subroutine handles overlapped reads.
    Private Sub pHandleOverlappedRead(ByVal Bytes2Read As Integer)
        Dim iReadChars, iRc, iRes, iLastErr As Integer
        muOverlapped.hEvent = CreateEvent(Nothing, 1, 0, Nothing)
        If muOverlapped.hEvent = 0 Then
            ' Can't create event
            Throw New ApplicationException( _
                "Error creating event for overlapped read.")
        Else
            ' Ovellaped reading
            If mbWaitOnRead = False Then
                ReDim mabtRxBuf(Bytes2Read - 1)
                iRc = ReadFile(mhRS, mabtRxBuf, Bytes2Read, _
                    iReadChars, muOverlapped)
                If iRc = 0 Then
                    iLastErr = GetLastError()
                    If iLastErr <> ERROR_IO_PENDING Then
                        Throw New ArgumentException("Overlapped Read Error: " & _
                            pErr2Text(iLastErr))
                    Else
                        ' Set Flag
                        mbWaitOnRead = True
                    End If
                Else
                    ' Read completed successfully
                    RaiseEvent DataReceived(Me, mabtRxBuf)
                End If
            End If
        End If
        ' Wait for operation to be completed
        If mbWaitOnRead Then
            iRes = WaitForSingleObject(muOverlapped.hEvent, miTimeout)
            Select Case iRes
                Case WAIT_OBJECT_0
                    ' Object signaled,operation completed
                    If GetOverlappedResult(mhRS, muOverlapped, _
                        iReadChars, 0) = 0 Then

                        ' Operation error
                        iLastErr = GetLastError()
                        If iLastErr = ERROR_IO_INCOMPLETE Then
                            Throw New ApplicationException( _
                                "Read operation incomplete")
                        Else
                            Throw New ApplicationException( _
                                "Read operation error " & iLastErr.ToString)
                        End If
                    Else
                        ' Operation completed
                        RaiseEvent DataReceived(Me, mabtRxBuf)
                        mbWaitOnRead = False
                    End If
                Case WAIT_TIMEOUT
                    Throw New IOTimeoutException("Timeout error")
                Case Else
                    Throw New ApplicationException("Overlapped read error")
            End Select
        End If
    End Sub

    ' This subroutine handles overlapped writes.
    Private Function pHandleOverlappedWrite(ByVal Buffer() As Byte) As Boolean
        Dim iBytesWritten, iRc, iLastErr, iRes As Integer, bErr As Boolean
        muOverlappedW.hEvent = CreateEvent(Nothing, 1, 0, Nothing)
        If muOverlappedW.hEvent = 0 Then
            ' Can't create event
            Throw New ApplicationException( _
                "Error creating event for overlapped write.")
        Else
            ' Overllaped write
            PurgeComm(mhRS, PURGE_RXCLEAR Or PURGE_TXCLEAR)
            mbWaitOnRead = True
            iRc = WriteFile(mhRS, Buffer, Buffer.Length, _
                iBytesWritten, muOverlappedW)
            If iRc = 0 Then
                iLastErr = GetLastError()
                If iLastErr <> ERROR_IO_PENDING Then
                    Throw New ArgumentException("Overlapped Read Error: " & _
                        pErr2Text(iLastErr))
                Else
                    ' Write is pending
                    iRes = WaitForSingleObject(muOverlappedW.hEvent, INFINITE)
                    Select Case iRes
                        Case WAIT_OBJECT_0
                            ' Object signaled,operation completed
                            If GetOverlappedResult(mhRS, muOverlappedW, _
                                iBytesWritten, 0) = 0 Then

                                bErr = True
                            Else
                                ' Notifies Async tx completion,stops thread
                                mbWaitOnRead = False
                                RaiseEvent TxCompleted(Me)
                            End If
                    End Select
                End If
            Else
                ' Wait operation completed immediatly
                bErr = False
            End If
        End If
        CloseHandle(muOverlappedW.hEvent)
        Return bErr
    End Function

    ' This subroutine sets the Comm Port timeouts.
    Private Sub pSetTimeout()
        Dim uCtm As COMMTIMEOUTS
        ' Set ComTimeout
        If mhRS = -1 Then
            Exit Sub
        Else
            ' Changes setup on the fly
            With uCtm
                .ReadIntervalTimeout = 0
                .ReadTotalTimeoutMultiplier = 0
                .ReadTotalTimeoutConstant = miTimeout
                .WriteTotalTimeoutMultiplier = 10
                .WriteTotalTimeoutConstant = 100
            End With
            SetCommTimeouts(mhRS, uCtm)
        End If
    End Sub

    ' This function returns an integer specifying the number of bytes 
    '   read from the Comm Port. It accepts a parameter specifying the number
    '   of desired bytes to read.
    Public Function Read(ByVal Bytes2Read As Integer) As Integer
        Dim iReadChars, iRc As Integer

        ' If Bytes2Read not specified uses Buffersize
        If Bytes2Read = 0 Then Bytes2Read = miBufferSize
        If mhRS = -1 Then
            Throw New ApplicationException( _
                "Please initialize and open port before using this method")
        Else
            ' Get bytes from port
            Try
                ' Purge buffers
                'PurgeComm(mhRS, PURGE_RXCLEAR Or PURGE_TXCLEAR)
                ' Creates an event for overlapped operations
                If meMode = Mode.Overlapped Then
                    pHandleOverlappedRead(Bytes2Read)
                Else
                    ' Non overlapped mode
                    ReDim mabtRxBuf(Bytes2Read - 1)
                    iRc = ReadFile(mhRS, mabtRxBuf, Bytes2Read, iReadChars, Nothing)
                    If iRc = 0 Then
                        ' Read Error
                        Throw New ApplicationException( _
                            "ReadFile error " & iRc.ToString)
                    Else
                        ' Handles timeout or returns input chars
                        If iReadChars < Bytes2Read Then
                            Throw New IOTimeoutException("Timeout error")
                        Else
                            mbWaitOnRead = True
                            Return (iReadChars)
                        End If
                    End If
                End If
            Catch Ex As Exception
                ' Others generic erroes
                Throw New ApplicationException("Read Error: " & Ex.Message, Ex)
            End Try
        End If
    End Function

    ' This subroutine writes the passed array of bytes to the 
    '   Comm Port to be written.
    Public Overloads Sub Write(ByVal Buffer As Byte())
        Dim iBytesWritten, iRc As Integer

        If mhRS = -1 Then
            Throw New ApplicationException( _
                "Please initialize and open port before using this method")
        Else
            ' Transmit data to COM Port
            Try
                If meMode = Mode.Overlapped Then
                    ' Overlapped write
                    If pHandleOverlappedWrite(Buffer) Then
                        Throw New ApplicationException( _
                            "Error in overllapped write")
                    End If
                Else
                    ' Clears IO buffers
                    PurgeComm(mhRS, PURGE_RXCLEAR Or PURGE_TXCLEAR)
                    iRc = WriteFile(mhRS, Buffer, Buffer.Length, _
                        iBytesWritten, Nothing)
                    If iRc = 0 Then
                        Throw New ApplicationException( _
                            "Write Error - Bytes Written " & _
                            iBytesWritten.ToString & " of " & _
                            Buffer.Length.ToString)
                    End If
                End If
            Catch Ex As Exception
                Throw
            End Try
        End If
    End Sub

    ' This subroutine writes the passed string to the 
    '   Comm Port to be written.
    Public Overloads Sub Write(ByVal Buffer As String)
        Dim oEncoder As New System.Text.ASCIIEncoding()
        Dim aByte() As Byte = oEncoder.GetBytes(Buffer)
        Me.Write(aByte)
    End Sub

#End Region


End Class
