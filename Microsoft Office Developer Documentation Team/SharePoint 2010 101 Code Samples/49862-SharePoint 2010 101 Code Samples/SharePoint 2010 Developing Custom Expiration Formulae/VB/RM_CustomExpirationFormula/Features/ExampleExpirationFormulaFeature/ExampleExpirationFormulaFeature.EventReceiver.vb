Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.Office.RecordsManagement.InformationPolicy
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security


<GuidAttribute("e4564980-adae-4fc9-b04a-ab6146d12912")> _
Public Class ExampleExpirationFormulaFeatureEventReceiver 
    Inherits SPFeatureReceiver

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)

        'The RM_CustomExpirationFormula.dll assembly will be deployed to the GAC
        'In this event of the feature receiver, we must let SharePoint know that
        'it is a custom expiration formula. 

        'Start by defining an XML Manifest that describes the custom formula
        Dim xmlManifestFormula As String = "<PolicyResource xmlns='urn:schemas-microsoft-com:office:server:policy' " + _
                                                "id='RM_CustomExpirationFormula.ExampleExpirationFormula' " + _
                                                "featureId='Microsoft.Office.RecordsManagement.PolicyFeatures.Expiration' " + _
                                                "type='DateCalculator'>" + _
                                                    "<Name>ExampleExpirationFormula</Name>" + _
                                                    "<Description>This expiration formula enforces expiration on the last day of the month after modification</Description>" + _
                                                    "<AssemblyName>RM_CustomExpirationFormula, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f61c4839a868f1ff</AssemblyName>" + _
                                                    "<ClassName>RM_CustomExpirationFormula.ExampleExpirationFormula</ClassName>" + _
                                            "</PolicyResource>"

        'Validate this manifest
        PolicyResource.ValidateManifest(xmlManifestFormula)

        'It checks out OK, so add it
        PolicyResourceCollection.Add(xmlManifestFormula)

    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)

        'Delete the custom formula from the policy resources collection
        PolicyResourceCollection.Delete("RM_CustomExpirationFormula.ExampleExpirationFormula")

    End Sub

End Class
