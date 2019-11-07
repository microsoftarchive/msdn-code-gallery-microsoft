 // Copyright (C) Microsoft Corporation.  All rights reserved.

// cominteropPart2\COMClient.cpp
// Build this part of the sample with the command line:
//  cl COMClient.cpp

#include <windows.h>
#include <stdio.h>

#pragma warning (disable: 4278)

// To use managed-code servers like the C# server,
// we have to import the common language runtime:
#import <mscorlib.tlb> raw_interfaces_only

// For simplicity, we ignore the server namespace and use named guids:
#if defined (USINGPROJECTSYSTEM)
#import "..\RegisterCSharpServerAndExportTLB\CSharpServer.tlb" no_namespace named_guids
#else  // Compiling from the command line, all files in the same directory
#import "CSharpServer.tlb" no_namespace named_guids
#endif
int main(int argc, char* argv[])
{
    IManagedInterface *cpi = NULL;
    int retval = 1;

    // Initialize COM and create an instance of the InterfaceImplementation class:
    CoInitialize(NULL);
    HRESULT hr = CoCreateInstance(CLSID_InterfaceImplementation,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IManagedInterface,
                                  reinterpret_cast<void**>(&cpi));

    if (FAILED(hr))
    {
        printf("Couldn't create the instance!... 0x%x\n", hr);
    }
    else
    {
        if (argc > 1)
        {
            printf("Calling function.\n");
            // The variable cpi now holds an interface pointer          // to the managed interface.
            // If you are on an OS that uses ASCII characters at the          // command prompt, notice that the ASCII characters are
            // automatically marshaled to Unicode for the C# code.

            if (cpi->PrintHi(argv[1]) == 33)
                retval = 0;

            printf("Returned from function.\n");
        }
        else
            printf ("Usage:  COMClient <name>\n");
        cpi->Release();
        cpi = NULL;
    }

    // Be a good citizen and clean up COM:
    CoUninitialize();
    return retval;
}

