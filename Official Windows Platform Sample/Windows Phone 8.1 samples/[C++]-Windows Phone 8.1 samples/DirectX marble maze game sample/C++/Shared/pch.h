//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <wrl.h>
#include <wrl/client.h>

#include <dxgi1_3.h>
#include <d3d11_2.h>
#include <d2d1_2.h>
#include <d2d1effects_1.h>
#include <dwrite_2.h>

#include <wincodec.h>
#include <memory>
#include <agile.h>
#include <ppltasks.h>

#define XM_STRICT_VECTOR4 1
#include <DirectXMath.h>
#include <DirectXPackedVector.h>
#include <DirectXColors.h>
#include <DirectXCollision.h>

#define XAUDIO2_HELPER_FUNCTIONS 1
#include <xaudio2.h>
#include <xaudio2fx.h>

#include <mmreg.h>
#include <mfidl.h>
#include <mfapi.h>
#include <mfreadwrite.h>
#include <mfmediaengine.h>

#if WINAPI_FAMILY != WINAPI_FAMILY_PHONE_APP
// XINPUT is not available on phone.
#include <XInput.h> 
#endif

#include <stdio.h>
#include <strsafe.h>
#include <vector>
#include <memory>
#include <map>
#include <queue>
#include <string>
#include <set>

