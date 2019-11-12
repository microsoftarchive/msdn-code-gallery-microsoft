//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <wrl.h>
#include <d3d11_1.h>
#include <d2d1_1.h>
#include <d2d1_1helper.h>
#include <d2d1effects.h>
#include <wincodec.h>
#include <math.h>
#include <agile.h>

// C headers:
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <math.h>

// C++ headers:
#include <algorithm>
#include <numeric>
#include <string>
#include <vector>
#include <utility>
#include <limits>

// Windows headers:
#include <windows.h>
#include <unknwn.h>
#include <dwrite_1.h>
#include <intsafe.h>
#include <strsafe.h>

// Ignore unreferenced parameters, since they are very common
// when implementing callbacks.
#pragma warning(disable : 4100)

////////////////////////////////////////
// Application specific headers/functions:

enum {
    UnicodeMax = 0x10FFFF
};

// Needed text editor backspace deletion.
inline bool IsSurrogate(UINT32 ch)
{
    // 0xD800 <= ch <= 0xDFFF
    return (ch & 0xF800) == 0xD800;
}

inline bool IsLeadingSurrogate(UINT32 ch)
{
    // 0xD800 <= ch <= 0xDBFF
    return (ch & 0xFC00) == 0xD800;
}

inline bool IsTrailingSurrogate(UINT32 ch)
{
    // 0xDC00 <= ch <= 0xDFFF
    return (ch & 0xFC00) == 0xDC00;
}

inline UINT32 MakeUnicodeCodepoint(UINT32 leading, UINT32 trailing)
{
    return ((leading & 0x03FF) << 10 | (trailing & 0x03FF)) + 0x10000;
}

// Acquires an additional reference, if non-null.
template <typename InterfaceType>
inline InterfaceType* SafeAcquire(InterfaceType* newObject)
{
    if (newObject != nullptr)
    {
        newObject->AddRef();
    }
    return newObject;
}

// Generic COM base implementation for classes, since DirectWrite uses
// callbacks for several different kinds of objects, particularly the
// script analysis source/sink.
//
// Example:
//
//  class TextAnalysis : public ComBase<QiList<IDWriteTextAnalysisSink>>
//
template <typename InterfaceChain>
class ComBase : public InterfaceChain
{
public:
    explicit ComBase()
    :   refValue_(0)
    { }

    // IUnknown interface
    IFACEMETHOD(QueryInterface)(IID const& iid, OUT void** ppObject)
    {
        *ppObject = nullptr;
        InterfaceChain::QueryInterfaceInternal(iid, ppObject);
        if (*ppObject == nullptr)
            return E_NOINTERFACE;

        AddRef();
        return S_OK;
    }

    IFACEMETHOD_(ULONG, AddRef)()
    {
        return InterlockedIncrement(&refValue_);
    }

    IFACEMETHOD_(ULONG, Release)()
    {
        ULONG newCount = InterlockedDecrement(&refValue_);
        if (newCount == 0)
            delete this;

        return newCount;
    }

    virtual ~ComBase()
    { }

protected:
    ULONG refValue_;

private:
    // No copy construction allowed.
    ComBase(const ComBase& b);
    ComBase& operator =(ComBase const&);
};


struct QiListNil
{
};


// When the QueryInterface list refers to itself as class,
// which hasn't fully been defined yet.
template <typename InterfaceName, typename InterfaceChain>
class QiListSelf : public InterfaceChain
{
public:
    inline void QueryInterfaceInternal(IID const& iid, OUT void** ppObject)
    {
        if (iid != __uuidof(InterfaceName))
            return InterfaceChain::QueryInterfaceInternal(iid, ppObject);

        *ppObject = static_cast<InterfaceName*>(this);
    }
};

// When this interface is implemented and more follow.
template <typename InterfaceName, typename InterfaceChain = QiListNil>
class QiList : public InterfaceName, public InterfaceChain
{
public:
    inline void QueryInterfaceInternal(IID const& iid, OUT void** ppObject)
    {
        if (iid != __uuidof(InterfaceName))
            return InterfaceChain::QueryInterfaceInternal(iid, ppObject);

        *ppObject = static_cast<InterfaceName*>(this);
    }
};

// When the this is the last implemented interface in the list.
template <typename InterfaceName>
class QiList<InterfaceName, QiListNil> : public InterfaceName
{
public:
    inline void QueryInterfaceInternal(IID const& iid, OUT void** ppObject)
    {
        if (iid != __uuidof(InterfaceName))
            return;

        *ppObject = static_cast<InterfaceName*>(this);
    }
};
