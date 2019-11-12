/*++

Copyright (c) 1997-2003  Microsoft Corporation
All rights reserved

Module Name:

    irda.c

Abstract:

    IRDA printing support in localmon

--*/

#include "precomp.h"


#pragma hdrstop

#include <DriverSpecs.h>
_Analysis_mode_(_Analysis_code_type_user_driver_)

#include "irda.h"

BOOL
IsIRDAInstalled(
    )
{
    BOOL        bRet        = FALSE;
    WORD        WSAVerReq   = MAKEWORD(1,1);
    SOCKET      hSock       = INVALID_SOCKET;
    WSADATA     WSAData     = {0};


    if ( WSAStartup(WSAVerReq, &WSAData) == ERROR_SUCCESS )
    {
        if ( (hSock = socket(AF_IRDA, SOCK_STREAM, 0)) != INVALID_SOCKET )
        {
            closesocket(hSock);
            bRet = TRUE;
        }

        WSACleanup();
    }

    return bRet;
}


VOID
CheckAndAddIrdaPort(
    _In_ PINILOCALMON    pIniLocalMon
    )
{
    PLCMINIPORT    pIniPort = NULL;


    LcmEnterSplSem();

    for ( pIniPort = pIniLocalMon->pIniPort ;
          pIniPort && !IS_IRDA_PORT(pIniPort->pName) ;
          pIniPort = pIniPort->pNext )
    ;

    LcmLeaveSplSem();

    if ( pIniPort || !IsIRDAInstalled() )
        return;

    //
    // Add the port to the list and write to registry
    //
    LcmCreatePortEntry(pIniLocalMon, szIRDA);
}

VOID
CloseIrdaConnection(
    _In_    PLCMINIPORT    pIniPort
    )
{
    PIRDA_INFO  pIrda = (PIRDA_INFO) pIniPort->pExtra;


    int ret = 0;
    if ( (SOCKET)pIniPort->hFile != INVALID_SOCKET ) {

        ret = closesocket((SOCKET)pIniPort->hFile);
        pIniPort->hFile = (HANDLE)INVALID_SOCKET;
    }

    if ( pIrda ) {
        //
        // check if overlapped send is not complete yet
        //
        if ( pIrda->WsaOverlapped.hEvent )
        {
            //
            // wait overlapped send to complete/cancel
            //
            if (ret == 0)
            {
                WaitForSingleObject (pIrda-> WsaOverlapped.hEvent, 5*60*1000);
            }
            WSACloseEvent(pIrda->WsaOverlapped.hEvent);
            pIrda-> WsaOverlapped.hEvent = NULL;
        }

        FreeSplMem(pIrda);
        pIniPort->pExtra = NULL;
    }
}

