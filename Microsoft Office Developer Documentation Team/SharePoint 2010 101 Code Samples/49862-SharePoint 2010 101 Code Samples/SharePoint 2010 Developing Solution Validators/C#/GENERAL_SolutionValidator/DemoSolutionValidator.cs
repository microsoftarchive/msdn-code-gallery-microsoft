using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//We need this namespace for the GUID that identifies the validator
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
//We need this namespace for the SPUserCode class
using Microsoft.SharePoint.Administration;
//We need this namespace for the SPSolutionValidationProperties class
using Microsoft.SharePoint.UserCode;

namespace GENERAL_SolutionValidator
{
    /// <summary>
    /// A solution validator is a class that can check sandboxed user solutions when they
    /// are activated. Your code can test the solution to increase your confidence in
    /// user solutions. For example, you could allow only Web Part solutions or check for
    /// a particular certificate. In this case, we'll simply log all user solution activations
    /// </summary>
    /// <remarks>
    /// This GIUD is essential and used to remove the Solution Validator when the feature
    /// is deactivated. You can use Tools/Create GUID to generate a unique one. 
    /// </remarks>
    [Guid("CC900DAA-7D8B-4C7D-B867-5E2CF138EFB6")]
    class DemoSolutionValidator : SPSolutionValidator
    {

        private const string VALIDATOR_NAME = "Demo Solution Validator";

        //Two constructors
        public DemoSolutionValidator(SPUserCodeService userCodeService) : base(VALIDATOR_NAME, userCodeService)
        {
            //SharePoint uses this value to determine if the validator has changed
            //Alter it for every version. 
            this.Signature = 1234;
        }

        public DemoSolutionValidator()
        {

        }

        //This method is called once for every Solution as it activates
        public override void ValidateSolution(SPSolutionValidationProperties properties)
        {
            base.ValidateSolution(properties);
            //We will mark every solution as valid, then log its activation to a SharePoint list
            //The Valid property is false by default. We must set it to true or else the 
            //Solution activation will fail. Usually you'd do this only after making
            //your checks.
            properties.Valid = true;
            //NOTE: If the user solution fails your tests, you should use the 
            //ValidationErrorMessage and ValidationErrorUrl properties to tell the
            //user why.

            //Get the SPSite where the solution was activated, ensuring correct disposal
            using (SPSite site = properties.Site)
            {
                //Get the top level SPWeb
                using (SPWeb topWeb = site.RootWeb)
                {

                    //Get the Annoucements list
                    SPList annoucementsList = topWeb.Lists["Announcements"];

                    //Create a new announcement
                    SPListItem newAnnouncement = annoucementsList.Items.Add();
                    newAnnouncement["Title"] = "A user solution has been activated";
                    newAnnouncement["Body"] = "The user solution name was: " + properties.Name;
                    newAnnouncement.Update();
                }
            }
        }

        //This method is called once for every Assembly in the solution
        public override void ValidateAssembly(SPSolutionValidationProperties properties, SPSolutionFile assembly)
        {
            base.ValidateAssembly(properties, assembly);
            //In this example, we run no special check on assemblies
            //But we must set Valid = true or the solution activation will not complete
            properties.Valid = true;
        }
    }
}
