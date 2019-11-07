/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    spltypes.h

--*/
#ifndef _SPLTYPES_H_
#define _SPLTYPES_H_

#define ILM_SIGNATURE   0x4d4c49  /* 'ILM' is the signature value */

typedef struct _LCMINIPORT  *PLCMINIPORT;
typedef struct _INIXCVPORT  *PINIXCVPORT;

typedef struct _INILOCALMON {
    DWORD signature;
    PMONITORINIT pMonitorInit;
    PLCMINIPORT pIniPort;
    PINIXCVPORT pIniXcvPort;
} INILOCALMON, *PINILOCALMON;

typedef struct _INIENTRY {
    DWORD       signature;
    DWORD       cb;
    struct _INIENTRY *pNext;
    DWORD       cRef;
    LPWSTR      pName;
} INIENTRY, *PINIENTRY;

// IMPORTANT: the offset to pNext in _LCMINIPORT must be the same as in INIXCVPORT (DeletePortNode)
typedef _Struct_size_bytes_(cb) struct _LCMINIPORT {       /* ipo */
    DWORD   signature;
    _Field_range_(>=, sizeof(struct _LCMINIPORT)) 
    DWORD   cb;
    _Field_size_opt_(1) 
    struct  _LCMINIPORT *pNext;
    DWORD   cRef;
    LPWSTR  pName;
    HANDLE  hFile;               // File handle
    DWORD   cbWritten;
    DWORD   Status;              // see PORT_ manifests
    LPWSTR  pPrinterName;
    LPWSTR  pDeviceName;
    HANDLE  hPrinter;
    DWORD   JobId;
    PINILOCALMON        pIniLocalMon;
    LPBYTE              pExtra;
} LCMINIPORT, *PLCMINIPORT;

#define IPO_SIGNATURE   0x5450  /* 'PT' is the signature value */

// IMPORTANT: the offset to pNext in INIXCVPORT must be the same as in INIPORT (DeletePortNode)
typedef _Struct_size_bytes_(cb) struct _INIXCVPORT {
    DWORD       signature;
    _Field_range_(>=, sizeof(struct _INIXCVPORT)) 
    DWORD       cb;
    _Field_size_opt_(1) 
    struct      _INIXCVPORT *pNext;
    DWORD       cRef;
    DWORD       dwMethod;
    LPWSTR      pszName;
    DWORD       dwState;
    ACCESS_MASK GrantedAccess;
    PINILOCALMON pIniLocalMon;
} INIXCVPORT, *PINIXCVPORT;

#define XCV_SIGNATURE   0x5843  /* 'XC' is the signature value */


#define PP_DOSDEVPORT     0x0001  // A port for which we did DefineDosDevice
#define PP_COMM_PORT      0x0002  // A port for which GetCommTimeouts was successful
#define PP_FILEPORT       0x0004  // The port is a file port
#define PP_STARTDOC       0x0008  // Port is in use (startdoc called)

#define FindPort(pIniLocalMon, psz)                          \
    (PLCMINIPORT)LcmFindIniKey( (PINIENTRY)pIniLocalMon->pIniPort, \
                                 (LPWSTR)(psz))

#endif // _SPLTYPES_H_


