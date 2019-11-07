//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#ifndef UIA_CLIENT_CONSTANTS
#define UIA_CLIENT_CONSTANTS

// Defining some useful UIA constants.
// These are drawn from UIAutomationClient.idl, which is part of the Desktop SDK
// but not available for use here.
const long UIA_SelectionPatternId          = 10001;
const long UIA_ValuePatternId              = 10002;
const long UIA_SelectionItemPatternId      = 10010;

const long UIA_AutomationFocusChangedEventId                  = 20005;
const long UIA_SelectionItem_ElementSelectedEventId           = 20012;

const long UIA_ControlTypePropertyId                          = 30003;
const long UIA_LocalizedControlTypePropertyId                 = 30004;
const long UIA_NamePropertyId                                 = 30005;
const long UIA_HasKeyboardFocusPropertyId                     = 30008;
const long UIA_IsKeyboardFocusablePropertyId                  = 30009;
const long UIA_IsEnabledPropertyId                            = 30010;
const long UIA_AutomationIdPropertyId                         = 30011;
const long UIA_HelpTextPropertyId                             = 30013;
const long UIA_IsControlElementPropertyId                     = 30016;
const long UIA_IsContentElementPropertyId                     = 30017;
const long UIA_IsPasswordPropertyId                           = 30019;
const long UIA_ValueValuePropertyId                           = 30045;
const long UIA_ProviderDescriptionPropertyId                  = 30107;

const long UIA_CustomControlTypeId        = 50025;
const long UIA_WindowControlTypeId        = 50032;

#endif

#include <wrl.h>
#include <d3d11_1.h>
#include <d2d1_1.h>
#include <d2d1effects.h>
#include <dwrite_1.h>
#include <wincodec.h>
#include <strsafe.h>

