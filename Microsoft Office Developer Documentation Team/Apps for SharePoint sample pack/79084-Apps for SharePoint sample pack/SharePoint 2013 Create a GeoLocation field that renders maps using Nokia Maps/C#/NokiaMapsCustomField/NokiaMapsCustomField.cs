using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace NokiaMapsCustomField
{
    public class NokiaMapsCustomField : SPFieldGeolocation
    {
        /// <summary>
        /// Create an instance of NokiaMapsCustomField object
        /// </summary>
        /// <param name="fields">Field collection</param>
        /// <param name="fieldName">Name of the field</param>
        public NokiaMapsCustomField(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName)
        {
        }
         /// <summary>
        /// Create an instance of NokiaMapsCustomField object
        /// </summary>
        /// <param name="fields">Field collection</param>
        /// <param name="typeName">type name of the field</param>
        /// <param name="displayName">display name of the field</param>
        public NokiaMapsCustomField(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }
        /// <summary>
        /// Override JSLink property
        /// </summary>
        public override string JSLink
        {
            get
            {
                return "NokiaMapsControl.js";
            }
            set
            {
                base.JSLink = value;
            }
        }

        /// <summary>
        /// get the field values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object GetFieldValue(string value)
        {
            return base.GetFieldValue(value);
        }

        /// <summary>
        /// get validated string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetValidatedString(object value)
        {
            return base.GetValidatedString(value);
        }

    }
}