DWORD
IrdaConnect(
    _In_    PLCMINIPORT    pIniPort
    )
{
    BOOL            bRet = FALSE;
    WORD            WSAVerReq = MAKEWORD(1,1);
    DWORD           dwIndex = 0, dwNeeded = BUF_SIZE, dwEnableIrLPT = TRUE,
                    dwLastError = ERROR_SUCCESS, dwSendPduLen = 0;
    LPSTR           pBuf = NULL;
    WSADATA         WSAData = {0};
    SOCKET          Socket = INVALID_SOCKET;
    PIRDA_INFO      pIrda = NULL;
    PDEVICELIST     pDevList = NULL;
    SOCKADDR_IRDA   PrinterAddr  = { AF_IRDA, 0, 0, 0, 0, "IrLPT" };


    SPLASSERT(pIniPort->hFile == (HANDLE)INVALID_SOCKET && pIniPort->pExtra == NULL);

    dwLastError = WSAStartup(WSAVerReq, &WSAData);

    if ( dwLastError )
        goto Done;

    pBuf = (LPSTR)AllocSplMem(dwNeeded);
    if ( !pBuf ) {

        dwLastError = GetLastError();
        goto Done;
    }

    if ( (Socket = WSASocket(AF_IRDA, SOCK_STREAM, 0, NULL, 0,
                             WSA_FLAG_OVERLAPPED)) == INVALID_SOCKET    ||
         getsockopt(Socket, SOL_IRLMP, IRLMP_ENUMDEVICES,
                    (LPSTR)pBuf, (int *)&dwNeeded) == SOCKET_ERROR ) {

        dwLastError = WSAGetLastError();
        goto Done;
    }

    if ( dwNeeded > BUF_SIZE ) {

        FreeSplMem(pBuf);

        pBuf = (LPSTR)AllocSplMem(dwNeeded);
        if ( !pBuf ) {

            dwLastError = GetLastError();
            goto Done;
        }

        if ( getsockopt(Socket, SOL_IRLMP, IRLMP_ENUMDEVICES,
                        (LPSTR)pBuf, (int *)&dwNeeded) == SOCKET_ERROR ) {

            dwLastError = WSAGetLastError();
            goto Done;
        }
    }

    pDevList = (PDEVICELIST) pBuf;

    //
    // Any of the devices a printer?
    //
    for ( dwIndex = 0 ; dwIndex < pDevList->numDevice ; ++dwIndex ) {

        if ( (pDevList->Device[dwIndex].irdaDeviceHints1 & PRINTER_HINT_BIT)  ||
             (pDevList->Device[dwIndex].irdaDeviceHints2 & PRINTER_HINT_BIT) )
            break;
    }

    //
    // Any printers found?
    //
    if ( dwIndex == pDevList->numDevice ) {

        dwLastError = ERROR_PRINTER_NOT_FOUND;
        goto Done;
    }

    //
    // Move printer's address into the socket address
    //
    memcpy(PrinterAddr.irdaDeviceID,
           pDevList->Device[dwIndex].irdaDeviceID,
           sizeof(PrinterAddr.irdaDeviceID));

    dwIndex = 0;
    dwNeeded = sizeof(dwSendPduLen);
    bRet = SOCKET_ERROR != setsockopt(Socket,
                                      SOL_IRLMP,
                                      IRLMP_IRLPT_MODE,
                                      (LPCSTR)&dwEnableIrLPT,
                                      sizeof(dwEnableIrLPT))    &&
           SOCKET_ERROR != connect(Socket,
                                   (const struct sockaddr *)&PrinterAddr,
                                   sizeof(PrinterAddr))         &&
           SOCKET_ERROR != getsockopt(Socket,
                                      SOL_IRLMP,
                                      IRLMP_SEND_PDU_LEN,
                                      (char *)&dwSendPduLen,
                                      (int *)&dwNeeded) &&
           SOCKET_ERROR != setsockopt(Socket,
                                      SOL_SOCKET,
                                      SO_SNDBUF,
                                      (LPCSTR)&dwIndex,
                                      sizeof(dwIndex));


    if ( bRet ) {

        SPLASSERT(pIniPort->pExtra == NULL);

        dwNeeded = sizeof(IRDA_INFO) + dwSendPduLen;

        pIrda = (PIRDA_INFO) AllocSplMem(dwNeeded);
        if ( !pIrda ) {

            bRet = FALSE;
            dwLastError = ERROR_NOT_ENOUGH_MEMORY;
            goto Done;
        }

        pIniPort->hFile     = (HANDLE)Socket;
        pIniPort->pExtra    = (LPBYTE)pIrda;

        pIrda->dwSendPduLen = dwSendPduLen;
        pIrda->pBuf         = ((LPBYTE) pIrda) + sizeof(IRDA_INFO);

    } else
        dwLastError = WSAGetLastError();

Done:
    FreeSplMem(pBuf);

    if ( !bRet ) {

        if ( Socket != INVALID_SOCKET )
            closesocket(Socket);

        FreeSplMem(pIniPort->pExtra);
        pIniPort->pExtra = NULL;
    }

    return bRet ? ERROR_SUCCESS : dwLastError;
}


BOOL
AbortThisJob(
    _In_    PLCMINIPORT    pIniPort
    )
