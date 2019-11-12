/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    ReportManager.cpp

Abstract:

    This module contains the implementation of the SPB accelerometer's
    report manager class.

--*/

#include "Internal.h"
#include "SensorDdi.h"

#include "ReportManager.h"
#include "ReportManager.tmh"

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::CReportManager()
//
//  Constructor
//
/////////////////////////////////////////////////////////////////////////
CReportManager::CReportManager() :
    m_pSensorDdi(nullptr),
    m_LastReportTickCount(0),
    m_ReportInterval(0),
    m_hDataEvent(nullptr),
    m_hDataEventingThread(nullptr),
    m_fDataEventingThreadActive(FALSE)
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::~CReportManager()
//
//  Destructor
//
/////////////////////////////////////////////////////////////////////////
CReportManager::~CReportManager()
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::Initialize()
//
//  This method is used to initialize the data eventing thread
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CReportManager::Initialize(
    _In_ CSensorDdi* pSensorDdi,
    _In_ ULONG initialReportInterval
    )
{
    FuncEntry();

    // Hold a weak reference to the DDI
    m_pSensorDdi = pSensorDdi;

    // Set the initial report interval
    SetReportInterval(initialReportInterval);

    // Create the data event and eventing thread
    m_hDataEvent = CreateEvent(  
        nullptr,   // No security attributes
        FALSE,     // Automatic-reset event object
        FALSE,     // Initial state is non-signaled
        nullptr);  // Unnamed object

    ActivateDataEventingThread();

    m_hDataEventingThread = CreateThread(
        nullptr,                                   // Cannot be inherited by child process
        0,                                         // Default stack size
        &CReportManager::_DataEventingThreadProc,  // Thread proc
        (LPVOID)this,                              // Thread proc argument
        0,                                         // Starting state = running
        nullptr);                                  // No thread identifier

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::Uninitialize()
//
//  This method is used to uninitialize the data eventing thread
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CReportManager::Uninitialize()
{
    FuncEntry();

    // Close event and thread handles
    if (m_hDataEventingThread != nullptr)
    {
        // De-activate and close the thread
        DeactivateDataEventingThread();
        SetEvent(m_hDataEvent);
        WaitForSingleObject(m_hDataEventingThread, INFINITE);
        CloseHandle(m_hDataEventingThread);
        m_hDataEventingThread = nullptr;
    }
    
    if (m_hDataEvent != nullptr)
    {
        CloseHandle(m_hDataEvent);
        m_hDataEvent = nullptr;
    }

    // Clear the weak reference to the DDI
    m_pSensorDdi = nullptr;

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::SetReportInterval
//
//  This method is used to set the report interval. The report manager
//  will not have the DDI post a new data event any more frequently
//  than this.
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CReportManager::SetReportInterval(
    _In_ DWORD reportInterval
    )
{
    FuncEntry();

    m_ReportInterval = reportInterval;

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::NewDataAvailable
//
//  This method is used to inform the report manger that new data is
//  available.
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CReportManager::NewDataAvailable()
{
    FuncEntry();

    if(m_hDataEvent != nullptr)
    {
        // Set the data event
        SetEvent(m_hDataEvent);
    }

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::ActivateDataEventingThread
//
//  This synchronized method is used to mark the data eventing thread as 
//  activate.
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CReportManager::ActivateDataEventingThread()
{
    FuncEntry();

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_ThreadCS);

    m_fDataEventingThreadActive = TRUE;

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::DeactivateDataEventingThread
//
//  This synchronized method is used to mark the data eventing thread as
//  inactivate.
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CReportManager::DeactivateDataEventingThread()
{
    FuncEntry();

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_ThreadCS);

    m_fDataEventingThreadActive = FALSE;

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CReportManager::_DataEventingThreadProc
//
//  This is the data eventing thread routine that listens for new data
//  events and informs the DDI after the report interval has expired.
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
DWORD WINAPI CReportManager::_DataEventingThreadProc(
    _In_ LPVOID pvData
    )
{
    FuncEntry();

    CReportManager* pThis = static_cast<CReportManager*>(pvData);

    if (pThis == nullptr)
    {
        return 0;
    }

    // Cast the argument to the correct type.
    //CSensorDdi* pThis = static_cast<CSensorDdi*>(pvData);

    HRESULT hr = CoInitializeEx(NULL, COINIT_MULTITHREADED);

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR, 
            "Failed to call CoInitialize on _DataEventingThreadProc thread, "
            "%!HRESULT!",
            hr);

        return 0;
    }
   
    while (WaitForSingleObject(pThis->m_hDataEvent, INFINITE) == WAIT_OBJECT_0)
    {
        if (pThis->IsDataEventingThreadActive() == FALSE)
        {
            break;
        }

        ULONGLONG currentTickCount = GetTickCount64();
        ULONG msSinceLastReport = 
            (ULONG)(currentTickCount - pThis->m_LastReportTickCount);

        ULONG reportInterval = pThis->m_ReportInterval;

        if (msSinceLastReport < reportInterval)
        {
            // The report interval has not passed since
            // the last report was sent so sleep
            Sleep(reportInterval - msSinceLastReport);
        }
            
        // Save the current tick count as the new
        // last report tick count
        pThis->m_LastReportTickCount = GetTickCount64();

        if (pThis->m_pSensorDdi != nullptr)
        {
            // Let the DDI know that the report interval
            // has expired and new data is available
            pThis->m_pSensorDdi->ReportIntervalExpired();
        }
    }
       
    CoUninitialize();

    FuncExit();

    return 0;
}
