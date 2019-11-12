//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::CustomUsbDeviceAccess;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Connecting To Device", "SDKSample.CustomUsbDeviceAccess.DeviceConnect" }, 
    { "Control Transfer", "SDKSample.CustomUsbDeviceAccess.ControlTransfer" },
    { "Interrupt Pipes", "SDKSample.CustomUsbDeviceAccess.InterruptPipes" },
    { "Bulk Pipes", "SDKSample.CustomUsbDeviceAccess.BulkPipes" },
    { "Usb Descriptors", "SDKSample.CustomUsbDeviceAccess.UsbDescriptors" },
    { "Interface Settings", "SDKSample.CustomUsbDeviceAccess.InterfaceSettings" },
    { "Sync with Device", "SDKSample.CustomUsbDeviceAccess.SyncDevice" }

}; 
