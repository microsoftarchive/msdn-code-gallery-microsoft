/****************************** Module Header ******************************\
 * Module Name:  CppHostCLR.cpp
 * Project:      CppHostCLR
 * Copyright (c) Microsoft Corporation.
 * 
 * The Common Language Runtime (CLR) allows a level of integration between 
 * itself and a host. This C++ code sample demonstrates using the Hosting 
 * Interfaces of .NET Framework 4.0 to host a specific version of 
 * CLR in the process, load a .NET assembly, and invoke the types in the 
 * assembly.
 * 
 * The code sample also demonstrates the new In-Process Side-by-Side feature in 
 * .NET Framework 4. The .NET Framework 4 runtime, and all future runtimes, are 
 * able to run in-process with one another. .NET Framework 4 runtime and beyond 
 * are also able to run in-process with any single older runtime. In ther words, 
 * you will be able to load 4, 5 and 2.0 in the same process, but you will not 
 * be able to load 1.1 and 2.0 in the same process. The code sample hosts .NET 
 * runtime 4.0 and 2.0 side by side, and loads a .NET 2.0 assembly into the two 
 * runtimes.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes and Imports
#include <stdio.h>
#include <windows.h>
#pragma endregion


// Two runtime host demo functions implemented in RuntimeHostV4.cpp.

HRESULT RuntimeHostV4Demo1(
    PCWSTR pszVersion, 
    PCWSTR pszAssemblyName, 
    PCWSTR pszClassName);

HRESULT RuntimeHostV4Demo2(
    PCWSTR pszVersion, 
    PCWSTR pszAssemblyPath, 
    PCWSTR pszClassName);


int wmain(int argc, wchar_t *argv[])
{
    // Demonstrate the CLR In-Process Side-by-Side feature that is new in 
    // .NET Framework 4.0.

    // Host the .NET runtime 4.0 and load a .NET 2.0 assembly. This operation 
    // should succeed and the assembly will run in the .NET runtime 4.0.
    RuntimeHostV4Demo1(L"v4.0.30319", L"CSNET2ClassLibrary", 
        L"CSNET2ClassLibrary.CSSimpleObject");

    wprintf(L"\n");

    // Host the .NET runtime 2.0 side by side with the .NET runtime 4.0 and 
    // load a .NET 2.0 assembly. This operation should succeed and the .NET 
    // 2.0 assembly will run in the .NET runtime 2.0.
    RuntimeHostV4Demo1(L"v2.0.50727", L"CSNET2ClassLibrary", 
        L"CSNET2ClassLibrary.CSSimpleObject");


    wprintf(L"\nPresss ENTER to continue ...\n");
    getchar();

    // Demonstrate hosting the .NET runtime 4.0 and using the ICLRRuntimeHost 
    // interface that was provided in .NET v2.0 to load a .NET 4.0 assembly 
    // and invoke its type.
    RuntimeHostV4Demo2(L"v4.0.30319", L"CSClassLibrary.dll", 
        L"CSClassLibrary.CSSimpleObject");

    return 0;
}