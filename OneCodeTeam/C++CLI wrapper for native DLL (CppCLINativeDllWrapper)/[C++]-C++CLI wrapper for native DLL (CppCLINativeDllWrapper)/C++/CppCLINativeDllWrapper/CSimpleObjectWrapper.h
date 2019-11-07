/****************************** Module Header ******************************\
Module Name:  CSimpleObjectWrapper.h
Project:      CppCLINativeDllWrapper
Copyright (c) Microsoft Corporation.

The code in this file declares the C++/CLI wrapper classes that allow you 
to call from any .NET code to the classes exported by a native C++ DLL 
module.

  CSCallNativeDllWrapper/VBCallNativeDllWrapper (any .NET clients)
          -->
      CppCLINativeDllWrapper (this C++/CLI wrapper)
              -->
          CppDynamicLinkLibrary (a native C++ DLL module)

The CSimpleObjectWrapper class wraps the native C++ class CSimpleObject, 
exported by CppDynamicLinkLibrary.dll.

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

#pragma once

// Include the header file of the native C++ module.
#include "CppDynamicLinkLibrary.h"

using namespace System;
using namespace System::Runtime::InteropServices;


namespace CppCLINativeDllWrapper
{
    /// <summary>
    /// This C++/CLI class wraps the native C++ class CSimpleObject exported 
    /// by CppDynamicLinkLibrary.dll. 
    ///</summary>
    public ref class CSimpleObjectWrapper
    {
    public:
        CSimpleObjectWrapper(void);
        virtual ~CSimpleObjectWrapper(void);

        // Property
        property float FloatProperty
        {
            float get(void);
            void set(float value);
        }

        // Method
        virtual String ^ ToString(void) override;

        // Static method
        static int GetStringLength(String ^ str);

    protected:
        !CSimpleObjectWrapper(void);

    private:
        CSimpleObject *m_impl;
    };
}