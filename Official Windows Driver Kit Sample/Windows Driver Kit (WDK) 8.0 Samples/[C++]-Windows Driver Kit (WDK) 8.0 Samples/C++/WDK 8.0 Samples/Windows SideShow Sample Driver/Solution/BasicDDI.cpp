//-----------------------------------------------------------------------
// <copyright file="BasicDDI.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      BasicDDI.cpp
//
// Description:
//      Implements the CWssBasicDDI class.  This class provides
//      WSS device specific functionality for each of the DDI
//      methods.
//
//-----------------------------------------------------------------------


#include "Common.h"
#include "BasicDDI.h"
#include <WindowsSideShow.h>
#include <intsafe.h>
#include <strsafe.h>
#include <Sddl.h>
#include "Device.h"
#include "DataManager.h"

// These includes allow for driver extensibility
#include "Renderer.h"
#include <PortableDevice.h>
#include <PortableDeviceTypes.h>


extern CDataManager* g_pDataManager;
extern CDevice g_Device;
static const WCHAR REG_NAME_APPORDER[] = L"AppOrder-";


/////////////////////////////////////////////////////////////////////////
//
// CWssBasicDDI()
//
// Constructor.
//
/////////////////////////////////////////////////////////////////////////
CWssBasicDDI::CWssBasicDDI() :
m_pUserSID(NULL),
m_cbUserSID(SECURITY_MAX_SID_SIZE)
{
    // Allocate memory for the user SID
    m_pUserSID = (SID*)LocalAlloc(LMEM_FIXED, m_cbUserSID);
    if (NULL != m_pUserSID)
    {
        ZeroMemory(m_pUserSID, m_cbUserSID);
    }
}


/////////////////////////////////////////////////////////////////////////
//
// ~CWssBasicDDI()
//
// Destructor
//
/////////////////////////////////////////////////////////////////////////
CWssBasicDDI::~CWssBasicDDI()
{
    // Delete the user SID
    if (NULL != m_pUserSID)
    {
        LocalFree(m_pUserSID);
        m_pUserSID = NULL;
    }
    m_cbUserSID = 0;
}


