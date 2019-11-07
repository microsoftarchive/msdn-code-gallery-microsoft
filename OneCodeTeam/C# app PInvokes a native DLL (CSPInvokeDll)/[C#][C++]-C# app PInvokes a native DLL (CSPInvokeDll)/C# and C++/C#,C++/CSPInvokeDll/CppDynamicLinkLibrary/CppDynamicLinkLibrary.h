/****************************** Module Header ******************************\
Module Name:  CppDynamicLinkLibrary.h
Project:      CppDynamicLinkLibrary
Copyright (c) Microsoft Corporation.

A dynamic-link library (DLL) is a module that contain functions and data 
that can be used by another module (application or DLL). This Win32 DLL 
code sample demonstrates exporting data, functions and classes for use in 
executables.

The sample DLL exports these data, functions and classes:

    // Global Data
    int g_nVal1
    int g_nVal2

    // Ordinary Functions
    int __cdecl GetStringLength1(PCWSTR pszString);
    int __stdcall GetStringLength2(PCWSTR pszString);

    // Callback Function
    int __stdcall Max(int a, int b, PFN_COMPARE cmpFunc)

    // Class
    class CSimpleObject
    {
    public:
        CSimpleObject(void);  // Constructor
        virtual ~CSimpleObject(void);  // Destructor
          
        // Property
        float get_FloatProperty(void);
        void set_FloatProperty(float newVal);

        // Method
        HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

        // Static method
        static int GetStringLength(PCWSTR pszString);

    private:
        float m_fField;
    };

Two methods are used to export the symbols from the sample DLL:

1. Export symbols from a DLL using .DEF files

A module-definition (.DEF) file is a text file containing one or more 
module statements that describe various attributes of a DLL. Create a .DEF 
file and use the .def file when building the DLL. Using this approach, we 
can export functions from the DLL by ordinal rather than by name. 

2. Export symbols from a DLL using __declspec(dllexport) 

__declspec(dllexport) adds the export directive to the object file so we do 
not need to use a .def file. This convenience is most apparent when trying 
to export decorated C++ function names. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>

// The following ifdef block is the standard way of creating macros which 
// make exporting from a DLL simpler. All files within this DLL are compiled 
// with the CPPDYNAMICLINKLIBRARY_EXPORTS symbol defined on the command line. 
// This symbol should not be defined on any project that uses this DLL. This 
// way any other project whose source files include this file see 
// SYMBOL_DECLSPEC and SYMBOL_DEF functions as being imported from a DLL, 
// whereas this DLL sees symbols defined with these macros as being exported.

#ifdef CPPDYNAMICLINKLIBRARY_EXPORTS
#define SYMBOL_DECLSPEC __declspec(dllexport)
#define SYMBOL_DEF
#else
#define SYMBOL_DECLSPEC __declspec(dllimport)
#define SYMBOL_DEF      __declspec(dllimport)
#endif


#pragma region Global Data

// An exported/imported global data using a DEF file
// Sym: g_nVal1
// See: CppDynamicLinkLibrary.def
//      CppDynamicLinkLibrary.cpp
// Ref: http://support.microsoft.com/kb/90530


// An exported/imported global data using __declspec(dllexport/dllimport)
// Sym: g_nVal2
// See: CppDynamicLinkLibrary.cpp
// Ref: http://support.microsoft.com/kb/90530
EXTERN_C SYMBOL_DECLSPEC int g_nVal2;

#pragma endregion


#pragma region Ordinary Functions

// An exported/imported cdecl(default) function by using a DEF file
// The default calling convention of the exported function is cdecl
// Sym: GetStringLength1
// See: Project Properties / C/C++ / Advanced / Calling Convention
//      CppDynamicLinkLibrary.def
//      CppDynamicLinkLibrary.cpp
// Ref: http://msdn.microsoft.com/en-us/library/d91k01sh.aspx
SYMBOL_DEF int /*__cdecl*/ GetStringLength1(PCWSTR pszString);

// An exported/imported stdcall function __declspec(dllexport/dllimport)
// Sym: _GetStringLength2@4 for Win32, and GetStringLength2 for x64
// See: CppDynamicLinkLibrary.cpp
// Ref: http://msdn.microsoft.com/en-us/library/a90k134d.aspx
EXTERN_C SYMBOL_DECLSPEC int __stdcall GetStringLength2(PCWSTR pszString);

#pragma endregion


#pragma region Callback Function

// Type-definition: 'PFN_COMPARE' now can be used as type
typedef int (CALLBACK *PFN_COMPARE)(int, int);

// An exported/imported stdcall function using a DEF file
// It requires a callback function as one of the arguments
// Sym: Max
// See: CppDynamicLinkLibrary.cpp
SYMBOL_DEF int __stdcall CompareInts(int a, int b, PFN_COMPARE cmpFunc);

#pragma endregion


#pragma region Class

// An exported/imported class using __declspec(dllexport/dllimport)
// It exports/imports all public members of the class
// See: CppDynamicLinkLibrary.cpp
// Ref: http://msdn.microsoft.com/en-us/library/a90k134d.aspx
class SYMBOL_DECLSPEC CSimpleObject
{
public:

    CSimpleObject(void);  // Constructor
    virtual ~CSimpleObject(void);  // Destructor

    // Property
    float get_FloatProperty(void);
    void set_FloatProperty(float newVal);

    // Method
    HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

    // Static method
    static int GetStringLength(PCWSTR pszString);

private:
    float m_fField;
};

#pragma endregion