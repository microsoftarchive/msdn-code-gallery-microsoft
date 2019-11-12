/*++

Copyright (c) 1997-2003  Microsoft Corporation
All rights reserved.

Module Name:

    irda.h

Abstract:

    Definitions used for IRDA printing

--*/
#ifndef _IRDA_H_
#define _IRDA_H_

#define     PRINTER_HINT_BIT     0x08
#define     DEVICE_LIST_LEN         5
#define     WRITE_TIMEOUT       60000   // 60 seconds
#define     BUF_SIZE            sizeof(DEVICELIST) + (DEVICE_LIST_LEN - 1) * sizeof(IRDA_DEVICE_INFO)


typedef struct _IRDA_INFO  {
    DWORD           dwBeginTime;
    DWORD           dwSendPduLen;
    WSAOVERLAPPED   WsaOverlapped;
    WSABUF          WsaBuf;
    LPBYTE          pBuf;
} IRDA_INFO, *PIRDA_INFO;

VOID
CheckAndAddIrdaPort(
    _In_    PINILOCALMON    pIniLocalMon
    );

VOID
CheckAndDeleteIrdaPort(
    _In_    PINILOCALMON    pIniLocalMon
    );

BOOL
IrdaStartDocPort(
    _Inout_ PLCMINIPORT    pIniPort
    );

BOOL
IrdaWritePort(
    _In_                HANDLE  hPort,
    _In_reads_bytes_(cbBuf)  LPBYTE  pBuf,
                        DWORD   cbBuf,
    _Out_               LPDWORD pcbWritten
    );

VOID
IrdaEndDocPort(
    _In_    PLCMINIPORT    pIniPort
    );

#endif // _IRDA_H_

