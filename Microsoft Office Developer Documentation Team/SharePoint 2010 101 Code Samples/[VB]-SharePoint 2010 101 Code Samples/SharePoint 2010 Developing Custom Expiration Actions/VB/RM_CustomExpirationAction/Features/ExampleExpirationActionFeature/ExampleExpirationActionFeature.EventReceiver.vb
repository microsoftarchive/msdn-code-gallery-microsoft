Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.Office.RecordsManagement.PolicyFeatures
Imports Microsoft.Office.RecordsManagement.InformationPolicy
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security


<GuidAttribute("d65003cf-1195-4173-9d73-45747167358f")> _
Public Class ExampleExpirationActionFeatureEventReceiver 
    Inherits SPFeatureReceiver

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)

        'The RM_CustomExpirationAction.dll assembly will be deployed to the GAC
        'In this event of the feature receiver, we must let SharePoint know that
        'it is a custom expiration action. 

        'Start by defining an XML Manifest that describes the custom action
        Dim xmlManifestAction As String = "<PolicyResource xmlns='urn:schemas-microsoft-com:office:server:policy' " + _
                                                "id='RM_CustomExpirationAction.ExampleExpirationAction' " + _
                                                "featureId='Microsoft.Office.RecordsManagement.PolicyFeatures.Expiration' " + _
                                                "type='Action'>" + _
                                                    "<Name>ExampleExpirationAction</Name>" + _
                                                    "<Description>This expiration action adds an announcement when an item expires</Description>" + _
                                                    "<AssemblyName>RM_CustomExpirationAction, Version=1.0.0.0, Culture=neutral, PublicKeyToken=725568f530aaa609</AssemblyName>" + _
                                                    "<ClassName>RM_CustomExpirationAction.ExampleExpirationAction</ClassName>" + _
                                          "</PolicyResource>"

        'Validate this manifest
        PolicyResource.ValidateManifest(xmlManifestAction)

        'It checks out OK, so add it
        PolicyResourceCollection.Add(xmlManifestAction)

    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'Delete the custom action from the policy resources collection
        PolicyResourceCollection.Delete("RM_CustomExpirationAction.ExampleExpirationAction")
    End Sub

End Class
