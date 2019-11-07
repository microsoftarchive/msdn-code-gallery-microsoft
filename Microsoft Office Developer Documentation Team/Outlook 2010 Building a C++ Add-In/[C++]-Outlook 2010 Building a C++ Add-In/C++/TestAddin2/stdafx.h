// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#ifndef STRICT
#define STRICT
#endif

#include "targetver.h"

#define _ATL_APARTMENT_THREADED
#define _ATL_NO_AUTOMATIC_NAMESPACE

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS	// some CString constructors will be explicit

#include "resource.h"
#include <atlbase.h>
#include <atlcom.h>
#include <atlctl.h>
#include <msxml2.h>
#include  <Mshtml.h>
#include 	<Winhttp.h>
#include <atlstr.h>
#include <atlsafe.h>
//#import "C:\Program Files\Common Files\DESIGNER\MSADDNDR.DLL" raw_interfaces_only, raw_native_types, no_namespace, named_guids, auto_search ,auto_rename
//#import "C:\Program Files\Microsoft Office\Office14\MSOUTL.OLB" raw_interfaces_only, raw_native_types,  named_guids, auto_search, auto_rename,rename_namespace("Outlook")
//#import "C:\Program Files\Common Files\Microsoft Shared\OFFICE14\MSO.DLL" raw_interfaces_only, named_guids, auto_search, auto_rename,rename_namespace("Office")

// Office type library (i.e. mso.dll)
#import "libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52"\
	auto_rename auto_search raw_interfaces_only rename_namespace("Office")
// Outlook type library (i.e. msoutl.olb)
#import "libid:00062FFF-0000-0000-C000-000000000046"\
	auto_rename auto_search raw_interfaces_only rename_namespace("Outlook")

// Forms type library (i.e. fm20.dll)
#import "libid:0D452EE1-E08F-101A-852E-02608C4D0BB4"\
	auto_rename auto_search raw_interfaces_only  rename_namespace("Forms")
using namespace Outlook;
using namespace Office;
using namespace Forms;
//add in type library 
#import "libid:AC0714F2-3D04-11D1-AE7D-00A0C90F26F4"\
	auto_rename auto_search raw_interfaces_only rename_namespace("AddinDesign")
using namespace AddinDesign;
#import "libid:00020905-0000-0000-C000-000000000046"\
	auto_rename auto_search raw_interfaces_only rename_namespace("Word")
////version("1.0") lcid("0")  raw_interfaces_only named_guids
//
//using namespace Word;


using namespace ATL;

extern "C" const GUID ;