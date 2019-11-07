/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    ReportManager.h

Abstract:

    This module contains the type definitions for the SPB accelerometer's
    report manager class.

--*/

#pragma once

class CSensorDdi;

class CReportManager :
    public CComObjectRoot
{
// Public methods
public:
    CReportManager();
    ~CReportManager();

    DECLARE_NOT_AGGREGATABLE(CReportManager)

    BEGIN_COM_MAP(CReportManager)        
    END_COM_MAP()
    
    VOID Initialize(
        _In_ CSensorDdi* pSensorDdi,
        _In_ ULONG initialReportInterval);
    VOID Uninitialize();
    VOID SetReportInterval(_In_ ULONG reportInterval);
    VOID NewDataAvailable();

// Private methods
private:
    static DWORD WINAPI _DataEventingThreadProc(_In_ LPVOID pvData);
    VOID ActivateDataEventingThread();
    VOID DeactivateDataEventingThread();
    BOOL IsDataEventingThreadActive() { return m_fDataEventingThreadActive; }

// Members
private:
    ULONGLONG                m_LastReportTickCount;
    ULONG                    m_ReportInterval;
    CSensorDdi*              m_pSensorDdi;

    //Eventing
    HANDLE                   m_hDataEvent;
    HANDLE                   m_hDataEventingThread;
    BOOL                     m_fDataEventingThreadActive;
    CComAutoCriticalSection  m_ThreadCS;

};

