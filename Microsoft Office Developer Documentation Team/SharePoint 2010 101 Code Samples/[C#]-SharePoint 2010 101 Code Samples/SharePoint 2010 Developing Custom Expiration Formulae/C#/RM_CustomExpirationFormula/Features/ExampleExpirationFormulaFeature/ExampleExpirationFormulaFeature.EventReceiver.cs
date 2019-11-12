using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Office.RecordsManagement.InformationPolicy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace RM_CustomExpirationFormula.Features.ExampleExpirationFormulaFeature
{

    [Guid("f341ad80-19a4-4fd0-9ec2-00d4e634727c")]
    public class ExampleExpirationFormulaFeatureEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //The RM_CustomExpirationFormula.dll assembly will be deployed to the GAC
            //In this event of the feature receiver, we must let SharePoint know that
            //it is a custom expiration formula. 

            //Start by defining an XML Manifest that describes the custom formula
            string xmlManifestFormula = "<PolicyResource xmlns='urn:schemas-microsoft-com:office:server:policy' " +
                                            "id='RM_CustomExpirationFormula.ExampleExpirationFormula' " +
                                            "featureId='Microsoft.Office.RecordsManagement.PolicyFeatures.Expiration' " +
                                            "type='DateCalculator'>" +
                                                "<Name>ExampleExpirationFormula</Name>" +
                                                "<Description>This expiration formula enforces expiration on the last day of the month after modification</Description>" +
                                                "<AssemblyName>RM_CustomExpirationFormula, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a032342a1fd34f08</AssemblyName>" +
                                                "<ClassName>RM_CustomExpirationFormula.ExampleExpirationFormula</ClassName>" +
                                        "</PolicyResource>";

            //Validate this manifest
            PolicyResource.ValidateManifest(xmlManifestFormula);

            //It checks out OK, so add it
            PolicyResourceCollection.Add(xmlManifestFormula);
        
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //Delete the custom formula from the policy resources collection
            PolicyResourceCollection.Delete("RM_CustomExpirationFormula.ExampleExpirationFormula");

        }
    }
}
