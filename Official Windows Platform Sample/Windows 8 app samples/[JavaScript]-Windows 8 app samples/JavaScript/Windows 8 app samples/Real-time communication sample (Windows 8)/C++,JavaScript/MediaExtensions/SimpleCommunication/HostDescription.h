//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <StspNetwork.h>

namespace Stsp { namespace Net {
class CHostDescription : public IHostDescription
{
public:
    static HRESULT CreateInstance(_In_ LPCWSTR pszHostName, _In_ LPCWSTR pszHostService, StspNetworkType eNetworkType,  _Outptr_ IHostDescription **ppHostDesc);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    // IHostDescription
    IFACEMETHOD_(LPCWSTR, GetHostName) ();
    IFACEMETHOD_(LPCWSTR, GetHostService) ();
    IFACEMETHOD_(StspNetworkType, GetNetworkType) ();

protected:
    CHostDescription();
    ~CHostDescription();

    HRESULT Initialize(_In_ LPCWSTR pszHostName, _In_ LPCWSTR pszHostService, StspNetworkType eNetworkType);

private:
    LONG _cRef;
    LPWSTR _pszHostName;
    LPWSTR _pszHostService;
    StspNetworkType _eNetworkType;
};
} } // namespace Stsp::Net
