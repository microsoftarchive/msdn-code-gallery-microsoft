//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var provisioningUtil = {};

(function () {
    "use strict";

    function parseErrorCode(errorCode) {
        var returnStr = "";
        switch (errorCode.toLowerCase()) {
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
    function parseResultXML(resultsXml) {
        try {
            var resultStr = "\nProvisioning Result:";

            var xmlDoc = new Windows.Data.Xml.Dom.XmlDocument;
            xmlDoc.loadXml(resultsXml);

            var errorCodeNodes = xmlDoc.selectNodes("//*[@errorCode != '00000000']");
            if (errorCodeNodes) {

                for (var index = 0; index < errorCodeNodes.length; index++) {
                    var errorCodeNode = errorCodeNodes.item(index);
                    var errorCode = errorCodeNode.getAttribute("errorCode");
                    var nodeName = errorCodeNodes.item(index).nodeName;
                    var description = parseErrorCode(errorCode);

                    switch (nodeName) {
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
                                var messagePosition = errorCodeNode.getAttribute("position");
                                var errorDetails = errorCodeNode.getAttribute("errorDetails");
                                resultStr += "\nError occured during provisioning Message[" + messagePosition + "], error code: " +
                                    errorCode + ", error details: " + errorDetails;
                            }
                            break;
                        case "DefaultProfile":
                        case "PurchaseProfile":
                        case "WLANProfile":
                        case "Plan":
                            {
                                var nameAttribute = errorCodeNode.getAttribute("name");
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

            WinJS.log && WinJS.log(resultStr, "sample", "status");
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
    provisioningUtil.parseResultXML = parseResultXML;
})();
