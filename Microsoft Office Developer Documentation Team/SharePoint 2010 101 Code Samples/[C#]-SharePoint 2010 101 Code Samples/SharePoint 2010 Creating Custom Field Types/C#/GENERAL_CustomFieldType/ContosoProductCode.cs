using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace GENERAL_CustomFieldType
{
    /// <summary>
    /// When a user creates a new column in a List or Content Type, they see a list of 
    /// Field Types, such as "Single Line of Text" and "Date and Time". By creating your
    /// own Field Type, you can extend this list. You can also include your own logic in 
    /// the new Field Type, such as custom code to set the default value or validate user
    /// input. This code example shows how to create a custom Field Type
    /// </summary>
    /// <remarks>
    /// To create a custom field type, create an empty SharePoint project and then add a 
    /// new class. The new class should inherit from a SPField class. In this case, the 
    /// field stores strings, so we inherit from SPFieldText. You must also create a 
    /// fldtypes*.xml file and deploy it to TEMPLATES\XML in the 14 hive. 
    /// </remarks>
    public class ContosoProductCode : SPFieldText
    {

        #region Constructors

        //You must create both these constructors, passing the parameters to the base class
        public ContosoProductCode(SPFieldCollection fields, string fName)
            : base(fields, fName)
        {

        }

        public ContosoProductCode(SPFieldCollection fields, string tName, string dName)
            : base(fields, tName, dName)
        {

        }

        #endregion

        //Override this property to formulate a default value. 
        public override string DefaultValue
        {
            get
            {
                return "CP0001";
            }
        }

        //Override this method to validate string data.
        public override string GetValidatedString(object value)
        {
            //Check that it starts with CP for "Contoso Product"
            if (!value.ToString().StartsWith("CP"))
            {
                //When you throw an SPFieldValidationException users see red text in the UI
                throw new SPFieldValidationException("Product code must start with 'CP' for Contoso Products");
            }

            //Check that it's 6 characters long
            if (value.ToString().Length != 6)
            {
                throw new SPFieldValidationException("Product code must be 6 characters long");
            }

            //Always convert to uppercase before writing to Content DB.
            return value.ToString().ToUpper();
        }

    }
}