/*++
        Tells if the job should be aborted. A job should be aborted if it has
        been deleted or it needs to be restarted.

--*/
{
    BOOL            bRet        = FALSE;
    DWORD           dwNeeded    = 0;
    LPJOB_INFO_1    pJobInfo    = NULL;


    dwNeeded = 0;

    GetJob(pIniPort->hPrinter, pIniPort->JobId, 1, NULL, 0, &dwNeeded);

    if ( GetLastError() != ERROR_INSUFFICIENT_BUFFER )
        goto Done;

    pJobInfo = (LPJOB_INFO_1) AllocSplMem(dwNeeded);
    if ( !pJobInfo     ||
         !GetJob(pIniPort->hPrinter, pIniPort->JobId,
                 1, (LPBYTE)pJobInfo, dwNeeded, &dwNeeded))

        goto Done;

    bRet = (pJobInfo->Status & JOB_STATUS_DELETING) ||
           (pJobInfo->Status & JOB_STATUS_DELETED)  ||
           (pJobInfo->Status & JOB_STATUS_RESTART);
Done:
    if ( pJobInfo )
        FreeSplMem(pJobInfo);

    return bRet;
}


VOID
IrdaDisconnect(
    _In_    PLCMINIPORT    pIniPort
    )
{
    DWORD       dwRet   = ERROR_SUCCESS;
    SOCKET      Socket  = (SOCKET) pIniPort->hFile;
    PIRDA_INFO  pIrda   = (PIRDA_INFO) pIniPort->pExtra;


    //
    // If the job has already been cancelled close socket and quit
    //
    if ( Socket == INVALID_SOCKET )
        goto Done;

    //
    // If a send is pending wait for all the data to go through indefinitly
    //
    if ( pIrda->WsaOverlapped.hEvent ) {

        do {

            dwRet = WaitForSingleObject(pIrda->WsaOverlapped.hEvent,
                                        WRITE_TIMEOUT);

            if ( dwRet == WAIT_TIMEOUT ) {

                //
                // If user has cancelled the job close connection
                //
                if ( AbortThisJob(pIniPort) )
                    goto Done;
            } else if ( dwRet != WAIT_OBJECT_0 )
                goto Done;
        } while ( dwRet == WAIT_TIMEOUT );

        //
        // IRDA can only send the whole packet so we do not check dwSent
        //
    }

    //
    // No more sends
    //
    shutdown(Socket, SD_SEND);

Done:
    CloseIrdaConnection(pIniPort);
}


BOOL
IrdaStartDocPort(
    _Inout_ PLCMINIPORT    pIniPort
    )
{
    HANDLE hToken       = NULL;
    DWORD  dwLastError  = ERROR_SUCCESS;


    //
    // If remote guest is the first user to print, then the connect fails.
    // Thus we need to revert to system context before calling IrdaConnect
    //

    hToken = RevertToPrinterSelf();

    if (!hToken) {
        return FALSE;
    }

    dwLastError = IrdaConnect(pIniPort);

    ImpersonatePrinterClient(hToken);

    if ( dwLastError ) {

        SetLastError(dwLastError);
        return FALSE;
    } else
        return TRUE;
}

