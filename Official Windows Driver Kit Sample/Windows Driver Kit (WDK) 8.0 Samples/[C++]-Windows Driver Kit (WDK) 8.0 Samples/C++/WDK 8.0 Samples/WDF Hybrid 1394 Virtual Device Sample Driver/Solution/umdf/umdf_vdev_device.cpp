/*++

Copyright (C) Microsoft Corporation, All Rights Reserved.

Module Name:

umdf_vdev_device.cpp

Abstract:

This module contains the implementation of the UMDF 1394 hybrid sample driver's
device callback object.


Environment:

Windows User-Mode Driver Framework (WUDF)

--*/

#include <initguid.h>
#include "umdf_vdev.h"
#include "wdf_common.h"
#include "umdf_vdev_device.tmh"


//
// This is a false positive in PreFast
// The object (device) uses the COM model of release
// upon the refcount reaching 0, so the object is then freed from 
// memory.
//
#pragma warning (disable: 28197 )


CVDevDevice::~CVDevDevice ()
{
}

HRESULT
CVDevDevice::CreateInstance ( 
                             _In_ IWDFDriver *FxDriver,
                             _In_ IWDFDeviceInitialize * FxDeviceInit,
                             _Out_ PCVDevDevice *Device)
/*++

Routine Description:

    This method creates and initializs an instance of the hybrid driver's 
    device callback object.

Arguments:
    FxDriver -  Framework driver interface.  
    FxDeviceInit - the settings for the device.
    Device - a location to store the referenced pointer to the device object.

Return Value:
    HRESTULT indication of success or failure

--*/
{
    PCVDevDevice vDevice;
    HRESULT hr;

    Enter();

    vDevice = new CVDevDevice ();

    if (NULL == vDevice)
    {
        return E_OUTOFMEMORY;
    }


    hr = vDevice->Initialize (FxDriver, FxDeviceInit);

    if (SUCCEEDED (hr)) 
    {
        *Device = vDevice;
    } 
    else 
    {
        vDevice->Release ();
    }

    ExitHR(hr);
    return hr;
}

HRESULT
CVDevDevice::Initialize ( 
                         _In_ IWDFDriver           * FxDriver,
                         _In_ IWDFDeviceInitialize * FxDeviceInit)
/*++

    Routine Description:
        This method initializes the device callback object and creates the
        partner device object.
        
    Arguments:
        FxDeviceInit - the settings for this device.

    Return Value:
        HRESTULT indication of success or failure

--*/
{
    IWDFDevice * fxDevice = NULL;

    HRESULT hr = S_OK;

    Enter();

    FxDeviceInit->SetLockingConstraint (None);
    FxDeviceInit->SetFilter();

    {
        IUnknown * unknown = this->QueryIUnknown ();

        hr = FxDriver->CreateDevice (FxDeviceInit, unknown, &fxDevice);

        unknown->Release ();
    }

    if (SUCCEEDED (hr)) 
    {
        m_FxDevice = fxDevice;

        //
        // We can release the reference as the lifespan is tied to the 
        // framework object.
        //
        fxDevice->Release();
    }

    ExitHR(hr);
    return hr;
}

HRESULT
CVDevDevice::Configure (VOID)
/*++

    Routine Description:
        This method is called after the device callback object has been initialized 
        and returned to the driver.  It would setup the device's queues and their 
        corresponding callback objects.

    Arguments:
        None
        
    Return Value:
        HRESTULT indication of success or failure

--*/
{   
    HRESULT hr = S_OK;

    Enter();

    //
    // Create the control queues and configure forwarding for IOCTL requests.
    //
    hr = CVDevParallelQueue::CreateInstance (
        this, 
        m_FxDevice, 
        &m_ParallelQueue);

    if (SUCCEEDED (hr)) 
    {       
        m_ParallelQueue->Release ();
    }
    else
    {
        ExitHR(hr);
        return hr;
    }


    hr = CVDevSequentialQueue::CreateInstance (
        this, 
        m_FxDevice, 
        &m_SequentialQueue);

    if (SUCCEEDED (hr)) 
    {
        m_SequentialQueue->Release ();
    }
    else
    {
        ExitHR(hr);
        return hr;
    }


    hr = m_FxDevice->CreateDeviceInterface (&GUID_UMDF_VDEV_HYBRID, NULL);

    ExitHR(hr);
    return hr;
}

HRESULT
CVDevDevice::QueryInterface (
                             _In_ REFIID InterfaceId,
                             _Outptr_ PVOID *Object)
/*++

    Routine Description:
        This method is called to get a pointer to one of the object's callback
        interfaces.  
        
    Arguments:
        InterfaceId - the interface being requested
        Object - a location to store the interface pointer if successful

    Return Value:
        S_OK or E_NOINTERFACE

--*/
{
    HRESULT hr;

    Enter();

    if (IsEqualIID(InterfaceId, __uuidof (IPnpCallbackHardware))) 
    {
        *Object = HardwareCallback ();
        hr = S_OK;
    }
    else
    {
        hr = CUnknown::QueryInterface (InterfaceId, Object);
    }

    ExitHR(hr);
    return hr;
}

HRESULT
CVDevDevice::OnReleaseHardware (
                                _In_ IWDFDevice * wdfDevice)
{

    UNREFERENCED_PARAMETER (wdfDevice);

    Enter();

    //
    // TODO: Well handle resource clean up here
    //

    ExitHR(S_OK);
    return S_OK;
}
                               



