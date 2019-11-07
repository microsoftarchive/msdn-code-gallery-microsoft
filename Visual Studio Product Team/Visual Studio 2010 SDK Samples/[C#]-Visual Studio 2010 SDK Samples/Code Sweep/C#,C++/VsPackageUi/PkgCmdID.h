/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

// PkgCmdID.h
// Command IDs used in defining command bars
//

// do not use #pragma once - used by ctc compiler
#ifndef __PKGCMDID_H_
#define __PKGCMDID_H_

///////////////////////////////////////////////////////////////////////////////
// Menu IDs



///////////////////////////////////////////////////////////////////////////////
// Menu Group IDs

#define IDG_CODESWEEP_CONFIG       0x1020
#define IDM_CODESWEEP_TASKLIST     0x2020
#define IDG_CODESWEEP_TASKLIST     0x2021

///////////////////////////////////////////////////////////////////////////////
// Command IDs

#define cmdidConfig 0x100
#define cmdidStopScan 0x101
#define cmdidRepeatLastScan 0x102
#define cmdidIgnore 0x103
#define cmdidDoNotIgnore 0x104
#define cmdidShowIgnoredInstances 0x105



///////////////////////////////////////////////////////////////////////////////
// Bitmap IDs
#define bmpCodeSweep 1
#define bmpStop 2
#define bmpRepeat 3
#define bmpIgnore 4
#define bmpShowIgnored 5


#endif // __PKGCMDID_H_