#pragma prefast (push)
#pragma prefast( disable:28159 "Usermode program can not use 'KeQueryTickCount' in ddk library")
BOOL
IrdaWritePort(
    _In_                HANDLE      hPort,
    _In_reads_bytes_(cbBuf)
                        LPBYTE      pBuf,
                        DWORD       cbBuf,
    _Out_               LPDWORD     pcbWritten
    )
{
    INT             iRet = NO_ERROR;
    DWORD           dwSent = 0, dwFlags = 0, dwTimeout = 0, dwBuffered = 0;
    PLCMINIPORT     pIniPort = (PLCMINIPORT)hPort;
    SOCKET          Socket = (SOCKET) pIniPort->hFile;
    PIRDA_INFO      pIrda = (PIRDA_INFO)pIniPort->pExtra;


    *pcbWritten = 0;

    //
    // When we have to close socket we fail the write.
    // If anothe write comes through it is because user wanted to retry
    //
    if ( Socket == INVALID_SOCKET ) {

        SPLASSERT(pIrda == NULL);

        SetJob(pIniPort->hPrinter, pIniPort->JobId, 0, NULL, JOB_CONTROL_RESTART);
        iRet = WSAENOTSOCK;
        goto Done;
    }

    SPLASSERT(pIrda != NULL);

    //
    // This is the time spooler issued the write to us
    //
    pIrda->dwBeginTime = GetTickCount();

    do {

        //
        // If event is non-NULL at the beginning we have a pending write from
        // last WritePort call
        //
        if ( pIrda->WsaOverlapped.hEvent ) {

            dwTimeout = GetTickCount() - pIrda->dwBeginTime;

            //
            // We want to wait for WRITE_TIMEOUT time from the time spooler
            // issued the WritePort.
            // If it is already more than that still check what happened to the
            // write before returning
            //
            if ( dwTimeout > WRITE_TIMEOUT )
                dwTimeout = 0;
            else
                dwTimeout = WRITE_TIMEOUT - dwTimeout;

            //
            // Let's wait for the timeout period for the last send to complete
            //
            if ( WAIT_OBJECT_0 != WaitForSingleObject(pIrda->WsaOverlapped.hEvent,
                                                      dwTimeout) ) {

                iRet = ERROR_TIMEOUT;
                goto Done;
            }

            //
            // What happened to the last send?
            //
            if ( WSAGetOverlappedResult(Socket, &pIrda->WsaOverlapped,
                                        &dwSent, FALSE, &dwFlags) == FALSE ) {

                iRet = WSAGetLastError();
                CloseIrdaConnection(pIniPort);
                goto Done;
            }

            //
            // IRDA can only send the whole packet so we do not check dwSent
            //

            //
            // Reset the manual reset event and do the next send
            //
            WSAResetEvent(pIrda->WsaOverlapped.hEvent);

            //
            // Have we already sent all the data?
            //
            if ( cbBuf == 0 ) {

                WSACloseEvent(pIrda->WsaOverlapped.hEvent);
                pIrda->WsaOverlapped.hEvent = NULL;
                goto Done;
            }
        } else {

            pIrda->WsaOverlapped.hEvent = WSACreateEvent();

            if ( !pIrda->WsaOverlapped.hEvent ) {

                iRet = GetLastError();
                CloseIrdaConnection(pIniPort);
                goto Done;
            }
        }

        do {

            //
            // Have we already sent all the data?
            //
            if ( cbBuf == 0 ) {

                WSACloseEvent(pIrda->WsaOverlapped.hEvent);
                pIrda->WsaOverlapped.hEvent = NULL;
                goto Done;
            }

            //
            // Send no more than pIrda->dwSendPduLen
            //
            if ( cbBuf < pIrda->dwSendPduLen )
                dwBuffered = cbBuf;
            else
                dwBuffered = pIrda->dwSendPduLen;

            pIrda->WsaBuf.len   = dwBuffered;
            pIrda->WsaBuf.buf   = (char *)pIrda->pBuf;

            #pragma prefast(suppress:__WARNING_READ_OVERRUN, "pIrda->pBuf contains sufficient space for dwBuffered bytes")
            CopyMemory(pIrda->pBuf, pBuf, dwBuffered);

            //
            // We are asking a non-blocking send. Typically this will
            // return with I/O pending
            //
            if ( WSASend(Socket, &pIrda->WsaBuf, 1, &dwSent,
                         MSG_PARTIAL, &pIrda->WsaOverlapped, NULL) != NO_ERROR ) {

                iRet = WSAGetLastError();
                break;
            }

            pBuf        += dwSent;
            cbBuf       -= dwSent;
            *pcbWritten += dwSent;
        } while ( iRet == NO_ERROR );

        if ( iRet == WSA_IO_PENDING ) {

            //
            // Lie to spooler we sent the whole data. Next time we will find out
            //
            pBuf        += dwBuffered;
            cbBuf       -= dwBuffered;
            *pcbWritten += dwBuffered;
            iRet = NO_ERROR;
        } else {

            CloseIrdaConnection(pIniPort);
        }
    } while ( cbBuf && iRet == NO_ERROR );


Done:
    if ( iRet != ERROR_SUCCESS )
        SetLastError(iRet);

    return iRet == ERROR_SUCCESS;
}
#pragma prefast (pop)

VOID
IrdaEndDocPort(
    _In_    PLCMINIPORT    pIniPort
    )
{

    IrdaDisconnect(pIniPort);
}

