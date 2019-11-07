//-----------------------------------------------------------------------
// <copyright file="DeviceSpecific.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      DeviceSpecific.h
//
// Description:
//      This header contains device specific information, for example,
//      how to send data to the XYZ device as opposed to the ABC device.
//
//-----------------------------------------------------------------------


#include "RenderedData.h"


HRESULT DeviceSpecificDisplayBitmap(const CRenderedData RenderedData);


HRESULT DeviceDisplayInitialization(void);
HRESULT DeviceButtonsInitialization(void);
HRESULT DeviceDisplayShutdown(void);
HRESULT DeviceButtonsShutdown(void);
