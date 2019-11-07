/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HealthHeartRateService.h
    
Abstract:

--*/

#pragma once

class HealthHeartRateService : public AbstractGattService
{
public:
    HealthHeartRateService();

    ~HealthHeartRateService();

    HRESULT OnMethodInvoke(
        _In_    REFGUID                Method,
        _In_    IPortableDeviceValues* pParams,
        _Out_   IPortableDeviceValues* pResults);

    VOID HeartRateMeasurementEvent(
        _In_ BTH_LE_GATT_EVENT_TYPE EventType,
        _In_ PVOID EventOutParameter
        );    

};



