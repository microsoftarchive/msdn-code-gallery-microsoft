;/*++ BUILD Version: 0001    // Increment this if a change has global effects
;
;   Copyright (c) 2000,2001 Microsoft Corporation
;
;   Module Name:
;       errlog.mc
;
;   Abstract:
;       Constant definitions for the log error code values.
;
;   Revision History:
;--*/
;
;#ifndef _ERRLOGMC_
;#define _ERRLOGMC_
;
;//
;//  Status values are 32 bit values layed out as follows:
;//
;//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
;//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
;//  +---+-+-------------------------+-------------------------------+
;//  |Sev|C|       Facility          |               Code            |
;//  +---+-+-------------------------+-------------------------------+
;//
;//  where
;//
;//      Sev - is the severity code
;//
;//          00 - Success
;//          01 - Informational
;//          10 - Warning
;//          11 - Error
;//
;//      C - is the Customer code flag
;//
;//      Facility - is the facility code
;//
;//      Code - is the facility's status code
;//
;
MessageIdTypedef=NTSTATUS

SeverityNames=(Success=0x0:STATUS_SEVERITY_SUCCESS
               Informational=0x1:STATUS_SEVERITY_INFORMATIONAL
               Warning=0x2:STATUS_SEVERITY_WARNING
               Error=0x3:STATUS_SEVERITY_ERROR
              )

FacilityNames=(System=0x0
               RpcRuntime=0x2:FACILITY_RPC_RUNTIME
               RpcStubs=0x3:FACILITY_RPC_STUBS
               Io=0x4:FACILITY_IO_ERROR_CODE
               ErrLog=0x5:FACILITY_DEVICE_ERROR_CODE
              )

MessageId=0x0001 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_NOT_SUPPORTED
Language=English
The request is not supported.
.

MessageId=0x0002 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_INSUFFICIENT_RESOURCES
Language=English
Failed allocating requested system resources.
.

MessageId=0x0003 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_DEVICE_REMOVED
Language=English
The device has been removed.
.

MessageId=0x0004 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_DEVICE_NOT_STARTED
Language=English
The device has not been started.
.

MessageId=0x0005 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_BUFFER_TOO_SMALL
Language=English
The buffer is too small for the requested data.
.

MessageId=0x0006 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_INVALID_BUFFER_SIZE
Language=English
The size of the buffer is invalid for the specified operation.
.

MessageId=0x0007 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_INVALID_PARAMETER
Language=English
An invalid parameter was passed to a function.
.

MessageId=0x0008 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_LOWERDRV_IRP_FAILED
Language=English
Lower driver has failed the request.
.

MessageId=0x0009 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_OPENREG_FAILED
Language=English
Failed opening registry key.
.

MessageId=0x000a Facility=ErrLog Severity=Error SymbolicName=ERRLOG_READREG_FAILED
Language=English
Failed reading registry value.
.

MessageId=0x000b Facility=ErrLog Severity=Error SymbolicName=ERRLOG_WRITEREG_FAILED
Language=English
Failed writing registry value.
.

MessageId=0x000c Facility=ErrLog Severity=Error SymbolicName=ERRLOG_REGTYPE_MISMATCH
Language=English
Registry data type mismatch.
.

MessageId=0x000d Facility=ErrLog Severity=Error SymbolicName=ERRLOG_MINIDRV_REG_FAILED
Language=English
This minidriver failed to register with HID class driver.
.

MessageId=0x0100 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_OPEN_KBDPORT_FAILED
Language=English
Open keyboard port failed.
.

;#ifdef DEBUG

MessageId=0x8000 Facility=ErrLog Severity=Informational SymbolicName=ERRLOG_DEBUG_INFORMATION
Language=English
The driver has reported the following debug information: %2.
.

MessageId=0x8001 Facility=ErrLog Severity=Warning SymbolicName=ERRLOG_DEBUG_WARNING
Language=English
The driver has reported the following debug warning: %2.
.

MessageId=0x8002 Facility=ErrLog Severity=Error SymbolicName=ERRLOG_DEBUG_ERROR
Language=English
The driver has reported the following debug error: %2.
.

;#endif

;#endif /* _ERRLOGMC_ */
