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
// Implementation of the Util class
//

#include "pch.h"
#include "Util.h"

using namespace SDKSample::ProvisioningAgent;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;

Util::Util()
{
}

String^ Util::ParseErrorCode(String^ errorCode)
{
    String^ returnStr = L"";

    if (_wcsicmp(errorCode->Data(), L"80070490") == 0) // ERROR_NOT_FOUND
    {
        returnStr += L", description of error \"The given interface or subscriber ID is not present\", error code: "
            + errorCode;
    }
    else if (_wcsicmp(errorCode->Data(), L"80070426") == 0) // ERROR_SERVICE_NOT_ACTIVE
    {
        returnStr += L", description of error \"The service has not been started\", error code: "
            + errorCode;
    }
    else if (_wcsicmp(errorCode->Data(), L"80070015") == 0) // ERROR_NOT_READY
    {
        returnStr += L", description of error \"The device is not ready\", error code: "
            + errorCode;
    }
    else if (_wcsicmp(errorCode->Data(), L"80070037") == 0) // ERROR_DEV_NOT_EXIST
    {
        returnStr += L", description of error \"The specified network resource or device is no longer available\", error code: "
            + errorCode;
    }
    else if (_wcsicmp(errorCode->Data(), L"800704b6") == 0)// ERROR_BAD_PROFILE
    {
        returnStr += L", description of error \"The network connection profile is corrupted\", error code: "
            + errorCode;
    }
    else
    {
        returnStr += L", error code: " + errorCode;
    }

    return returnStr;
}

/* *
* * Provisiong result XML looks like this:
* *   <?xml version="1.0"?>
* *   <CarrierProvisioningResults>
* *       <MBNProfiles>
* *           <DefaultProfile name="Foo1" errorCode="800704b6" />
* *           <PurchaseProfile name="Foo2" errorCode="00000000 />
* *           <Messages>
* *                <Message position="1" errorCode="82170008" errorDetails="error description" />
* *                <Message position="2" errorCode="00000000" errorDetails="error description" />
* *                <Message position="3" errorCode="82170008" errorDetails="error description" />
* *           </Messages>
* *       </MBNProfiles>
* *       <WLANProfiles errorCode="80070426" />
* *       <Provisioning errorCode="82170008" />
* *       <Plans>
* *           <Plan name="PlanA" errorCode="00000000" />
* *           <Plan name="PlanB" errorCode="82170012" />
* *           <Plan name="PlanC" errorCode="00000000" />
* *       </Plans>
* *   </CarrierProvisioningResults>
* */
String^ Util::ParseResultXML(String^ resultsXml)
{
    String^ resultStr = L"\nProvisioning Result:";

    auto xmlDoc = ref new XmlDocument();
    xmlDoc->LoadXml(resultsXml);

    auto errorCodeNodes = xmlDoc->SelectNodes(L"//*[@errorCode != '00000000']");
    if (errorCodeNodes->Length != 0)
    {

        for (unsigned int index = 0; index < errorCodeNodes->Length; index++)
        {
            auto errorCodeNode = errorCodeNodes->Item(index);
            auto errorCode = ((errorCodeNode->Attributes)->GetNamedItem(L"errorCode"))->NodeValue;
            auto nodeName = (errorCodeNodes->Item(index))->NodeName;
            auto description = ParseErrorCode(dynamic_cast<String^>(errorCode));

            if ((_wcsicmp(nodeName->Data(), L"MBNProfiles") == 0) ||
                (_wcsicmp(nodeName->Data(), L"WLANProfiles") == 0) ||
                (_wcsicmp(nodeName->Data(), L"Plans") == 0) ||
                (_wcsicmp(nodeName->Data(), L"Provisioning") == 0))
            {
                resultStr += L"\nError occured during provisioning at top level node \"" + nodeName +
                    L"\", and hence there will be no child node attached to this node" + description;
            }
            else if (_wcsicmp(nodeName->Data(), L"Message") == 0)
            {
                auto messagePosition = ((errorCodeNode->Attributes)->GetNamedItem(L"position"))->NodeValue;
                auto errorDetails = ((errorCodeNode->Attributes)->GetNamedItem(L"errorDetails"))->NodeValue;
                resultStr += "\nError occured during provisioning Message[" + messagePosition + "], error code: " +
                    errorCode + ", error details: " + errorDetails;
            }
            else if ((_wcsicmp(nodeName->Data(), L"DefaultProfile") == 0) ||
                (_wcsicmp(nodeName->Data(), L"PurchaseProfile") == 0) ||
                (_wcsicmp(nodeName->Data(), L"WLANProfile") == 0) ||
                (_wcsicmp(nodeName->Data(), L"Plan") == 0))
            {
                auto nameAttribute = ((errorCodeNode->Attributes)->GetNamedItem(L"name"))->NodeValue;
                resultStr += "\nError occured during provisioning " + nodeName + ", name: " +
                    nameAttribute + description;
            }
            else
            {
                resultStr += "\nError occured during provisioning " + nodeName + ", error code: " + errorCode;
            }
        }
    }

    return resultStr;
}

String^ Util::PrintExceptionCode(Exception^ error)
{
    String^ errorDescription = "";

    wchar_t buf[5000];
    HRESULT hr = S_OK;

    if (error->Message)
    {
        hr = StringCchPrintf(buf, ARRAYSIZE(buf), L"Unexpected exception occured:%s, error number: 0x%x", error->Message->Data(), error->HResult);
    }
    else
    {
        hr = StringCchPrintf(buf, ARRAYSIZE(buf), L"Unexpected exception occured, error number: 0x%x", error->HResult);
    }

    if (FAILED(hr))
    {
        throw Exception::CreateException(hr);
    }
    errorDescription = ref new String(buf);

    return errorDescription;
}
