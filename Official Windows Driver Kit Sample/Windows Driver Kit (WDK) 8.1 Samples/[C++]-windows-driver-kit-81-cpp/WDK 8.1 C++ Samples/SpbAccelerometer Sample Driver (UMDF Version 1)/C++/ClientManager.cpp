/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    ClientManager.cpp

Abstract:

    This module contains the implementation of the SPB accelerometer's
    client manager class.

--*/

#include "Internal.h"
#include "Adxl345.h"

#include <limits.h>
#include <float.h>

#include "ClientManager.h"
#include "ClientManager.tmh"

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::CClientManager()
//
//  Constructor.
//
/////////////////////////////////////////////////////////////////////////
CClientManager::CClientManager() :
    m_ClientCount(0),
    m_SubscriberCount(0),
    m_spMinSensitivityValues(nullptr),
    m_minReportInterval(0),
    m_minReportIntervalExplicitlySet(FALSE)
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::~CClientManager()
//
//  Destructor
//
/////////////////////////////////////////////////////////////////////////
CClientManager::~CClientManager()
{
    {
        // Synchronize access to the client list and associated members
        CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

        POSITION position;
        CLIENT_ENTRY entry;

        _ATLTRY
        {
            position = m_pClientList.GetStartPosition();

            while (position != nullptr)
            {
                entry = m_pClientList.GetNextValue(position);
                SAFE_RELEASE(entry.pDesiredSensitivityValues);
            }
        
            m_pClientList.RemoveAll();
        }
        _ATLCATCH (e)
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Error clearing client list, %!HRESULT!",
                e);
        }
    }

    {
        // Synchronize access to the minimum property cache
        CComCritSecLock<CComAutoCriticalSection> propsLock(m_MinPropsCS);

        if (m_spMinSensitivityValues != nullptr)
        {
            m_spMinSensitivityValues->Clear();
        }
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::Initialize
//
//  This method is used to initialize the client manager and its internal
//  member objects.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::Initialize(
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (m_spMinSensitivityValues == nullptr)
    {
        // Create a new PortableDeviceValues to store the property VALUES
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_spMinSensitivityValues));
    }

    // Set default change sensitivities
    if (SUCCEEDED(hr))
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        var.vt = VT_R8;
        var.dblVal = DEFAULT_ACCELEROMETER_CHANGE_SENSITIVITY;
                
        hr = m_spMinSensitivityValues->SetValue(
            SENSOR_DATA_TYPE_ACCELERATION_X_G,
            &var);

        if (SUCCEEDED(hr))
        {
            hr = m_spMinSensitivityValues->SetValue(
                SENSOR_DATA_TYPE_ACCELERATION_Y_G,
                &var);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spMinSensitivityValues->SetValue(
                SENSOR_DATA_TYPE_ACCELERATION_Z_G,
                &var);
        }

        PropVariantClear(&var);
    }
    
    // Set default report interval
    if (SUCCEEDED(hr))
    {
        m_minReportInterval = 
            DEFAULT_ACCELEROMETER_CURRENT_REPORT_INTERVAL;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::Connect
//
//  This method is used to indicate that a new client has connected.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::Connect(
    _In_ IWDFFile* pClientFile
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Create a new PortableDeviceValues to store the property VALUES
        CComPtr<IPortableDeviceValues> spValues;
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&spValues));

        if (SUCCEEDED(hr))
        {
            // Initialize the desired change sensitivity
            // property values
            PROPVARIANT var;
            PropVariantInit(&var);

            var.vt = VT_R8;
            var.dblVal = CHANGE_SENSITIVITY_NOT_SET;
                
            hr = spValues->SetValue(
                SENSOR_DATA_TYPE_ACCELERATION_X_G,
                &var);

            if (SUCCEEDED(hr))
            {
                hr = spValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_Y_G,
                    &var);
            }

            if (SUCCEEDED(hr))
            {
                hr = spValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_Z_G,
                    &var);
            }

            PropVariantClear(&var);
        }

        if (SUCCEEDED(hr))
        {
            // Synchronize access to the client list and associated members
            CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

            // Sanity check the client count
            if (m_pClientList.GetCount() != m_ClientCount)
            {
                hr = HRESULT_FROM_WIN32(ERROR_INVALID_STATE);

                Trace(
                    TRACE_LEVEL_ERROR,
                    "Invalid ClientManager state detected: client list "
                    "entries = %d, client count = %d, %!HRESULT!",
                    (ULONG)m_pClientList.GetCount(),
                    m_ClientCount,
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                // Check to see if the client is already
                // in the client list
                CLIENT_ENTRY entry;

                if (m_pClientList.Lookup(pClientFile, entry) == true)
                {
                    // The client already exists
                    hr = HRESULT_FROM_WIN32(ERROR_FILE_EXISTS);

                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Client %p already exists in the client list, %!HRESULT!",
                        pClientFile,
                        hr);
                }
        
                if (SUCCEEDED(hr))
                {
                    // Store the file id and initialize the entry
                    entry.fSubscribed = FALSE;
                    entry.pDesiredSensitivityValues = spValues;
                    entry.desiredReportInterval = 
                        CURRENT_REPORT_INTERVAL_NOT_SET;
                
                    entry.pDesiredSensitivityValues->AddRef();

                    _ATLTRY
                    {
                        m_pClientList[pClientFile] = entry;
                    }
                    _ATLCATCH (e)
                    {
                        hr = e;
                        Trace(
                            TRACE_LEVEL_ERROR,
                            "Error adding entry to client list, %!HRESULT!",
                            hr);
                    }

                    if (SUCCEEDED(hr))
                    {
                        Trace(
                            TRACE_LEVEL_INFORMATION,
                            "Client %p has connected",
                            pClientFile);

                        // Increment client count
                        m_ClientCount++;

                        // Recalculate new minimum properties
                        hr = RecalculateProperties();
                    }
                }
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::Disconnect
//
//  This method is used to indicate that a client has disconnected.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::Disconnect(
    _In_ IWDFFile* pClientFile
    )
{
    FuncEntry();

    // Synchronize access to the client list and associated members
    CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    // Sanity check the client count

    if (SUCCEEDED(hr))
    {
        if (m_ClientCount == 0)
        {
            hr = HRESULT_FROM_WIN32(ERROR_INVALID_STATE);

            Trace(
                TRACE_LEVEL_ERROR,
                "Invalid ClientManager state detected: attempting to "
                "disconnect client %p with client count = 0, %!HRESULT!",
                pClientFile,
                hr);
        }
    }

    if (SUCCEEDED(hr))
    {
        if (m_pClientList.GetCount() != m_ClientCount)
        {
            hr = HRESULT_FROM_WIN32(ERROR_INVALID_STATE);

            Trace(
                TRACE_LEVEL_ERROR,
                "Invalid ClientManager state detected: client list "
                "entries = %d, client count = %d, %!HRESULT!",
                (ULONG)m_pClientList.GetCount(),
                m_ClientCount,
                hr);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Ensure the client is in the client list
        CLIENT_ENTRY entry;

        if (m_pClientList.Lookup(pClientFile, entry) == false)
        {
            // The client isn't connected
            hr = HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "Client %p was not found in the client list, %!HRESULT!",
                pClientFile,
                hr);
        }
        
        if (SUCCEEDED(hr))
        {
            // Clear the change sensitivity values
            SAFE_RELEASE(entry.pDesiredSensitivityValues);

            _ATLTRY
            {
                // Remove the entry from the client list
                if (m_pClientList.RemoveKey(pClientFile) == false)
                {
                    // Entry is not in client list or could
                    // not be removed
                    hr = E_UNEXPECTED;
                }
            }
            _ATLCATCH (e)
            {
                hr = e;
            }

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Error removing entry from client list, %!HRESULT!",
                    hr);
                    
            }
            
            if (SUCCEEDED(hr))
            {
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Client %p has disconnected",
                    pClientFile);

                // Decrement client count
                m_ClientCount--;

                // Recalculate new minimum properties
                hr = RecalculateProperties();
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::Subscribe
//
//  This method is used to indicate that a client has subscribed to events.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::Subscribe(
    _In_ IWDFFile* pClientFile
    )
{
    FuncEntry();

    // Synchronize access to the client list and associated members
    CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Ensure the client is in the client list
        CLIENT_ENTRY entry;

        if (m_pClientList.Lookup(pClientFile, entry) == false)
        {
            // The client isn't connected
            hr = HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "Client %p was not found in the client list, %!HRESULT!",
                pClientFile,
                hr);
        }
        
        if (SUCCEEDED(hr))
        {
            // Mark the client as subscribed
            entry.fSubscribed = TRUE;

            _ATLTRY
            {
                m_pClientList[pClientFile] = entry;
            }
            _ATLCATCH (e)
            {
                hr = e;
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Error updating entry in client list, %!HRESULT!",
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Client %p has subscribed to events",
                    pClientFile);

                // Increment subscriber count
                m_SubscriberCount++;

                // Recalculate new minimum properties
                hr = RecalculateProperties();
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::Unsubscribe
//
//  This method is used to indicate that a client has unsubscribed from events.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::Unsubscribe(
    _In_ IWDFFile* pClientFile
    )
{
    FuncEntry();

    // Synchronize access to the client list and associated members
    CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Ensure the client is in the client list
        CLIENT_ENTRY entry;

        if (m_pClientList.Lookup(pClientFile, entry) == false)
        {
            // The client isn't connected
            hr = HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "Client %p was not found in the client list, %!HRESULT!",
                pClientFile,
                hr);
        }
        
        if (SUCCEEDED(hr))
        {
            // Mark the client as unsubscribed
            entry.fSubscribed = FALSE;

            _ATLTRY
            {
                m_pClientList[pClientFile] = entry;
            }
            _ATLCATCH (e)
            {
                hr = e;
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Error updating entry in client list, %!HRESULT!",
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Client %p has unsubscribed from events",
                    pClientFile);

                // Decrement subscriber count
                m_SubscriberCount--;

                // Recalculate new minimum properties
                hr = RecalculateProperties();
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::GetClientCount
//
//  This method is used to retrieve the number of connected clients.
//
//  Parameters:
//
//  Return Values:
//      number of clients
//
/////////////////////////////////////////////////////////////////////////
ULONG CClientManager::GetClientCount()
{
    FuncEntry();

    // Synchronize access to the client list and associated members
    CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

    FuncExit();

    return m_ClientCount;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::GetSubscriberCount
//
//  This method is used to retrieve the number of clients subscribed to
//	events.
//
//  Parameters:
//
//  Return Values:
//      number of clients subscribed to events
//
/////////////////////////////////////////////////////////////////////////
ULONG CClientManager::GetSubscriberCount()
{
    FuncEntry();

    // Synchronize access to the client list and associated members
    CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

    FuncExit();

    return m_SubscriberCount;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::GetDataUpdateMode
//
//  This method is used to retrieve the current data update mode.
//
//  Parameters:
//
//  Return Values:
//      current data update mode
//
/////////////////////////////////////////////////////////////////////////
DATA_UPDATE_MODE CClientManager::GetDataUpdateMode()
{
    FuncEntry();

    DATA_UPDATE_MODE mode;

    // Synchronize access to the client list and associated members
    CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

    if (m_ClientCount == 0)
    {
        mode = DataUpdateModeOff;
    }
    else
    {
        if ((m_SubscriberCount > 0) || 
            (m_minReportIntervalExplicitlySet == TRUE))
        {
            mode = DataUpdateModeEventing;
        }
        else
        {
            mode = DataUpdateModePolling;
        }
    }

    FuncExit();

    return mode;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::SetDesiredProperty
//
//  This method is used to set a client's desired settable property
//  values.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//      key - the desired settable property key
//      pVar - pointer to the key value

//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::SetDesiredProperty(
    _In_  IWDFFile* pClientFile,
    _In_  REFPROPERTYKEY key, 
    _In_  PROPVARIANT* pVar, 
    _Out_ PROPVARIANT* pVarResult
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if ((pClientFile == nullptr) ||
        (pVar == nullptr) ||
        (pVarResult == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Change sensitivity
        if (IsEqualPropertyKey(
            key, 
            SENSOR_PROPERTY_CHANGE_SENSITIVITY) == TRUE)
        {
            // Change sensitivity is a per data field
            // property and is stored as an IPortableDeviceValues
            if (pVar->vt != VT_UNKNOWN)
            {
                hr = E_INVALIDARG;
            }

            if (SUCCEEDED(hr))
            {
                CComPtr<IPortableDeviceValues> spPerDataFieldValues;
                CComPtr<IPortableDeviceValues> spPerDataFieldResults;
                                
                spPerDataFieldValues =
                    static_cast<IPortableDeviceValues*>(pVar->punkVal);

                // Set the client's desired change sensitivity
                hr = SetDesiredChangeSensitivity(
                    pClientFile,
                    spPerDataFieldValues,
                    &spPerDataFieldResults);

                if (SUCCEEDED(hr))
                {
                    pVarResult->vt = VT_UNKNOWN;
                    pVarResult->punkVal = static_cast<IUnknown*>(
                        spPerDataFieldResults.Detach());
                }
            }

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to set desired change sensitivity "
                    "for client %p, %!HRESULT!",
                    pClientFile,
                    hr);
            }
        }

        // Report interval
        else if (IsEqualPropertyKey(
            key, 
            SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL) == TRUE)
        {
            // Report interval is type unsigned long
            if (pVar->vt != VT_UI4)
            {
                hr = E_INVALIDARG;
            }

            if (SUCCEEDED(hr))
            {
                ULONG reportInterval = pVar->ulVal;

                // Inform the client manager of the client's
                // desired report interval
                hr = SetDesiredReportInterval(
                    pClientFile,
                    reportInterval);

                if (SUCCEEDED(hr))
                {
                    *pVarResult = *pVar;
                }
            }

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to set desired report interval "
                    "for client %p, %!HRESULT!",
                    pClientFile,
                    hr);
            }
        }

        // Other property key
        else
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "The specified key is not one of the settable "
                "property values, %!HRESULT!",
                hr);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::GetArbitratedProperty
//
//  This method is used to retrieve the arbitrated settable property
//  value for all subscribed clients.
//
//  Parameters:
//      key - the requested property key
//      pValues - pointer to the IPortableDeviceValues where the
//          arbitrated property should be added
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::GetArbitratedProperty(
    _In_  REFPROPERTYKEY key,
    _Out_ PROPVARIANT* pVar
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pVar == nullptr)

    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Change sensitivity
        if (IsEqualPropertyKey(key, SENSOR_PROPERTY_CHANGE_SENSITIVITY))
        {
            // Create a IPortableDeviceValues to copy the 
            // change sensitivity values into
            CComPtr<IPortableDeviceValues> spMinSensitivityValuesCopy;
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&spMinSensitivityValuesCopy));

            if (SUCCEEDED(hr))
            {
                // Synchronize access to the minimum property cache
                CComCritSecLock<CComAutoCriticalSection> propsLock(m_MinPropsCS);

                // Copy the per data field values
                hr = CopyValues(
                    m_spMinSensitivityValues, 
                    spMinSensitivityValuesCopy);
            }

            if (SUCCEEDED(hr))
            {
                pVar->vt = VT_UNKNOWN;
                pVar->punkVal = static_cast<IUnknown*>(
                    spMinSensitivityValuesCopy.Detach());
            }

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to retrieve the change sensitivity value, "
                    "%!HRESULT!",
                    hr);
            }
        
        }
        // Report interval
        else if (IsEqualPropertyKey(key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL))
        {
            // Synchronize access to minimum property cache
            CComCritSecLock<CComAutoCriticalSection> propsLock(m_MinPropsCS);

            hr = InitPropVariantFromUInt32(
                m_minReportInterval,
                pVar);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to retrieve the report interval value, "
                    "%!HRESULT!",
                    hr);
            }
        }
        // Other non-settable property
        else
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "The specified key is not one of the settable "
                "property values, %!HRESULT!",
                hr);
        }
    }

    FuncExit();

    return hr;

}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::SetDesiredChangeSensitivity
//
//  This method is used to indicate a client's desired change sensitivity
//  values for each data field.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//      pValues - collection of per data field change sensitivity values
//      ppResults - an IPortableDeviceValues pointer that receives the 
//          list of per data field change sensitivity values if successful 
//          or an error code
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::SetDesiredChangeSensitivity(
    _In_  IWDFFile* pClientFile,
    _In_  IPortableDeviceValues* pValues,
    _Out_ IPortableDeviceValues** ppResults
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if ((pClientFile == nullptr) ||
        (pValues == nullptr) ||
        (ppResults == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Synchronize access to the client list and associated members
        CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

        DWORD count = 0;
        BOOL fError = FALSE;

        // Ensure the client is in the client list
        CLIENT_ENTRY entry;

        if (m_pClientList.Lookup(pClientFile, entry) == false)
        {
            // The client isn't connected
            hr = HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "Client %p was not found in the client list, %!HRESULT!",
                pClientFile,
                hr);
        }
        
        if (SUCCEEDED(hr))
        {
            // CoCreate an object to store the per data field
            // property value results
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                NULL,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(ppResults));
        }

        if (SUCCEEDED(hr))
        {
            // Get the count of change sensitivity values
            // the client has set.
            hr = pValues->GetCount(&count);
        }

        if (SUCCEEDED(hr))
        {
            // Loop through each key and get each desired change
            // sensitivity
            for (DWORD i = 0; i < count; i++)
            {
                PROPERTYKEY key = WPD_PROPERTY_NULL;
                PROPVARIANT var;

                PropVariantInit( &var );

                hr = pValues->GetAt(i, &key, &var);

                if (SUCCEEDED(hr))
                {
                    HRESULT hrTemp = S_OK;

                    // Confirm the key is one of the supported
                    // sensor data types and the variable type is double
                    if (IsEqualPropertyKey(key, 
                        SENSOR_DATA_TYPE_ACCELERATION_X_G) ||
                        IsEqualPropertyKey(key, 
                        SENSOR_DATA_TYPE_ACCELERATION_Y_G) ||
                        IsEqualPropertyKey(key, 
                        SENSOR_DATA_TYPE_ACCELERATION_Z_G))
                    {
                        if (var.vt == VT_R8)
                        {
                            DOUBLE changeSensitivity;
                            changeSensitivity = var.dblVal;

                            // Validate the change sensitivity value
                            if (changeSensitivity < 0.0)
                            {
                                // x < 0.0 is invalid
                                hrTemp = E_INVALIDARG;
                            }
                            else if ((changeSensitivity > 0) && 
                                (changeSensitivity <
                                    ACCELEROMETER_MIN_CHANGE_SENSITIVITY))
                            {
                                // For 0.0 < x < MIN_CS, round up to MIN_CS
                                var.dblVal = ACCELEROMETER_MIN_CHANGE_SENSITIVITY;
                            }
                            else if (changeSensitivity > 
                                ACCELEROMETER_MAX_CHANGE_SENSITIVITY)
                            {
                                // For x > MAX_CS, round down to MAX_CS
                                var.dblVal = ACCELEROMETER_MAX_CHANGE_SENSITIVITY;
                            }
                        }
                        else if (var.vt == VT_NULL)
                        {
                            // Client sets CS to VT_NULL to clear
                            // desired change sensitivty (i.e. don't care)
                            hrTemp = InitPropVariantFromDouble(
                                CHANGE_SENSITIVITY_NOT_SET, 
                                &var);

                            if (SUCCEEDED(hrTemp))
                            {
                                Trace(
                                    TRACE_LEVEL_INFORMATION,
                                    "Client %p specified VT_NULL for change "
                                    "sensitivity, setting to %fl for 'NOT_SET'",
                                    pClientFile,
                                    var.dblVal);
                            }
                        }
                        else 
                        {
                            hrTemp = E_INVALIDARG;
                        }

                        if (SUCCEEDED(hrTemp))
                        {
                            // Update the client's desired change
                            // sensitivity value
                            if (SUCCEEDED(hrTemp))
                            {
                                hrTemp = entry.
                                    pDesiredSensitivityValues->SetValue(
                                        key,
                                        &var);

                                if (SUCCEEDED(hrTemp))
                                {
                                    _ATLTRY
                                    {
                                        m_pClientList[pClientFile] = entry;
                                    }
                                    _ATLCATCH (e)
                                    {
                                        hrTemp = e;
                                        Trace(
                                            TRACE_LEVEL_ERROR,
                                            "Error updating entry in client "
                                            "list, %!HRESULT!",
                                            hr);
                                    }

                                    if (SUCCEEDED(hrTemp))
                                    {
                                        if (var.vt == VT_R8)
                                        {
                                            Trace(
                                                TRACE_LEVEL_INFORMATION,
                                                "Change sensitivity set to %fl "
                                                "for client %p",
                                                var.dblVal,
                                                pClientFile);
                                        }

                                        (*ppResults)->SetValue(
                                            key, 
                                            &var);
                                    }
                                }
                            }
                        }

                        if (FAILED(hrTemp))
                        {
                            Trace(
                                TRACE_LEVEL_ERROR,
                                "Failed to set the desired change sensitivity "
                                "value, %!HRESULT!",
                                hrTemp);

                            fError = TRUE;
                            (*ppResults)->SetErrorValue(key, hrTemp);
                        }
                    }

                    // Other property key
                    else
                    {
                        Trace(
                            TRACE_LEVEL_ERROR,
                            "Change sensitivity is not supported for the "
                            "specified property key, %!HRESULT!",
                            hrTemp);

                        fError = TRUE;
                        (*ppResults)->SetErrorValue(
                            key, 
                            HRESULT_FROM_WIN32(ERROR_NOT_FOUND));
                    }
                }

                PropVariantClear(&var);
                
                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to get property key and value, %!HRESULT!",
                        hr);

                    break;
                }
            }

            if (SUCCEEDED(hr))
            {
                // Recalculate new minimum properties
                hr = RecalculateProperties();
            }
        }

        if (SUCCEEDED(hr) && (fError == TRUE))
        {
            hr = S_FALSE;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::SetDesiredReportInterval
//
//  This method is used to indicate a client's desired report interval
//  value.
//
//  Parameters:
//      pClientFile - interface pointer to the application's file handle
//      reportInterval - client's desired report interval
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::SetDesiredReportInterval(
    _In_ IWDFFile* pClientFile,
    _In_ ULONG reportInterval
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Validate the report interval value, 0 means use default
        if ((reportInterval != 0) && 
            (reportInterval < ACCELEROMETER_MIN_REPORT_INTERVAL))
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        // Synchronize access to the client list and associated members
        CComCritSecLock<CComAutoCriticalSection> clientLock(m_ClientListCS);

        // Ensure the client is in the client list
        CLIENT_ENTRY entry;

        if (m_pClientList.Lookup(pClientFile, entry) == false)
        {
            // The client isn't connected
            hr = HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);

            Trace(
                TRACE_LEVEL_ERROR,
                "Client %p was not found in the client list, %!HRESULT!",
                pClientFile,
                hr);
        }
        
        if (SUCCEEDED(hr))
        {
            // Save the change sensitivity value
            entry.desiredReportInterval = reportInterval;

            _ATLTRY
            {
                m_pClientList[pClientFile] = entry;
            }
            _ATLCATCH (e)
            {
                hr = e;
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Error updating entry in client list, %!HRESULT!",
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Report interval set to %lu for "
                    "client %p",
                    reportInterval,
                    pClientFile);

                // Recalculate new minimum properties
                hr = RecalculateProperties();
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CClientManager::RecalculateProperties
//
//  This method is used to recalculate the settable properties.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::RecalculateProperties(
    )
{
    FuncEntry();

    // No need for synchronization, calling function should
    // already be holding the client lock

    HRESULT hr = S_OK;

    DOUBLE minChangeSensitivityX = DBL_MAX;
    DOUBLE minChangeSensitivityY = DBL_MAX;
    DOUBLE minChangeSensitivityZ = DBL_MAX;
    ULONG minReportInterval = ULONG_MAX;
    BOOLEAN minReportIntervalSet = FALSE;
    
    POSITION position = NULL;

    // Loop through each client and update the minimum
    // property values as necessary
    _ATLTRY
    {
        position = m_pClientList.GetStartPosition();
    }
    _ATLCATCH (e)
    {
        hr = e;
        Trace(
            TRACE_LEVEL_ERROR,
            "Error getting start of client list, %!HRESULT!",
            hr);
    }

    while (SUCCEEDED(hr) && (position != nullptr))
    {
        CLIENT_ENTRY entry = {0};

        _ATLTRY
        {
            entry = m_pClientList.GetNextValue(position);
        }
        _ATLCATCH (e)
        {
            hr = e;
            Trace(
                TRACE_LEVEL_ERROR,
                "Error getting next entry in client list, %!HRESULT!",
                hr);
        }

        if (SUCCEEDED(hr))
        {
            PROPVARIANT var;
            DOUBLE clientChangeSensitivity;
            ULONG clientReportInterval;
            
            PropVariantInit(&var);

            hr = entry.pDesiredSensitivityValues->
                GetValue(SENSOR_DATA_TYPE_ACCELERATION_X_G, &var);

            if (SUCCEEDED(hr))
            {
                clientChangeSensitivity = var.dblVal;

                // If the client has set a lower change sensitivity,
                // update the minimum
                if ((clientChangeSensitivity != 
                        CHANGE_SENSITIVITY_NOT_SET) &&
                    (clientChangeSensitivity < minChangeSensitivityX))
                {
                    minChangeSensitivityX = clientChangeSensitivity;
                }
            }

            if (SUCCEEDED(hr))
            {
                PropVariantClear(&var);
                
                hr = entry.pDesiredSensitivityValues->
                    GetValue(SENSOR_DATA_TYPE_ACCELERATION_Y_G, &var);

                if (SUCCEEDED(hr))
                {
                    clientChangeSensitivity = var.dblVal;

                    // If the client has set a lower change sensitivity,
                    // update the minimum
                    if ((clientChangeSensitivity != 
                            CHANGE_SENSITIVITY_NOT_SET) &&
                        (clientChangeSensitivity < minChangeSensitivityY))
                    {
                        minChangeSensitivityY = clientChangeSensitivity;
                    }
                }
            }

            if (SUCCEEDED(hr))
            {
                PropVariantClear(&var);
                
                hr = entry.pDesiredSensitivityValues->
                    GetValue(SENSOR_DATA_TYPE_ACCELERATION_Z_G, &var);

                if (SUCCEEDED(hr))
                {
                    clientChangeSensitivity = var.dblVal;

                    // If the client has set a lower change sensitivity,
                    // update the minimum
                    if ((clientChangeSensitivity != 
                            CHANGE_SENSITIVITY_NOT_SET) &&
                        (clientChangeSensitivity < minChangeSensitivityZ))
                    {
                        minChangeSensitivityZ = clientChangeSensitivity;
                    }
                }
            }

            if (SUCCEEDED(hr))
            {
                clientReportInterval = entry.desiredReportInterval;

                if (SUCCEEDED(hr))
                {
                    // If the client has set a lower report interval,
                    // update the minimum
                    if ((clientReportInterval != 
                            CURRENT_REPORT_INTERVAL_NOT_SET) &&
                        (clientReportInterval < minReportInterval))
                    {
                        minReportInterval = clientReportInterval;
                        minReportIntervalSet = TRUE;
                    }
                }
            }
                
            PropVariantClear(&var);
        }
    }

    if (SUCCEEDED(hr))
    {
        // If the property values were not set by subscribed clients,
        // set them to their defaults
        if (minChangeSensitivityX == DBL_MAX)
        {
            minChangeSensitivityX = DEFAULT_ACCELEROMETER_CHANGE_SENSITIVITY;
        }

        if (minChangeSensitivityY == DBL_MAX)
        {
            minChangeSensitivityY = DEFAULT_ACCELEROMETER_CHANGE_SENSITIVITY;
        }

        if (minChangeSensitivityZ == DBL_MAX)
        {
            minChangeSensitivityZ = DEFAULT_ACCELEROMETER_CHANGE_SENSITIVITY;
        }

        if (minReportInterval == ULONG_MAX)
        {
            minReportInterval = DEFAULT_ACCELEROMETER_CURRENT_REPORT_INTERVAL;
        }

        {
            // Synchronize access to the minimum property cache
            CComCritSecLock<CComAutoCriticalSection> propsLock(m_MinPropsCS);

            // Update the minimum property member variables
            PROPVARIANT var;
            PropVariantInit(&var);
            var.vt = VT_R8;
        
            var.dblVal = minChangeSensitivityX;
            hr = m_spMinSensitivityValues->SetValue(
                SENSOR_DATA_TYPE_ACCELERATION_X_G, 
                &var);

            if (SUCCEEDED(hr))
            {
                var.dblVal = minChangeSensitivityY;
                hr = m_spMinSensitivityValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_Y_G, 
                    &var);
            }

            if (SUCCEEDED(hr))
            {
                var.dblVal = minChangeSensitivityZ;
                hr = m_spMinSensitivityValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_Z_G, 
                    &var);
            }
    
            // Set default report interval
            if (SUCCEEDED(hr))
            {
                m_minReportInterval = minReportInterval;
                m_minReportIntervalExplicitlySet = minReportIntervalSet;
            }

            PropVariantClear(&var);
        }
    }

    if (SUCCEEDED(hr))
    {
        Trace(
            TRACE_LEVEL_INFORMATION,
            "Recalculated settable property values:"
            "  X change sensitivity = %fl"
            "  Y change sensitivity = %fl"
            "  Z change sensitivity = %fl"
            "  report interval = %lu",
            minChangeSensitivityX,
            minChangeSensitivityY,
            minChangeSensitivityZ,
            minReportInterval);
    }
    else
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to recalculate minimum property values, %!HRESULT!",
            hr);
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CClientManager::CopyValues
//
//  Copies values from the source list to the target list.
//
//  Parameters: 
//      pSourceValues - an IPortableDeviceValues containing the list
//          of source values
//      pTargetValues - an IPortableDeviceValues to contain the list
//          the copied values
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CClientManager::CopyValues(
    _In_    IPortableDeviceValues* pSourceValues,
    _Inout_ IPortableDeviceValues* pTargetValues
    )
{
    FuncEntry();

    HRESULT hr = S_OK;
    DWORD count = 0;
    PROPERTYKEY key = {0};
    PROPVARIANT var = {0};

    if ((pSourceValues == nullptr) ||
        (pTargetValues == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        hr = pSourceValues->GetCount(&count);
        
        if (SUCCEEDED(hr))
        {
            // Loop through each source key and copy to the
            // destination collection
            for (DWORD i = 0; i < count; i++)
            {
                PropVariantInit(&var);

                hr = pSourceValues->GetAt(i, &key, &var);
                
                if (SUCCEEDED(hr))
                {
                    hr = pTargetValues->SetValue(key, &var);
                }
                
                PropVariantClear(&var);
            }

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to copy values, %!HRESULT!", 
                    hr);
            }
        }
    }

    FuncExit();

    return hr;
}
