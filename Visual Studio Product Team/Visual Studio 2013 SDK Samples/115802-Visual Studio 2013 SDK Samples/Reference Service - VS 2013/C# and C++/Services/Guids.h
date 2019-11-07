/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

/*
Do not use #pragma once, as this file needs to be included twice.  Once to declare the externs
for the GUIDs, and again right after including initguid.h to actually define the GUIDs.
*/

// Guid for the service provider VS Package
// {99831B4A-E492-4bea-9D46-EE583E74FACF}
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_ServiceProviderPackage, 
0x99831b4a, 0xe492, 0x4bea, 0x9d, 0x46, 0xee, 0x58, 0x3e, 0x74, 0xfa, 0xcf);
#endif

// Guid for the local service provider, which is not a VS Package
// {CA55325C-2902-4c48-8361-4B52B65C2441}
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_LocalServiceProvider, 
0xca55325c, 0x2902, 0x4c48, 0x83, 0x61, 0x4b, 0x52, 0xb6, 0x5c, 0x24, 0x41);
#endif

// Guid for the service consumer VS Package
// {0B5D28FC-9064-42c1-AD91-6B6A6DE8F5BC}
#define CLSID_ServiceConsumerPackageDefine { 0xb5d28fc, 0x9064, 0x42c1, { 0xad, 0x91, 0x6b, 0x6a, 0x6d, 0xe8, 0xf5, 0xbc } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_ServiceConsumerPackage, 
0xb5d28fc, 0x9064, 0x42c1, 0xad, 0x91, 0x6b, 0x6a, 0x6d, 0xe8, 0xf5, 0xbc);
#endif

// Guid for the service consumer VS Package Command Set
// {52D7C004-17ED-475c-9777-D66BE9AA88C4}
#define guidConsumerCommandSetDefine { 0x52d7c004, 0x17ed, 0x475c, { 0x97, 0x77, 0xd6, 0x6b, 0xe9, 0xaa, 0x88, 0xc4 } }
#ifdef DEFINE_GUID
DEFINE_GUID(guidConsumerCommandSet, 
0x52d7c004, 0x17ed, 0x475c, 0x97, 0x77, 0xd6, 0x6b, 0xe9, 0xaa, 0x88, 0xc4);
#endif

