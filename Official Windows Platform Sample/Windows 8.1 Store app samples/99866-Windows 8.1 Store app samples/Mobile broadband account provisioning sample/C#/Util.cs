//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Xml.Dom;
using SDKTemplate;
using System;

namespace ProvisioningAgent
{
    public class Util
    {
        public Util()
        {
        }

        public string ParseErrorCode(string errorCode)
        {
            string returnStr = "";

            switch (errorCode.ToLower())
            {
                case "80070490": // ERROR_NOT_FOUND
                    returnStr += ", description of error \"The given interface or subscriber ID is not present\", error code: "
                        + errorCode;
                    break;
                case "80070426": // ERROR_SERVICE_NOT_ACTIVE
                    returnStr += ", description of error \"The service has not been started\", error code: "
                        + errorCode;
                    break;
                case "80070015": // ERROR_NOT_READY
                    returnStr += ", description of error \"The device is not ready\", error code: "
                        + errorCode;
                    break;
                case "80070037": // ERROR_DEV_NOT_EXIST
                    returnStr += ", description of error \"The specified network resource or device is no longer available\", error code: "
                        + errorCode;
                    break;
                case "800704b6": // ERROR_BAD_PROFILE
                    returnStr += ", description of error \"The network connection profile is corrupted\", error code: "
                        + errorCode;
                    break;
                default:
                    returnStr += ", error code: " + errorCode;
                    break;
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
        public string ParseResultXML(string resultsXml)
        {
            string resultStr = "\nProvisioning Result:";

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resultsXml);

            var errorCodeNodes = xmlDoc.SelectNodes("//*[@errorCode != '00000000']");
            if (errorCodeNodes.Length != 0)
            {

                for (uint index = 0; index < errorCodeNodes.Length; index++)
                {
                    var errorCodeNode = errorCodeNodes.Item(index);
                    var errorCode = errorCodeNode.Attributes.GetNamedItem("errorCode").NodeValue;
                    var nodeName = errorCodeNodes.Item(index).NodeName;
                    var description = ParseErrorCode((string)errorCode);

                    switch (nodeName)
                    {
                        case "MBNProfiles":
                        case "WLANProfiles":
                        case "Plans":
                        case "Provisioning":
                            {
                                resultStr += "\nError occured during provisioning at top level node \"" + nodeName +
                                    "\", and hence there will be no child node attached to this node" + description;
                            }
                            break;
                        case "Message":
                            {
                                //var messagePosition = errorCodeNode.GetAttribute("position");
                                //var errorDetails = errorCodeNode.GetAttribute("errorDetails");
                                var messagePosition = errorCodeNode.Attributes.GetNamedItem("position").NodeValue;
                                var errorDetails = errorCodeNode.Attributes.GetNamedItem("errorDetails").NodeValue;
                                resultStr += "\nError occured during provisioning Message[" + messagePosition + "], error code: " +
                                    errorCode + ", error details: " + errorDetails;
                            }
                            break;
                        case "DefaultProfile":
                        case "PurchaseProfile":
                        case "WLANProfile":
                        case "Plan":
                            {
                                //var nameAttribute = errorCodeNode.GetAttribute("name");
                                var nameAttribute = errorCodeNode.Attributes.GetNamedItem("name").NodeValue;
                                resultStr += "\nError occured during provisioning " + nodeName + ", name: " +
                                    nameAttribute + description;
                            }
                            break;
                        default:
                            resultStr += "\nError occured during provisioning " + nodeName + ", error code: " + errorCode;
                            break;
                    }
                }
            }
            return resultStr;

        }
    }
}
