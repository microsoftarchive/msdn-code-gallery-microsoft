/*++

Copyright (c) 1998  Microsoft Corporation

Module Name:

    async.h

Abstract

    1394 async wrappers

--*/

INT_PTR CALLBACK
AllocateAddressRangeDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_AllocateAddressRange(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
FreeAddressRangeDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_FreeAddressRange(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
AsyncReadDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_AsyncRead(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
AsyncWriteDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_AsyncWrite(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
AsyncLockDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_AsyncLock(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
AsyncStreamDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_AsyncStream(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );
/*
INT_PTR CALLBACK
AsyncLoopbackDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_AsyncStartLoopback(
    HWND                    hWnd,
    _In_ PSTR               szDeviceName,
    PASYNC_LOOPBACK_PARAMS  asyncLoopbackParams
    );

void
w1394_AsyncStopLoopback(
    HWND                    hWnd,
    PASYNC_LOOPBACK_PARAMS  asyncLoopbackParams
    );

void
w1394_AsyncStreamStartLoopback(
    HWND                            hWnd,
    _In_ PSTR                       szDeviceName,
    PASYNC_STREAM_LOOPBACK_PARAMS   streamLoopbackParams
    );

void
w1394_AsyncStreamStopLoopback(
    HWND                            hWnd,
    PASYNC_STREAM_LOOPBACK_PARAMS   streamLoopbackParams
    );
*/