/////////////////////////////////////////////////////////////////////////
//
// Initialize()
//
// Perform initialization here.
//
// Parameters: None
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWssBasicDDI::Initialize(IWDFNamedPropertyStore* pStore)
{
    HRESULT hr = S_OK;

    if (NULL != pStore)
    {
        m_pPropertyStore = pStore;
    }
    else
    {
        hr = E_INVALIDARG;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWssBasicDDI::Deinitialize
//
// Perform deinitialization here
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWssBasicDDI::Deinitialize(void)
{
    HRESULT hr = S_OK;

    APPLICATION_ID* pApps = NULL;
    DWORD           cApps = 0;

    // Store the current gadget order so we use the same one next time
    if (NULL != g_pDataManager)
    {
        hr = g_pDataManager->GetApplications(&pApps, &cApps);
        if (SUCCEEDED(hr))
        {
            hr = SetStoredApplicationOrder(pApps, cApps);

            CoTaskMemFree(pApps);
        }
    }

    m_pPropertyStore.Release();

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWssBasicDDI::GetApplicationOrderString
//
// This helper method returns a string that can be used as a key to
// locate the ApplicationOrder setting in the property store.
//
// Parameters:
//      ppwszKey - pointer to a pointer to string
//
// Return Values:
//      S_OK: successfully generated the string
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWssBasicDDI::GetApplicationOrderString(_Outptr_result_maybenull_ LPWSTR* ppwszKey)
{
    if (NULL == ppwszKey)
    {
        return E_INVALIDARG;
    }

    HRESULT hr      = S_OK;
    LPWSTR  wszSid  = NULL;
    size_t  cchSid  = 0;

    // Initialize out parameter
    *ppwszKey = NULL;

    //
    // Convert the SID to a string, and prepend the AppOrder string
    //
    if (ConvertSidToStringSid(m_pUserSID, &wszSid))
    {
        hr = StringCchLength(wszSid, SECURITY_MAX_SID_SIZE, &cchSid);
        if (SUCCEEDED(hr))
        {
            const size_t cchKeyName = cchSid + sizeof(REG_NAME_APPORDER)/sizeof(WCHAR) + 1;

            *ppwszKey = (LPWSTR)::CoTaskMemAlloc(cchKeyName * sizeof(WCHAR));
            if (NULL == *ppwszKey)
            {
                hr = E_OUTOFMEMORY;
            }
            else
            {
                hr = StringCchCopy(*ppwszKey, cchKeyName, REG_NAME_APPORDER);
                if (SUCCEEDED(hr))
                {
                    hr = StringCchCat(*ppwszKey, cchKeyName, wszSid);
                }
            }
        }
    }
    else
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }

    if (NULL != wszSid)
    {
        LocalFree(wszSid);
    }

    if (FAILED(hr) && (NULL != *ppwszKey))
    {
        ::CoTaskMemFree(*ppwszKey);
        *ppwszKey = NULL;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnSetCurrentUser()
//
// Sets the current user of the device.
//
// Parameters:
//      user [in]
//          A const pointer to a SID that identifies the new current user of the
//          device.  Note that ownership of the SID is NOT passed to the callee.
//
// Return Values:
//      S_OK: The user has been set to the SID passed in, or the SID passed in
//            is the same as the user currently set
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnSetCurrentUser(_In_ const SID* pUser)
{
    HRESULT hr = S_OK;

    if (NULL == pUser)
    {
        return E_POINTER;
    }

    if (NULL == m_pUserSID)
    {
        return E_OUTOFMEMORY;
    }

    // If the SID passed in is the same as the user currently set, then there is no work to do
    if (TRUE == EqualSid(m_pUserSID, const_cast<SID*>(pUser)))
    {
        return S_OK;
    }

    // Set the current user to the SID passed in
    if (TRUE == CopySid((DWORD)m_cbUserSID, m_pUserSID, const_cast<SID*>(pUser)))
    {
        if (NULL != g_pDataManager)
        {
            // A new user has logged on. For this sample, we want to wipe the data off the device
            hr = g_pDataManager->DeleteAllApplications();
            if (SUCCEEDED(hr))
            {
                APPLICATION_ID* pAppIds = NULL;
                DWORD           cAppIds = 0;

                // Get the cached application order from the property store for this user
                hr = GetStoredApplicationOrder(&pAppIds, &cAppIds);
                if (SUCCEEDED(hr))
                {
                    // Set the application order in the data manager
                    HRESULT hrChangeSortOrder = g_pDataManager->ChangeSortOrder(pAppIds, cAppIds);
                    if (FAILED(hrChangeSortOrder))
                    {
                        hr = hrChangeSortOrder;
                    }
                }
                else
                {
                    // Not a failure; it just means we don't have any stored order
                    hr = S_OK;
                }

                if (NULL != pAppIds)
                {
                    ::CoTaskMemFree(pAppIds);
                }
            }
        }
        else
        {
            hr = E_POINTER;
        }
    }
    else
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnGetCurrentUser()
//
// Retrieves the current user of the device.
//
// Parameters:
//      param1 [in]
//          A pointer to SID pointer that upon return will point to the SID of
//          the current user of the device.  Note that the caller will free the
//          SID by calling LocalFree().
//
//          If the device is a "console user model" device (device owned by the
//          person interactively logged on at the console), then this will
//          always return the WinInteractiveSid SID. If the device is an
//          "assigned user model" device (where the device chooses who the owner
//          is), then this should return the SID of the currently owned user.
//
// Return Values:
//      S_OK: On success.
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetCurrentUser(_Out_ SID** ppUser)
{
    HRESULT hr = S_OK;

    if (NULL == ppUser)
    {
        return E_POINTER;
    }

    DWORD cbUser = SECURITY_MAX_SID_SIZE;
    *ppUser = (SID*)LocalAlloc(LMEM_FIXED, cbUser);
    if (NULL != *ppUser)
    {
        if (TRUE == CreateWellKnownSid(WinInteractiveSid, NULL, *ppUser, &cbUser))
        {
            hr = S_OK;
        }
        else
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnSetUserState()
//
// Sets a users' availability status.  This method is called to indicate to a
// device if a user is available.  This information is useful for devices that
// allow users to assign a specific user as the owner of the device.
//
// Parameters:
//      user [in]
//          A const pointer to a SID that identifies the user's who state is
//          changing.
//      state [in]
//          The state of the changing user; possible values include:
//          AVAILABLE -     A user has become available to the platform, such as
//                          from a logon.
//          UNAVAILABLE -   A user has become unavailable to the platform, such
//                          as from a logoff.
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnSetUserState(_In_ const SID* pUser,
                                                _In_ const USER_STATE state)
{
    (pUser);
    (state);
    HRESULT hr = S_OK;

    return hr;
}


__override HRESULT CWssBasicDDI::OnSetTime(_In_ const FILETIME time)
{
    (time);

    return S_OK;
}


__override HRESULT CWssBasicDDI::OnSetTimeZone(_In_ const SIDESHOW_TIME_ZONE_INFORMATION* pTimeZoneInformation)
{
    (pTimeZoneInformation);

    return S_OK;
}


__override HRESULT CWssBasicDDI::OnSetShortDateFormat(_In_ const SID*  user,
                                                      _In_ LPCWSTR wszDateFormat)
{
    (user);
    (wszDateFormat);

    return S_OK;
}


__override HRESULT CWssBasicDDI::OnSetLongDateFormat(_In_ const SID*  user,
                                                     _In_ LPCWSTR wszDateFormat)
{
    (user);
    (wszDateFormat);

    return S_OK;
}


__override HRESULT CWssBasicDDI::OnSetShortTimeFormat(_In_ const SID* user,
                                                      _In_ LPCWSTR wszTimeFormat)
{
    (user);
    (wszTimeFormat);

    return S_OK;
}


__override HRESULT CWssBasicDDI::OnSetLongTimeFormat(_In_ const SID* user,
                                                     _In_ LPCWSTR wszTimeFormat)
{
    (user);
    (wszTimeFormat);

    return S_OK;
}


/////////////////////////////////////////////////////////////////////////
//
// OnGetDeviceFirmwareVersion()
//
// Retrieves the firmware version of the firmware running on the device.
//
// Parameters:
//      pwszVersion [in]
//          A pointer to a LPWSTR that upon return will point to a string that
//          indicates the firmware version running on the device.  This string
//          will be freed by the caller with a call to CoTaskMemFree().
// Return Values:
//      S_OK: On success.
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetDeviceFirmwareVersion(_Outptr_result_maybenull_ LPWSTR* pwszVersion)
{
    if (NULL == pwszVersion)
    {
        return E_POINTER;
    }

    //
    // These values are hard coded here (string is set in DeviceSpecific.cpp).
    // Alternatively, they could be retrieved from the device.
    //
    size_t SizeOfDeviceFirmwareVersion = (wcslen(CDevice::GetDeviceFirmwareVersion()) + 1) * sizeof(wchar_t);
    *pwszVersion = (LPWSTR)CoTaskMemAlloc(SizeOfDeviceFirmwareVersion);
    if (NULL != *pwszVersion)
    {
        memcpy(*pwszVersion, CDevice::GetDeviceFirmwareVersion(), SizeOfDeviceFirmwareVersion);
    }
    else
    {
        return E_OUTOFMEMORY;
    }

    return S_OK;
}


/////////////////////////////////////////////////////////////////////////
//
// CWssBasicDDI::OnGetDeviceManufacturer
//
// Retrieves the device manufacturer string from the device
//
// Parameters:
//      pwszManufacturer - pointer to a LPWSTR that upon return will
//      point to a string that indicates the manufacturer of the device.
//      The string will be freed by the caller with a call to CoTaskMemFree()
//
// Return Values:
//      S_OK: successfully returned a string
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetDeviceManufacturer(_Outptr_result_maybenull_ LPWSTR* pwszManufacturer)
{
    if (NULL == pwszManufacturer)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    //
    // These values are hard coded here (string is set in DeviceSpecific.cpp).
    // Alternatively, they could be retrieved from the device.
    //
    size_t SizeOfDeviceManufacturer = (wcslen(CDevice::GetDeviceManufacturer()) + 1) * sizeof(wchar_t);
    *pwszManufacturer = (LPWSTR)CoTaskMemAlloc(SizeOfDeviceManufacturer);
    if (NULL != *pwszManufacturer)
    {
        memcpy(*pwszManufacturer, CDevice::GetDeviceManufacturer(), SizeOfDeviceManufacturer);
    }
    else
    {
        return E_OUTOFMEMORY;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWssBasicDDI::OnGetDeviceName
//
// Retrieves the device name string from the device.
//
// Parameters:
//      pwszName - pointer to a LPWSTR that upon return will point to
//      a string that indicates the name of the device.  The string will
//      be freed by the caller with a call to CoTaskMemFree().
//
// Return Values:
//      S_OK: successfully returned a string
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetDeviceName(_Outptr_result_maybenull_ LPWSTR* pwszName)
{
    if (NULL == pwszName)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    //
    // These values are hard coded here (string is set in DeviceSpecific.cpp).
    // Alternatively, they could be retrieved from the device.
    //
    size_t SizeOfDeviceFriendlyName = (wcslen(CDevice::GetDeviceFriendlyName()) + 1) * sizeof(wchar_t);
    *pwszName = (LPWSTR)CoTaskMemAlloc(SizeOfDeviceFriendlyName);
    if (NULL != *pwszName)
    {
        memcpy(*pwszName, CDevice::GetDeviceFriendlyName(), SizeOfDeviceFriendlyName);
    }
    else
    {
        return E_OUTOFMEMORY;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnGetDeviceEndpoints()
//
// Retrieves the Endpoints that are available on the device.
//
// Parameters:
//      rgEndpoints [out]
//          A pointer to an array of ENDPOINT_IDs that upon return will contain
//          the available endpoints on the device.  Ownership of the array is
//          passed to the caller, who should free the array by calling
//          CoTaskMemFree().
//      pcEndpoints [out]
//          A pointer to a DWORD that upon return will contain the count of
//          endpoint IDs in the array of endpoint, rgEndpoints.
//
// Return Values:
//      S_OK: Success
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetDeviceEndpoints(_Outptr_result_maybenull_ ENDPOINT_ID** rgEndpoints,
                                                      _Out_ DWORD* pcEndpoints)
{
    if ((NULL == rgEndpoints) || (NULL == pcEndpoints))
    {
        return E_POINTER;
    }

    *rgEndpoints = (ENDPOINT_ID*)CoTaskMemAlloc(sizeof(ENDPOINT_ID));
    if (NULL != *rgEndpoints)
    {
        memcpy(*rgEndpoints, &SIDESHOW_ENDPOINT_SIMPLE_CONTENT_FORMAT, sizeof(SIDESHOW_ENDPOINT_SIMPLE_CONTENT_FORMAT));
        *pcEndpoints = 1;
    }
    else
    {
        *pcEndpoints = 0;
        return E_OUTOFMEMORY;
    }
    
    return S_OK;
}


/////////////////////////////////////////////////////////////////////////
//
// OnGetDeviceCapabilities()
//
// This method, given a capability descriptor, will return a value for that
// capability indicating the existence and extent of the capability.
//
// Parameters:
//      pKey : [in]
//        A pointer to a PROPERTYKEY the describes the capability to querry.
//      pvValue : [out]
//        A pointer to a PROPVARIANT that upon return will contain the value for
//        the capability specified by the pguidCatagory + pid.
//
// Return Values:
//        S_OK : Success
//        S_FALSE : The specified capability is not supported by the device.
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetDeviceCapabilities(_In_ const PROPERTYKEY* pKey,
                                                         _Out_ PROPVARIANT* pvValue)
{
    if ((NULL == pKey) ||
        (NULL == pvValue))
    {
        return E_POINTER;
    }

    // This sample driver is hard-coded to not respond to any requests for device
    // capabilities. This should be changed to support well-known device capabilities
    // and any optional proprietary ones.
    PropVariantInit(pvValue);

    return S_OK;
}


/////////////////////////////////////////////////////////////////////////
//
// OnGetApplicationOrder()
//
// Retrieves the order that applications should be listed in by the CPL.
//
// Parameters:
//      ppAppIds [out]
//          A pointer to an array of APPLICATION_IDs that upon return will
//          contain the application IDs in the order that they should be
//          presented in the WSS CPL.
//      pcAppIds [out]
//          A pointer to a DWORD that upon return will contain the count of
//          APPLICATION_ID contained in the array pointed to by ppAppIds.
//
// Return Values:
//      S_OK: Success
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetApplicationOrder(_Outptr_result_maybenull_ APPLICATION_ID** ppAppIds,
                                                       _Out_ DWORD* pcAppIds)
{
    if ((NULL == m_pUserSID) ||
        (0 == IsValidSid(m_pUserSID)) ||
        (NULL == m_pPropertyStore) ||
        (NULL == g_pDataManager))
    {
        return E_UNEXPECTED;
    }

    //
    // Get the gadget order from the DataManager
    //
    return g_pDataManager->GetApplications(ppAppIds, pcAppIds);
}


/////////////////////////////////////////////////////////////////////////
//
// GetStoredApplicationOrder()
//
// Retrieves the gadget order from the property store.  The memory must
// be CoTaskMemFree'd by the caller.
//
// Parameters:
//      ppAppIds [out]
//          A pointer to an array of APPLICATION_IDs that upon return will
//          contain the application IDs in the order that they should be
//          presented on the device
//      pcAppIds [out]
//          A pointer to a DWORD that upon return will contain the count of
//          APPLICATION_ID contained in the array pointed to by ppAppIds.
//
// Return Values:
//      S_OK: Success
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWssBasicDDI::GetStoredApplicationOrder(APPLICATION_ID** ppAppIds,
                                                DWORD* pcAppIds)
{
    if ((NULL == m_pUserSID) ||
        (0 == IsValidSid(m_pUserSID)) ||
        (NULL == m_pPropertyStore))
    {
        return E_UNEXPECTED;
    }

    HRESULT hr = S_OK;
    LPTSTR wszKey = NULL;

    // Initialize out parameters
    *ppAppIds = NULL;
    *pcAppIds = 0;

    //
    // Get the key to use for the property store
    //
    hr = GetApplicationOrderString(&wszKey);

    //
    // Put the data into a PROPVARIANT, and store it in the property store
    //
    if ((SUCCEEDED(hr)) && (NULL != wszKey))
    {
        PROPVARIANT pvBlob = {0};

        PropVariantInit(&pvBlob);

        hr = m_pPropertyStore->GetNamedValue(wszKey, &pvBlob);
        if (SUCCEEDED(hr) &&
            (VT_BLOB == pvBlob.vt) &&
            (0 == (pvBlob.blob.cbSize % sizeof(APPLICATION_ID))))
        {
            *pcAppIds = pvBlob.blob.cbSize / sizeof(APPLICATION_ID);
            *ppAppIds = (APPLICATION_ID*)pvBlob.blob.pBlobData;
        }

        // Don't clear the PROPVARIANT because we don't want to erase the memory that we pass out of this method
    }

    if (NULL != wszKey)
    {
        ::CoTaskMemFree(wszKey);
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// SetStoredApplicationOrder()
//
// Stores the application order in the property store for the device
//
// Parameters:
//      pApps [in]
//          An array of APPLICATION_IDs.
//      cApps [out]
//          A DWORD that is the count of application IDs in the array, pApps.
//
// Return Values:
//      S_OK: Success
//      E_INVALIDARG: cApps was == 0 or > 1024
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWssBasicDDI::SetStoredApplicationOrder(const APPLICATION_ID* pApps,
                                                const DWORD cApps)
{
    if ((NULL == m_pUserSID) ||
        (0 == IsValidSid(m_pUserSID)) ||
        (NULL == m_pPropertyStore))
    {
        return E_UNEXPECTED;
    }

    HRESULT hr = S_OK;
    LPTSTR wszKey = NULL;

    //
    // Get the key to use for the property store
    //
    hr = GetApplicationOrderString(&wszKey);

    //
    // Put the data into a PROPVARIANT, and store it in the property store
    //
    if ((SUCCEEDED(hr)) && (NULL != wszKey))
    {
        PROPVARIANT pvBlob = {0};

        PropVariantInit(&pvBlob);

        pvBlob.vt = VT_BLOB;
        pvBlob.blob.cbSize = cApps * sizeof(APPLICATION_ID);
        pvBlob.blob.pBlobData = (BYTE*)pApps;

        hr = m_pPropertyStore->SetNamedValue(wszKey, &pvBlob);

        // Don't clear the PROPVARIANT because we don't want to erase the memory that we don't own.
    }

    if (NULL != wszKey)
    {
        ::CoTaskMemFree(wszKey);
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnSetApplicationOrder()
//
// Sends an array of application IDs to the device that is the order in that
// they are presented in the WSS CPL.
//
// Parameters:
//      pApps [in]
//          An array of APPLICATION_IDs.
//      cApps [out]
//          A DWORD that is the count of application IDs in the array, pApps.
//
// Return Values:
//      S_OK: Success
//      E_INVALIDARG: cApps was == 0 or > 1024
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnSetApplicationOrder(_In_reads_(cApps) const APPLICATION_ID* pApps,
                                                       _In_ const DWORD cApps)
{
    if (NULL == g_pDataManager)
    {
        return E_UNEXPECTED;
    }

    HRESULT hr = S_OK;

    // Store the order in the property store
    hr = SetStoredApplicationOrder(pApps, cApps);
    if (SUCCEEDED(hr))
    {
        // Ensure the data manager updates to reflect the new order
        hr = g_pDataManager->ChangeSortOrder(pApps, cApps);
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnGetPreEnabledApplications()
//
// Retrieves the list of gadgets that should be pre-enabled for all users.
//
// Parameters:
//      ppAppIds [out]
//          A pointer to an array of APPLICATION_IDs that upon return will
//          contain the application IDs that should be pre-enabled for this
//          device.  The corresponding endpoint is at the same index in
//          the endpoint array
//      ppEndpointIds [out]
//          A pointer to an array of ENDPOINT_IDs that upon return will
//          contain the endpoint IDs that should be pre-enabled for this
//          device.  The corresponding application is at the same index in
//          the application array
//      pcAppIds [out]
//          A pointer to a DWORD that upon return will contain the count of
//          APPLICATION_ID/ENDPOINT_ID pairs contained in the arrays pointed
//          to by ppAppIds and ppEndpointIds.
//
// Return Values:
//      S_OK: Success
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnGetPreEnabledApplications(_Outptr_result_buffer_maybenull_(*pcApplications) APPLICATION_ID** ppAppIds,
                                                             _Outptr_result_buffer_maybenull_(*pcApplications) ENDPOINT_ID** ppEndpointIds,
                                                             _Out_ DWORD* pcApplications)
{
    if ((NULL == ppAppIds) ||
        (NULL == ppEndpointIds) ||
        (NULL == pcApplications))
    {
        return E_POINTER;
    }

    // This sample driver hard-codes 0 pre-enabled applications (gadgets). You could alternatively ask
    // a device for its list of pre-enabled applications by retrieving a set of AppIds/EndpointIds.
    // You could also set properties in the inf file for pre-enabled applications which could be read here.
    *pcApplications = 0;
    *ppAppIds = NULL;
    *ppEndpointIds = NULL;

    // Sample code for pre-enabled applications (gadgets)
    // The following commented out code uses hard-coded AppIds.

    //const DWORD dwNumberOfPreEnabledGadgets = 1;

    //*ppAppIds = (APPLICATION_ID*)CoTaskMemAlloc(sizeof(APPLICATION_ID) * dwNumberOfPreEnabledGadgets);
    //if (NULL == *ppAppIds)
    //{
        //return E_OUTOFMEMORY;
    //}

    //*ppEndpointIds = (ENDPOINT_ID*)CoTaskMemAlloc(sizeof(ENDPOINT_ID) * dwNumberOfPreEnabledGadgets);
    //if (NULL == *ppEndpointIds)
    //{
        //CoTaskMemFree(*ppAppIds);
        //*ppAppIds = NULL;
        //return E_OUTOFMEMORY;
    //}

    //GUID fakeAppIdGuid = {0x11111111, 0x2222, 0x3333, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB};
    //(*ppAppIds)[0] = fakeAppIdGuid;
    //(*ppEndpointIds)[0] = SIDESHOW_ENDPOINT_SIMPLE_CONTENT_FORMAT;
    //*pcApplications = dwNumberOfPreEnabledGadgets;

    return S_OK;
}


/////////////////////////////////////////////////////////////////////////
//
// OnAddApplication()
//
// This method constructs a packet who's purpose is to register an application
// with the device, and send that packet to the device.  Once this method has
// been called, it may be followed by calls to OnAddContent(), where the appID
// is the AppID specified in DoAddApp().
//
// Parameters:
//      pAppId : [in]
//        A const pointer to a GUID that is the application ID that will be
//        registered with the device and used for subsequent calls to
//        OnAddContent().
//      pEndPoint : [in]
//        A const pointer to a GUID that is the renderer/endpoint ID that the
//        registered app is to be associated with.
//      wszName : [in]
//        A LPCWSTR that is the name of the application that the device may use
//        for display purposes.
//      policy : [in]
//        A DWORD that indicates the cache policy to be used by the device for
//        the app.  policy will be a value that may have the following bit flags
//        OR'ed together:
//              KeepOldest
//              KeepFrequentlyAccessed
//              KeepRecentlyAccessed
//
//
// Return Values:
//        S_OK : Success
//        E_POINTER : wszName was NULL
//        E_OUTOFMEMORY : Couldn't allocate the necessary memory
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnAddApplication(_In_ const SID* user,
                                                  _In_ REFAPPLICATION_ID rAppId,
                                                  _In_ REFENDPOINT_ID rEndPoint,
                                                  _In_ LPCWSTR wszName,
                                                  _In_ const DWORD dwCachePolicy,
                                                  _In_ const DWORD dwOnlineOnly,
                                                  _In_reads_bytes_opt_(cbLargeIcon) const unsigned char* pbLargeIcon,
                                                  _In_ const DWORD cbLargeIcon,
                                                  _In_reads_bytes_opt_(cbSmallIcon) const unsigned char* pbSmallIcon,
                                                  _In_ const DWORD cbSmallIcon,
                                                  _In_reads_bytes_opt_(cbMiniIcon) const unsigned char* pbMiniIcon,
                                                  _In_ const DWORD cbMiniIcon)
{
    (user);
    (rEndPoint);
    (pbSmallIcon);
    (cbSmallIcon);
    (pbMiniIcon);
    (cbMiniIcon);
    (dwCachePolicy);
    (dwOnlineOnly);

    HRESULT hr = S_OK;

    if (NULL == wszName)
    {
        return E_POINTER;
    }

    const size_t SizeOfNameCopy = wcslen(wszName) + 1;
    wchar_t* wszNameCopy = new(std::nothrow) wchar_t[SizeOfNameCopy];
    if (NULL != wszNameCopy)
    {
        wcscpy_s(wszNameCopy, SizeOfNameCopy, wszName);
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }

    unsigned char* pbLargeIconCopy = NULL;
    size_t SizeOfLargeIconCopy = 0;
    if (SUCCEEDED(hr))
    {
        if (NULL != pbLargeIcon)
        {
            SizeOfLargeIconCopy = cbLargeIcon;
            pbLargeIconCopy = new(std::nothrow) unsigned char[SizeOfLargeIconCopy];
            if (NULL != pbLargeIconCopy)
            {
                memcpy(pbLargeIconCopy, pbLargeIcon, SizeOfLargeIconCopy);
            }
            else
            {
                hr = E_OUTOFMEMORY;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        if (NULL != g_pDataManager)
        {
            hr = g_pDataManager->AddApplication(rAppId,
                                                wszNameCopy,
                                                SizeOfNameCopy,
                                                (HICON*)pbLargeIconCopy,
                                                SizeOfLargeIconCopy);
        }
        else
        {
            hr = E_POINTER;
        }
    }

    // Cleanup if there's an error
    if (FAILED(hr))
    {
        if (NULL != wszNameCopy)
        {
            delete [] wszNameCopy;
            wszNameCopy = NULL;
        }

        if (NULL != pbLargeIconCopy)
        {
            delete [] pbLargeIconCopy;
            pbLargeIconCopy = NULL;
        }
    }

    return hr;
}


__override HRESULT CWssBasicDDI::OnRemoveApplication(_In_ const SID* user,
                                                     _In_ REFAPPLICATION_ID rAppId)
{
    (user);

    HRESULT hr = E_FAIL;
    if (NULL != g_pDataManager)
    {
        hr = g_pDataManager->DeleteApplication(rAppId);
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}


__override HRESULT CWssBasicDDI::OnRemoveAllApplications(_In_ const SID* user)
{
    (user);

    HRESULT hr = E_FAIL;
    if (NULL != g_pDataManager)
    {
        hr = g_pDataManager->DeleteAllApplications();
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}


__override HRESULT CWssBasicDDI::OnSetNotificationsEnabled(_In_ const SID* user,
                                                           _In_ const BOOL fIsEnabled)
{
    (user);
    (fIsEnabled);

    return S_OK;
}


// Return Values:
//        S_OK : Success
//        E_POINTER : wszTitle or wszMessage was NULL
//        E_OUTOFMEMORY : Couldn't allocate the necessary memory
__override HRESULT CWssBasicDDI::OnAddNotification(_In_ const SID* user,
                                                   _In_ REFAPPLICATION_ID rAppId,
                                                   _In_ const NOTIFICATION_ID notificationId,
                                                   _In_ const FILETIME ftExpiration,
                                                   _In_ LPCWSTR wszTitle,
                                                   _In_ LPCWSTR wszMessage,
                                                   _In_reads_bytes_opt_(cbImage) const unsigned char* pbImage,
                                                   _In_ const DWORD cbImage)
{
    (user);

    HRESULT hr = S_OK;

    if ((NULL == wszTitle) || (NULL == wszMessage))
    {
        return E_POINTER;
    }

    const size_t SizeOfTitleCopy = wcslen(wszTitle) + 1;
    wchar_t* wszTitleCopy = new(std::nothrow) wchar_t[SizeOfTitleCopy];
    if (NULL != wszTitleCopy)
    {
        wcscpy_s(wszTitleCopy, SizeOfTitleCopy, wszTitle);
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }

    wchar_t* wszMessageCopy = NULL;
    size_t SizeOfMessageCopy = 0;
    if (SUCCEEDED(hr))
    {
        SizeOfMessageCopy = wcslen(wszMessage) + 1;
        wszMessageCopy = new(std::nothrow) wchar_t[SizeOfMessageCopy];
        if (NULL != wszMessageCopy)
        {
            wcscpy_s(wszMessageCopy, SizeOfMessageCopy, wszMessage);
        }
        else
        {
            hr = E_OUTOFMEMORY;
        }
    }

    unsigned char* pbImageCopy = NULL;
    size_t SizeOfImageCopy = 0;
    if (SUCCEEDED(hr))
    {
        if (NULL != pbImage)
        {
            SizeOfImageCopy = cbImage;
            pbImageCopy = new(std::nothrow) unsigned char[SizeOfImageCopy];
            if (NULL != pbImageCopy)
            {
                memcpy(pbImageCopy, pbImage, SizeOfImageCopy);
            }
            else
            {
                hr = E_OUTOFMEMORY;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        if (NULL != g_pDataManager)
        {
            hr = g_pDataManager->AddNotification(rAppId,
                                                 notificationId,
                                                 wszTitleCopy,
                                                 SizeOfTitleCopy,
                                                 wszMessageCopy,
                                                 SizeOfMessageCopy,
                                                 (HICON*)pbImageCopy,
                                                 SizeOfImageCopy,
                                                 ftExpiration);
        }
        else
        {
            hr = E_POINTER;
        }
    }

    // Cleanup if there's an error
    if (FAILED(hr))
    {
        if (NULL != wszTitleCopy)
        {
            delete [] wszTitleCopy;
            wszTitleCopy = NULL;
        }

        if (NULL != wszMessageCopy)
        {
            delete [] wszMessageCopy;
            wszMessageCopy = NULL;
        }

        if (NULL != pbImageCopy)
        {
            delete [] pbImageCopy;
            pbImageCopy = NULL;
        }
    }

    return hr;
}


__override HRESULT CWssBasicDDI::OnRemoveNotification(_In_ const SID* user,
                                                      _In_ REFAPPLICATION_ID rAppId,
                                                      _In_ const NOTIFICATION_ID notificationId)
{
    (user);
    HRESULT hr = E_FAIL;
    if (NULL != g_pDataManager)
    {
        hr = g_pDataManager->DeleteNotification(rAppId,
                                                notificationId);
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}


__override HRESULT CWssBasicDDI::OnRemoveAllNotifications(_In_ const SID* user,
                                                          _In_ REFAPPLICATION_ID rAppId)
{
    (user);

    HRESULT hr = g_pDataManager->DeleteAllNotificationsForAnApplication(rAppId);

    return hr;
}


__override HRESULT CWssBasicDDI::OnAddContent(_In_ const SID* user,
                                              _In_ REFAPPLICATION_ID rAppId,
                                              _In_ REFENDPOINT_ID rRendererId,
                                              _In_ const CONTENT_ID contentId,
                                              _In_reads_bytes_opt_(cbData) const unsigned char* pData, // This is UTF8 data
                                              _In_ const DWORD cbData)
{
    (user);
    (rRendererId);

    HRESULT hr = S_OK;

    //
    // We are only interested in glance text, which is identified by content
    // id == 0.
    //
    if ((0 == contentId) && (NULL != pData))
    {
        // Get number of elements in pData
        int nSizeOfDataCopy = MultiByteToWideChar(CP_UTF8, // Code page
                                                  0, // Character-type options
                                                  (LPCSTR)pData, // String to map
                                                  cbData, // Number of bytes in string
                                                  NULL,
                                                  0); // Only return the buffer size (in wide characters) required

        wchar_t* wszDataCopy = NULL;
        if (0 < nSizeOfDataCopy)
        {
            wszDataCopy = new(std::nothrow) wchar_t[nSizeOfDataCopy];
            if (NULL != wszDataCopy)
            {
                if (0 == MultiByteToWideChar(CP_UTF8, // Code page
                                             0, // Character-type options
                                             (LPCSTR)pData, // String to map
                                             cbData, // Number of bytes in string
                                             wszDataCopy, // Wide-character buffer
                                             nSizeOfDataCopy)) // Size of buffer
                {
                    // Error block
                    delete [] wszDataCopy;
                    nSizeOfDataCopy = 1;
                    wszDataCopy = new(std::nothrow) wchar_t[nSizeOfDataCopy];
                    if (NULL == wszDataCopy)
                    {
                        hr = E_OUTOFMEMORY;
                    }
                }
            }
            else
            {
                hr = E_OUTOFMEMORY;
            }
        }
        else
        {
            nSizeOfDataCopy = 1;
            wszDataCopy = new(std::nothrow) wchar_t[nSizeOfDataCopy];
            if (NULL == wszDataCopy)
            {
                hr = E_OUTOFMEMORY;
            }
        }

        if (SUCCEEDED(hr))
        {
            wszDataCopy[nSizeOfDataCopy - 1] = L'\0'; // Safety just in case pData isn't properly NULL terminated
            if (NULL != g_pDataManager)
            {
                hr = g_pDataManager->UpdateApplicationNode(rAppId,
                                                           wszDataCopy,
                                                           nSizeOfDataCopy);
            }
            else
            {
                hr = E_POINTER;
            }
        }

        // Cleanup if there's an error
        if (FAILED(hr) && (NULL != wszDataCopy))
        {
            delete [] wszDataCopy;
            wszDataCopy = NULL;
        }
    }

    return hr;
}


__override HRESULT CWssBasicDDI::OnRemoveContent(_In_ const SID* user,
                                                 _In_ REFAPPLICATION_ID rAppId,
                                                 _In_ REFENDPOINT_ID rRendererId,
                                                 _In_ const CONTENT_ID contentId)
{
    (user);
    (rRendererId);
    (rAppId);
    (contentId);

    // Optionally, this could be implemented for content ID 0

    return S_OK;
}


__override HRESULT CWssBasicDDI::OnRemoveAllContent(_In_ const SID* user,
                                                    _In_ REFAPPLICATION_ID rAppId,
                                                    _In_ REFENDPOINT_ID rRendererId)
{
    (user);
    (rAppId);
    (rRendererId);

    return S_OK;
}


// This method is one of two ways to support extensibility in the driver
// This one allows custom WPD commands to be sent to the driver and processed
__override HRESULT CWssBasicDDI::OnProcessWpdMessage(_In_ IUnknown* pinValues,
                                                     _In_ IUnknown* poutValues)
{
    (poutValues);

    IPortableDeviceValues* pPortableDeviceValuesParams = NULL;
    // Also define pPortableDeviceValuesResults if you intend to send results information
    //IPortableDeviceValues* pPortableDeviceValuesResults = NULL;

    HRESULT hr = pinValues->QueryInterface(IID_IPortableDeviceValues, (VOID**)&pPortableDeviceValuesParams);
    if (FAILED(hr))
    {
        return hr;
    }
    if (NULL == pPortableDeviceValuesParams)
    {
        return E_POINTER;
    }

    // Check if the incoming message is a custom command we intend to act on
    PROPERTYKEY propertyKey = {0};
    hr = pPortableDeviceValuesParams->GetGuidValue(WPD_PROPERTY_COMMON_COMMAND_CATEGORY, &propertyKey.fmtid);
    if (S_OK == hr)
    {
        hr = pPortableDeviceValuesParams->GetUnsignedIntegerValue(WPD_PROPERTY_COMMON_COMMAND_ID, &propertyKey.pid);
    }
    if (S_OK == hr)
    {
        // The next lines check if the custom command is something this driver cares about
        // and acts on it if necessary
        if (IsEqualPropertyKey(CUSTOM_COMMAND_INVERT_COLORS, propertyKey))
        {
            DWORD dwInvertColors = 0;
            hr = pPortableDeviceValuesParams->GetUnsignedIntegerValue(CUSTOM_COMMAND_INVERT_COLORS_SETVALUE, &dwInvertColors);
            if (S_OK == hr)
            {
                // Change whether the renderer should invert the colors
                CRendererBase::SetInvertColors((dwInvertColors) ? true : false);
                g_Device.HandleDeviceEvent(CDevice::RenderAgain);
            }
        }
        else if (IsEqualPropertyKey(CUSTOM_COMMAND_CHANGE_SOMETHING, propertyKey))
        {
            DWORD dwSomething = 0;
            hr = pPortableDeviceValuesParams->GetUnsignedIntegerValue(CUSTOM_COMMAND_CHANGE_SOMETHING_SETVALUE, &dwSomething);
            if (S_OK == hr)
            {
                // Change Something Here
                // If needed, render the page again: g_Device.HandleDeviceEvent(CDevice::RenderAgain);
            }
        }
    }

    pPortableDeviceValuesParams->Release();
    pPortableDeviceValuesParams = NULL;

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// OnSetLanguage()
//
// Sets the current language on the device.
//
// Parameters:
//      wszLang [in]
//          A string that indicates the current language.
//
// Return Values:
//      S_OK: Success
//
/////////////////////////////////////////////////////////////////////////
__override HRESULT CWssBasicDDI::OnSetLanguage(_In_ const SID*  user,
                                               _In_ LPCWSTR wszLang)
{
    (user);
    (wszLang);

    return S_OK;
}
