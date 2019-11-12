//******************************************************************
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// This code is licensed under the Visual Studio SDK license terms.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//******************************************************************
///////////////////////////////////////////////////////////////////////////////////////
// CSetupWatcher Sample//
///////////////////
//
// This code is provided “AS IS” with no warranties and confers no rights.
// Use of this code sample is subject to the terms specified at http://www.microsoft.com/info/cpyright.htm
// 
// Usage:
// Facility for hooking up external (local out-of-proc) setup UI using named pipes.
// Uses a design similar to MSI's external UI handler (and the same callback prototypes),
// but can be integrated into any kind of setup engine.
//

#include <msiquery.h>

// Typedef is missing from older MSI versions
#ifndef INSTALLUI_HANDLER_RECORD
typedef int (WINAPI *INSTALLUI_HANDLER_RECORD)(LPVOID pvContext, UINT iMessageType, MSIHANDLE hRecord);
#endif

// Define a custom progress type in the INSTALLMESSAGE_* range.
#define INSTALLMESSAGE_SUITEPROGRESS 0x7A000000L

class CSetupWatcher
{
public:

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher constructor
    //
    // Creates a new setup watcher instance, for use either by the process watching
    // the setup, or by the setup process being watched.
    //
    // szName   - Identifies the setup instance being watched. The watcher and
    //            the watchee must use the same name. The name should be unique
    //            enough to avoid conflicting with other instances on the system.
    //
    // fWatcher - True if the calling process is the watcher process, false if the
    //            calling process is the setup process being watched.
    //
    CSetupWatcher(const wchar_t* szName, bool fWatcher=true)
        : m_fWatcher(fWatcher),
          m_szName(szName != NULL && szName[0] != L'\0' ? szName : L"SetupWatcher"),
          m_szPipeName(NULL),
          m_hPipe(NULL),
          m_fConnecting(false),
          m_fConnected(false),
          m_dwReplyTimeout(DEFAULT_REPLY_TIMEOUT),
          m_hReceiveThread(NULL),
          m_hReceiveStopEvent(NULL),
          m_pMsgBuf(NULL),
          m_cbMsgBuf(0)
    {    
        ZeroMemory(&m_overlapped, sizeof(OVERLAPPED));
        m_overlapped.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher destructor
    //
    // Closes any open handles and frees any allocated memory.
    //
    ~CSetupWatcher()
    {
        if (m_hReceiveThread != NULL)
        {
            SetEvent(m_hReceiveStopEvent);
            WaitForSingleObject(m_hReceiveThread, INFINITE);
            CloseHandle(m_hReceiveThread);
            m_hReceiveThread = NULL;
        }
        if (m_hPipe != NULL)
        {
            CloseHandle(m_hPipe);
            m_hPipe = NULL;
        }
        if (m_overlapped.hEvent != NULL)
        {
            CloseHandle(m_overlapped.hEvent);
            m_overlapped.hEvent = NULL;
        }
        if (m_szPipeName != NULL)
        {
            delete[] m_szPipeName;
            m_szPipeName = NULL;
        }
        if (m_pMsgBuf != NULL)
        {
            delete[] m_pMsgBuf;
            m_pMsgBuf = NULL;
        }
        m_fConnecting = false;
        m_fConnected = false;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::Connect()
    //
    // Connects the inter-process communication channel.
    // (Currently implemented as a named pipe.)
    //
    // This method must be called first by the watcher process, then by the watched
    // setup process. The method does not block; the watcher will asynchronously wait
    // for the watched process to make the connection.
    //
    // Returns: 0 on success, Win32 error code on failure.
    //
    virtual DWORD Connect()
    {
        const wchar_t* szPipePrefix = L"\\\\.\\pipe\\";
        DWORD cchPipeNameBuf = wcslen(szPipePrefix) + wcslen(m_szName) + 1;
        m_szPipeName = new wchar_t[cchPipeNameBuf];

        if (m_szPipeName == NULL)
        {
            return ERROR_OUTOFMEMORY;
        }
        else
        {
            wcscpy_s(m_szPipeName, cchPipeNameBuf, szPipePrefix);
            wcscat_s(m_szPipeName, cchPipeNameBuf, m_szName);

            if (m_fWatcher)
            {
                return this->ConnectPipeServer();
            }
            else
            {
                return this->ConnectPipeClient();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::IsConnected()
    //
    // Checks if the watcher process and setup process are currently connected.
    //
    virtual bool IsConnected() const
    {
        return m_fConnected;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::ReceiveMessages()
    //
    // For use by the watcher process. Watches for messages in the input buffer and calls
    // the callback for each one.
    //
    // This method does not block; it spawns a separate thread to do the work.
    //
    // pHandler  - Callback function to handle any messages received.
    //
    // pvContext - Optional context pointer to pass to the callback function.
    //
    // Returns: 0 on success, Win32 error code on failure.
    //
    virtual DWORD ReceiveMessages(INSTALLUI_HANDLER pHandler, void* pvContext)
    {
        return this->ReceiveMessages(pHandler, NULL, pvContext);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::ReceiveMessages()
    //
    // For use by the watcher process. Watches for messages in the input buffer and calls
    // the callback for each one.
    //
    // This method does not block; it spawns a separate thread to do the work.
    //
    // pHandler  - Callback function to handle any messages received.
    //
    // pvContext - Optional context pointer to pass to the callback function.
    //
    // Returns: 0 on success, Win32 error code on failure.
    //
    virtual DWORD ReceiveMessages(INSTALLUI_HANDLER_RECORD pHandler, void* pvContext)
    {
        return this->ReceiveMessages(NULL, pHandler, pvContext);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::SendMessage()
    //
    // For use by the watched setup process. Sends a message to the watcher and
    // synchronously waits on a reply, up to the timeout value.
    //
    // iMessageType - Type of message being sent, typically one of the
    //                INSTALLMESSAGE_* values, optionally combined with other flags.
    //
    // szMessage    - Message string being sent.
    //
    // pdwReply     - [OUT] Receives the reply code from the watcher.
    //
    // Returns: 0 on success, Win32 error code on failure.
    // Returns WAIT_TIMEOUT if no reply was received in time.
    //
    virtual DWORD SendMessage(UINT iMessageType, const wchar_t* szMessage, DWORD* pdwReply)
    {
        if (m_fWatcher)
        {
            return ERROR_INVALID_OPERATION;
        }

        if (!m_fConnected)
        {
            *pdwReply = (DWORD) -1;
            return 0;
        }

        MSIHANDLE hRec = NULL;
        
        if (szMessage != NULL)
        {
            hRec = MsiCreateRecord(0);
            MsiRecordSetString(hRec, 0, szMessage);
        }

        DWORD dwRet = this->SendMessage(iMessageType, hRec);

        if (hRec != NULL)
        {
            MsiCloseHandle(hRec);
        }
        
        if (dwRet != 0)
        {
            return dwRet;
        }

        return this->ReceiveReply(pdwReply);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::SendMessage()
    //
    // For use by the watched setup process. Sends a message to the watcher and
    // synchronously waits on a reply, up to the timeout value.
    //
    // iMessageType - Type of message being sent, typically one of the
    //                INSTALLMESSAGE_* values, optionally combined with other flags.
    //
    // hRec         - Message record being sent.
    //
    // pdwReply     - [OUT] Receives the reply code from the watcher.
    //
    // Returns: 0 on success, Win32 error code on failure.
    // Returns WAIT_TIMEOUT if no reply was received in time.
    //
    virtual DWORD SendMessage(UINT iMessageType, MSIHANDLE hRec, DWORD* pdwReply)
    {
        if (m_fWatcher)
        {
            return ERROR_INVALID_OPERATION;
        }

        if (!m_fConnected)
        {
            *pdwReply = (DWORD) -1;
            return 0;
        }

        DWORD dwRet = this->SendMessage(iMessageType, hRec);
        if (dwRet != 0)
        {
            return dwRet;
        }

        return this->ReceiveReply(pdwReply);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // CSetupWatcher::SetReplyTimeout()
    //
    // For use by the watched setup process. Configures the amount of time to
    // wait for a reply to a message. If a reply is not received within that time,
    // then the connection is broken.
    //
    virtual void SetReplyTimeout(DWORD dwMilliseconds)
    {
        m_dwReplyTimeout = dwMilliseconds;
    }

/////////////////////////////////////////////////////////////////////////////////////////
// PUBLIC DOCUMENTATION ENDS HERE                                                      //
// Code below contains only private implementation details.                            //
/////////////////////////////////////////////////////////////////////////////////////////

private:

    //
    // Called only by the watcher process.
    // Create a new thread to handle receiving messages.
    //
    DWORD ReceiveMessages(INSTALLUI_HANDLER pHandler,
        INSTALLUI_HANDLER_RECORD pRecHandler, void* pvContext)
    {
        if (!m_fWatcher || m_hReceiveStopEvent != NULL)
        {
            return ERROR_INVALID_OPERATION;
        }

        DWORD dwRet = 0;

        m_hReceiveStopEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

        if (m_hReceiveStopEvent == NULL)
        {
            dwRet = GetLastError();
        }
        else
        {
            void** pvParams = new void*[4];
            if (pvParams == NULL)
            {
                dwRet = ERROR_OUTOFMEMORY;
            }
            else
            {
                pvParams[0] = this;
                pvParams[1] = pHandler;
                pvParams[2] = pRecHandler;
                pvParams[3] = pvContext;
            
                if (m_hReceiveThread != NULL)
                {
                    CloseHandle(m_hReceiveThread);
                }

                m_hReceiveThread = CreateThread(NULL, 0,
                    CSetupWatcher::ReceiveMessagesThread, pvParams, 0, NULL);
                
                if (m_hReceiveThread == NULL)
                {
                    dwRet = GetLastError();
                    CloseHandle(m_hReceiveStopEvent);
                    m_hReceiveStopEvent = NULL;
                }
            }
        }

        return dwRet;
    }

    //
    // Called only by the watcher process.
    // First verify the connection is complete. Then continually read and parse messages,
    // invoke the callback, and send the replies.
    //
    static DWORD WINAPI ReceiveMessagesThread(void* pv)
    {
        void** pvParams = (void**) pv;
        CSetupWatcher* pThis = (CSetupWatcher*) pvParams[0];
        INSTALLUI_HANDLER pHandler = (INSTALLUI_HANDLER) pvParams[1];
        INSTALLUI_HANDLER_RECORD pRecHandler = (INSTALLUI_HANDLER_RECORD) pvParams[2];
        void* pvContext = pvParams[3];
        delete[] pvParams;

        DWORD dwRet;

        dwRet = pThis->CompleteConnection();
        if (dwRet != 0)
        {
            if (dwRet == ERROR_OPERATION_ABORTED) dwRet = 0;
        }

        while (pThis->m_fConnected)
        {
            DWORD dwType;
            MSIHANDLE hRec;
            dwRet = pThis->ReceiveMessage(&dwType, &hRec);
            if (dwRet != 0)
            {
                if (dwRet == ERROR_OPERATION_ABORTED ||
                    dwRet == ERROR_BROKEN_PIPE || dwRet == ERROR_NO_DATA)
                {
                    dwRet = 0;
                }
            }
            else
            {
                DWORD dwReply = pThis->DispatchMessage(dwType, hRec,
                    pHandler, pRecHandler, pvContext);

                if (hRec != NULL)
                {
                    MsiCloseHandle(hRec);
                }

                dwRet = pThis->SendReply(dwReply);
                if (dwRet != 0 && dwRet != ERROR_BROKEN_PIPE && dwRet != ERROR_NO_DATA)
                {
                    dwRet = 0;
                }
            }
        }

        CloseHandle(pThis->m_hReceiveStopEvent);
        pThis->m_hReceiveStopEvent = NULL;
        return dwRet;
    }

    //
    // Called only by the watcher process's receive thread.
    // Try to complete and verify an asynchronous connection operation.
    //
    DWORD CompleteConnection()
    {
        DWORD dwRet = 0;
        if (m_fConnecting)
        {
            HANDLE hWaitHandles[] = { m_overlapped.hEvent, m_hReceiveStopEvent };
            DWORD dwWaitRes = WaitForMultipleObjects(2, hWaitHandles, FALSE, INFINITE);

            if (dwWaitRes == WAIT_OBJECT_0)
            {
                m_fConnecting = false;

                DWORD dwUnused;
                if (GetOverlappedResult(m_hPipe, &m_overlapped, &dwUnused, FALSE))
                {
                    m_fConnected = true;
                }
                else
                {
                    dwRet = GetLastError();
                }
            }
            else if (dwWaitRes == WAIT_FAILED)
            {
                CancelIo(m_hPipe);
                dwRet = GetLastError();
            }
            else
            {
                CancelIo(m_hPipe);
                dwRet = ERROR_OPERATION_ABORTED;
            }
        }
        return dwRet;
    }

    //
    // Called only by the watcher process's receive thread.
    // Read one message and convert it to an MSI record.
    //
    DWORD ReceiveMessage(DWORD* pdwType, MSIHANDLE* phRec)
    {
        DWORD dwRet = 0;

        *pdwType = 0;
        *phRec = NULL;
        
        DWORD cbMsg = 0;
        dwRet = this->ReadPipe((BYTE*) &cbMsg, sizeof(DWORD));
        if (dwRet == 0 && cbMsg == 0)
        {
            dwRet = ERROR_OPERATION_ABORTED;
        }
        if (dwRet == 0)
        {
            cbMsg -= sizeof(DWORD);

            if (!this->CheckMessageBuf(cbMsg))
            {
                return ERROR_OUTOFMEMORY;
            }

            dwRet = this->ReadPipe(m_pMsgBuf, cbMsg);
            if (dwRet == 0)
            {
                *pdwType = ((DWORD*) m_pMsgBuf)[0];
                DWORD cFields = ((DWORD*) m_pMsgBuf)[1];

                if (cFields > 0)
                {
                    *phRec = MsiCreateRecord(cFields - 1);
                    if (*phRec != NULL)
                    {
                        wchar_t* pData = (wchar_t*) (m_pMsgBuf + 2 * sizeof(DWORD));
                        wchar_t* pEnd = (wchar_t*) (m_pMsgBuf + cbMsg);
                        for (DWORD i = 0; i < cFields && pData < pEnd; i++)
                        {
                            MsiRecordSetString(*phRec, i, pData);
                            while(pData < pEnd && *pData != L'\0') pData++;
                            pData++;
                        }
                    }
                }
            }

        }

        return dwRet;
    }

    //
    // Called only by the watcher process's receive thread.
    // Read part of a message, allowing interruption by the stop event.
    //
    DWORD ReadPipe(BYTE* pBuf, DWORD cbRead)
    {
        DWORD dwRet = 0;
        ResetEvent(m_overlapped.hEvent);
        if (!ReadFile(m_hPipe, pBuf, cbRead, NULL, &m_overlapped))
        {
            dwRet = GetLastError();
            if (dwRet == ERROR_IO_PENDING)
            {
                HANDLE hWaitHandles[] = { m_overlapped.hEvent, m_hReceiveStopEvent };
                dwRet = WaitForMultipleObjects(2, hWaitHandles, FALSE, INFINITE);

                if (dwRet == WAIT_OBJECT_0)
                {
                    DWORD dwUnused;
                    if (!GetOverlappedResult(m_hPipe, &m_overlapped, &dwUnused, FALSE))
                    {
                        dwRet = GetLastError();
                    }
                }
                else if (dwRet == WAIT_FAILED)
                {
                    dwRet = GetLastError();
                }
                else
                {
                    dwRet = ERROR_OPERATION_ABORTED;
                }
            }
        }

        if (dwRet != 0)
        {
            CancelIo(m_hPipe);
            DisconnectNamedPipe(m_hPipe);
            m_fConnected = false;
        }

        return dwRet;
    }

    //
    // Called only by the watcher process.
    // Given a message, invoke the callback.
    //
    DWORD DispatchMessage(DWORD dwMsgType, MSIHANDLE hRec, INSTALLUI_HANDLER pHandler,
        INSTALLUI_HANDLER_RECORD pRecHandler, void* pvContext)
    {
        DWORD dwReply = 0;

        if (pHandler != NULL)
        {
            wchar_t szEmpty[1] = {0};
            if (hRec != NULL)
            {
                DWORD cchMsg = 0;
                DWORD dwRet = MsiFormatRecord(NULL, hRec, szEmpty, &cchMsg);
                if (dwRet == ERROR_MORE_DATA)
                {
                    wchar_t* szMsg = new wchar_t[++cchMsg];
                    dwRet = MsiFormatRecord(NULL, hRec, szMsg, &cchMsg);
                    if (dwRet == 0)
                    {
                        dwReply = pHandler(pvContext, dwMsgType, szMsg);
                    }
                    delete[] szMsg;
                }
            }
            else
            {
                dwReply = pHandler(pvContext, dwMsgType, szEmpty);
            }
        }
        else if (pRecHandler != NULL)
        {
            dwReply = pRecHandler(pvContext, dwMsgType, hRec);
        }

        return dwReply;
    }

    //
    // Called only by the watcher process.
    // Just send a reply code over the pipe.
    //
    DWORD SendReply(DWORD dwReply)
    {
        DWORD dwRet = 0;
        ResetEvent(m_overlapped.hEvent);
        if (!WriteFile(m_hPipe, (BYTE*) &dwReply, sizeof(DWORD), NULL, &m_overlapped))
        {
            dwRet = GetLastError();
            if (dwRet == ERROR_IO_PENDING)
            {
                dwRet = 0;
                DWORD dwUnused;
                if (!GetOverlappedResult(m_hPipe, &m_overlapped, &dwUnused, TRUE))
                {
                    dwRet = GetLastError();
                }
            }
        }

        if (dwRet != 0)
        {
            DisconnectNamedPipe(m_hPipe);
            m_fConnected = false;
        }

        return dwRet;
    }

    //
    // Called only by the watched setup process.
    // Format an MSI record into message bytes and write them to the pipe.
    //
    DWORD SendMessage(DWORD dwType, MSIHANDLE hRec)
    {
        DWORD dwRet = 0;
        DWORD cFields = 0;

        if (hRec != NULL)
        {
            cFields = MsiRecordGetFieldCount(hRec) + 1;
        }

        DWORD cbMsg = 3 * sizeof(DWORD);
        wchar_t szEmpty[1] = {0};
        for (DWORD i = 0; i < cFields; i++)
        {
            DWORD cchField = 0;
            dwRet = MsiRecordGetString(hRec, i, szEmpty, &cchField);
            if (dwRet == ERROR_MORE_DATA)
            {
                cbMsg += (cchField + 1) * sizeof(wchar_t);
            }
        }

        if (!this->CheckMessageBuf(cbMsg))
        {
            return ERROR_OUTOFMEMORY;
        }

        ((DWORD*) m_pMsgBuf)[0] = cbMsg;
        ((DWORD*) m_pMsgBuf)[1] = dwType;
        ((DWORD*) m_pMsgBuf)[2] = cFields;

        wchar_t* pData = (wchar_t*) (m_pMsgBuf + 3 * sizeof(DWORD));
        wchar_t* pEnd = (wchar_t*) (m_pMsgBuf + cbMsg);
        for (DWORD i = 0; i < cFields && pData < pEnd; i++)
        {
            pData[0] = L'\0';
            DWORD cchField = 0;
            dwRet = MsiRecordGetString(hRec, i, szEmpty, &cchField);
            if (dwRet == ERROR_MORE_DATA && (int) cchField < pEnd - pData)
            {
                cchField = pEnd - pData;
                MsiRecordGetString(hRec, i, pData, &cchField);
            }
            pData[cchField] = L'\0';
            pData += cchField + 1;
        }

        dwRet = 0;
        ResetEvent(m_overlapped.hEvent);
        if (!WriteFile(m_hPipe, m_pMsgBuf, cbMsg, NULL, &m_overlapped))
        {
            dwRet = GetLastError();
            if (dwRet == ERROR_IO_PENDING)
            {
                dwRet = 0;
                DWORD dwUnused;
                if (!GetOverlappedResult(m_hPipe, &m_overlapped, &dwUnused, TRUE))
                {
                    dwRet = GetLastError();
                }
            }
        }
        
        if (dwRet != 0)
        {
            m_fConnected = false;
            CloseHandle(m_hPipe);
            m_hPipe = NULL;
        }

        return dwRet;
    }

    //
    // Called only by the watched setup process.
    // Wait for a reply code on the pipe. If no reply is received before the timeout,
    // then give up and close the connection.
    //
    DWORD ReceiveReply(DWORD* pdwReply)
    {
        DWORD dwRet = 0;
        ResetEvent(m_overlapped.hEvent);
        if (!ReadFile(m_hPipe, pdwReply, sizeof(DWORD), NULL, &m_overlapped))
        {
            dwRet = GetLastError();
            if (dwRet == ERROR_IO_PENDING)
            {
                dwRet = WaitForSingleObject(m_overlapped.hEvent, m_dwReplyTimeout);
                if (dwRet == WAIT_OBJECT_0)
                {
                    DWORD dwUnused;
                    if (!GetOverlappedResult(m_hPipe, &m_overlapped, &dwUnused, FALSE))
                    {
                        dwRet = GetLastError();
                    }
                }
            }
        }

        if (dwRet != 0)
        {
            m_fConnected = false;
            CloseHandle(m_hPipe);
            m_hPipe = NULL;
        }

        return dwRet;
    }

    //
    // Called only by the watcher process.
    // Creates a named pipe instance and begins asynchronously waiting
    // for a connection from the watched setup process.
    //
    DWORD ConnectPipeServer()
    {
        DWORD dwRet = 0;
        const int BUFSIZE = 1024; // Suggested pipe I/O buffer sizes
        m_hPipe = CreateNamedPipe(
            m_szPipeName,
            PIPE_ACCESS_DUPLEX | FILE_FLAG_OVERLAPPED,
            PIPE_TYPE_BYTE | PIPE_READMODE_BYTE,
            1, BUFSIZE, BUFSIZE, 0, NULL);
        if (m_hPipe == INVALID_HANDLE_VALUE)
        {
            m_hPipe = NULL;
            dwRet = GetLastError();
        }
        else if (ConnectNamedPipe(m_hPipe, &m_overlapped))
        {
            m_fConnected = true;
        }
        else
        {
            dwRet = GetLastError();
            if (dwRet == ERROR_IO_PENDING)
            {
                dwRet = 0;
                m_fConnecting = true;
            }
        }
        return dwRet;
    }

    //
    // Called only by the watched setup process.
    // Attemps to open a connection to an existing named pipe instance
    // which should have already been created by the watcher process.
    //
    DWORD ConnectPipeClient()
    {
        DWORD dwRet = 0;
        m_hPipe = CreateFile(
            m_szPipeName, GENERIC_READ | GENERIC_WRITE, 
            0, NULL, OPEN_EXISTING,    FILE_FLAG_OVERLAPPED, NULL);
        if (m_hPipe != INVALID_HANDLE_VALUE)
        {
            m_fConnected = true;
        }
        else
        {
            m_hPipe = NULL;
            dwRet = GetLastError();
        }
        return dwRet;
    }

    //
    // Ensures that the message buffer is large enough to hold a message,
    // reallocating the buffer if necessary.
    //
    BOOL CheckMessageBuf(DWORD cbMsg)
    {
        if (m_cbMsgBuf < cbMsg)
        {
            if (m_pMsgBuf != NULL)
            {
                delete[] m_pMsgBuf;
            }
            m_cbMsgBuf = cbMsg;
            m_pMsgBuf = new BYTE[m_cbMsgBuf];
            if (m_pMsgBuf == NULL)
            {
                m_cbMsgBuf = 0;
            }
        }
        return m_pMsgBuf != NULL;
    }

private:

    // True if this is the watcher process, false if this is the watched setup process.
    const bool m_fWatcher;

    // Name of this SetupWatcher instance. 
    const wchar_t* m_szName;

    // "\\.\pipe\name"
    wchar_t* m_szPipeName;
    
    // Handle to the pipe instance.
    HANDLE m_hPipe;

    // Handle to the thread that receives messages.
    HANDLE m_hReceiveThread;

    // Handle to the event used to signal the receive thread to exit.
    HANDLE m_hReceiveStopEvent;

    // All pipe I/O is done in overlapped mode to avoid unintentional blocking.
    OVERLAPPED m_overlapped;
    
    // Dynamically-resized buffer for sending and receiving messages.
    BYTE* m_pMsgBuf;

    // Current size of the message buffer.
    DWORD m_cbMsgBuf;

    // True if an asynchronous connection operation is currently in progress.
    bool m_fConnecting;

    // True if the pipe is currently connected.
    bool m_fConnected;

    // Length of time in milliseconds to wait for a reply from the watcher.
    DWORD m_dwReplyTimeout;

    // This is only designed for local IPC, so replys should be very fast.
    // Still, give the watcher process a little time to process the message.
    static const int DEFAULT_REPLY_TIMEOUT = 2000;
};
