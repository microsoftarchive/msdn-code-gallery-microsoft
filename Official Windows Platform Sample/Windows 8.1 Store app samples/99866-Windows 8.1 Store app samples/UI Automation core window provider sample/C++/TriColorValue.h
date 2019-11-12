//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

#ifdef __cplusplus_winrt
using namespace UiaCoreWindowProvider;
#else
using namespace ABI::UiaCoreWindowProvider;
#endif

// Helper class for manipulating TriColorValues
class TriColorValueHelper
{
public:
    static bool IsFirst(TriColorValue value)
    {
        return (value == TriColorValue::Red);
    }

    static bool IsLast(TriColorValue value)
    {
        return (value == TriColorValue::Green);
    }

    static TriColorValue NextValue(TriColorValue value)
    {
        return (TriColorValue)((int)value + 1);
    }

    static TriColorValue PreviousValue(TriColorValue value)
    {
        return (TriColorValue)((int)value - 1);
    }

    static HRESULT ValueToString(TriColorValue value, _Out_ BSTR *pszValue)
    {
        switch (value)
        {
        case TriColorValue::Red: *pszValue = SysAllocString(L"Red"); break;
        case TriColorValue::Green: *pszValue = SysAllocString(L"Green"); break;
        case TriColorValue::Yellow: *pszValue = SysAllocString(L"Yellow"); break;
        default: *pszValue = SysAllocString(L"Unknown Value"); break;
        }
        return (*pszValue != nullptr) ? S_OK : E_OUTOFMEMORY;
    }
};
