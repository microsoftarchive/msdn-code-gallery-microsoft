/*++
    Copyright (c) 2000,2002 Microsoft Corporation

    Module Name:
        HBtnKey.h

    Abstract:
        Contains OEM specific definitions.

    Environment:
        Kernel mode

--*/

#ifndef _HBTNKEY_H
#define _HBTNKEY_H

//
// Constants
//
#define HBTNF_KBD_CONNECTED             0x80000000
#define HBTNF_KBD_E1                    0x40000000
#define HBTNF_KBD_E0                    0x20000000

#define OEM_VENDOR_ID                   0x3647          //"MST"
#define OEM_PRODUCT_ID                  0x0001
#define OEM_VERSION_NUM                 1

#define REPORTID_BTN                    1
#define REPORTID_SAS                    2
#ifdef SIMULATE_KBD
#define REPORTID_KBD                    3
#define REPORTID_LED                    4
#endif

#define SASF_DELETE                     0x01
#define SASF_LEFT_CTRL                  0x02
#define SASF_LEFT_ALT                   0x04
#define SASF_CAD                        (SASF_DELETE |          \
                                         SASF_LEFT_CTRL |       \
                                         SASF_LEFT_ALT)

//
// Type definitions
//

//
// This must match the report descriptor, so make sure it is byte align.
//
#if _MSC_VER >= 1200

#pragma warning(push)

#endif

#pragma warning(disable:4201) // nameless struct/union

#include <pshpack1.h>
typedef struct _OEM_BTN_REPORT
{
    ULONG dwButtonStates;
} OEM_BTN_REPORT, *POEM_BTN_REPORT;

typedef struct _OEM_SAS_REPORT
{
    UCHAR bKeys;
    UCHAR abReserved[3];
} OEM_SAS_REPORT, *POEM_SAS_REPORT;

#ifdef SIMULATE_KBD
typedef struct _OEM_KBD_REPORT
{
    UCHAR bModifiers;
    UCHAR abKeys[6];
} OEM_KBD_REPORT, *POEM_KBD_REPORT;

typedef struct _OEM_LED_REPORT
{
    UCHAR bLEDStates;
} OEM_LED_REPORT, *POEM_LED_REPORT;

typedef struct _HID_OUTPUT_REPORT
{
    UCHAR  ReportID;
    OEM_LED_REPORT LEDReport;
} HID_OUTPUT_REPORT, *PHID_OUTPUT_REPORT;
#endif

typedef struct _HID_INPUT_REPORT
{
    UCHAR  ReportID;
    union
    {
        OEM_BTN_REPORT BtnReport;
        OEM_SAS_REPORT SASReport;
#ifdef SIMULATE_KBD
        OEM_KBD_REPORT KbdReport;
#endif
    };
} HID_INPUT_REPORT, *PHID_INPUT_REPORT;

#if _MSC_VER >= 1200

#pragma warning(pop)

#else
#pragma warning(default:4201)
#endif

typedef struct _OEM_EXTENSION
{
    ULONG          dwLastButtons;
#ifdef SIMULATE_KBD
    OEM_KBD_REPORT KbdReport;
#endif
} OEM_EXTENSION, *POEM_EXTENSION;
#include <poppack.h>

//
// Global Data Declarations
//
extern UCHAR gReportDescriptor[];
extern ULONG gdwcbReportDescriptor;
extern HID_DESCRIPTOR gHidDescriptor;
extern PWSTR gpwstrManufacturerID;
extern PWSTR gpwstrProductID;
extern PWSTR gpwstrSerialNumber;

#endif  //ifndef _HBTNKEY_H

