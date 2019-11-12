//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// PrinterEnumeration.cpp
// Implementation of the printer enumeration code.
//

#include "pch.h"
#include "PrinterEnumeration.h"

using namespace SDKSample;
using namespace SDKSample::DeviceAppForPrinters2;

using namespace Concurrency;

using namespace Platform;
using namespace Platform::Collections;

using namespace Windows::Foundation;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation::Collections;

using namespace Pnp;

/* static */ task<IVector<PrinterInfo^>^>
PrinterEnumeration::EnumeratePrintersAsync(String^ packageFamilyName)
{
    String^ selector("System.Devices.InterfaceClassGuid:=\"{0ecef634-6ef0-472a-8085-5ad023ecbccd}\"");

    // Look for the container ID and printer name for each device interface.
    auto propertiesToRetrieve = ref new Vector<String^>();
    propertiesToRetrieve->Append("System.Devices.ContainerId");
    propertiesToRetrieve->Append("System.DeviceInterface.PrinterName");

    // Create an async task to look for all the printer device interfaces.
    // Once the task returns, it will enumerate printers associated with the WSDA
    return create_task(DeviceInformation::FindAllAsync(selector, propertiesToRetrieve))
    .then([packageFamilyName] (DeviceInformationCollection^ printerDeviceInterfaceCollection)
    {
        PrinterEnumeration^ pe = ref new PrinterEnumeration(packageFamilyName);
        return pe->EnumerateAssociatedPrintersAsync(printerDeviceInterfaceCollection);
    })
    .then([] (IVector<PrinterInfo^>^ printQueueCollection)
    {
        return printQueueCollection;
    });
}

task<IVector<PrinterInfo^>^>
PrinterEnumeration::EnumerateAssociatedPrintersAsync(DeviceInformationCollection^ deviceInfoCollection)
{
    // Look for the app package family name for this device container.
    auto propertiesToRetrieve = ref new Vector<String^>();
    propertiesToRetrieve->Append("System.Devices.AppPackageFamilyName");

    // For each deviceInfo object in the collection, create a task that asynchronously retrieves its device
    // container and inspects it to determine if it is associated with the device app.
    std::vector<task<void>> tasks;

    for (DeviceInformation^ deviceInfo : deviceInfoCollection)
    {
        // Get the device container Id, printer name and interface Id.
        String^ containerId = safe_cast<Guid>(deviceInfo->Properties->Lookup("System.Devices.ContainerId")).ToString();
        String^ printerName = safe_cast<String^>(deviceInfo->Properties->Lookup("System.DeviceInterface.PrinterName"));
        String^ deviceId = deviceInfo->Id;

        auto singleTask = create_task(PnpObject::CreateFromIdAsync(PnpObjectType::DeviceContainer, containerId, propertiesToRetrieve))
        .then([this, deviceId, printerName] (Pnp::PnpObject^ deviceContainer)
        {
            // Check if the app package name for this container is a match.
            bool isMatch = IsContainerRegisteredWithDeviceApp(deviceContainer);
            if (isMatch)
            {
                printQueueCollection->Append(ref new PrinterInfo(printerName, deviceId));
            }
        });

        tasks.push_back(singleTask);
    }

    // When all the above tasks complete asynchronously, return the print queue collection to the caller.
    return when_all(tasks.begin(), tasks.end())
    .then([this] () -> IVector<PrinterInfo^>^
    {
        return printQueueCollection;
    });
}

bool
PrinterEnumeration::IsContainerRegisteredWithDeviceApp(Pnp::PnpObject^ deviceContainer)
{
    bool isMatch = false;

    // Look up the package family name which is an IPropertyValue.
    IPropertyValue^ packageFamilyNameObject =
        safe_cast<IPropertyValue^>(deviceContainer->Properties->Lookup("System.Devices.AppPackageFamilyName"));
    if (packageFamilyNameObject != nullptr)
    {
        // Convert property value to a string array.
        Platform::Array<Platform::String^>^ packageFamilyNameArray;
        packageFamilyNameObject->GetStringArray(&packageFamilyNameArray);

        if (packageFamilyNameArray != nullptr)
        {
            // Iterate through the string array.
            for (uint32 i = 0; i < packageFamilyNameArray->Length; i++)
            {
                // Compare the package family name with this app.
                Platform::String^ packageFamilyName = packageFamilyNameArray->Data[i];
                if (this->packageFamilyName == packageFamilyName)
                {
                    isMatch = true;
                    break;
                }
            }
        }
    }

    return isMatch;
}

