using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using Microsoft.SharePoint.Phone.Application;
using Microsoft.SharePoint.Client;
using System.Collections.ObjectModel;
using System.Device.Location;

namespace SPListAppUICustomization
{
    [DataContract]
    public class DesignDisplayItemViewModel : DisplayItemViewModelBase
    {
        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for data binding to Display form UI
        /// </summary>
        public DesignDisplayItemViewModel()
        {

            //Title
            this["Title"] = "Sample Text";

            //Description
            this["Description"] = "Sample Text";

            //Product_x0020_Category
            this["Product_x0020_Category"] = "Sample Choice";

            //Quantity
            this["Quantity"] = "99.5";

            //Order_x0020_Date
            this["Order_x0020_Date"] = "1/21/2012";

            //Fulfillment_x0020_Date
            this["Fulfillment_x0020_Date"] = "1/21/2012";

            //Rush
            this["Rush"] = true;

            //Contact_x0020_Number
            this["Contact_x0020_Number"] = "Sample Text";

            //Modified
            this["Modified"] = "1/21/2012";

            //Created
            this["Created"] = "1/21/2012";

            //Author
            this["Author"] = "John Doe";

            //Editor
            this["Editor"] = "John Doe";

            //_UIVersionString
            this["_UIVersionString"] = "Sample Text";

            //Attachments
            this["Attachments"] = "FileName1.txt;Picture.txt;";

        }


        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for data binding to Display form UI
        /// </summary>
        /// <param name="fieldName" />
        /// <returns />
        public override object this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                base[fieldName] = value;
            }
        }
    }
}

