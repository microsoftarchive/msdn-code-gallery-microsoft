/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Helpers.h

Abstract:

--*/

#pragma once

#ifndef SAFE_RELEASE
    #define SAFE_RELEASE(p) if( NULL != p ) { ( p )->Release(); p = NULL; }
#endif

// {CDD18979-A7B0-4D5E-9EB2-0A826805CBBD}
DEFINE_PROPERTYKEY(PRIVATE_SAMPLE_DRIVER_WUDF_DEVICE_OBJECT, 0xCDD18979, 0xA7B0, 0x4D5E, 0x9E, 0xB2, 0x0A, 0x82, 0x68, 0x05, 0xCB, 0xBD, 2);
// {9BD949E5-59CF-41AE-90A9-BE1D044F578F}
DEFINE_PROPERTYKEY(PRIVATE_SAMPLE_DRIVER_WPD_SERIALIZER_OBJECT, 0x9BD949E5, 0x59CF, 0x41AE, 0x90, 0xA9, 0xBE, 0x1D, 0x04, 0x4F, 0x57, 0x8F, 2);
// {4DF6C8C7-2CE5-457C-9F53-EFCECAA95C04}
DEFINE_PROPERTYKEY(PRIVATE_SAMPLE_DRIVER_CLIENT_CONTEXT_MAP, 0x4DF6C8C7, 0x2CE5, 0x457C, 0x9F, 0x53, 0xEF, 0xCE, 0xCA, 0xA9, 0x5C, 0x04, 2);
// {67BA8D9E-1DC4-431C-B89C-9D03F7D8C223}
DEFINE_PROPERTYKEY(PRIVATE_SAMPLE_DRIVER_REQUEST_FILENAME, 0x67BA8D9E, 0x1DC4, 0x431C, 0xB8, 0x9C, 0x9D, 0x03, 0xF7, 0xD8, 0xC2, 0x23, 2);


// Access Scope is a bit mask, where each bit enables access to a particular scope
// for example, Bluetooth GATT Service is bit 1.   
// The next scope, if any, will be in bit 2
// Full device access is a combination of all, requires all bits to be set
typedef enum tagACCESS_SCOPE
{
    BLUETOOTH_GATT_SERVICE_ACCESS = 1,
    FULL_DEVICE_ACCESS = 0xFFFFFFFF
}ACCESS_SCOPE;

typedef enum tagDevicePropertyAttributesType
{
    UnspecifiedForm_CanRead_CanWrite_CannotDelete_Fast,
    UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast,
} DevicePropertyAttributesType;

typedef struct tagPropertyAttributeInfo
{
    const PROPERTYKEY*                pKey;
    VARTYPE                           Vartype;
    DevicePropertyAttributesType      AttributesType;
    PCWSTR                            wszName;
} PropertyAttributeInfo;

typedef struct tagMethodtAttributeInfo
{
    const GUID*                       pMethodGuid;
    PCWSTR                            wszName;
    ULONG                             ulMethodAttributeAccess;
} MethodAttributeInfo;


typedef struct tagMethodParameterAttributeInfo
{
    const GUID*                       pMethodGuid;
    const PROPERTYKEY*                pParameter;
    VARTYPE                           Vartype;
    WPD_PARAMETER_USAGE_TYPES         UsageType;
    WpdParameterAttributeForm         Form;
    DWORD                             Order;
    PCWSTR                            wszName;
} MethodParameterAttributeInfo;

typedef struct tagEventAttributeInfo
{
    const GUID*                       pEventGuid;
    PCWSTR                            wszName;
} EventAttributeInfo;

typedef struct tagEventParameterAttributeInfo
{
    const GUID*                       pEventGuid;
    const PROPERTYKEY*                pParameter;
    VARTYPE                           Vartype;
} EventParameterAttributeInfo;

typedef struct tagFormatAttributeInfo
{
    const GUID*                       pFormatGuid;
    PCWSTR                            wszName;
} FormatAttributeInfo;

FORCEINLINE
VOID
InitializeListHead(
    _Out_ PLIST_ENTRY ListHead
    )
{
    ListHead->Flink = ListHead->Blink = ListHead;
}

FORCEINLINE
BOOLEAN
RemoveEntryList(
    _In_ PLIST_ENTRY Entry
    )
{
    PLIST_ENTRY Blink;
    PLIST_ENTRY Flink;

    Flink = Entry->Flink;
    Blink = Entry->Blink;
    Blink->Flink = Flink;
    Flink->Blink = Blink;
    return (BOOLEAN)(Flink == Blink);
}

FORCEINLINE
PLIST_ENTRY
RemoveHeadList(
    _Inout_ PLIST_ENTRY ListHead
    )
{
    PLIST_ENTRY Flink;
    PLIST_ENTRY Entry;

    Entry = ListHead->Flink;
    Flink = Entry->Flink;
    ListHead->Flink = Flink;
    Flink->Blink = ListHead;
    return Entry;
}

