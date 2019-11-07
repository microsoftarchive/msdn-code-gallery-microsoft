/****************************** Module Header ******************************\
Module Name:  CppDynamicLinkLibrary.cpp
Project:      CppDynamicLinkLibrary
Copyright (c) Microsoft Corporation.

Defines the exported data and functions of the DLL application.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "CppDynamicLinkLibrary.h"
#include <strsafe.h>

#pragma region DLLMain
BOOL APIENTRY DllMain(HMODULE hModule,
                      DWORD  ul_reason_for_call,
                      LPVOID lpReserved
                      )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}
#pragma endregion


#pragma region Global Data

// An exported/imported global data using a DEF file
// Initialize it to be 1
int g_nVal1 = 1;


// An exported/imported global data using __declspec(dllexport/dllimport)
// Initialize it to be 2
SYMBOL_DECLSPEC int g_nVal2 = 2;

#pragma endregion


#pragma region Ordinary Functions


// An exported/imported cdecl(default) function using a DEF file
int /*__cdecl*/ GetStringLength1(PCWSTR pszString)
{
    return static_cast<int>(wcslen(pszString));
}


// An exported/imported stdcall function using __declspec(dllexport/dllimport)
SYMBOL_DECLSPEC int __stdcall GetStringLength2(PCWSTR pszString)
{
    return static_cast<int>(wcslen(pszString));
}

#pragma endregion


#pragma region Callback Function

// An exported/imported stdcall function using a DEF file
// It requires a callback function as one of the arguments
int __stdcall CompareInts(int a, int b, PFN_COMPARE cmpFunc)
{
	// Make the callback to the comparison function

	// If a is greater than b, return a; 
    // If b is greater than or equal to a, return b.
    return ((*cmpFunc)(a, b) > 0) ? a : b;
}

#pragma endregion


#pragma region Class

// Constructor of the simple C++ class
CSimpleObject::CSimpleObject(void) : m_fField(0.0f)
{
}


// Destructor of the simple C++ class
CSimpleObject::~CSimpleObject(void)
{
}


float CSimpleObject::get_FloatProperty(void)
{
	return this->m_fField;
}


void CSimpleObject::set_FloatProperty(float newVal)
{
	this->m_fField = newVal;
}


HRESULT CSimpleObject::ToString(PWSTR pszBuffer, DWORD dwSize)
{
    return StringCchPrintf(pszBuffer, dwSize, L"%.2f", this->m_fField);
}


int CSimpleObject::GetStringLength(PCWSTR pszString)
{
    return static_cast<int>(wcslen(pszString));
}

#pragma endregion