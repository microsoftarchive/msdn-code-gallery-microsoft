//-----------------------------------------------------------------------
// <copyright file="BasicDDI.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      BasicDDI.h
//
// Description:
//      This module implements the ISideShowDriver DDI which is used
//      by the Windows SideShow class extension.
//
//-----------------------------------------------------------------------

#pragma once

#include <WindowsSideShowClassExtension.h>

class CWssBasicDDI :
    public CComObjectRoot,
    public ISideShowDriver
{
public:
    CWssBasicDDI();
    virtual ~CWssBasicDDI();

    HRESULT Initialize(IWDFNamedPropertyStore* pStore);

    HRESULT Deinitialize(void);

    DECLARE_NOT_AGGREGATABLE(CWssBasicDDI)

    BEGIN_COM_MAP(CWssBasicDDI)
        COM_INTERFACE_ENTRY(ISideShowDriver)
    END_COM_MAP()

    //
    // Overriden DDI methods
    //
public:
    __override STDMETHOD(OnSetCurrentUser)(_In_ const SID* pUser);

    __override STDMETHOD(OnGetCurrentUser)(_Out_ SID** ppUser);

    __override STDMETHOD(OnSetUserState)(_In_ const SID* pUser,
                                         _In_ const USER_STATE state);

    __override STDMETHOD(OnSetTime)(_In_ const FILETIME time);

    __override STDMETHOD(OnSetTimeZone)(_In_ const SIDESHOW_TIME_ZONE_INFORMATION* pTimeZoneInformation);

    __override STDMETHOD(OnSetShortDateFormat)(_In_ const SID* user,
                                               _In_ LPCWSTR wszDateFormat);

    __override STDMETHOD(OnSetLongDateFormat)(_In_ const SID* user,
                                              _In_ LPCWSTR wszDateFormat);

    __override STDMETHOD(OnSetShortTimeFormat)(_In_ const SID* user,
                                               _In_ LPCWSTR wszTimeFormat);

    __override STDMETHOD(OnSetLongTimeFormat)(_In_ const SID* user,
                                              _In_ LPCWSTR wszTimeFormat);

    __override STDMETHOD(OnGetDeviceFirmwareVersion)(_Outptr_result_maybenull_ LPWSTR* pwszVersion);

    __override STDMETHOD(OnGetDeviceManufacturer)(_Outptr_result_maybenull_ LPWSTR* pwszManufacturer);

    __override STDMETHOD(OnGetDeviceName)(_Outptr_result_maybenull_ LPWSTR* pwszName);

    __override STDMETHOD(OnGetDeviceEndpoints)(_Outptr_result_maybenull_ ENDPOINT_ID** rgEndpoints,
                                               _Out_ DWORD* pcEndpoints);

    __override STDMETHOD(OnGetDeviceCapabilities)(_In_ const PROPERTYKEY* pKey,
                                                  _Out_ PROPVARIANT* pvValue);

    __override STDMETHOD(OnGetPreEnabledApplications)(_Outptr_result_buffer_maybenull_(*pcApplications) APPLICATION_ID** ppguidApps,
                                                      _Outptr_result_buffer_maybenull_(*pcApplications) ENDPOINT_ID** ppEndpoints,
                                                      _Out_ DWORD* pcApplications);

    __override STDMETHOD(OnGetApplicationOrder)(_Outptr_result_maybenull_ APPLICATION_ID** ppAppIds,
                                                _Out_ DWORD* pcAppIds);

    __override STDMETHOD(OnSetApplicationOrder)(_In_reads_(cApps) const APPLICATION_ID* pApps,
                                                _In_ const DWORD cApps);

    __override STDMETHOD(OnAddApplication)(_In_ const SID* user,
                                           _In_ REFAPPLICATION_ID rAppId,
                                           _In_ REFENDPOINT_ID rEndPoint,
                                           _In_ LPCWSTR wszName,
                                           _In_ const DWORD cachePolicy,
                                           _In_ const DWORD dwOnlineOnly,
                                           _In_reads_bytes_opt_(cbLargeIcon) const unsigned char* pbLargeIcon,
                                           _In_ const DWORD cbLargeIcon,
                                           _In_reads_bytes_opt_(cbSmallIcon) const unsigned char* pbSmallIcon,
                                           _In_ const DWORD cbSmallIcon,
                                           _In_reads_bytes_opt_(cbMiniIcon) const unsigned char* pbMiniIcon,
                                           _In_ const DWORD cbMiniIcon);

    __override STDMETHOD(OnRemoveApplication)(_In_ const SID* user,
                                              _In_ REFAPPLICATION_ID rAppId);

    __override STDMETHOD(OnRemoveAllApplications)(_In_ const SID* user);

    __override STDMETHOD(OnSetNotificationsEnabled)(_In_ const SID* pUserSid,
                                                    _In_ const BOOL fIsEnabled);

    __override STDMETHOD(OnAddNotification)(_In_ const SID* user,
                                            _In_ REFAPPLICATION_ID rAppId,
                                            _In_ const NOTIFICATION_ID notificationId,
                                            _In_ const FILETIME ftExpiration,
                                            _In_ LPCWSTR wszTitle,
                                            _In_ LPCWSTR wszMessage,
                                            _In_reads_bytes_opt_(cbImage) const unsigned char* pbImage,
                                            _In_ const DWORD cbImage);

    __override STDMETHOD(OnRemoveNotification)(_In_ const SID* user,
                                               _In_ REFAPPLICATION_ID rAppId,
                                               _In_ const NOTIFICATION_ID notificationId);

    __override STDMETHOD(OnRemoveAllNotifications)(_In_ const SID* user,
                                                   _In_ REFAPPLICATION_ID rAppId);

    __override STDMETHOD(OnAddContent)(_In_ const SID* user,
                                       _In_ REFAPPLICATION_ID rAppId,
                                       _In_ REFENDPOINT_ID rRendererId,
                                       _In_ const CONTENT_ID contentId,
                                       _In_reads_bytes_opt_(cbData) const unsigned char* pData,
                                       _In_ const DWORD cbData);

    __override STDMETHOD(OnRemoveContent)(_In_ const SID* user,
                                          _In_ REFAPPLICATION_ID rAppId,
                                          _In_ REFENDPOINT_ID rRenderId,
                                          _In_ const CONTENT_ID contentId);

    __override STDMETHOD(OnRemoveAllContent)(_In_ const SID* user,
                                             _In_ REFAPPLICATION_ID rAppId,
                                             _In_ REFENDPOINT_ID rRenderId);

    __override STDMETHOD(OnProcessWpdMessage)(_In_ IUnknown* pinValues,
                                              _In_ IUnknown* poutValues);

    __override STDMETHOD(OnSetLanguage)(_In_ const SID* user,
                                        _In_ LPCWSTR wszLang);

protected:
    STDMETHOD(GetApplicationOrderString)(_Outptr_result_maybenull_ LPWSTR* ppwszKey);

    STDMETHOD(GetStoredApplicationOrder)(APPLICATION_ID** ppAppIds,
                                         DWORD* pcAppIds);

    STDMETHOD(SetStoredApplicationOrder)(const APPLICATION_ID* pApps,
                                         const DWORD cApps);

protected:
    SID* m_pUserSID;
    size_t m_cbUserSID; // This is the maximum size of a SID, not the actual size of the current m_pUserSID

    CComPtr<IWDFNamedPropertyStore> m_pPropertyStore;
};
