/****************************** Module Header ******************************\
Module Name:  CppCLINETAssemblyWrapper.cpp
Project:      CppCLINETAssemblyWrapper
Copyright (c) Microsoft Corporation.

The code in this file implements the C++ wrapper class CSSimpleObjectWrapper 
for the .NET class CSSimpleObject defined in the .NET class library 
CSClassLibrary. Your native C++ application can include this wrapper class 
and link to the DLL to indirectly call the .NET class.

  CppCallNETAssemblyWrapper (a native C++ application)
          -->
      CppCLINETAssemblyWrapper (this C++/CLI wrapper)
              -->
          CSClassLibrary (a .NET assembly)

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "CppCLINETAssemblyWrapper.h"

#include <msclr/marshal.h>
using namespace msclr::interop;

#include <strsafe.h>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace CSClassLibrary;
#pragma endregion


CSSimpleObjectWrapper::CSSimpleObjectWrapper(void)
{
    // Instantiate the C# class CSSimpleObject.
    CSSimpleObject ^ obj = gcnew CSSimpleObject();

    // Pin the CSSimpleObject .NET object, and record the address of the 
    // pinned object in m_impl. 
    m_impl = GCHandle::ToIntPtr(GCHandle::Alloc(obj)).ToPointer(); 
}

CSSimpleObjectWrapper::~CSSimpleObjectWrapper(void)
{
    // Get the GCHandle associated with the pinned object based on its 
    // address, and free the GCHandle. At this point, the CSSimpleObject 
    // object is eligible for GC.
    GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
    h.Free();
}

float CSSimpleObjectWrapper::get_FloatProperty(void)
{
    // Get the pinned CSSimpleObject object from its memory address.
    GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
    CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

    // Redirect the call to the corresponding property of the wrapped 
    // CSSimpleObject object.
    return obj->FloatProperty;
}

void CSSimpleObjectWrapper::set_FloatProperty(float fVal)
{
    // Get the pinned CSSimpleObject object from its memory address.
    GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
    CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

    // Redirect the call to the corresponding property of the wrapped 
    // CSSimpleObject object.
    obj->FloatProperty = fVal;
}

HRESULT CSSimpleObjectWrapper::ToString(PWSTR pszBuffer, DWORD dwSize)
{
    // Get the pinned CSSimpleObject object from its memory address.
    GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
    CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

    String ^ str;
    HRESULT hr;
    try
    {
        // Redirect the call to the corresponding method of the wrapped 
        // CSSimpleObject object.
        str = obj->ToString();
    }
    catch (Exception ^ e)
    {
        hr = Marshal::GetHRForException(e);
    }

    if (SUCCEEDED(hr))
    {
        // Convert System::String to PCWSTR.
        marshal_context ^ context = gcnew marshal_context();
        PCWSTR pszStr = context->marshal_as<const wchar_t*>(str);
        hr = StringCchCopy(pszBuffer, dwSize, pszStr == NULL ? L"" : pszStr);
        delete context; // This will also free the memory pointed by pszStr
    }

    return hr;
}

int CSSimpleObjectWrapper::GetStringLength(PCWSTR pszString)
{
    return CSSimpleObject::GetStringLength(gcnew String(pszString));
}