FORCEINLINE
VOID
InsertTailList(
    _Inout_ PLIST_ENTRY ListHead,
    _Inout_ __drv_aliasesMem PLIST_ENTRY Entry
    )
{
    PLIST_ENTRY Blink;

    Blink = ListHead->Blink;
    Entry->Flink = ListHead;
    Entry->Blink = Blink;
    Blink->Flink = Entry;
    ListHead->Blink = Entry;
}

FORCEINLINE
VOID
InsertHeadList(
    _Inout_ PLIST_ENTRY ListHead,
    _Inout_ __drv_aliasesMem PLIST_ENTRY Entry
    )
{
    PLIST_ENTRY Flink;

    Flink = ListHead->Flink;
    Entry->Flink = Flink;
    Entry->Blink = ListHead;
    Flink->Blink = Entry;
    ListHead->Flink = Entry;
}


BOOLEAN
FORCEINLINE
IsListEmpty(
    _In_ const LIST_ENTRY * ListHead
    )
{
    return (BOOLEAN)(ListHead->Flink == ListHead);
}

HRESULT
GetDeviceAddressFromDevice(
    _In_ IWDFDevice * Device,
    _Out_ PBTH_ADDR pBthAddr
    );

class ContextMap : public IUnknown
{
public:
    ContextMap(WpdBaseDriver * pWpdBaseDriver);

    ~ContextMap();

public: // IUnknown
    ULONG __stdcall AddRef()
    {
        InterlockedIncrement((long*) &m_cRef);
        return m_cRef;
    }

    _At_(this, __drv_freesMem(Mem)) 
    ULONG __stdcall Release()
    {
        ULONG ulRefCount = m_cRef - 1;

        if (InterlockedDecrement((long*) &m_cRef) == 0)
        {
            delete this;
            return 0;
        }
        return ulRefCount;
    }

    HRESULT __stdcall QueryInterface(
        REFIID riid,
        void** ppv)
    {
        HRESULT hr = S_OK;

        if(riid == IID_IUnknown)
        {
            *ppv = static_cast<IUnknown*>(this);
            AddRef();
        }
        else
        {
            *ppv = NULL;
            hr = E_NOINTERFACE;
        }
        return hr;
    }



public: // Context accessor methods

    // If successful, this method AddRef's the context and returns
    // a context key
    HRESULT Add(
        _In_    IUnknown*     pContext,
        _Inout_ CAtlStringW&  key)
    {
        CComCritSecLock<CComAutoCriticalSection> Lock(m_CriticalSection);
        HRESULT  hr          = S_OK;
        GUID     guidContext = GUID_NULL;
        CComBSTR bstrContext;

        // Create a unique context key
        hr = CoCreateGuid(&guidContext);
        if (hr == S_OK)
        {
            bstrContext = guidContext;
            if(bstrContext.Length() > 0)
            {
                key = bstrContext;
            }
            else
            {
                hr = E_OUTOFMEMORY;
            }
        }

        if (hr == S_OK)
        {
            // Insert this into the map
            POSITION  elementPosition = m_Map.SetAt(key, pContext);
            if(elementPosition != NULL)
            {
                // AddRef since we are holding onto it
                pContext->AddRef();
            }
            else
            {
                hr = E_OUTOFMEMORY;
            }
        }
        return hr;
    }

    void Remove(
        const CAtlStringW&  key)
    {
        CComCritSecLock<CComAutoCriticalSection> Lock(m_CriticalSection);
        // Get the element
        IUnknown* pContext = NULL;

        if (m_Map.Lookup(key, pContext) == true)
        {
            // Remove the entry for it
            m_Map.RemoveKey(key);

            // Release it
            pContext->Release();
        }
    }

    // Returns the context pointer.  If not found, return value is NULL.
    // If non-NULL, caller is responsible for Releasing when it is done,
    // since this method will AddRef the context.
    IUnknown* GetContext(
        const CAtlStringW&  key)
    {
        CComCritSecLock<CComAutoCriticalSection> Lock(m_CriticalSection);
        // Get the element
        IUnknown* pContext = NULL;

        if (m_Map.Lookup(key, pContext) == true)
        {
            // AddRef
            pContext->AddRef();
        }
        return pContext;
    }


private:
    CComAutoCriticalSection         m_CriticalSection;
    CAtlMap<CAtlStringW, IUnknown*> m_Map;
    DWORD                           m_cRef;
    CComPtr<WpdBaseDriver>          m_WpdBaseDriver;
};


// This class is used to store the connected client information.
class ClientContext : public IUnknown
{
public:
    ClientContext() :
        MajorVersion(0),
        MinorVersion(0),
        Revision(0),
        m_cRef(1)
    {
    }

    ~ClientContext()
    {
    }

public: // IUnknown
    ULONG __stdcall AddRef()
    {
        InterlockedIncrement((long*) &m_cRef);
        return m_cRef;
    }

    _At_(this, __drv_freesMem(Mem)) 
    ULONG __stdcall Release()
    {
        ULONG ulRefCount = m_cRef - 1;

        if (InterlockedDecrement((long*) &m_cRef) == 0)
        {
            delete this;
            return 0;
        }
        return ulRefCount;
    }

