//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#ifndef Assert
#if defined(DEBUG) || defined(_DEBUG)
#define Assert(b) if (!(b)) { OutputDebugStringA("Assert: " #b "\n"); }
#else
#define Assert(b)
#endif // DEBUG || _DEBUG
#endif

#include <wrl.h>
#include <d3d11_1.h>
#include <d2d1_1.h>
#include <d2d1effects.h>
#include <dwrite_1.h>
#include <wincodec.h>
#include <math.h>
#include <DirectXMath.h>
#include <ppltasks.h>
#include <shcore.h>
#include <collection.h>
#include <assert.h>
#include <agile.h>

#include <list>
#include <vector>

#include "BasicMath.h"
#include "BasicCamera.h"
#include "BasicLoader.h"
#include "BasicShapes.h"
#include "BasicReaderWriter.h"
#include "DDSTextureLoader.h"

#include "DirectXSample.h"
#include "windows.ui.xaml.media.dxinterop.h"
