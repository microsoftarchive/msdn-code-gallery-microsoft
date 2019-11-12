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

namespace SPListAppForNotifications
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

            //AssignedTo
            this["AssignedTo"] = "Sample Text";

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

