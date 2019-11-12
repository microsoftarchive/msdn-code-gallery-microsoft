/****************************** Module Header ******************************\
Module Name:  CppCLINETAssemblyWrapper.h
Project:      CppCLINETAssemblyWrapper
Copyright (c) Microsoft Corporation.

The code in this file declares the C++ wrapper class CSSimpleObjectWrapper 
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

#pragma once

#include <windows.h>

#ifdef CPPCLINETASSEMBLYWRAPPER_EXPORTS
#define SYMBOL_DECLSPEC __declspec(dllexport)
#else
#define SYMBOL_DECLSPEC	__declspec(dllimport)
#endif


// This native C++ class wraps the C# class CSSimpleObject defined in the 
// .NET class library CSClassLibrary. 
class SYMBOL_DECLSPEC CSSimpleObjectWrapper
{
public:
    CSSimpleObjectWrapper(void);
    virtual ~CSSimpleObjectWrapper(void);

    // Property
    float get_FloatProperty(void);
    void set_FloatProperty(float fVal);

    // Method
    HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

    // Static method
    static int GetStringLength(PCWSTR pszString);

private:
    void *m_impl;
};