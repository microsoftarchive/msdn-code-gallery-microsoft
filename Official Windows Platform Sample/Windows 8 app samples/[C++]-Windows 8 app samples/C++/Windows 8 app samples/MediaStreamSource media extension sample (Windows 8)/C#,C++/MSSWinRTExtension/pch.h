//
// pch.h
// Header for standard system include files.
//

#pragma once
#include "MSSWinRTExtension.h"

#include <windows.h>
#include <mfidl.h>
#include <mfapi.h>
#include <mferror.h>
#include <assert.h>

#include <robuffer.h>

#include <wrl\client.h>
#include <wrl\implements.h>
#include <wrl\ftm.h>
#include <wrl\event.h> 
#include <wrl\module.h>
#include <wrl\wrappers\corewrappers.h>

#include <windows.media.h>
#include <windows.storage.streams.h>

#include "CritSec.h"
#include "linklist.h"

using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;

using namespace ABI::Windows::Storage::Streams;
