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



// package guid
// { cc69a0bd-0289-4a6e-9c20-96a186f7242a }
#define guidPackagePkg { 0xCC69A0BD, 0x289, 0x4A6E, { 0x9C, 0x20, 0x96, 0xA1, 0x86, 0xF7, 0x24, 0x2A } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_Package,
0xCC69A0BD, 0x289, 0x4A6E, 0x9C, 0x20, 0x96, 0xA1, 0x86, 0xF7, 0x24, 0x2A );
#endif

// Command set guid for our commands (used with IOleCommandTarget)
// { 2de6da59-08d0-4ee1-abd9-bd58ca6db684 }
#define guidPackageCmdSet { 0x2DE6DA59, 0x8D0, 0x4EE1, { 0xAB, 0xD9, 0xBD, 0x58, 0xCA, 0x6D, 0xB6, 0x84 } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_PackageCmdSet, 
0x2DE6DA59, 0x8D0, 0x4EE1, 0xAB, 0xD9, 0xBD, 0x58, 0xCA, 0x6D, 0xB6, 0x84 );
#endif

//Guid for the image list referenced in the VSCT file
// { 34997c5c-b223-4ce4-935a-9279cd5d6595 }
#define guidImages { 0x34997C5C, 0xB223, 0x4CE4, { 0x93, 0x5A, 0x92, 0x79, 0xCD, 0x5D, 0x65, 0x95 } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_Images, 
0x34997C5C, 0xB223, 0x4CE4, 0x93, 0x5A, 0x92, 0x79, 0xCD, 0x5D, 0x65, 0x95 );
#endif