    HRESULT __stdcall QueryInterface(
        REFIID riid,
        void** ppv)
    {
        HRESULT hr = S_OK;

        if(riid == IID_IUnknown)
        {
            *ppv = static_cast<IUnknown*>(this);
            AddRef();
        }
        else
        {
            *ppv = NULL;
            hr = E_NOINTERFACE;
        }

        return hr;
    }

private:
    DWORD m_cRef;

public:
    CAtlStringW ClientName;
    CAtlStringW EventCookie;
    DWORD       MajorVersion;
    DWORD       MinorVersion;
    DWORD       Revision;
};


class PropVariantWrapper : public tagPROPVARIANT
{
public:
    PropVariantWrapper()
    {
        PropVariantInit(this);
    }

    PropVariantWrapper(LPCWSTR pszSrc)
    {
        PropVariantInit(this);

        *this = pszSrc;
    }

    virtual ~PropVariantWrapper()
    {
        Clear();
    }

    void Clear()
    {
        PropVariantClear(this);
    }

    PropVariantWrapper& operator= (const ULONG ulValue)
    {
        Clear();
        vt      = VT_UI4;
        ulVal   = ulValue;

        return *this;
    }

    PropVariantWrapper& operator= (_In_ LPCWSTR pszSrc)
    {
        Clear();

        pwszVal = AtlAllocTaskWideString(pszSrc);
        if(pwszVal != NULL)
        {
            vt = VT_LPWSTR;
        }
        return *this;
    }

    PropVariantWrapper& operator= (_In_ IUnknown* punkSrc)
    {
        Clear();

        // Need to AddRef as PropVariantClear will Release
        if (punkSrc != NULL)
        {
            vt      = VT_UNKNOWN;
            punkVal = punkSrc;
            punkVal->AddRef();
        }
        return *this;
    }

    void SetErrorValue(const HRESULT hr)
    {
        Clear();
        vt      = VT_ERROR;
        scode   = hr;
    }

    void SetBoolValue(const bool bValue)
    {
        Clear();
        vt      = VT_BOOL;
        if(bValue)
        {
            boolVal = VARIANT_TRUE;
        }
        else
        {
            boolVal = VARIANT_FALSE;
        }
    }
};

HRESULT UpdateDeviceFriendlyName(
    _In_    IPortableDeviceClassExtension*  pPortableDeviceClassExtension,
    _In_    LPCWSTR                         wszDeviceFriendlyName);

HRESULT RegisterServices(
    _In_    IPortableDeviceClassExtension*  pPortableDeviceClassExtension,
            const bool                      bUnregister);

HRESULT CheckRequestFilename(
    _In_    LPCWSTR  pszRequestFilename);

HRESULT AddStringValueToPropVariantCollection(
    _Out_   IPortableDevicePropVariantCollection* pCollection,
    _In_    LPCWSTR                               wszValue);

HRESULT PostWpdEvent(
    _In_    IPortableDeviceValues*  pCommandParams,
    _In_    IPortableDeviceValues*  pEventParams);

HRESULT GetClientContextMap(
    _In_        IPortableDeviceValues*  pParams,
    _Outptr_ ContextMap**            ppContextMap);

_Success_(return == S_OK)
HRESULT GetClientContext(
    _In_        IPortableDeviceValues*  pParams,
    _In_        LPCWSTR                 pszContextKey,
    _Outptr_    IUnknown**              ppContext);

HRESULT GetClientEventCookie(
    _In_        IPortableDeviceValues*  pParams,
    _Outptr_result_maybenull_ LPWSTR*             ppszEventCookie);

HRESULT AddPropertyAttributesByType(
    _In_          const DevicePropertyAttributesType type,
    _Inout_       IPortableDeviceValues*           pAttributes);

HRESULT SetPropertyAttributes(
                                REFPROPERTYKEY                  Key,
    _In_reads_(cAttributeInfo)  const PropertyAttributeInfo*    AttributeInfo,
    _In_                        DWORD                           cAttributeInfo,
    _Inout_                     IPortableDeviceValues*          pAttributes);

HRESULT SetMethodParameters(
                                REFGUID Method,
    _In_reads_(cAttributeInfo)  const MethodParameterAttributeInfo*  AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceKeyCollection*       pParameters);


HRESULT SetMethodParameterAttributes(
                                REFPROPERTYKEY                          Parameter,
    _In_reads_(cAttributeInfo)  const  MethodParameterAttributeInfo*    AttributeInfo,
    _In_                        DWORD                                   cAttributeInfo,
    _Inout_                     IPortableDeviceValues*                  pAttributes);

HRESULT SetEventParameterAttributes(
                                REFPROPERTYKEY                      Parameter,
    _In_reads_(cAttributeInfo)  const EventParameterAttributeInfo*  AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceValues*              pAttributes);

HRESULT SetEventParameters(
                                REFGUID Event,
    _In_reads_(cAttributeInfo)  const EventParameterAttributeInfo*  AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceKeyCollection*       pParameters);

VOID ConvertFileTimeToUlonglong(
    _In_                        FILETIME * fTime,
    _Out_                       ULONGLONG * pResult);


