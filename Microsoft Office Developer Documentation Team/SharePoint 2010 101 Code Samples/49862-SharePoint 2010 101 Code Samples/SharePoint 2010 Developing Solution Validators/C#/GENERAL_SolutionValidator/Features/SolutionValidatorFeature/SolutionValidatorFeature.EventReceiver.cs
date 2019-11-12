using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.UserCode;

namespace GENERAL_SolutionValidator.Features.SolutionValidatorFeature
{
    /// <summary>
    /// This feature receiver installs the custom solution validator on feature activation.
    /// It also cleans up on feature deactivation
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [Guid("308cc83e-344d-48a8-be73-6501e9de97f8")]
    public class SolutionValidatorFeatureEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Add the Custom Solution Validators to the Solution Validators collection
            SPUserCodeService.Local.SolutionValidators.Add(new DemoSolutionValidator(SPUserCodeService.Local));
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //Remove the Solution Validator. The Guid is the one that marks the DemoSolutionValidator class
            SPUserCodeService.Local.SolutionValidators.Remove(new Guid("CC900DAA-7D8B-4C7D-B867-5E2CF138EFB6"));
        }

    }
}
