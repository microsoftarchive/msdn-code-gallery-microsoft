/****************************** Module Header ******************************\
Module Name:  NativeMethods.cpp
Project:      CppCLINativeDllWrapper
Copyright (c) Microsoft Corporation.

The code in this file implements the C++/CLI wrapper classes that allow you 
to call from any .NET code to the functions exported by a native C++ DLL 
module.

  CSCallNativeDllWrapper/VBCallNativeDllWrapper (any .NET clients)
          -->
      CppCLINativeDllWrapper (this C++/CLI wrapper)
              -->
          CppDynamicLinkLibrary (a native C++ DLL module)

The NativeMethods class wraps the global functions exported by 
CppDynamicLinkLibrary.dll.

The interoperability features supported by Visual C++/CLI offer a 
particular advantage over other .NET languages when it comes to 
interoperating with native modules. Apart from the traditional explicit 
P/Invoke, C++/CLI allows implicit P/Invoke, also known as C++ Interop, or 
It Just Work (IJW), which mixes managed code and native code almost 
invisibly. The feature provides better type safety, easier coding, greater  
performance, and is more forgiving if the native API is modified. You can 
use the technology to build .NET wrappers for native C++ classes and 
functions if their source code is available, and allow any .NET clients to 
access the native C++ classes and functions through the wrappers.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "NativeMethods.h"
using namespace CppCLINativeDllWrapper;

#include <msclr/marshal.h>
using namespace msclr::interop;
#pragma endregion


int NativeMethods::GetStringLength1(String ^ str)
{
    // Marshal System::String to PCWSTR, and call the C++ function.
    marshal_context ^ context = gcnew marshal_context();
    PCWSTR pszString = context->marshal_as<const wchar_t*>(str);
    int length = ::GetStringLength1(pszString);
    delete context;
    return length;
}

int NativeMethods::GetStringLength2(String ^ str)
{
    // Marshal System::String to PCWSTR, and call the C++ function.
    marshal_context ^ context = gcnew marshal_context();
    PCWSTR pszString = context->marshal_as<const wchar_t*>(str);
    int length = ::GetStringLength2(pszString);
    delete context;
    return length;
}

int NativeMethods::Max(int a, int b, CompareCallback ^ cmpFunc)
{
    // Convert the delegate to a function pointer.
    IntPtr pCmpFunc = Marshal::GetFunctionPointerForDelegate(cmpFunc);
    return ::CompareInts(a, b, static_cast<::PFN_COMPARE>(pCmpFunc.ToPointer()));
}