/*++
    Copyright (c) Microsoft Corporation

    Module Name:
        errlog.c

    Abstract: This module contains the error log related functions.

    Environment:
        Kernel mode

--*/

#include "pch.h"

#define MODULE_ID               0xff

PDRIVER_OBJECT gDriverObj = NULL;

/*++
    @doc    INTERNAL

    @func   VOID | LogError | Log an error entry in the event log.

    @parm   IN NTSTATUS | ErrorCode | Error code.
    @parm   IN NTSTATUS | NTStatus | NT status code if applicable.
    @parm   IN ULONG | UniqueID | Unique error ID, used when there are more
            than one place reporting the same ErrorCode.
    @parm   IN PCWSTR | String1 | Optional substitution string 1.
    @parm   IN PCWSTR | String2 | Optional substitution string 2.

    @rvalue None.
--*/

VOID
LogError(
    IN NTSTATUS ErrorCode,
    IN NTSTATUS NTStatus,
    IN ULONG    UniqueID OPTIONAL,
    IN PCWSTR   String1  OPTIONAL,
    IN PCWSTR   String2  OPTIONAL
    )
{
    TEnter(Func, ("(ErrorCode=%x,NTStatus=%x,UniqueID=%x,Str1=%S,Str2=%S)",
                  ErrorCode, NTStatus, UniqueID, String1? String1: L"",
                  String2? String2: L""));

    TAssert(gDriverObj != NULL);
    if (gDriverObj != NULL)
    {
        ULONG_PTR len1, len2, len;
        PIO_ERROR_LOG_PACKET ErrEntry;

        len1 = String1? (wcslen(String1) + 1)*sizeof(WCHAR): 0;
        len2 = String2? (wcslen(String2) + 1)*sizeof(WCHAR): 0;
        len = len1 + len2 + FIELD_OFFSET(IO_ERROR_LOG_PACKET, DumpData);
        len = max(len, sizeof(IO_ERROR_LOG_PACKET));
        // Thoroughly check the value of len to prevent buffer underflows/overflows
        if ((len > 0) && (len <= 255) && (len1 <= len) && (len2 <= len))
        {
            ErrEntry = IoAllocateErrorLogEntry(gDriverObj, (UCHAR)len);
            if (ErrEntry)
            {
                PUCHAR pbBuff = (PUCHAR)ErrEntry +
                                        FIELD_OFFSET(IO_ERROR_LOG_PACKET,
                                                     DumpData);

                ErrEntry->NumberOfStrings = 0;
                if (len1 > 0)
                {
                    ErrEntry->NumberOfStrings++;
                    RtlCopyMemory(pbBuff, String1, len1);
                    pbBuff += len1;
                }
                if (len2 > 0)
                {
                    ErrEntry->NumberOfStrings++;
                    RtlCopyMemory(pbBuff, String2, len2);
                    pbBuff += len2;
                }
                ErrEntry->StringOffset = FIELD_OFFSET(IO_ERROR_LOG_PACKET,
                                                      DumpData);

                ErrEntry->ErrorCode = ErrorCode;
                ErrEntry->FinalStatus = NTStatus;
                ErrEntry->UniqueErrorValue = UniqueID;
                IoWriteErrorLogEntry(ErrEntry);
            }
            else
            {
                TWarn(("Failed to allocate error log entry (len=%d).", (int)len));
            }
        }
        else
        {
            TWarn(("Error log entry too big (len=%d).", (int)len));
        }
    }

    TExit(Func, ("!"));
    return;
}       //LogError

#ifdef DEBUG
/*++
    @doc    INTERNAL

    @func   VOID | LogDbgMsg | Log an error message in the event log.

    @parm   IN NTSTATUS | ErrorCode | Error code.
    @parm   IN NTSTATUS | NTStatus | NT status code if applicable.
    @parm   IN LPCSTR | pszFormat | Points to the format string.
    @parm   ...

    @rvalue None.

    @note   Please note that it is a very bad practice to embed unlocalizable
            strings in the code.  Therefore, this function is DEBUG only just
            for the convenience of debugging.  A formal error message should
            be done via the LogError function and the error message text should
            be defineed in the message file (errcodes.mc) with an assigned
            error code.
--*/

VOID __cdecl
LogDbgMsg(
    IN NTSTATUS ErrorCode,
    IN NTSTATUS NTStatus OPTIONAL,
    _In_z_ LPCSTR pszFormat,
    ...
    )
{
    #define MAX_ERRMSG_LEN      ((ERROR_LOG_MAXIMUM_SIZE -                     \
                                  FIELD_OFFSET(IO_ERROR_LOG_PACKET, DumpData)) \
                                 /sizeof(WCHAR))
    static char szErrMsg[MAX_ERRMSG_LEN] = {0};

    TEnter(Func, ("(ErrorCode=%x,NTStatus=%x,Format=%s)",
                  ErrorCode, NTStatus, pszFormat));

    TAssert(gDriverObj != NULL);
    if (gDriverObj != NULL)
    {
        va_list arglist;
        NTSTATUS status;
        size_t iLen = 0;
        ULONG_PTR iTotalLen;
        PIO_ERROR_LOG_PACKET ErrEntry;

        va_start(arglist, pszFormat);
        status = RtlStringCchVPrintfA(szErrMsg,
                               ARRAYSIZE(szErrMsg),
                               pszFormat,
                               arglist);
        va_end(arglist);
        if (NT_SUCCESS(status))
        {
            status = RtlStringCchLengthA(szErrMsg, ARRAYSIZE(szErrMsg), &iLen);
        }

        if (NT_SUCCESS(status))
        {
            iTotalLen = FIELD_OFFSET(IO_ERROR_LOG_PACKET, DumpData) +
                        (iLen + 1)*sizeof(WCHAR);
            iTotalLen = max(iTotalLen, sizeof(IO_ERROR_LOG_PACKET));
            ErrEntry = IoAllocateErrorLogEntry(gDriverObj, (UCHAR)iTotalLen);
            if (ErrEntry)
            {
                ErrEntry->NumberOfStrings = 1;
                ErrEntry->ErrorCode = ErrorCode;
                ErrEntry->StringOffset = FIELD_OFFSET(IO_ERROR_LOG_PACKET,
                                                      DumpData);
                mbstowcs((WCHAR *)ErrEntry->DumpData, szErrMsg, iLen);
                ErrEntry->FinalStatus = NTStatus;
                IoWriteErrorLogEntry(ErrEntry);
            }
            else
            {
                TWarn(("Failed to allocate error log entry (len=%d).",
                       (int)iTotalLen));
            }

            if (ErrorCode == ERRLOG_DEBUG_INFORMATION)
            {
                TInfo(("%s", szErrMsg));
            }
            else if (ErrorCode == ERRLOG_DEBUG_WARNING)
            {
                TWarn(("%s", szErrMsg));
            }
            else if (ErrorCode == ERRLOG_DEBUG_ERROR)
            {
                TErr(("%s", szErrMsg));
            }
        }
    }

    TExit(Func, ("!"));
    return;
}       //LogDbgMsg
#endif  //ifdef DEBUG

