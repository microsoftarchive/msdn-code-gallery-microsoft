'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Data.Xml.Dom
Imports SDKTemplate

Public Class Util
    Public Sub New
    End Sub

    Public Function ParseErrorCode(errorCode As String) As String
        Dim returnStr As String = ""

        Select Case errorCode.ToLower
            Case "80070490"
                ' ERROR_NOT_FOUND
                returnStr &= ", description of error ""The given interface or subscriber ID is not present"", error code: " & errorCode
                Exit Select
            Case "80070426"
                ' ERROR_SERVICE_NOT_ACTIVE
                returnStr &= ", description of error ""The service has not been started"", error code: " & errorCode
                Exit Select
            Case "80070015"
                ' ERROR_NOT_READY
                returnStr &= ", description of error ""The device is not ready"", error code: " & errorCode
                Exit Select
            Case "80070037"
                ' ERROR_DEV_NOT_EXIST
                returnStr &= ", description of error ""The specified network resource or device is no longer available"", error code: " & errorCode
                Exit Select
            Case "800704b6"
                ' ERROR_BAD_PROFILE
                returnStr &= ", description of error ""The network connection profile is corrupted"", error code: " & errorCode
                Exit Select
            Case Else
                returnStr &= ", error code: " & errorCode
                Exit Select
        End Select
        Return returnStr
    End Function

    ' *
    '        * * Provisiong result XML looks like this:
    '        * *   <?xml version="1.0"?>
    '        * *   <CarrierProvisioningResults>
    '        * *       <MBNProfiles>
    '        * *           <DefaultProfile name="Foo1" errorCode="800704b6" />
    '        * *           <PurchaseProfile name="Foo2" errorCode="00000000 />
    '        * *           <Messages>
    '        * *                <Message position="1" errorCode="82170008" errorDetails="error description" />
    '        * *                <Message position="2" errorCode="00000000" errorDetails="error description" />
    '        * *                <Message position="3" errorCode="82170008" errorDetails="error description" />
    '        * *           </Messages>
    '        * *       </MBNProfiles>
    '        * *       <WLANProfiles errorCode="80070426" />
    '        * *       <Provisioning errorCode="82170008" />
    '        * *       <Plans>
    '        * *           <Plan name="PlanA" errorCode="00000000" />
    '        * *           <Plan name="PlanB" errorCode="82170012" />
    '        * *           <Plan name="PlanC" errorCode="00000000" />
    '        * *       </Plans>
    '        * *   </CarrierProvisioningResults>
    '        *

    Public Function ParseResultXML(resultsXml As String) As String
        Dim resultStr As String = vbCrLf & "Provisioning Result:"

        Dim xmlDoc As New XmlDocument
        xmlDoc.LoadXml(resultsXml)

        Dim errorCodeNodes = xmlDoc.SelectNodes("//*[@errorCode != '00000000']")
        If errorCodeNodes.Length <> 0 Then

            For index As UInteger = 0 To errorCodeNodes.Length - 1
                Dim errorCodeNode = errorCodeNodes.ElementAt(index)
                Dim errorCode = errorCodeNode.Attributes.GetNamedItem("errorCode").NodeValue
                Dim nodeName = errorCodeNodes.ElementAt(index).NodeName
                Dim description = ParseErrorCode(DirectCast(errorCode, String))

                Select Case nodeName
                    Case "MBNProfiles", "WLANProfiles", "Plans", "Provisioning"
                        If True Then
                            resultStr &= vbCrLf & "Error occured during provisioning at top level node """ & nodeName & """, and hence there will be no child node attached to this node" & description
                        End If
                        Exit Select
                    Case "Message"
                        If True Then
                            'var messagePosition = errorCodeNode.GetAttribute("position");
                            'var errorDetails = errorCodeNode.GetAttribute("errorDetails");
                            Dim messagePosition = errorCodeNode.Attributes.GetNamedItem("position").NodeValue
                            Dim errorDetails = errorCodeNode.Attributes.GetNamedItem("errorDetails").NodeValue
                            resultStr &= vbCrLf & "Error occured during provisioning Message[" & messagePosition.ToString & "], error code: " & errorCode & ", error details: " & errorDetails.ToString
                        End If
                        Exit Select
                    Case "DefaultProfile", "PurchaseProfile", "WLANProfile", "Plan"
                        If True Then
                            'var nameAttribute = errorCodeNode.GetAttribute("name");
                            Dim nameAttribute = errorCodeNode.Attributes.GetNamedItem("name").NodeValue
                            resultStr &= vbCrLf & "Error occured during provisioning " & nodeName & ", name: " & nameAttribute.ToString & description
                        End If
                        Exit Select
                    Case Else
                        resultStr &= vbCrLf & "Error occured during provisioning " & nodeName & ", error code: " & errorCode
                        Exit Select
                End Select
            Next
        End If
        Return resultStr

    End Function
End Class
